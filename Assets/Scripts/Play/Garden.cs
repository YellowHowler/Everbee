using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnumDef;
using StructDef;
using ClassDef;

public class Garden : MonoBehaviour
{
    [HideInInspector] public static Garden Instance { get; private set; }

    public GameObject[] flowerObjs;
    public GameObject BoundaryObject;

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

	private void OnDestroy()
	{
		Instance = null;
	}

	public void InitDefault()
    {
        AddNewFlower(FlowerType.Cosmos, new Vector3(0, 0.4f, 0));
        AddNewFlower(FlowerType.Lavender, new Vector3(2f, 0.4f, 0));
        AddNewFlower(FlowerType.OxeyeDaisy, new Vector3(3.7f, 0.4f, 0));
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
	public class CSaveData
	{
        public List<string> mFlowers = new List<string>();
		public List<FlowerSpot.CSaveData> mFlowerSpotList = new List<FlowerSpot.CSaveData>();
	}

	public void ExportTo(CSaveData savedata)
	{
        /*savedata.mFlowers.Clear();
        for(int i=0; i<flo)

		savedata.mFlowerSpotList.Clear();

		for(int i=0; i<mFlowerSpotList.Count; ++i)
		{
			FlowerSpot.CSaveData spotdata = new FlowerSpot.CSaveData();
			mFlowerSpotList[i].ExportTo(spotdata);
			savedata.mFlowerSpotList.Add(spotdata);
		}*/
	}

	public void ImportFrom(CSaveData savedata)
	{
		mFlowerSpotList.Clear();

		/*for(int i=0; i<savedata.mFlowerSpotList.Count; ++i)
		{
            AddNewFlower()
			var comb = AddNewHoneycomb(Vector3.zero);
			comb.ImportFrom(savedata.mHoneycombList[i]);
		}

		if(savedata.mMaxItemAmounts == null)
			mMaxItemAmounts = null;
		else
		{
			if((mMaxItemAmounts == null) || (mMaxItemAmounts.Length != savedata.mMaxItemAmounts.Length))
				mMaxItemAmounts = new GameResAmount[savedata.mMaxItemAmounts.Length];
		}

		for(int i = 0;i<savedata.mMaxItemAmounts.Length;++i)
			mMaxItemAmounts[i] = savedata.mMaxItemAmounts[i];
        */

        InitDefault();  // 임시
	}
}
