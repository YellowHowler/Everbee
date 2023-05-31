using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnumDef;
using StructDef;
using ClassDef;

public class Garden : MonoBehaviour
{
    List<FlowerSpot> mFlowerSpotList = new List<FlowerSpot>();

    public FlowerSpot GetUsableFlowerSpot()
    {
        foreach(FlowerSpot s in mFlowerSpotList)
        {
            if(s.occupied == false && s.isTarget == false) return s;
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
