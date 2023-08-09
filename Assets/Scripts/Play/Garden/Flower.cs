using EnumDef;
using StructDef;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using TMPro;

public class Flower:MonoBehaviour
{
	[HideInInspector] public Garden mGarden;
	public string FlowerName;
	public FlowerSpot[] mFlowerSpots;
	public float XPosition { get { return transform.localPosition.x; } set { transform.localPosition = new Vector3(value, 0, 0); } }

	private ParticleSystem mParticle;
	private SpriteOutline mOutline;

	private int mClickNum = 0;

	private void Start()
	{
		mParticle = GetComponentInChildren<ParticleSystem>();
		mParticle.Stop();

		mOutline = GetComponent<SpriteOutline>();
		mOutline.DisableOutline();
	}

	private void OnMouseDown()
	{
		if(mClickNum > 5)
		{
			return;
		}

		bool playParticle = Mng.play.kHive.FlowerClick(GameResType.Nectar, new GameResAmount(mFlowerSpots[0].nectarAmount.amount / 20, mFlowerSpots[0].nectarAmount.unit));
		playParticle = Mng.play.kHive.FlowerClick(GameResType.Pollen, new GameResAmount(mFlowerSpots[0].pollenAmount.amount / 20, mFlowerSpots[0].pollenAmount.unit)) || playParticle;
		
		if(playParticle)
		{
			PlayParticles();
		}
		else
		{
			Mng.canvas.DisplayWarning("No available storage space");
		}
	}

	private void OnMouseOver()
    {
        if(PopupBase.IsTherePopup() || Mng.play.kHive.mIsBuilding) 
        {
            return;
        }

        Mng.play.kGarden.mHoveredFlower = this;
        mOutline.EnableOutline();
    }

    private void OnMouseExit()
    {
        if(Mng.play.kGarden.mHoveredFlower == this)
        {
            Mng.play.kGarden.mHoveredFlower = null;
        }

        mOutline.DisableOutline();
    }

	public void PlayParticles()
    {
		StartCoroutine(PlayParticlesCor());
    }

	private IEnumerator PlayParticlesCor()
    {
		mClickNum++;
        mParticle.Play();
        yield return new WaitForSeconds(0.5f);
        mParticle.Stop();
		mClickNum--;
    }

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