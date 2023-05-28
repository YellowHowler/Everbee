using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnumDef;
using StructDef;

public class Hive : MonoBehaviour
{
    public QueenBee kQueenBee;

    private List<Honeycomb> mHoneycombList = new List<Honeycomb>();
    private List<Bee> mBeeList = new List<Bee>();

    public bool isFull(Honeycomb _honeycomb) //그 셀이 꽉 차있는지 확인
    {
        if(_honeycomb.unit != _honeycomb.maxUnit) return (int)_honeycomb.unit < (int)_honeycomb.maxUnit;
        else return (int)_honeycomb.amount < (int)_honeycomb.maxAmount;
    }

    public Honeycomb GetEmptyHoneycomb(Resource _type) //벌이 자원을 어디에 저장해야 하는지
    {
        foreach (Honeycomb h in mHoneycombList)
        {
            if(h.type == Resource.empty || (h.type == _type && !isFull(h))) return h;
        }

        return new Honeycomb(-1, Vector3.zero, 0f, Unit.microgram); //null이 안됨
    }

    public void GetAllHoneycombs() //현재 씬에 있는 모든 Honeycomb 다 가져온다
    {
        mHoneycombList.Clear();

        Transform[] honeycombTransforms = transform.GetChild(0).GetComponentsInChildren<Transform>();

        for(int i = 0; i < honeycombTransforms.Length; i++)
        {
            Transform child = honeycombTransforms[i];
            mHoneycombList.Add(new Honeycomb(i, child.position, 5f, Unit.milligram)); //임시
        }
    }

    public void AddNewHoneycomb(Vector3 _pos, float _maxAmount, Unit _maxUnit)
    {
        mHoneycombList.Add(new Honeycomb(mHoneycombList.Count, _pos, _maxAmount, _maxUnit));
    }

    private void Start()
    {
        GetAllHoneycombs();
    }

    private void Update()
    {
        
    }
}
