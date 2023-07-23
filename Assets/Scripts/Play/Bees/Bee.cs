using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnumDef;
using StructDef;
using ClassDef;
using TMPro;

public class Bee : MonoBehaviour
{
    public const int c_BeeLevel = 8;

    public Vector3 pos { get { return transform.position; } set { transform.position = value; } }
    [HideInInspector] public int kLevel;
    [HideInInspector] public float kExp = 0; // 0~1 

    [HideInInspector] public BeeStage mCurStage;

    [HideInInspector] public GameResAmount mHoneyFeedAmount = new GameResAmount(0.8f, GameResUnit.Microgram);
    [HideInInspector] public GameResAmount mPollenFeedAmount = new GameResAmount(0.8f, GameResUnit.Microgram);
    private int mEggLevel = 1;
    private int mPupaLevel = 5;

    private int mEggHatchTime = 5;
    private int mPupaConvertTime = 5;
    [HideInInspector] public int mConvertTime;

    public GameObject mEggObj;
    public GameObject mLarvaeObj;
    public GameObject mPupaObj;
    public GameObject mBeeObj;

    [HideInInspector] public Job kCurrentJob = Job.Collect;

    [HideInInspector] public GameResAmount mCurrentPollen = new GameResAmount(0, GameResUnit.Microgram);
    [HideInInspector] public GameResAmount mCurrentNectar = new GameResAmount(0, GameResUnit.Microgram);
    [HideInInspector] public GameResAmount mCurrentHoney = new GameResAmount(0, GameResUnit.Microgram);
    [HideInInspector] public GameResAmount mCurrentWax = new GameResAmount(0, GameResUnit.Microgram);

    [HideInInspector] public GameResAmount mMaxPollen;
    [HideInInspector] public GameResAmount mMaxNectar;
    [HideInInspector] public GameResAmount mMaxHoney;
    [HideInInspector] public GameResAmount mMaxWax;

    private float mSpeed = 3f;
    private float mFlowerCollectTime = 2f; // 꽃에서 자원 모으는데 걸리는 시간

    public CTargetLink<Bee, FlowerSpot> mTargetFlowerSpot;
    public CTargetLink<Bee, Honeycomb> mTargetHoneycomb;
    public CTargetLink<Bee, Bee> mTargetBee;

    private bool mFirst = true;
    private bool mCanWork = false;
    private bool mAtTarget = false; // 목적지에 다다랐는가.
    public BeeThinking Thinking = BeeThinking.None;

    // UpdateJob 이 Start 보다 먼저 불리기도 하기 때문에 Start 에서 mFirst = true 를 해주면 안된다.

    private void Awake()
    {
        mTargetFlowerSpot = new CTargetLink<Bee, FlowerSpot>(this);
        mTargetHoneycomb = new CTargetLink<Bee, Honeycomb>(this);
        mTargetBee = new CTargetLink<Bee, Bee>(this);
    }

    void Update()
    {
      
    }

    public void UpdateJob(Job _job)
    {
        kCurrentJob = _job;

        Mng.canvas.kBeeInfo.UpdateStat(this);
    }

    private GameObject GetCurrentStageObject()
    {
        switch(mCurStage)
        {
            case BeeStage.Egg: return mEggObj;
            case BeeStage.Larvae: return mLarvaeObj;
            case BeeStage.Pupa: return mPupaObj;
            case BeeStage.Bee: return mBeeObj;
            default: return mBeeObj;
        }
    }

    public void UpdateStage(BeeStage _stage, bool forced = false)
    {
        if (!forced && mCurStage == _stage)
            return;

        mCurStage = _stage;

        switch (_stage)
        {
            case BeeStage.Egg:
                StartCoroutine(EggHatchCor());
                break;
            case BeeStage.Larvae:
                break;
            case BeeStage.Pupa:
                StartCoroutine(PupaConvertCor());
                break;
            case BeeStage.Bee:
                mCanWork = false;
                DoJob();
                break;
        }

        UpdateStageSprite();
        Mng.canvas.kBeeInfo.UpdateStat(this);
    }

    private void UpdateStageSprite()
    {
        mEggObj.SetActive(false);
        mLarvaeObj.SetActive(false);
        mPupaObj.SetActive(false);
        mBeeObj.SetActive(false);

        GetCurrentStageObject().SetActive(true);
    }

    public Sprite GetCurrentSprite()
    {
        return GetCurrentStageObject().GetComponent<SpriteRenderer>().sprite;
    }

    public void UpdateLevel(int _level)
    {
        kLevel = _level;
        mMaxHoney = Mng.play.UpdateUnit(new GameResAmount(0.2f*kLevel, GameResUnit.Milligram));
        mMaxNectar = Mng.play.UpdateUnit(new GameResAmount(0.2f*kLevel, GameResUnit.Milligram));
        mMaxPollen = Mng.play.UpdateUnit(new GameResAmount(2f*kLevel, GameResUnit.Milligram));
        mMaxWax = Mng.play.UpdateUnit(new GameResAmount(0.2f*kLevel, GameResUnit.Milligram));

        mHoneyFeedAmount = Mng.play.UpdateUnit(new GameResAmount(Mathf.Clamp(kLevel * 0.1f, 0, 1) * 0.15f*kLevel, GameResUnit.Milligram));
        mPollenFeedAmount = Mng.play.UpdateUnit(new GameResAmount(Mathf.Clamp(kLevel * 0.1f, 0, 1) * 1.5f*kLevel, GameResUnit.Milligram));

        if(_level == mPupaLevel)
        {
            UpdateStage(BeeStage.Pupa);
        }

        Mng.canvas.kBeeInfo.UpdateStat(this);
    }

    private IEnumerator EggHatchCor()
    {
        WaitForSeconds sec = new WaitForSeconds(1);

        mConvertTime = mEggHatchTime;
        for(int i = 0; i < mEggHatchTime; i++)
        {
            mConvertTime--;
            Mng.canvas.kBeeInfo.UpdateStat(this);
            yield return sec;
        }
        UpdateStage(BeeStage.Larvae);
        UpdateLevel(1);
    }

    private IEnumerator PupaConvertCor()
    {
        WaitForSeconds sec = new WaitForSeconds(1);

        mConvertTime = mPupaConvertTime;
        for(int i = 0; i < mPupaConvertTime; i++)
        {
            mConvertTime--;
            Mng.canvas.kBeeInfo.UpdateStat(this);
            yield return sec;
        }
        UpdateLevel(c_BeeLevel);
        UpdateStage(BeeStage.Bee);
    }

    public void UpdateExp(float _exp)
    {
        kExp += _exp;
        Mng.canvas.kBeeInfo.UpdateStat(this);

        while(kExp >= 1)
        {
            kExp -= 1;
            UpdateLevel(kLevel + 1);
        }
    }

    public void Feed()
    {
        mCurrentHoney = Mng.play.SubtractResourceAmounts(mCurrentHoney, mHoneyFeedAmount);
        mCurrentPollen = Mng.play.SubtractResourceAmounts(mCurrentPollen, mPollenFeedAmount);
        UpdateExp(0.25f);
    }

    private void OnMouseDown()
    {
        if(Mng.canvas.kBeeInfo.gameObject.activeSelf || !PopupBase.IsTherePopup())
        {
            Mng.canvas.kBeeInfo.Show();
            Mng.play.kCamera.SetFollow(transform);
            Mng.canvas.kBeeInfo.SetBee(this);
        }
    }

    private IEnumerator CallDoJob()
    {
        yield return new WaitForSeconds(0.3f);
        DoJob();
    }

    private void LinkTo(FlowerSpot spot)
    {
        if (spot != null)
            mTargetFlowerSpot.LinkTo(spot.mTargetBee);
        else
            mTargetFlowerSpot.Unlink();
    }
    private void LinkTo(Bee bee)
    {
        if (bee != null)
            mTargetBee.LinkTo(bee.mTargetBee);
        else
            mTargetBee.Unlink();
    }
    private void LinkTo(Honeycomb comb)
    {
        if (comb != null)
            mTargetHoneycomb.LinkTo(comb.mTargetBee);
        else
            mTargetHoneycomb.Unlink();
    }

    private void DoJob()
    {
        if (mCurStage != BeeStage.Bee)
            return;

        if(mCanWork == false)
        {
			//mCanWork = true;
			//print("idle");

            var randomHoneycomb = Mng.play.kHive.GetRandomHoneycomb();
            if (randomHoneycomb == null)
            {
                StartCoroutine(CallDoJob());
                return;
            }

			Vector3 randomPos = randomHoneycomb.transform.position;
			if(mFirst)
            {
                // 처음에는 즉시 이동
                transform.position = randomPos;
                mFirst = false;
                mCanWork = true;
                StartCoroutine(CallDoJob());
            }
            else
                StartCoroutine(GoToPos(randomPos));
        }
        else if(kCurrentJob == Job.Collect)
        {
            if(mCurrentNectar.amount == 0 && mCurrentPollen.amount == 0 && !mAtTarget) //없으면 꽃 찾아서 가기
            {
                LinkTo(PlayManager.Instance.kGarden.GetUsableFlowerSpot());

                if(!mTargetFlowerSpot.IsLinked()) 
                {
                    Thinking = BeeThinking.NoAvailableFlower;
                    mCanWork = false;
                    StartCoroutine(CallDoJob());
                    return;
                }

                Thinking = BeeThinking.MovingToFlower;
                StartCoroutine(GoToPos(mTargetFlowerSpot.GetObject().pos));
                return;
            }
            else if(mCurrentNectar.amount == 0 && mCurrentPollen.amount == 0 && mAtTarget) //꽃에 도착
            {
                StartCoroutine(CollectFromFlower());
                return;
            }
            else if(mCurrentPollen.amount != 0 && mAtTarget == false) // 자원이 생겼으니 저장하러 가기
            {
                StorePollen();
                return;
            }
            else if (mCurrentPollen.amount != 0 && mAtTarget == true && mTargetHoneycomb.IsLinked()) 
            {
                Honeycomb targetHoneyComb = mTargetHoneycomb.GetObject();

                if(!(targetHoneyComb.IsUsable(GameResType.Pollen) == true && targetHoneyComb.kStructureType == StructureType.Storage))
                {   
                    Thinking = BeeThinking.NoAvailablePollenStorage;
                    mAtTarget = false;
                    StartCoroutine(CallDoJob());
                    return;
                }

                mCurrentPollen = targetHoneyComb.StoreResource(GameResType.Pollen, mCurrentPollen);
                
                Mng.canvas.kBeeInfo.UpdateStat(this);
                mAtTarget = false;
                mTargetHoneycomb.Unlink();
                StartCoroutine(CallDoJob());
                return;
            }
            else if (mCurrentNectar.amount != 0 && mAtTarget == false) // 자원이 생겼으니 저장하러 가기
            {
                StoreNectar();
                return;
            }
            else if (mCurrentNectar.amount != 0 && mAtTarget == true && mTargetHoneycomb.IsLinked())
            {
                Honeycomb targetHoneyComb = mTargetHoneycomb.GetObject();

                if(!(targetHoneyComb.IsUsable(GameResType.Nectar) == true && targetHoneyComb.kStructureType == StructureType.Storage))
                {   
                    Thinking = BeeThinking.NoAvailableNectarStorage;
                    mAtTarget = false;
                    StartCoroutine(CallDoJob());
                    return;
                }

                mCurrentNectar = targetHoneyComb.StoreResource(GameResType.Nectar, mCurrentNectar);
                Mng.canvas.kBeeInfo.UpdateStat(this);
                mAtTarget = false; 
                mTargetHoneycomb.Unlink();
                StartCoroutine(CallDoJob());
                return;
            }
            else
            {
                Thinking = BeeThinking.None;
                mCanWork = false;
                StartCoroutine(CallDoJob());
                return;
            }
        }
        else if(kCurrentJob == Job.Build)
        {
            mCanWork = false;
            StartCoroutine(CallDoJob());
            return;
        }
        else if(kCurrentJob == Job.Feed)
        {
            // 애벌레에게 먹이기 위해 Honey 나 Pollen 을 구해야 한다.

            if (mAtTarget == false && (mCurrentHoney.amount == 0 && mCurrentPollen.amount == 0))
            {
                FetchPollen();
                return;
            }
            else if (mCurrentPollen.amount == 0 && mAtTarget == true && mTargetHoneycomb.IsLinked())
            {
                Honeycomb targetHoneyComb = mTargetHoneycomb.GetObject();

                if(!((targetHoneyComb.type == GameResType.Pollen && targetHoneyComb.amount.amount > 0) && targetHoneyComb.kStructureType == StructureType.Storage))
                {   
                    Thinking = BeeThinking.NoPollenInThisStorage;
                    mAtTarget = false;
                    StartCoroutine(CallDoJob());
                    return;
                }

                mCurrentPollen = targetHoneyComb.FetchResource(GameResType.Pollen, mCurrentPollen, mMaxPollen);
                Mng.canvas.kBeeInfo.UpdateStat(this);
                mAtTarget = false; 
                mTargetHoneycomb.Unlink();
                StartCoroutine(CallDoJob());
                return;
            }
            else if (mCurrentHoney.amount == 0 && mAtTarget == false)
            {
                FetchHoney();
            }
            else if (mCurrentHoney.amount == 0 && mAtTarget == true && mTargetHoneycomb.IsLinked())
            {
                Honeycomb targetHoneyComb = mTargetHoneycomb.GetObject();

                if(!((targetHoneyComb.type == GameResType.Honey && targetHoneyComb.amount.amount > 0) && targetHoneyComb.kStructureType == StructureType.Storage))
                {   
                    Thinking = BeeThinking.NoHoneyInThisStorage;
                    mAtTarget = false;
                    StartCoroutine(CallDoJob());
                    return;
                }

                mCurrentHoney = targetHoneyComb.FetchResource(GameResType.Honey, mCurrentHoney, mMaxHoney);
                Mng.canvas.kBeeInfo.UpdateStat(this);
                mAtTarget = false; 
                mTargetHoneycomb.Unlink();
                StartCoroutine(CallDoJob());
                return;
            }
            else if(mCurrentHoney.amount > 0 && mCurrentPollen.amount > 0 && mAtTarget == false)
            {
                LinkTo(PlayManager.Instance.kBees.FindLarvae(this));

                if(!mTargetBee.IsLinked()) 
                {
                    Thinking = BeeThinking.NoAvailableLarvae;
                    mCanWork = false;
                    StartCoroutine(CallDoJob());
                    return;
                }

                Thinking = BeeThinking.MovingToLarvae;
                StartCoroutine(GoToPos(mTargetBee.GetObject().gameObject.transform.position));
                return;
            }
            else if(mCurrentHoney.amount > 0 && mCurrentPollen.amount > 0 && mAtTarget == true)
            {
                if(!mTargetBee.IsLinked() || mTargetBee.GetObject().mCurStage != BeeStage.Larvae || Vector3.Distance(mTargetBee.GetObject().gameObject.transform.position, transform.position) > 0.1f)
                {
                    mCanWork = false;
                    StartCoroutine(CallDoJob());
                    return;
                }

                StartCoroutine(FeedLarvae());
                return;
            }
            else
            {
                Thinking = BeeThinking.None;
                mCanWork = false;
                StartCoroutine(CallDoJob());
                return;
            }
        }
    }

    private void StorePollen()
    {
        LinkTo(PlayManager.Instance.kHive.GetUsableStorage(GameResType.Pollen));

        if (!mTargetHoneycomb.IsLinked())
        {
            StoreNectar();
            return;
        }

        Thinking = BeeThinking.MovingToStorage;
        StartCoroutine(GoToPos(mTargetHoneycomb.GetObject().pos));
    }

    private void StoreNectar()
    {
        LinkTo(PlayManager.Instance.kHive.GetUsableStorage(GameResType.Nectar));

        if (!mTargetHoneycomb.IsLinked())
        {
            Thinking = BeeThinking.NoAvailableNectarStorage;
            mCanWork = false;
            StartCoroutine(CallDoJob());
            return;
        }

        Thinking = BeeThinking.MovingToStorage;
        StartCoroutine(GoToPos(mTargetHoneycomb.GetObject().pos));
    }

    private void FetchPollen()
    {
        LinkTo(PlayManager.Instance.kHive.GetFetchableStorage(GameResType.Pollen));

        if (!mTargetHoneycomb.IsLinked())
        {
            FetchHoney();
            return;
        }

        Thinking = BeeThinking.MovingToStorage;
        StartCoroutine(GoToPos(mTargetHoneycomb.GetObject().pos));
    }

    private void FetchHoney()
    {
        LinkTo(PlayManager.Instance.kHive.GetFetchableStorage(GameResType.Honey));

        if (!mTargetHoneycomb.IsLinked())
        {
            Thinking = BeeThinking.NoAvailableHoneyStorage;
            mCanWork = false;
            StartCoroutine(CallDoJob());
            return;
        }

        Thinking = BeeThinking.MovingToStorage;
        StartCoroutine(GoToPos(mTargetHoneycomb.GetObject().pos));
    }

    private IEnumerator CollectFromFlower()
    {
        Thinking = BeeThinking.CollectingFromFlower;
        yield return new WaitForSeconds(mFlowerCollectTime); //이동안 ui 표시
        mAtTarget = false;
        AddResource(mTargetFlowerSpot.GetObject().pollenAmount, mTargetFlowerSpot.GetObject().nectarAmount);
        Mng.canvas.kBeeInfo.UpdateStat(this);

        mTargetFlowerSpot.Unlink();

        StartCoroutine(CallDoJob());
    }

    private IEnumerator FeedLarvae()
    {
        Thinking = BeeThinking.Feeding;
        yield return new WaitForSeconds(0.5f); 

        var targetBee = mTargetBee.GetObject();

        mCurrentHoney = targetBee.AddResource(GameResType.Honey, mCurrentHoney);
        mCurrentPollen = targetBee.AddResource(GameResType.Pollen, mCurrentPollen);
        Mng.canvas.kBeeInfo.UpdateStat(this);

        yield return new WaitForSeconds(0.5f); 

        if(Mng.play.CompareResourceAmounts(targetBee.mPollenFeedAmount, targetBee.mCurrentPollen) == true && Mng.play.CompareResourceAmounts(targetBee.mHoneyFeedAmount, targetBee.mCurrentHoney) == true)
        {
            targetBee.Feed();
        }

        Thinking = BeeThinking.None;
        mTargetBee.Unlink();
        mAtTarget = false;

        StartCoroutine(CallDoJob());
    }

    public GameResAmount AddResource(GameResAmount _pollenAmount, GameResAmount _nectarAmount) //벌 저장공간에 이만큼 더하기
    {
        GameResAmount newPollenAmount = PlayManager.Instance.AddResourceAmounts(mCurrentPollen, _pollenAmount);
        GameResAmount newNectarAmount = PlayManager.Instance.AddResourceAmounts(mCurrentNectar, _nectarAmount);

        if(PlayManager.Instance.CompareResourceAmounts(mMaxPollen, newPollenAmount) == true)
        {
            mCurrentPollen = mMaxPollen;
            Mng.canvas.kBeeInfo.UpdateStat(this);
            return Mng.play.SubtractResourceAmounts(newPollenAmount, mMaxPollen);
        }
        if (PlayManager.Instance.CompareResourceAmounts(mMaxNectar, newNectarAmount) == true)
        {
            mCurrentNectar = mMaxNectar;
            Mng.canvas.kBeeInfo.UpdateStat(this);
            return Mng.play.SubtractResourceAmounts(newNectarAmount, mMaxNectar);
        }

        mCurrentPollen = newPollenAmount;
        mCurrentNectar = newNectarAmount;

        Mng.canvas.kBeeInfo.UpdateStat(this);

        return new GameResAmount(0f, GameResUnit.Microgram);
    }

    public GameResAmount AddResource(GameResType _type, GameResAmount _amount) 
    {
        switch(_type)
        {
            case GameResType.Pollen:
                GameResAmount newPollenAmount = PlayManager.Instance.AddResourceAmounts(mCurrentPollen, _amount);
                if(PlayManager.Instance.CompareResourceAmounts(mMaxPollen, newPollenAmount) == true)
                {
                    mCurrentPollen = mMaxPollen;
                    Mng.canvas.kBeeInfo.UpdateStat(this);
                    return Mng.play.SubtractResourceAmounts(newPollenAmount, mMaxPollen);
                }
                mCurrentPollen = newPollenAmount;
                break;
            case GameResType.Nectar:
                GameResAmount newNectarAmount = PlayManager.Instance.AddResourceAmounts(mCurrentNectar, _amount);
                if(PlayManager.Instance.CompareResourceAmounts(mMaxNectar, newNectarAmount) == true)
                {
                    mCurrentNectar = mMaxNectar;
                    return Mng.play.SubtractResourceAmounts(newNectarAmount, mMaxNectar);
                }
                mCurrentNectar = newNectarAmount;
                Mng.canvas.kBeeInfo.UpdateStat(this);
                break;
            case GameResType.Honey:
                GameResAmount newHoneyAmount = PlayManager.Instance.AddResourceAmounts(mCurrentHoney, _amount);
                if(PlayManager.Instance.CompareResourceAmounts(mMaxHoney, newHoneyAmount) == true)
                {
                    mCurrentHoney = mMaxNectar;
                    return Mng.play.SubtractResourceAmounts(newHoneyAmount, mMaxHoney);
                }
                mCurrentHoney = newHoneyAmount;
                Mng.canvas.kBeeInfo.UpdateStat(this);
                break;
            case GameResType.Wax:
                GameResAmount newWaxAmount = PlayManager.Instance.AddResourceAmounts(mCurrentWax, _amount);
                if(PlayManager.Instance.CompareResourceAmounts(mMaxHoney, newWaxAmount) == true)
                {
                    mCurrentWax = mMaxWax;
                    return Mng.play.SubtractResourceAmounts(newWaxAmount, mMaxWax);
                }
                mCurrentWax = newWaxAmount;
                Mng.canvas.kBeeInfo.UpdateStat(this);
                break;
        }

        Mng.canvas.kBeeInfo.UpdateStat(this);
        return new GameResAmount(0f, GameResUnit.Microgram);
    }

    private IEnumerator GoToPos(Vector3 _targetPos)
    {
        if (mCurStage != BeeStage.Bee)
            yield break;

        mAtTarget = false;

        float waitSec = 0.05f;

        WaitForSeconds sec = new WaitForSeconds(waitSec);

        while(Vector3.Distance(transform.position, _targetPos) > 0.005f)
        {
            transform.position = Vector3.MoveTowards(transform.position, _targetPos, waitSec * mSpeed);
            yield return sec;
        }

        if(mCanWork == true)
        {
            mAtTarget = true;
        }

        if(mCanWork == false)
        {
            yield return new WaitForSeconds(UnityEngine.Random.Range(3, 6));
        }

        mCanWork = true;
        StartCoroutine(CallDoJob());
    }


	// 세이브/로드 관련
	[Serializable]
	public class CSaveData
	{
        public Vector3 Pos;
		public int kLevel;
		public float kExp = 0; // 0~1 
		public BeeStage mCurStage;
        public Job kCurrentJob = Job.Idle;

		public GameResAmount mHoneyFeedAmount = new GameResAmount(0.8f,GameResUnit.Microgram);
		public GameResAmount mPollenFeedAmount = new GameResAmount(0.8f,GameResUnit.Microgram);

		public GameResAmount mCurrentPollen = new GameResAmount(0,GameResUnit.Microgram);
		public GameResAmount mCurrentNectar = new GameResAmount(0,GameResUnit.Microgram);
		public GameResAmount mCurrentHoney = new GameResAmount(0,GameResUnit.Microgram);
		public GameResAmount mCurrentWax = new GameResAmount(0,GameResUnit.Microgram);
	}

	public void ExportTo(CSaveData savedata)
	{
        savedata.Pos = pos;
		savedata.kLevel = kLevel;
		savedata.kExp = kExp;
        savedata.mCurStage = mCurStage;
        savedata.kCurrentJob = kCurrentJob;

		savedata.mHoneyFeedAmount = mHoneyFeedAmount;
		savedata.mPollenFeedAmount = mPollenFeedAmount;

		savedata.mCurrentPollen = mCurrentPollen;
		savedata.mCurrentNectar = mCurrentNectar;
		savedata.mCurrentHoney = mCurrentHoney;
		savedata.mCurrentWax = mCurrentWax;
	}

	public void ImportFrom(CSaveData savedata)
	{
        pos = savedata.Pos;
		kLevel = savedata.kLevel;
		kExp = savedata.kExp;
		mCurStage = savedata.mCurStage;
        kCurrentJob = savedata.kCurrentJob;

		mHoneyFeedAmount = savedata.mHoneyFeedAmount;
		mPollenFeedAmount = savedata.mPollenFeedAmount;

		mCurrentPollen = savedata.mCurrentPollen;
		mCurrentNectar = savedata.mCurrentNectar;
		mCurrentHoney = savedata.mCurrentHoney;
		mCurrentWax = savedata.mCurrentWax;

		UpdateStage(mCurStage, true);
		UpdateLevel(kLevel);

        if (mCurStage != BeeStage.Bee)
            mFirst = false;
	}
}
