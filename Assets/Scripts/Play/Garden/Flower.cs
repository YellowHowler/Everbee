using EnumDef;
using StructDef;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using TMPro;

public class Flower: MonoBehaviour
{
	[HideInInspector] public Garden mGarden;
	[HideInInspector] public Flower type;
	public string FlowerName;
	public FlowerSpot[] mFlowerSpots;

	public bool flipped;

	public FlowerStage stage;

	[HideInInspector] public long age = 0;
	[SerializeField] public long kSproutAge = 10;
	[SerializeField] public long kSeedlingAge = 20;
	[SerializeField] public long kAdultAge = 30;
	[SerializeField] public long kFlowerAge = 40;

	[HideInInspector] public long savedTime;
	[HideInInspector] public long curTime;

	[HideInInspector] public GameResAmount mPollenAmount = new GameResAmount(0f, GameResUnit.Microgram);
	[SerializeField] public GameResAmount kNeedPollenAmount;

	public Sprite[] kStageSprites;
	public Sprite[] kPollenSprites;
	[HideInInspector] public float XPosition { get { return transform.localPosition.x; } set { transform.localPosition = new Vector3(value, 0, 0); } }

	private SpriteRenderer mSpriteRenderer;
	private SpriteRenderer mPollenSpriteRenderer;
	private ParticleSystem mClickParticle;
	private ParticleSystem mPollenReceiveParticle;
	private SpriteOutline mOutline;

	private int mClickNum = 0;

	private void Awake()
	{
		mClickParticle = transform.Find("BurstParticle").GetComponent<ParticleSystem>();
		mPollenReceiveParticle = transform.Find("PollenReceiveParticle").GetComponent<ParticleSystem>();
		mFlowerSpots = GetComponentsInChildren<FlowerSpot>();
		mSpriteRenderer = GetComponent<SpriteRenderer>();
		mPollenSpriteRenderer = transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>();
		mClickParticle.Stop();

		mOutline = GetComponent<SpriteOutline>();
		mOutline.DisableOutline();
	}

	public void SetValues(Flower _type, Garden _garden, bool _flipped, GameResAmount _needPollenAmount)
	{
		type = _type;
		kNeedPollenAmount = _needPollenAmount;
		mGarden = _garden;
		flipped = _flipped;
	}

	public void SetValues(Flower _type, Garden _garden, bool _flipped, FlowerStage _stage, GameResAmount _needPollenAmount)
	{
		type = _type;
		stage = _stage;

		switch(stage)
		{
			case FlowerStage.Seed:
				age = 0;
				break;
			case FlowerStage.Sprout:
				age = kSproutAge;
				break;
			case FlowerStage.Seedling:
				age = kSeedlingAge;
				break;
			case FlowerStage.Adult:
				age = kAdultAge;
				break;
			case FlowerStage.Flower:
				age = kFlowerAge;
				break;
		}

		kNeedPollenAmount = _needPollenAmount;
		mGarden = _garden;
		flipped = _flipped;
	}

	public void CalculateGrowth(long secondsPassed) 
	{
		UpdateAge(age + secondsPassed);
	}

	public void InitDefault()
	{
		transform.localRotation = Quaternion.Euler(0, flipped ? 0 : 180, 0);

		UpdateStage(stage);
		mGarden.GetAllFlowerSpots();

		StartCoroutine(GrowCor());
	}

	public void UpdateSprite()
	{
		mSpriteRenderer.sprite = kStageSprites[(int)stage];
		print((int)stage);
		mPollenSpriteRenderer.sprite = kPollenSprites[(int)((Mng.play.GetResourcePercent(mPollenAmount, kNeedPollenAmount)/100) * (kPollenSprites.Length - 1))];
	}

	public void UpdateStage(FlowerStage _stage)
	{
		stage = _stage;
		UpdateSprite();
	}

	public void UpdateAge(long _age)
	{
		age = _age;

		if(_age >= kFlowerAge)
		{
			UpdateStage(FlowerStage.Flower);
		}
		else if(_age >= kAdultAge)
		{
			UpdateStage(FlowerStage.Adult);
		}
		else if(_age >= kSeedlingAge)
		{
			UpdateStage(FlowerStage.Seedling);
		}
		else if(_age >= kSproutAge)
		{
			UpdateStage(FlowerStage.Sprout);
		}
	}

	public GameResAmount UpdatePollenAmount(GameResAmount _pollenAmount)
	{
		mPollenAmount = _pollenAmount;

		GameResAmount retAmount = new GameResAmount(0f, GameResUnit.Microgram);

		if(Mng.play.CompareResourceAmounts(kNeedPollenAmount, mPollenAmount))
		{
			retAmount = Mng.play.SubtractResourceAmounts(mPollenAmount, kNeedPollenAmount);
			mPollenAmount = new GameResAmount(0f, GameResUnit.Microgram);

			mGarden.AddFlowerInRandomPos(type, FlowerStage.Sprout, transform.position.x);
		}
		else
		{
			mPollenAmount = _pollenAmount;	
		}

		UpdateSprite();

		return retAmount;
	}

	public GameResAmount AddPollenAmount(GameResAmount _pollenAmount, bool _playParticle)
	{
		if(_playParticle)
		{
			StartCoroutine(PlayParticlesCor(mPollenReceiveParticle, 0.5f));
		}

		return UpdatePollenAmount(Mng.play.AddResourceAmounts(_pollenAmount, mPollenAmount));
	}

	private void OnMouseDown()
	{
		if(stage != FlowerStage.Flower)
		{
			return;
		}
		
		if(mClickNum > 5)
		{
			return;
		}

		if(PopupBase.IsTherePopup() || Mng.play.kHive.mIsBuilding == true || Mng.play.kHive.mIsPlacingItem == true)
        {
            return;
        }

		bool playParticle = Mng.play.kHive.FlowerClick(GameResType.Nectar, new GameResAmount(mFlowerSpots[0].nectarAmount.amount / 20, mFlowerSpots[0].nectarAmount.unit));
		playParticle = Mng.play.kHive.FlowerClick(GameResType.Pollen, new GameResAmount(mFlowerSpots[0].pollenAmount.amount / 20, mFlowerSpots[0].pollenAmount.unit)) || playParticle;
		
		if(playParticle)
		{
			PlayParticles(mClickParticle, 0.5f);
		}
		else
		{
			Mng.canvas.DisplayWarning("No available storage space");
		}
	}

	private void OnMouseOver()
    {
        if(PopupBase.IsTherePopup() || Mng.play.kHive.mIsBuilding || (stage != FlowerStage.Flower && Mng.play.kHive.mIsPlacingItem == false) )
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

	public void PlayParticles(ParticleSystem _ps, float _duration)
    {
		StartCoroutine(PlayParticlesCor(_ps, _duration));
    }

	private IEnumerator PlayParticlesCor(ParticleSystem _ps, float _duration)
    {
		if(_ps == mClickParticle) 
		{
			mClickNum++;	
		}

        _ps.Play();
        yield return new WaitForSeconds(_duration);
        _ps.Stop();

		if(_ps == mClickParticle) 
		{
			mClickNum--;	
		}
    }

	private IEnumerator GrowCor()
	{
		WaitForSeconds sec = new WaitForSeconds(1);

		while(age < 1000000)
		{
			yield return sec;
			CalculateGrowth(1);
		}
	}

	// 세이브/로드 관련
	[Serializable]

	public class CSaveData
	{
		public Flower type;
		public string FlowerName;
		public float XPosition;
		public bool flipped;

		public FlowerStage stage;
		public long age;

		public long savedTime;

		public GameResAmount mPollenAmount;
		public GameResAmount kNeedPollenAmount;

		public FlowerSpot.CSaveData[] FlowerSpots;
	}

	public void ExportTo(CSaveData savedata)
	{
		savedata.type = type;
		savedata.FlowerName = FlowerName;
		savedata.XPosition = XPosition;
		savedata.flipped = flipped;

		savedata.stage = stage;
		savedata.age = age;

		savedata.savedTime = savedTime;

		savedata.mPollenAmount = mPollenAmount;
		savedata.kNeedPollenAmount = kNeedPollenAmount;

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
		type = savedata.type;
		FlowerName = savedata.FlowerName;
		XPosition = savedata.XPosition;
		flipped = savedata.flipped;

		UpdateStage(savedata.stage);

		savedTime = savedata.savedTime;
		curTime = (long)(DateTime.Now.Ticks/10000000);

		age = savedata.age;
		CalculateGrowth(curTime - savedTime);

		kNeedPollenAmount = savedata.kNeedPollenAmount;
		UpdatePollenAmount(savedata.mPollenAmount);

		// mFlowerSpots 는 고정되어 있는 것이기 때문에 건들이지 않는다.
		if ( (savedata.FlowerSpots != null) && (mFlowerSpots != null) && (savedata.FlowerSpots.Length == mFlowerSpots.Length) )
		{
			for(int i=0; i<savedata.FlowerSpots.Length; ++i)
				mFlowerSpots[i].ImportFrom(savedata.FlowerSpots[i]);
		}

		InitDefault();
	}
}