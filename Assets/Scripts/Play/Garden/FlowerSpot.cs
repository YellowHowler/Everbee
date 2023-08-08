using EnumDef;
using StructDef;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowerSpot : MonoBehaviour
{
    [HideInInspector] public Garden mGarden;

    public Vector3 pos;
	public CTargetLink<FlowerSpot, Bee> mTargetBee;

    public bool occupied;

    public float nectar;
    public GameResUnit nectarUnit;
    public float pollen;
    public GameResUnit pollenUnit;

    public GameResAmount nectarAmount;
    public GameResAmount pollenAmount;

    private void Awake()
    {
		mTargetBee = new CTargetLink<FlowerSpot, Bee>(this);

        nectarAmount = new GameResAmount(nectar, nectarUnit);
        pollenAmount = new GameResAmount(pollen, pollenUnit);

        pos = transform.position;
    }


	// 세이브/로드 관련
	[Serializable]
	public class CSaveData
	{
		public Vector3 pos;

		public bool occupied;

		public float nectar;
		public GameResUnit nectarUnit;
		public float pollen;
		public GameResUnit pollenUnit;

		public GameResAmount nectarAmount;
		public GameResAmount pollenAmount;
	}

	public void ExportTo(CSaveData savedata)
	{
		savedata.pos = pos;

		savedata.nectar = nectar;
		savedata.nectarUnit = nectarUnit;
		savedata.pollen = pollen;
		savedata.pollenUnit = pollenUnit;

		savedata.nectarAmount = nectarAmount;
		savedata.pollenAmount = pollenAmount;
	}

	public void ImportFrom(CSaveData savedata)
	{
		pos = savedata.pos;

		nectar = savedata.nectar;
		nectarUnit = savedata.nectarUnit;
		pollen = savedata.pollen;
		pollenUnit = savedata.pollenUnit;

		nectarAmount = savedata.nectarAmount;
		pollenAmount = savedata.pollenAmount;
	}
}
