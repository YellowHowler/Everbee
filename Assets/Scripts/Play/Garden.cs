using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnumDef;
using StructDef;
using ClassDef;

public class Garden : MonoBehaviour
{
    [HideInInspector] public static Garden Instance;

    public GameObject[] flowerObjs;

    List<FlowerSpot> mFlowerSpotList = new List<FlowerSpot>();

    private float flowerY = 0.4f; // 꽃들의 y 좌표값

    public FlowerSpot GetUsableFlowerSpot()
    {
        foreach(FlowerSpot s in mFlowerSpotList)
        {
            if(s.occupied == false && s.isTarget == false) return s;
        }

        return null;
    }

    /// <summary> 현재 씬에 있는 모든 Flower Spot들을 가져온다 </summary>
    public void GetAllFlowerSpots() 
    {
        mFlowerSpotList.Clear();

        GameObject[] flowerSpots = GameObject.FindGameObjectsWithTag("FlowerSpot");

        foreach(GameObject f in flowerSpots)
        {
            mFlowerSpotList.Add(f.GetComponent<FlowerSpot>()); //임시
        }
    }

    public void AddNewFlower(FlowerType _flower, Vector3 _pos)
    {
        GameObject newFlower = Instantiate(flowerObjs[(int)_flower], _pos, Quaternion.identity, transform.GetChild(0));

        for (int i = 0; i < newFlower.transform.childCount; i++)
        {
            FlowerSpot flowerSpot = newFlower.transform.GetChild(i).GetComponent<FlowerSpot>();
            flowerSpot.mGarden = this;

            mFlowerSpotList.Add(flowerSpot);
        }
    }

    private void Awake()
    {
        Instance = this;

        //Flower Create, List Insert
        //GetAllFlowerSpots();
        print(mFlowerSpotList.Count);
    }

    private void Start()
    {
        AddNewFlower(FlowerType.Cosmos, new Vector3(0, 0.4f, 0));
        AddNewFlower(FlowerType.Lavender, new Vector3(2f, 0.4f, 0));
        AddNewFlower(FlowerType.OxeyeDaisy, new Vector3(3.7f, 0.4f, 0));
    }

    private void Update()
    {
        
    }
}
