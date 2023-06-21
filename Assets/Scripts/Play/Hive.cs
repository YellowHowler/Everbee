using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnumDef;
using ClassDef;
using StructDef;
using HedgehogTeam.EasyTouch;
using UnityEditor.PackageManager;

public class Hive : MonoBehaviour
{
    public static Hive Instance;

    [HideInInspector] public float mHoneycombRadiusX = 0.75f;
    [HideInInspector] public float mHoneycombRadiusY = 0.7f;
    private Vector3 mHoneycombOrigin;

    public GameObject kHoneycombObj;
    public GameObject kHoverObj;

    public QueenBee kQueenBee;

    public Sprite[] kHoneycombNectarSprites;
    public Sprite[] kHoneycombPollenSprites;
    public Sprite[] kHoneycombHoneySprites;
    public Sprite[] kHoneycombWaxSprites;

    private List<Honeycomb> mHoneycombList = new List<Honeycomb>();
    private List<Bee> mBeeList = new List<Bee>();

    [Header("Honeycomb 이름 보기")]
    public bool kIsDrawHoneycombName = true;
    [Header("Honeycomb 모양 보기")]
    public bool kIsDrawHoneycombShape = true;

    [HideInInspector] public bool mIsBuilding = false;
    [HideInInspector] public BuildType mBuildType = BuildType.None;

    /// <summary> 벌이 자원을 어디에 저장해야 하는지 </summary>
    public Honeycomb GetUsableHoneycomb(GameResType _type)
    {
        foreach (Honeycomb h in mHoneycombList)
        {
            if (h.kStructType == StructureType.Storage && (h.type == GameResType.Empty || (h.type == _type && h.IsFull() == false)) && h.isTarget == false)
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

        Debug.Log(gesture.pickedObject.name);
    }

    private Vector3 GetHexagonPos(Vector3 _pos, HoneycombDirection _dir)
    {
        switch(_dir)
        {
            case HoneycombDirection.None:
                return _pos;
            case HoneycombDirection.TopLeft:
                return _pos + Vector3.left * mHoneycombRadiusX + Vector3.up * Mathf.Sqrt(3) * mHoneycombRadiusY;
            case HoneycombDirection.TopRight:
                return _pos + Vector3.right * mHoneycombRadiusX + Vector3.up * Mathf.Sqrt(3) * mHoneycombRadiusY;
            case HoneycombDirection.Left:
                return _pos + Vector3.left * mHoneycombRadiusX * 2;
            case HoneycombDirection.Right:
                return _pos + Vector3.right * mHoneycombRadiusX * 2;
            case HoneycombDirection.BottomLeft:
                return _pos + Vector3.left * mHoneycombRadiusX + Vector3.down * Mathf.Sqrt(3) * mHoneycombRadiusY;
            case HoneycombDirection.BottomRight:
                return _pos + Vector3.right * mHoneycombRadiusX + Vector3.down * Mathf.Sqrt(3) * mHoneycombRadiusY;
        }

        return _pos;
    }

    private void Start()
    {
        kHoverObj.SetActive(false);

        EasyTouch.On_TouchStart += OnTouch;

        Vector3 newPos = transform.position;

        for(int i = 0; i < 3; i++)
        {
            for(int j = 0; j < 5; j++)
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
    }

    private void Awake()
    {
        Instance = this;
        mHoneycombOrigin = transform.position;
    }

    private void Update()
    {
        
    }

    public void SetDrawBuild(StructureType _type)
    {
        mIsBuilding = true;
        mBuildType = _type;

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
            int yPos = Mng.play.RoundFloat((touchPos.y - mHoneycombOrigin.y) / (mHoneycombRadiusY * 2));

            if (Mng.play.GetMod(yPos, 2) == 1)
            {
                xPos = Mng.play.RoundFloat((touchPos.x - mHoneycombOrigin.x) / (mHoneycombRadiusX) / 2 - 0.5f) * 2 + 1;
            }
            else if (Mng.play.GetMod(yPos, 2) == 0)
            {
                xPos = Mng.play.RoundFloat((touchPos.x - mHoneycombOrigin.x) / (mHoneycombRadiusX) / 2) * 2;
            }

            kHoverObj.transform.localPosition = new Vector3(xPos * mHoneycombRadiusX, yPos * mHoneycombRadiusY * 2, transform.position.z);

            yield return null;
        }
    }
}
