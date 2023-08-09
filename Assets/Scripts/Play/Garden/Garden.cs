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

	private List<Flower> mFlowers = new List<Flower>();
    private List<FlowerSpot> mFlowerSpotList = new List<FlowerSpot>();
    private float flowerY = 0.4f; // 꽃들의 y 좌표값

    public GameObject kBackground;

    public GameObject kGrassBG;
    private float grassY = 0f;
    private float grassDist = 1.36f;

    [HideInInspector] public Flower mHoveredFlower;

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
            foreach(var spot in flower.mFlowerSpots)
                mFlowerSpotList.Add(spot);
    }

    public Flower AddNewFlower(Flower _flower, float _xPos, bool _isImport)
    {
        var newFlower = Instantiate(_flower.gameObject, new Vector3(_xPos, flowerY, 0.01f), Quaternion.identity, transform.GetChild(0)).GetComponent<Flower>();

        newFlower.mFlowerSpots = newFlower.GetComponentsInChildren<FlowerSpot>();

        newFlower.mGarden = this;
        foreach(var spot in newFlower.mFlowerSpots)
            spot.mGarden = this;

        mFlowers.Add(newFlower);

        if(_isImport == false)
        {
            SetFlowerPosition(newFlower, _xPos);
        }

        return newFlower;
    }

    public void SetFlowerPosition(Flower _flower, float _xPos)
    {
        _flower.XPosition = _xPos;
        _flower.gameObject.transform.position = new Vector3(_xPos, flowerY, 0.01f);

        PlayManager.Instance.kGardenXBound.start = Mathf.Min(_xPos - 3f,PlayManager.Instance.kGardenXBound.start);
		PlayManager.Instance.kGardenXBound.end = Mathf.Max(_xPos + 3f,PlayManager.Instance.kGardenXBound.end);

		PlayManager.Instance.kGardenYBound.start = flowerY + 0.6f;
		PlayManager.Instance.kGardenYBound.end = flowerY + 2;

        Mng.play.UpdateBackground();
    }

    public void SetGrassBG(float _xMin, float _xMax)
    {
        foreach (Transform child in kBackground.transform) 
        {
			if(child.gameObject.tag == "GrassBG") 
            {
                Destroy(child.gameObject);
            }
		}

        for(float x = _xMin - 5; x <= _xMax + 5; x+=grassDist)
        {
            Instantiate(kGrassBG, new Vector3(x, grassY, 0), Quaternion.identity, kBackground.transform);
        }
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
        AddNewFlower(mFlowerTemplates[0], 0, false);
        AddNewFlower(mFlowerTemplates[1], 2, false);
        AddNewFlower(mFlowerTemplates[2], 3.7f, false);

        GetAllFlowerSpots();
    }

    private void Update()
    {
        
    }

	public Rect ComputeWorldBoundary(Rect rect)
	{
		rect.xMin = Mathf.Min(rect.xMin, PlayManager.Instance.kGardenXBound.start);
		rect.xMax = Mathf.Max(rect.xMax, PlayManager.Instance.kGardenXBound.end);
		rect.yMin = Mathf.Min(rect.yMin, PlayManager.Instance.kGardenYBound.start);
		rect.yMax = Mathf.Max(rect.yMax, PlayManager.Instance.kGardenYBound.end);

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
                    var flower = AddNewFlower(templ, flowersavedata.XPosition, true);
                    flower.ImportFrom(flowersavedata);
                    SetFlowerPosition(flower, flower.XPosition);
                    break;
                }
            }
        }

        GetAllFlowerSpots();
	}
}
