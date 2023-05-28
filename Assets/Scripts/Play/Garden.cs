using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnumDef;
using StructDef;

public class Garden : MonoBehaviour
{
    List<FlowerSpot> mFlowerSpotList = new List<FlowerSpot>();

    public FlowerSpot GetEmptyFlowerSpot()
    {
        foreach(FlowerSpot s in mFlowerSpotList)
        {
            if(!s.occupied) return s;
        }

        return new FlowerSpot(-1, Vector3.zero, FlowerType.cosmos);
    }

    void Awake()
    {
        //Flower Create, List Insert
    }

    void Update()
    {
        
    }
}
