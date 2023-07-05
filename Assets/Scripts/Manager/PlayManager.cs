using ClassDef;
using EnumDef;
using HedgehogTeam.EasyTouch;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StructDef;
using static UnityEngine.UI.CanvasScaler;

public class PlayManager : MonoBehaviour
{
    public static PlayManager Instance;

    public Hive kHive;
    public Garden kGarden;
    public MainCanvas kMainCanvas;
    public Bees kBees;

    public PlayerCamera kCamera;

    public GameResAmount[] kStorageResourceAmounts;

    public VectorBound kHiveXBound = new VectorBound(10000, -10000);
    public VectorBound kHiveYBound = new VectorBound(10000, -10000);

    void Awake()
    {
        Instance = this;

        kHive = GameObject.Find("Stage/Hive").GetComponent<Hive>();
        kGarden = GameObject.Find("Stage/Garden").GetComponent<Garden>();
        kMainCanvas = GameObject.Find("Canvas").GetComponent<MainCanvas>();

        kCamera = Camera.main.GetComponent<PlayerCamera>();

        kStorageResourceAmounts = new GameResAmount[]
                                    {
                                        new GameResAmount(0, GameResUnit.Microgram),
                                        new GameResAmount(0, GameResUnit.Microgram),
                                        new GameResAmount(0, GameResUnit.Microgram),
                                        new GameResAmount(0, GameResUnit.Microgram)
                                    };

        EasyTouch.On_TouchStart += OnTouch;
    }

    private void Start()
    {
        
    }

    public void GameStart()
    {
    }
    
    // Update is called once per frame
    void Update()
    {
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

    public void AddResourceToStorage(GameResType _type, GameResAmount _amount)
    {
        kStorageResourceAmounts[(int)_type] = AddResourceAmounts(kStorageResourceAmounts[(int)_type], _amount);

        kMainCanvas.kResource.UpdateText();
    }

    public void SubtractResourceFromStorage(GameResType _type, GameResAmount _amount)
    {
        kStorageResourceAmounts[(int)_type] = SubtractResourceAmounts(kStorageResourceAmounts[(int)_type], _amount);

        kMainCanvas.kResource.UpdateText();
    }

    private void OnTouch(Gesture gesture)
    {
        if (gesture.pickedObject == null)
            return;

        //Mng.canvas.kResource.SetText("??");

        /*
        if( kHive.kQueenBee.gameObject == gesture.pickedObject )
        {
            PlayerCamera.Instance.SetFollow(kHive.kQueenBee.transform);
        }
        */
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

    public float GetResourcePercent(GameResAmount _amount, GameResAmount _maxAmount)
    {
        if(CompareResourceAmounts(_maxAmount, _amount) == true)
        {
            return 100;
        }
        if((int)_maxAmount.unit != (int)_amount.unit)
        {
            return 0;
        }

        return Mathf.Clamp(_amount.amount / _maxAmount.amount, 0, 1) * 100;
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

    /// <summary> ù��°�� �� ũ�� false, �ι�°�� �� ũ�� true </summary>
    public bool CompareResourceAmounts(GameResAmount _resAmountA, GameResAmount _resAmountB)
    {
        if((int)_resAmountA.unit != (int) _resAmountB.unit)
        {
            return (int)_resAmountA.unit < (int)_resAmountB.unit;
        }

        return _resAmountA.amount < _resAmountB.amount;
    }

    public bool IsSameAmount(GameResAmount _resAmountA, GameResAmount _resAmountB)
    {
        if(_resAmountA.amount == 0 && _resAmountB.amount == 0) 
        {
            return true;
        }

        return _resAmountA.amount == _resAmountB.amount && _resAmountA.unit == _resAmountB.unit;
    }

    public Vector3 SetZ(Vector3 _pos, float _z)
    {
        return new Vector3(_pos.x, _pos.y, _z);
    }
}

