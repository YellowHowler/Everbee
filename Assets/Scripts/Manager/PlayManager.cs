using ClassDef;
using EnumDef;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StructDef;
using static UnityEngine.UI.CanvasScaler;

public class PlayManager : MonoBehaviour
{
    static public bool MustLoadSaveData = false;

    public static PlayManager Instance { get; private set; }

    public Hive kHive;
    public Garden kGarden;
    public MainCanvas kMainCanvas;
    public Bees kBees;
    public Inventory kInventory;
    public Hummingbird kBird;

    public PlayerCamera kCamera;

    public GameResAmount[] kStorageResourceAmounts;

    public VectorBound kHiveXBound = new VectorBound(-50, -10000);
    public VectorBound kHiveYBound = new VectorBound(10000, 50);

    public VectorBound kGardenXBound = new VectorBound(10000, -10000);
    public VectorBound kGardenYBound = new VectorBound(10000, -10000);

	public EventFuncDispatcher EscapeKeyDispatcher = new EventFuncDispatcher();
	public Rect WorldBoundary { get; private set; }


	void Awake()
    {
        Instance = this;

        kHive = GameObject.Find("Stage/Hive").GetComponent<Hive>();
        kGarden = GameObject.Find("Stage/Garden").GetComponent<Garden>();
        kMainCanvas = GameObject.Find("Canvas").GetComponent<MainCanvas>();
        kInventory = GameObject.Find("Inventory").GetComponent<Inventory>();
        kBird = GameObject.Find("Hummingbird").GetComponent<Hummingbird>();

        kCamera = Camera.main.GetComponent<PlayerCamera>();

        kStorageResourceAmounts = new GameResAmount[]
                                    {
                                        new GameResAmount(0, GameResUnit.Microgram),
                                        new GameResAmount(0, GameResUnit.Microgram),
                                        new GameResAmount(0, GameResUnit.Microgram),
                                        new GameResAmount(0, GameResUnit.Microgram)
                                    };

        //EasyTouch.On_TouchStart += OnTouch;
    }

	private void OnDestroy()
	{
		Instance = null;
	}

    private void InitDefault()
    {
		kHive.InitDefault();
		kGarden.InitDefault();
		kBees.InitDefault();
		kInventory.Init(MainCanvas.Instance.kInven.kItemImages.Length);
		MainCanvas.Instance.kInven.Init();
	}
	private void Start()
    {
        if (MustLoadSaveData)
        {
            MustLoadSaveData = false;
            if (!SaveManager.Instance.Load())
                InitDefault();
        }
        else
            InitDefault();

		kHive.RecountAllResources();
    }

    public void GameStart()
    {
    }

    public void UpdateBackground()
    {
        ComputeWorldBoundary();
        kGarden.SetBound(WorldBoundary.xMin, WorldBoundary.xMax);
    }

	// Update is called once per frame
	private void Update()
	{
		ComputeWorldBoundary();

		if(Input.GetKeyDown(KeyCode.Escape))
		{
			// EscapeKeyDispatcher 는 팝업창이 비어있을 때에만 유효

			var lastPopup = PopupBase.GetLastPopup();
			if(lastPopup != null)
			{
				lastPopup.ProcessEscapeKey();
			}
			else
			{
				EscapeKeyDispatcher.Dispatch((func) =>
				{
					return func();
				});

                if (!EscapeKeyDispatcher.Consumed)
                {
                    MainCanvas.Instance.kDialoguePopup.Show("Do you want to quit?\r\n(Your progress will be saved) ", LHS.CLHSDialogUI.EButtonType.YESNO, (result) => 
                    {
                        if (result == LHS.CLHSDialogUI.EDialogResult.YES)
                        {
							SaveManager.Instance.Save();
							UnityEngine.SceneManagement.SceneManager.LoadScene("Scenes/TitleScene"); 
                        }

                        return true; 
                    });
                    
                    return;
                }
			}
		}
	}

	public int RoundFloat(float _num)
    {
        return (int)Mathf.Floor(_num + 0.5f);
    }

    public int GetMod(int _a, int _b)
    {
        return ( _a % _b + _b ) % _b;
    }

    public string GetTimeText(int _seconds)
    {
        int min = _seconds / 60;
        int sec = _seconds % 60;

        if(sec >= 10)
        {
            return min + ":" + sec;
        }
        else
        {
            return min + ":0" + sec;
        }
    }

    public Quaternion GetPointingRotation(Vector3 _originPos, Vector3 _targetPos, Quaternion _curRot)
    {
        Vector2 direction = new Vector2(
            _originPos.x - _targetPos.x,
            _originPos.y - _targetPos.y
        );

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion angleAxis = Quaternion.AngleAxis(angle - 90f, Vector3.forward);
        Quaternion rotation = Quaternion.Slerp(_curRot, angleAxis, 100f);
        
        return rotation;
    }

    public void ResetResourceOfStorage(GameResType _type)
    {
        if (_type == GameResType.Empty)
            return;

        kStorageResourceAmounts[(int)_type] = new GameResAmount(0, kStorageResourceAmounts[(int)_type].unit);
    }

    public void AddResourceToStorage(GameResType _type, GameResAmount _amount)
    {
		if ( (_type == GameResType.Empty) || (_type >= GameResType.Wax) )
			return;

		kStorageResourceAmounts[(int)_type] = AddResourceAmounts(kStorageResourceAmounts[(int)_type], _amount);

        kMainCanvas.kResource.UpdateText();
    }

    public void SubtractResourceFromStorage(GameResType _type, GameResAmount _amount)
    {
		if(_type == GameResType.Empty)
			return;

		kStorageResourceAmounts[(int)_type] = SubtractResourceAmounts(kStorageResourceAmounts[(int)_type], _amount);

        kMainCanvas.kResource.UpdateText();
    }


    public GameResAmount AddResourceAmounts(GameResAmount _resAmountA, GameResAmount _resAmountB)
    {
        int aUnit = (int)_resAmountA.unit;
        int bUnit = (int)_resAmountB.unit;

        if (_resAmountA.amount == 0)
        {
            return _resAmountB;
        }
        if(_resAmountB.amount == 0)
        {
            return _resAmountA;
        }

        
        if (Mathf.Abs(aUnit - bUnit) >= 2)
        {
            if(aUnit > bUnit) 
            {
                return _resAmountA;
            }
            else
            {
                return _resAmountB;
            }
        }

        float retAmount = 0;
        GameResUnit retUnit = GameResUnit.Microgram;

        if (aUnit == bUnit)
        {
            retAmount = _resAmountA.amount + _resAmountB.amount;
            retUnit = _resAmountA.unit;
        }
        else
        {
            if(aUnit > bUnit)
            {
                retAmount = _resAmountA.amount + _resAmountB.amount * 0.001f;
                retUnit = _resAmountA.unit;
            }
            else
            {
                retAmount = _resAmountB.amount + _resAmountA.amount * 0.001f;
                retUnit = _resAmountB.unit;
            }
        }

        return UpdateUnit(new GameResAmount(retAmount, retUnit));
    }

    public GameResAmount SubtractResourceAmounts(GameResAmount _resAmountA, GameResAmount _resAmountB)
    {
        _resAmountB.amount = -1 * _resAmountB.amount;
        return AddResourceAmounts(_resAmountA, _resAmountB);
    }

    public GameResAmount MultiplyResourceAmountWithConstant(GameResAmount _amount, float _m)
    {
        GameResAmount retAmount = new GameResAmount(_amount.amount * _m, _amount.unit);
        return UpdateUnit(retAmount);
    }   

    public float GetResourcePercent(GameResAmount _amount, GameResAmount _maxAmount)
    {
        if(CompareResourceAmounts(_maxAmount, _amount) == true)
        {
            return 100;
        }

        int curUnit = (int)_amount.unit;
        int maxUnit = (int)_maxAmount.unit;
        
        switch(maxUnit - curUnit)
        {
            case 0: 
                return Mathf.Clamp(_amount.amount / _maxAmount.amount, 0, 1) * 100;
            case 1: 
                return Mathf.Clamp(_amount.amount / (_maxAmount.amount * 1000), 0, 1) * 100;
            default:
                return 0;
        }

        
    }

    public GameResAmount UpdateUnit(GameResAmount _amount)
    {
        while(_amount.amount >= 1000f || _amount.amount < 1f)
        {
            if(_amount.amount >= 1000f)
            {
                _amount.amount /= 1000;
                _amount.unit = (GameResUnit)((int)_amount.unit + 1);
            }
            else if (_amount.amount < 1)
            {
                if (_amount.unit == GameResUnit.Microgram)
                {
                    return new GameResAmount(0f, GameResUnit.Microgram);
                }
                _amount.amount *= 1000;
                _amount.unit = (GameResUnit)((int)_amount.unit - 1);
            }
        }
        
        return _amount;
    }

    public bool IsBaseResource(GameResType _type)
    {
        return _type == GameResType.Pollen || _type == GameResType.Honey || _type == GameResType.Nectar || _type == GameResType.Wax;
    }
    /// <summary> ù��°�� �� ũ�� false, �ι�°�� �� ũ�� true </summary>
    
    public bool CompareResourceAmounts(GameResAmount _resAmountA, GameResAmount _resAmountB)
    {
        if(IsSameAmount(_resAmountA, _resAmountB)) return true;

        if((int)_resAmountA.unit != (int) _resAmountB.unit)
        {
            return (int)_resAmountA.unit < (int)_resAmountB.unit;
        }

        return _resAmountA.amount < _resAmountB.amount;
    }

    public bool IsSameAmount(GameResAmount _resAmountA, GameResAmount _resAmountB)
    {
        return Mathf.Abs(_resAmountA.amount - _resAmountB.amount) <= 0.01f && _resAmountA.unit == _resAmountB.unit;
    }

    public bool IsAmountZero(GameResAmount _amount)
    {
        return IsSameAmount(_amount, new GameResAmount(0f, GameResUnit.Microgram));
    }

    public bool IsWithinCollider(Collider2D _col, Vector3 _point)
    {
        return _col.OverlapPoint(Mng.play.SetZ(_point, _col.gameObject.transform.position.z));
    }

    public Vector3 SetZ(Vector3 _pos, float _z)
    {
        return new Vector3(_pos.x, _pos.y, _z);
    }

	public void ComputeWorldBoundary()
	{
		Rect rect = Rect.zero;
		rect.xMin = -40;
        rect.yMin = 10000000;
		rect.xMax = 50;
        rect.yMax = 60;

		rect = kHive.ComputeWorldBoundary(rect);
		rect = kGarden.ComputeWorldBoundary(rect);

		WorldBoundary = rect;
	}
}

