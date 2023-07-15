using System;
using System.Collections;
using UnityEngine;

public class Flower:MonoBehaviour
{
	[HideInInspector] public Garden mGarden;
	public string FlowerName;
	public FlowerSpot[] m_FlowerSpots;
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

		if (m_FlowerSpots == null)
			savedata.FlowerSpots = null;
		else
		{
			if ( (savedata.FlowerSpots == null) || (savedata.FlowerSpots.Length != m_FlowerSpots.Length) )
				savedata.FlowerSpots = new FlowerSpot.CSaveData[m_FlowerSpots.Length];

			for(int i=0; i<m_FlowerSpots.Length; ++i)
			{
				var spot = new FlowerSpot.CSaveData();
				m_FlowerSpots[i].ExportTo(spot);
				savedata.FlowerSpots[i] = spot;
			}
		}
	}

	public void ImportFrom(CSaveData savedata)
	{
		FlowerName = savedata.FlowerName;
		XPosition = savedata.XPosition;

		// m_FlowerSpots 는 고정되어 있는 것이기 때문에 건들이지 않는다.
		if ( (savedata.FlowerSpots != null) && (m_FlowerSpots != null) && (savedata.FlowerSpots.Length == m_FlowerSpots.Length) )
		{
			for(int i=0; i<savedata.FlowerSpots.Length; ++i)
				m_FlowerSpots[i].ImportFrom(savedata.FlowerSpots[i]);
		}
	}
}