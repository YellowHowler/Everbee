using System;
using System.Collections;
using UnityEngine;

public class Flower:MonoBehaviour
{
	[HideInInspector] public Garden mGarden;
	public string FlowerName;
	public FlowerSpot[] mFlowerSpots;
	public float XPosition { get { return transform.localPosition.x; } set { transform.localPosition = new Vector3(value, 0, 0); } }


	// 세이브/로드 관련
	[Serializable]
	public class CSaveData
	{
		public string FlowerName;
		public float XPosition;

		public FlowerSpot.CSaveData[] FlowerSpots;
	}

	public void ExportTo(CSaveData savedata)
	{
		savedata.FlowerName = FlowerName;
		savedata.XPosition = XPosition;

		if (mFlowerSpots == null)
			savedata.FlowerSpots = null;
		else
		{
			if ( (savedata.FlowerSpots == null) || (savedata.FlowerSpots.Length != mFlowerSpots.Length) )
				savedata.FlowerSpots = new FlowerSpot.CSaveData[mFlowerSpots.Length];

			for(int i=0; i<mFlowerSpots.Length; ++i)
			{
				var spot = new FlowerSpot.CSaveData();
				mFlowerSpots[i].ExportTo(spot);
				savedata.FlowerSpots[i] = spot;
			}
		}
	}

	public void ImportFrom(CSaveData savedata)
	{
		FlowerName = savedata.FlowerName;
		XPosition = savedata.XPosition;

		// mFlowerSpots 는 고정되어 있는 것이기 때문에 건들이지 않는다.
		if ( (savedata.FlowerSpots != null) && (mFlowerSpots != null) && (savedata.FlowerSpots.Length == mFlowerSpots.Length) )
		{
			for(int i=0; i<savedata.FlowerSpots.Length; ++i)
				mFlowerSpots[i].ImportFrom(savedata.FlowerSpots[i]);
		}
	}
}