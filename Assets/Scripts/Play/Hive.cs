using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnumDef;
using ClassDef;
using StructDef;

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
            mHoneycombList.Add(h.GetComponent<Honeycomb>()); //임시
            h.GetComponent<Honeycomb>().mHive = this;
        }
    }

    public void AddNewHoneycomb(Vector3 _pos, GameResAmount _amount)
    {
        GameObject newHoneycomb = Instantiate(kHoneycombObj, _pos, Quaternion.identity, transform);
        mHoneycombList.Add(newHoneycomb.GetComponent<Honeycomb>());
    }

    private void Start()
    {
        
    }

    private void Awake()
    {
        GetAllHoneycombs();
    }

    private void Update()
    {
        
    }
}
