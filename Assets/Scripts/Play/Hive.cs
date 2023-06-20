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

    [HideInInspector] public float mHonecombRadius = 0.8f;
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

    private void Start()
    {
        kHoverObj.SetActive(false);

        EasyTouch.On_TouchStart += OnTouch;

        AddNewHoneycomb(transform.position, new GameResAmount(0f, GameResUnit.Microgram));
    }

    private void Awake()
    {
        Instance = this;
        mHoneycombOrigin = transform.position;
    }

    private void Update()
    {
        
    }

    public void HexToWorldPos(Vector2 _hex)
    {

    }

    public void SetDrawBuild(StructureType _type)
    {
        kHoverObj.SetActive(true);

        StartCoroutine(SetBuildCor());
    }

    public IEnumerator SetBuildCor()
    {
        while (true)
        {
            /*
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            Vector2 touchPos = new Vector2(pos.x, pos.y);
            Ray2D ray = new Ray2D(touchPos, Vector2.zero);

            RaycastHit2D rayHit = Physics2D.Raycast(ray.origin, ray.origin);

            if (rayHit.collider == null)
            {
                kHoverObj.transform.position = new Vector3(pos.x, pos.y, 0);                
            }
            else
            {
                kHoverObj.transform.position = new Vector3(rayHit.point.x, rayHit.point.y, 0);
            }            

            if(Input.GetMouseButtonDown(1))
            {
                kHoverObj.SetActive(false);
                yield break;
            }
            */

            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 touchPos = new Vector2(pos.x, pos.y);

            int xPos = Mathf.RoundToInt((touchPos.x - mHoneycombOrigin.x) / (mHonecombRadius));
            int yPos = Mathf.RoundToInt((touchPos.y - mHoneycombOrigin.y) / (mHonecombRadius * 2));

            if (yPos % 2 == 0)
            {
                //xPos = xPos
            }

            yield return null;
        }
    }
}
