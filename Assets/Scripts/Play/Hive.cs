using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnumDef;
using ClassDef;
using StructDef;
using HedgehogTeam.EasyTouch;

public class Hive : MonoBehaviour
{
    public GameObject kHoneycombObj;

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
            if ((h.type == GameResType.Empty || (h.type == _type && h.IsFull() == false)) && h.isTarget == false)
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

            /*
            PlayManager.Instance.kHiveXBound.start = Mathf.Min(honeycomb.pos.x - 0.5f, PlayManager.Instance.kHiveXBound.start);
            PlayManager.Instance.kHiveXBound.end = Mathf.Max(honeycomb.pos.x + 0.5f, PlayManager.Instance.kHiveXBound.end);

            PlayManager.Instance.kHiveYBound.start = Mathf.Min(honeycomb.pos.y - 0.5f, PlayManager.Instance.kHiveYBound.start);
            PlayManager.Instance.kHiveYBound.end = Mathf.Max(honeycomb.pos.y + 0.5f, PlayManager.Instance.kHiveYBound.end);
            */
        }
    }

    public void AddNewHoneycomb(Vector3 _pos, GameResAmount _amount)
    {
        GameObject newHoneycomb = Instantiate(kHoneycombObj, _pos, Quaternion.identity, transform);
        Honeycomb honeycomb = newHoneycomb.GetComponent<Honeycomb>();

        mHoneycombList.Add(honeycomb);

        PlayManager.Instance.kHiveXBound.start = Mathf.Min(honeycomb.pos.x - 0.5f, PlayManager.Instance.kHiveXBound.start);
        PlayManager.Instance.kHiveXBound.end = Mathf.Max(honeycomb.pos.x + 0.5f, PlayManager.Instance.kHiveXBound.end);
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
        EasyTouch.On_TouchStart += OnTouch;
    }

    private void Awake()
    {
        GetAllHoneycombs();
    }

    private void Update()
    {
        
    }
}
