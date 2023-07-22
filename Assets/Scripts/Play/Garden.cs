using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnumDef;
using StructDef;
using ClassDef;

public class Garden : MonoBehaviour
{
    public static Garden Instance { get; private set; }

    public List<Flower> mFlowerTemplates;   // 이것을 가지고 생성함
	public GameObject BoundaryObject;

	private List<Flower> mFlowers = new List<Flower>();
    private List<FlowerSpot> mFlowerSpotList = new List<FlowerSpot>();
    private float flowerY = 0.4f; // 꽃들의 y 좌표값

    public FlowerSpot GetUsableFlowerSpot()
    {
        int offset = UnityEngine.Random.Range(0, mFlowerSpotList.Count);
        for(int i=0; i<mFlowerSpotList.Count; ++i)
        {
            int index = (i + offset) % mFlowerSpotList.Count;
            var spot = mFlowerSpotList[index];
            if (spot.occupied == false && !spot.mTargetBee.IsLinked()) 
                return spot;
        }

        return null;
    }

    /// <summary> 현재 씬에 있는 모든 Flower Spot들을 가져온다 </summary>
    public void GetAllFlowerSpots() 
    {
        mFlowerSpotList.Clear();

        foreach(var flower in mFlowers)
            foreach(var spot in flower.m_FlowerSpots)
                mFlowerSpotList.Add(spot);
    }

    public Flower AddNewFlower(Flower _flower, float x)
    {
        var newFlower = Instantiate(_flower.gameObject, new Vector3(x, flowerY, 0), Quaternion.identity, transform.GetChild(0)).GetComponent<Flower>();

        newFlower.mGarden = this;
        foreach(var spot in newFlower.m_FlowerSpots)
            spot.mGarden = this;

        mFlowers.Add(newFlower);

        return newFlower;
    }

    private void Awake()
    {
        Instance = this;
    }

	private void OnDestroy()
	{
		Instance = null;
	}

	public void InitDefault()
    {
        AddNewFlower(mFlowerTemplates[0], 0);
        AddNewFlower(mFlowerTemplates[1], 2);
        AddNewFlower(mFlowerTemplates[2], 3.7f);

        GetAllFlowerSpots();
    }

    private void Update()
    {
        
    }

	public Rect ComputeWorldBoundary(Rect rect)
	{
        var boundaryPos = BoundaryObject.transform.position;
        var boundaryScale = BoundaryObject.transform.localScale;

		rect.xMin = Mathf.Min(rect.xMin, boundaryPos.x - boundaryScale.x / 2);
		rect.xMax = Mathf.Max(rect.xMax, boundaryPos.x + boundaryScale.x / 2);
		rect.yMin = Mathf.Min(rect.yMin, boundaryPos.y - boundaryScale.y / 2);
		rect.yMax = Mathf.Max(rect.yMax, boundaryPos.y + boundaryScale.y / 2);

		return rect;
	}


    // 세이브/로드 관련
    [Serializable]
	public class CSaveData
	{
        public List<Flower.CSaveData> Flowers = new List<Flower.CSaveData>();
	}

	public void ExportTo(CSaveData savedata)
	{
        savedata.Flowers.Clear();

        for(int i=0; i<mFlowers.Count; ++i)
        {
            var flowersavedata = new Flower.CSaveData();
            mFlowers[i].ExportTo(flowersavedata);
            savedata.Flowers.Add(flowersavedata);
        }
	}

	public void ImportFrom(CSaveData savedata)
	{
		mFlowers.Clear();

        for(int i=0; i<savedata.Flowers.Count; ++i)
        {
            var flowersavedata = savedata.Flowers[i];

            foreach(var templ in mFlowerTemplates)
            {
                if (templ.FlowerName == flowersavedata.FlowerName)
                {
                    var flower = AddNewFlower(templ, flowersavedata.XPosition);
                    flower.ImportFrom(flowersavedata);
                    break;
                }
            }
        }

        GetAllFlowerSpots();
	}
}
