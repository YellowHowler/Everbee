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

    public float mFloorY = -19.2f;

    public GameObject kHoneycombObj;
    public GameObject kItemObj;

	public GameObject kHoverObj;
    private SpriteRenderer kHoverObjSpriteRenderer;

	public Sprite[] kHoneycombNectarSprites;
    public Sprite[] kHoneycombPollenSprites;
    public Sprite[] kHoneycombHoneySprites;
    public Sprite[] kHoneycombWaxSprites;

    public Sprite[] kBuildSprites;

    private List<Honeycomb> mHoneycombList = new List<Honeycomb>();

    [Header("Honeycomb 이름 보기")]
    public bool kIsDrawHoneycombName = true;
    [Header("Honeycomb 모양 보기")]
    public bool kIsDrawHoneycombShape = true;

    [HideInInspector] public bool mIsBuilding = false;
    [HideInInspector] public StructureType mStructureType = StructureType.None;

    public Transform kItems;

    [HideInInspector] public GameResAmount[] mMaxItemAmounts;

    [HideInInspector] public GameResAmount mQueenHoneyNeed;
    [HideInInspector] public GameResAmount mQueenPollenNeed;

    [HideInInspector] public bool mGuidingQueen  = false;
    [HideInInspector] public QueenBee mTargetQueen  = null;



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

	/// <summary> 벌이 자원을 어디에 저장해야 하는지 </summary>
	public Honeycomb GetUsableHoneycomb(GameResType _type)
    {
        foreach (Honeycomb h in mHoneycombList)
        {
            if (h.kStructureType == StructureType.Storage && (h.type == GameResType.Empty || (h.type == _type && h.IsFull() == false)) && h.isTarget == false)
            {
                return h;
            }
        }

        return null;
    }

    public Honeycomb GetHoneycombOfType(GameResType _type)
    {
        foreach (Honeycomb h in mHoneycombList)
        {
            if (h.kStructureType == StructureType.Storage && (h.type == _type && h.amount.amount > 0) && h.isTarget == false)
            {
                return h;
            }
        }

        return null;
    }

    /*public void GetAllHoneycombs() //현재 씬에 있는 모든 Honeycomb 다 가져온다
    {
        mHoneycombList.Clear();

        GameObject[] honeycombs = GameObject.FindGameObjectsWithTag("Honeycomb");

        foreach (GameObject h in honeycombs)
        {
            Honeycomb honeycomb = h.GetComponent<Honeycomb>();
            mHoneycombList.Add(honeycomb); //임시
            honeycomb.mHive = this;

            PlayManager.Instance.kHiveXBound.start = Mathf.Min(honeycomb.pos.x - 0.5f, PlayManager.Instance.kHiveXBound.start);
            PlayManager.Instance.kHiveXBound.end = Mathf.Max(honeycomb.pos.x + 0.5f, PlayManager.Instance.kHiveXBound.end);

            PlayManager.Instance.kHiveYBound.start = Mathf.Min(honeycomb.pos.y - 0.5f, PlayManager.Instance.kHiveYBound.start);
            PlayManager.Instance.kHiveYBound.end = Mathf.Max(honeycomb.pos.y + 0.5f, PlayManager.Instance.kHiveYBound.end);
        }
    }*/

    public Honeycomb AddNewHoneycomb()
    {
        GameObject newHoneycomb = Instantiate(kHoneycombObj, Vector3.zero, Quaternion.identity, transform.GetChild(0));
        Honeycomb honeycomb = newHoneycomb.GetComponent<Honeycomb>();
        honeycomb.mHive = this;

        mHoneycombList.Add(honeycomb);

        return honeycomb;
    }

    public void SetHoneycombPosition(Honeycomb comb, Vector3 _pos)
    {
        comb.pos = _pos;

		PlayManager.Instance.kHiveXBound.start = Mathf.Min(_pos.x - 0.5f,PlayManager.Instance.kHiveXBound.start);
		PlayManager.Instance.kHiveXBound.end = Mathf.Max(_pos.x + 0.5f,PlayManager.Instance.kHiveXBound.end);

		PlayManager.Instance.kHiveYBound.start = Mathf.Min(_pos.y - 0.5f,PlayManager.Instance.kHiveYBound.start);
		PlayManager.Instance.kHiveYBound.end = Mathf.Max(_pos.y + 0.5f,PlayManager.Instance.kHiveYBound.end);
	}

	public Honeycomb GetRandomHoneycomb()
    {
        return mHoneycombList.Count > 0 ? mHoneycombList[UnityEngine.Random.Range(0, mHoneycombList.Count)] : null;
    }

    //private void OnTouch(Gesture gesture)
    //{
    //    if (gesture.pickedObject == null)
    //        return;

    //    //Mng.canvas.kResource.SetText("??");

    //    /*
    //    if( kHive.kQueenBee.gameObject == gesture.pickedObject )
    //    {
    //        PlayerCamera.Instance.SetFollow(kHive.kQueenBee.transform);
    //    }
    //    */

    //    Debug.Log(gesture.pickedObject.name);
    //}

    private Vector3 GetHexagonPos(Vector3 _pos, HoneycombDirection _dir)
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

        Vector3 newPos = new Vector3(transform.position.x, transform.position.y, 0.05f);

        for(int i = 0; i < 6; i++)
        {
            for(int j = 0; j < 10; j++)
            {
                var comb = AddNewHoneycomb();
                SetHoneycombPosition(comb, newPos);

                comb.amount = new GameResAmount(0f, GameResUnit.Microgram);

                if(i % 2 == 0)
                {
                    newPos = GetHexagonPos(newPos, HoneycombDirection.Right);
                }
                else if(i % 2 == 1)
                {
                    newPos = GetHexagonPos(newPos, HoneycombDirection.Left);
                }

                comb.InitDefault();
            }

            newPos = GetHexagonPos(newPos, HoneycombDirection.TopRight);
        }

        mMaxItemAmounts = new GameResAmount[4];

        mMaxItemAmounts[0] = new GameResAmount(10, GameResUnit.Milligram);
        mMaxItemAmounts[1] = new GameResAmount(10, GameResUnit.Milligram);
        mMaxItemAmounts[2] = new GameResAmount(500, GameResUnit.Milligram);
        mMaxItemAmounts[3] = new GameResAmount(10, GameResUnit.Milligram);

        mQueenHoneyNeed = new GameResAmount(1, GameResUnit.Milligram);
        mQueenPollenNeed = new GameResAmount(100, GameResUnit.Milligram);
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
        print(_pos);
        float minDistance = mHoneycombRadiusY;
        Honeycomb retHoneycomb = null;

        foreach(Honeycomb honeycomb in mHoneycombList)
        {
            float distance = Mathf.Sqrt(Mathf.Pow((honeycomb.pos.x - _pos.x),2) + Mathf.Pow((honeycomb.pos.y - _pos.y),2));

            if(distance < minDistance)
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

        Mng.canvas.EnableToggleButtons();
        kHoverObj.SetActive(false);
        Mng.canvas.HideBuildCancel();
        Mng.canvas.kInven.gameObject.SetActive(true);
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
        while (mIsBuilding == true)
        {
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

			kHoverObjSpriteRenderer.color = hoverEnabled ? Color.white : new Color(0.5f,0.5f,0.5f,0.5f);

			yield return null;
        }
        
        EndBuild();
    }

    public void GuideQueen(QueenBee _queen)
    {
        mGuidingQueen = true;
        mTargetQueen = _queen;
        Mng.canvas.DisableToggleButtons();
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
            var comb = AddNewHoneycomb();
            comb.ImportFrom(savedata.mHoneycombList[i]);
            SetHoneycombPosition(comb, comb.pos);
        }

		mQueenHoneyNeed = savedata.mQueenHoneyNeed;
		mQueenPollenNeed = savedata.mQueenPollenNeed;
	}
}
