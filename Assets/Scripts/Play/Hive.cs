using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnumDef;
using ClassDef;
using StructDef;
//using HedgehogTeam.EasyTouch;
using UnityEditor.PackageManager;

public class Hive : MonoBehaviour
{
    public static Hive Instance;

    public float mHoneycombRadiusX = 0.5f;
    public float mHoneycombRadiusY = 0.8f;
    public float mHoneycombDistance_X_Horizontal = 2;
    public float mHoneycombDistance_X_Other = 1.5f;

    private Vector3 mHoneycombOrigin;

    public float mFloorY = -19.2f;

    public GameObject kHoneycombObj;
    public GameObject kHoverObj;
    public GameObject kItemObj;

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

    public GameResAmount mQueenHoneyNeed{get; set;}
    public GameResAmount mQueenPollenNeed{get; set;}

    [HideInInspector] public bool mGuidingQueen  = false;
    [HideInInspector] public QueenBee mTargetQueen  = null;

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

    public void GetAllHoneycombs() //현재 씬에 있는 모든 Honeycomb 다 가져온다
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
    }

    public void AddNewHoneycomb(Vector3 _pos, GameResAmount _amount)
    {
        GameObject newHoneycomb = Instantiate(kHoneycombObj, _pos, Quaternion.identity, transform.GetChild(0));
        Honeycomb honeycomb = newHoneycomb.GetComponent<Honeycomb>();
        honeycomb.mHive = this;

        mHoneycombList.Add(honeycomb);

        PlayManager.Instance.kHiveXBound.start = Mathf.Min(_pos.x - 0.5f, PlayManager.Instance.kHiveXBound.start);
        PlayManager.Instance.kHiveXBound.end = Mathf.Max(_pos.x + 0.5f, PlayManager.Instance.kHiveXBound.end);

        PlayManager.Instance.kHiveYBound.start = Mathf.Min(_pos.y - 0.5f, PlayManager.Instance.kHiveYBound.start);
        PlayManager.Instance.kHiveYBound.end = Mathf.Max(_pos.y + 0.5f, PlayManager.Instance.kHiveYBound.end);
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

        //EasyTouch.On_TouchStart += OnTouch;

        Vector3 newPos = new Vector3(transform.position.x, transform.position.y, 0.05f);

        for(int i = 0; i < 6; i++)
        {
            for(int j = 0; j < 10; j++)
            {
                AddNewHoneycomb(newPos, new GameResAmount(0f, GameResUnit.Microgram));

                if(i % 2 == 0)
                {
                    newPos = GetHexagonPos(newPos, HoneycombDirection.Right);
                }
                else if(i % 2 == 1)
                {
                    newPos = GetHexagonPos(newPos, HoneycombDirection.Left);
                }
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

    private void Awake()
    {
        Instance = this;
        mHoneycombOrigin = transform.position;
    }

    private void Update()
    {
        
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
    }

    public void SetDrawBuild(StructureType _type)
    {
        Mng.canvas.kIsViewingMenu = false;
        mIsBuilding = true;
        mStructureType = _type;

        Mng.canvas.DisableToggleButtons();
        Mng.canvas.ShowBuildCancel();

        kHoverObj.GetComponent<SpriteRenderer>().sprite = kBuildSprites[(int)_type - 1];
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
}
