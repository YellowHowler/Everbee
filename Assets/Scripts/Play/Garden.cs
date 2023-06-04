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

    /// <summary> ���� ���� �ִ� ��� Flower Spot���� �����´� </summary>
    public void GetAllFlowerSpots() 
    {
        mFlowerSpotList.Clear();

        GameObject[] flowerSpots = GameObject.FindGameObjectsWithTag("FlowerSpot");

        foreach(GameObject f in flowerSpots)
        {
            mFlowerSpotList.Add(f.GetComponent<FlowerSpot>()); //�ӽ�
        }
    }

    void Awake()
    {
        //Flower Create, List Insert
        GetAllFlowerSpots();
        print(mFlowerSpotList.Count);
    }

    void Update()
    {
        
    }
}
