using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnumDef;
using ClassDef;
using StructDef;
using System;
//using HedgehogTeam.EasyTouch;

public class Hive : MonoBehaviour
{
    public static Hive Instance { get; private set; }

    public float mHoneycombRadiusX = 0.5f;
    public float mHoneycombRadiusY = 0.8f;
    public float mHoneycombDistance_X_Horizontal = 2;
    public float mHoneycombDistance_X_Other = 1.5f;

    private Vector3 mHoneycombOrigin;

    [HideInInspector] public float mHoneycombZ = 0.05f;
    [HideInInspector]public float mFloorY = -19.2f;

    public GameObject kHoneycombObj;
    public GameObject kItemObj;

	public GameObject kHoverObj;
    private SpriteRenderer kHoverObjSpriteRenderer;

	public Sprite[] kHoneycombNectarSprites;
    public Sprite[] kHoneycombPollenSprites;
    public Sprite[] kHoneycombHoneySprites;
    public Sprite[] kHoneycombWaxSprites;

    public Sprite[] kBuildSprites;

    private float mHoneycombYLimit = 16;

    private List<Honeycomb> mHoneycombList = new List<Honeycomb>();

    [Header("Honeycomb 이름 보기")]
    public bool kIsDrawHoneycombName = true;
    [Header("Honeycomb 모양 보기")]
    public bool kIsDrawHoneycombShape = true;

    [HideInInspector] public QueenBee mActiveQueenBee;

    [HideInInspector] public Honeycomb mHoveredHoneycomb;
    [HideInInspector] public Bee mHoveredBee;
    [HideInInspector] public QueenBee mHoveredQueenBee;

    [HideInInspector] public bool mIsPlacingItem = false;
    [HideInInspector] public Item mPlaceItem;

    [HideInInspector] public bool mIsBuilding = false;
    [HideInInspector] public StructureType mStructureType = StructureType.None;

    public Transform kItems;

    [HideInInspector] public GameResAmount[] mMaxItemAmounts;

    [HideInInspector] public GameResAmount mQueenHoneyNeed;
    [HideInInspector] public GameResAmount mQueenPollenNeed;

    [HideInInspector] public Dictionary<StructureType, GameResAmount> mWaxCosts = new Dictionary<StructureType, GameResAmount>();

    [HideInInspector] public bool mMouseOverBuildCancel = false;

	private void Awake()
	{
		Instance = this;
		mHoneycombOrigin = transform.position;
		kHoverObjSpriteRenderer = kHoverObj.GetComponent<SpriteRenderer>();
	}

	private void OnDestroy()
	{
        if (PlayManager.Instance != null)
            PlayManager.Instance.EscapeKeyDispatcher.DelFunc(OnEscapeKeyPressed);

		Instance = null;
	}
    private bool OnEscapeKeyPressed()
    {
        if (mIsBuilding)
        {
            mIsBuilding = false;
            return false;
        }
		
		return true;
	}

	public void RecountAllResources()
	{
        // Hive 안에 있는 재료들 양을 전부 다시 계산한다.
        for(int i=0; i<PlayManager.Instance.kStorageResourceAmounts.Length; ++i)
            PlayManager.Instance.ResetResourceOfStorage((GameResType)i);

        foreach(var comb in mHoneycombList)
            PlayManager.Instance.AddResourceToStorage(comb.type, comb.amount);
	}

	/// <summary> 벌이 자원을 어디에 저장해야 하는지 </summary>
	public Honeycomb GetUsableStorage(GameResType _type, bool _notBee)
    {
        // 랜덤으로 고른다.
        int offset = UnityEngine.Random.Range(0, mHoneycombList.Count);

        //이미 해당 자원이 들어있는 거 우선으로 고른다
        for(int i=0; i<mHoneycombList.Count; ++i)
        {
            int index = (i + offset) % mHoneycombList.Count;
            var comb = mHoneycombList[index];

            if (comb.kStructureType == StructureType.Storage && (comb.type == _type && comb.IsFull() == false) && (_notBee || !comb.mTargetBee.IsLinked()))
            {
                return comb;
            }
        }

        for(int i=0; i<mHoneycombList.Count; ++i)
        {
            int index = (i + offset) % mHoneycombList.Count;
            var comb = mHoneycombList[index];

            if (comb.kStructureType == StructureType.Storage && (comb.type == GameResType.Empty || (comb.type == _type && comb.IsFull() == false)) && (_notBee || !comb.mTargetBee.IsLinked()))
            {
                return comb;
            }
        }

        return null;
    }

    public Honeycomb GetFetchableStorage(GameResType _type)
    {
        // 랜덤으로 고른다.
        int offset = UnityEngine.Random.Range(0, mHoneycombList.Count);
        for(int i=0; i<mHoneycombList.Count; ++i)
        {
            int index = (i + offset) % mHoneycombList.Count;
            var comb = mHoneycombList[index];

            if (comb.kStructureType == StructureType.Storage && (comb.type == _type && comb.amount.amount > 0) && !comb.mTargetBee.IsLinked())
            {
                return comb;
            }
        }

        return null;
    }

    public Honeycomb GetUsableHoneyCombOfStructure(StructureType _stype)
    {
        // 랜덤으로 고른다.
        int offset = UnityEngine.Random.Range(0, mHoneycombList.Count);
        for(int i=0; i<mHoneycombList.Count; ++i)
        {
            int index = (i + offset) % mHoneycombList.Count;
            var comb = mHoneycombList[index];

            if (comb.kStructureType == _stype && (comb.type == GameResType.Empty || !comb.IsFull()) && !comb.mTargetBee.IsLinked())
            {
                return comb;
            }
        }

        return null;
    }

    public bool FlowerClick(GameResType _type, GameResAmount _amount)
    {
        int storeInvenSlot = Mng.play.kInventory.GetAvailableSlot(_type);

        if(storeInvenSlot != -1)
        {
            Mng.play.kInventory.AddSlotAmount(storeInvenSlot, _type, _amount);                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                          
            return true;
        }

        Honeycomb storeHoneycomb = GetUsableStorage(_type, true);

        if(storeHoneycomb != null)
        {
            storeHoneycomb.StoreResource(_type, _amount);
            return true;
        }

        return false;
    }

    public Honeycomb FindBuildingHoneycomb()
    {
        int offset = UnityEngine.Random.Range(0, mHoneycombList.Count);
        for(int i=0; i<mHoneycombList.Count; ++i)
        {
            int index = (i + offset) % mHoneycombList.Count;
            var comb = mHoneycombList[index];

            if (comb.kStructureType == StructureType.Building)
            {
                return comb;
            }
        }

        return null;
    }

    public Honeycomb AddNewHoneycomb(Vector3 _pos, bool _isImport)
    {
        _pos = Mng.play.SetZ(_pos, mHoneycombZ);
        var dupCheck = GetHoneycombFromPos(_pos);

        if(dupCheck != null || _pos.y > mHoneycombYLimit)
        {
            return null;
        }

        GameObject newHoneycomb = Instantiate(kHoneycombObj, _pos, Quaternion.identity, transform.GetChild(0));
        Honeycomb honeycomb = newHoneycomb.GetComponent<Honeycomb>();
        honeycomb.mHive = this;

        mHoneycombList.Add(honeycomb);

        if(_isImport == false)
        {
            SetHoneycombPosition(honeycomb, _pos);
        }

        honeycomb.SetStructure(StructureType.None, true);
        return honeycomb;
    }

    public void SetHoneycombPosition(Honeycomb comb, Vector3 _pos)
    {
        comb.pos = _pos;

		PlayManager.Instance.kHiveXBound.start = Mathf.Min(_pos.x - 6f,PlayManager.Instance.kHiveXBound.start);
		PlayManager.Instance.kHiveXBound.end = Mathf.Max(_pos.x + 6f,PlayManager.Instance.kHiveXBound.end);

		PlayManager.Instance.kHiveYBound.start = Mathf.Min(_pos.y - 1f,PlayManager.Instance.kHiveYBound.start);
		PlayManager.Instance.kHiveYBound.end = Mathf.Max(_pos.y + 1f,PlayManager.Instance.kHiveYBound.end);

        Mng.play.UpdateBackground();
	}

	public Honeycomb GetRandomHoneycomb()
    {
        return mHoneycombList.Count > 0 ? mHoneycombList[UnityEngine.Random.Range(0, mHoneycombList.Count)] : null;
    }

    public Vector3 GetHexagonPos(Vector3 _pos, HoneycombDirection _dir)
    {
        switch(_dir)
        {
            case HoneycombDirection.None:
                return _pos;
            case HoneycombDirection.TopLeft:
                return _pos + Vector3.left * mHoneycombRadiusX + Vector3.up * mHoneycombDistance_X_Other * mHoneycombRadiusY;
            case HoneycombDirection.TopRight:
                return _pos + Vector3.right * mHoneycombRadiusX + Vector3.up * mHoneycombDistance_X_Other * mHoneycombRadiusY;
            case HoneycombDirection.Left:
                return _pos + Vector3.left * mHoneycombRadiusX * mHoneycombDistance_X_Horizontal;
            case HoneycombDirection.Right:
                return _pos + Vector3.right * mHoneycombRadiusX * mHoneycombDistance_X_Horizontal;
            case HoneycombDirection.BottomLeft:
                return _pos + Vector3.left * mHoneycombRadiusX + Vector3.down * mHoneycombDistance_X_Other * mHoneycombRadiusY;
            case HoneycombDirection.BottomRight:
                return _pos + Vector3.right * mHoneycombRadiusX + Vector3.down * mHoneycombDistance_X_Other * mHoneycombRadiusY;
        }

        return _pos;
    }

    private void Start()
    {
        kHoverObj.SetActive(false);

		PlayManager.Instance.EscapeKeyDispatcher.AddFunc(OnEscapeKeyPressed);
	}

	public void InitDefault()
    {
        //EasyTouch.On_TouchStart += OnTouch;

        Vector3 newPos = new Vector3(transform.position.x, transform.position.y+5, 0.05f);

        for(int i = 0; i < 6; i++)
        {
            var honeycomb = AddNewHoneycomb(newPos, false);
            GetHoneycombFromPos(newPos).mStartStructureType = StructureType.Storage;
            newPos = GetHexagonPos(newPos, HoneycombDirection.Right);
        }

        mMaxItemAmounts = new GameResAmount[4];

        mMaxItemAmounts[0] = new GameResAmount(10, GameResUnit.Milligram);
        mMaxItemAmounts[1] = new GameResAmount(10, GameResUnit.Milligram);
        mMaxItemAmounts[2] = new GameResAmount(500, GameResUnit.Milligram);
        mMaxItemAmounts[3] = new GameResAmount(10, GameResUnit.Milligram);

        mQueenHoneyNeed = new GameResAmount(1, GameResUnit.Milligram);
        mQueenPollenNeed = new GameResAmount(100, GameResUnit.Milligram);

        mWaxCosts.Add(StructureType.Storage, new GameResAmount(0, GameResUnit.Microgram));
        mWaxCosts.Add(StructureType.Dryer, new GameResAmount(10, GameResUnit.Microgram));
    }

	private void Update()
    {
		if (!PopupBase.IsTherePopup())
		{
	        if (Input.GetMouseButton(1)) // 오른버튼 누르면 취소
	        {
		        mIsBuilding = false;
		        return;
	        }
		}
	}

	public Honeycomb GetHoneycombFromPos(Vector3 _pos)
    {
        float minDistance = mHoneycombRadiusY;
        Honeycomb retHoneycomb = null;

        foreach(Honeycomb honeycomb in mHoneycombList)
        {
            float distance = Mathf.Sqrt(Mathf.Pow((honeycomb.pos.x - _pos.x),2) + Mathf.Pow((honeycomb.pos.y - _pos.y),2));

            if(distance <= minDistance)
            {
                minDistance = distance;
                retHoneycomb = honeycomb;
            }
        }

        return retHoneycomb;
    }

    public void EndBuild()
    {
        mIsBuilding = false;

        HideEmptyHoneycombs();

        Mng.canvas.EnableToggleButtons();
        kHoverObj.SetActive(false);
        Mng.canvas.HideBuildCancel();
        Mng.canvas.kInven.gameObject.SetActive(true);
    }

    public void ShowEmptyHoneycombs()
    {
        foreach(Honeycomb honeycomb in mHoneycombList)
        {
            honeycomb.kEmptyObj.SetActive(true);
        }
    }

    public void HideEmptyHoneycombs()
    {
        foreach(Honeycomb honeycomb in mHoneycombList)
        {
            honeycomb.kEmptyObj.SetActive(false);
        }
    }

    public void SetDrawBuild(StructureType _type)
    {
        mIsBuilding = true;
        mStructureType = _type;

        Mng.canvas.DisableToggleButtons();
        Mng.canvas.ShowBuildCancel();

        kHoverObjSpriteRenderer.sprite = kBuildSprites[(int)_type - 1];
        kHoverObj.SetActive(true);

        StartCoroutine(SetBuildCor());
    }

    public IEnumerator SetBuildCor()
    {
        if(mStructureType == StructureType.Storage)
        {
            ShowEmptyHoneycombs();
        }

        while (mIsBuilding == true)
        {
            kHoverObjSpriteRenderer.enabled = !mMouseOverBuildCancel;

            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 touchPos = new Vector2(pos.x, pos.y);

            int xPos = 0;
            int yPos = Mng.play.RoundFloat((touchPos.y - mHoneycombOrigin.y) / (mHoneycombRadiusY * 1.5f));

            if (Mng.play.GetMod(yPos, 2) == 1)
            {
                xPos = Mng.play.RoundFloat((touchPos.x - mHoneycombOrigin.x) / (mHoneycombRadiusX) / 2 - 0.5f) * 2 + 1;
            }
            else if (Mng.play.GetMod(yPos, 2) == 0)
            {
                xPos = Mng.play.RoundFloat((touchPos.x - mHoneycombOrigin.x) / (mHoneycombRadiusX) / 2) * 2;
            }

            kHoverObj.transform.localPosition = new Vector3(xPos * mHoneycombRadiusX, yPos * mHoneycombRadiusY * 1.5f, transform.position.z);

			// 해당 위치의 honeyObj 을 가져온다.
			var hoverPos = kHoverObj.transform.position;
            Honeycomb found = null;
            foreach(var honeyObj in mHoneycombList)
            {
                if ((honeyObj.transform.position - hoverPos).sqrMagnitude < 0.05f)
                {
                    found = honeyObj;
                    break;
                }
            }

            bool hoverEnabled = false;
            if (found == null)
            {
                hoverEnabled = false;
            }
            else
            {
                switch(mStructureType)
                {
                    case StructureType.Storage:
                        hoverEnabled = (found.kStructureType == StructureType.None);
                        break;

                    case StructureType.Dryer:
                        hoverEnabled = (found.kStructureType == StructureType.Storage);
                        break;

					case StructureType.Coalgulate:
						hoverEnabled = (found.kStructureType == StructureType.Storage);
						break;

					case StructureType.Hatchtery:
						hoverEnabled = (found.kStructureType == StructureType.None);
						break;
				}
			}

			kHoverObjSpriteRenderer.color = hoverEnabled ? new Color(0.5f,1f,0.5f,1f) : new Color(1f,0.5f,0.5f,1f);

			yield return null;
        }
        
        EndBuild();
    }

    public Rect ComputeWorldBoundary(Rect rect)
    {
        // PlayManager.Instance.kHiveBound 는 월드 좌표다.

        rect.xMin = Mathf.Min(rect.xMin, PlayManager.Instance.kHiveXBound.start);
        rect.xMax = Mathf.Max(rect.xMax, PlayManager.Instance.kHiveXBound.end);
		rect.yMin = Mathf.Min(rect.yMin, PlayManager.Instance.kHiveYBound.start);
		rect.yMax = Mathf.Max(rect.yMax, PlayManager.Instance.kHiveYBound.end);

        return rect;
	}

    [Serializable]
	// 세이브/로드 관련
	public class CSaveData
	{
		public GameResAmount[] mMaxItemAmounts;
		public List<Honeycomb.CSaveData> mHoneycombList = new List<Honeycomb.CSaveData>();

		public GameResAmount mQueenHoneyNeed;
		public GameResAmount mQueenPollenNeed;
	}

	public void ExportTo(CSaveData savedata)
	{
        savedata.mHoneycombList.Clear();

		if(mMaxItemAmounts == null)
			savedata.mMaxItemAmounts = null;
		else
		{
			if((savedata.mMaxItemAmounts == null) || (savedata.mMaxItemAmounts.Length != mMaxItemAmounts.Length))
				savedata.mMaxItemAmounts = new GameResAmount[mMaxItemAmounts.Length];
		}

		for(int i = 0;i<mMaxItemAmounts.Length;++i)
			savedata.mMaxItemAmounts[i] = mMaxItemAmounts[i];

		for(int i=0; i<mHoneycombList.Count; ++i)
        {
            Honeycomb.CSaveData combSavedata = new Honeycomb.CSaveData();
            mHoneycombList[i].ExportTo(combSavedata);
            savedata.mHoneycombList.Add(combSavedata);
        }

        savedata.mQueenHoneyNeed = mQueenHoneyNeed;
        savedata.mQueenPollenNeed = mQueenPollenNeed;
	}

	public void ImportFrom(CSaveData savedata)
	{
		if(savedata.mMaxItemAmounts == null)
			mMaxItemAmounts = null;
		else
		{
			if((mMaxItemAmounts == null) || (mMaxItemAmounts.Length != savedata.mMaxItemAmounts.Length))
				mMaxItemAmounts = new GameResAmount[savedata.mMaxItemAmounts.Length];
		}

		for(int i = 0;i<savedata.mMaxItemAmounts.Length;++i)
			mMaxItemAmounts[i] = savedata.mMaxItemAmounts[i];

		for(int i=0; i<mHoneycombList.Count; ++i)
            GameObject.Destroy(mHoneycombList[i].gameObject);
        mHoneycombList.Clear();

        for(int i=0; i<savedata.mHoneycombList.Count; ++i)
        {
            var comb = AddNewHoneycomb(Vector3.zero, true);
            comb.ImportFrom(savedata.mHoneycombList[i]);
            SetHoneycombPosition(comb, comb.pos);
        }

		mQueenHoneyNeed = savedata.mQueenHoneyNeed;
		mQueenPollenNeed = savedata.mQueenPollenNeed;
	}
}
