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

        Transform ch = transform.GetChild(0);
        Transform[] honeycombTransforms = ch.GetComponentsInChildren<Transform>();

        for (int i = 0; i < honeycombTransforms.Length; i++)
        {
            Transform child = honeycombTransforms[i];
            mHoneycombList.Add(child.gameObject.GetComponent<Honeycomb>()); //임시
        }
    }

    public void AddNewHoneycomb(Vector3 _pos, GameResAmount _amount)
    {
        GameObject newHoneycomb = Instantiate(kHoneycombObj, _pos, Quaternion.identity, transform);
        mHoneycombList.Add(newHoneycomb.GetComponent<Honeycomb>());
    }

    private void Start()
    {
        GetAllHoneycombs();
    }

    private void Update()
    {
        
    }
}
