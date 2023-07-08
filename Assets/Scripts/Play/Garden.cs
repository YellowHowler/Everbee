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
    public GameObject BoundaryObject;

    List<FlowerSpot> mFlowerSpotList = new List<FlowerSpot>();

    private float flowerY = 0.4f; // �ɵ��� y ��ǥ��

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
}
