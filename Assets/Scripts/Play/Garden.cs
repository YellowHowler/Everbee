using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnumDef;
using StructDef;
using ClassDef;

public class Garden : MonoBehaviour
{
    List<FlowerSpot> mFlowerSpotList = new List<FlowerSpot>();

    public FlowerSpot GetEmptyFlowerSpot()
    {
        foreach(FlowerSpot s in mFlowerSpotList)
        {
            if(!s.occupied) return s;
        }

        return null;
    }

    void Awake()
    {
        //Flower Create, List Insert
    }

    void Update()
    {
        
    }
}
