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

    public Animator kBeeAnimator;

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

    public SpriteOutline kOutline;
    public ParticleSystem kPollenParticle;

    // UpdateJob 이 Start 보다 먼저 불리기도 하기 때문에 Start 에서 mFirst = true 를 해주면 안된다.

    private void Awake()
    {
        mTargetFlowerSpot = new CTargetLink<Bee, FlowerSpot>(this);
        mTargetHoneycomb = new CTargetLink<Bee, Honeycomb>(this);
        mTargetBee = new CTargetLink<Bee, Bee>(this);

        kOutline = GetComponent<SpriteOutline>();
        kOutline.DisableOutline();
        kPollenParticle.Stop();
    }

    void Update()
    {
        
    }

    public GameResAmount GetCurAmount(GameResType _type)
    {
        switch(_type)
        {
            case GameResType.Honey:
                return mCurrentHoney;
            case GameResType.Nectar:
                return mCurrentNectar;
            case GameResType.Pollen:
                return mCurrentPollen;
            case GameResType.Wax:
                return mCurrentWax;
            default:
                return new GameResAmount(0f, GameResUnit.Microgram);
        }
    }

    public GameResAmount GetMaxAmount(GameResType _type)
    {
        switch(_type)
        {
            case GameResType.Honey:
                return mMaxHoney;
            case GameResType.Nectar:
                return mMaxNectar;
            case GameResType.Pollen:
                return mMaxPollen;
            case GameResType.Wax:
                return mMaxWax;
            default:
                return new GameResAmount(0f, GameResUnit.Microgram);
        }
    }

    public void UpdateJob(Job _job)
    {
        kCurrentJob = _job;

        Mng.canvas.kBeeInfo.UpdateStat(this);
    }

    public void UpdateThinking(BeeThinking _thinking)
    {
        Thinking = _thinking;
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
                kOutline.sr = mEggObj.GetComponent<SpriteRenderer>();
                StartCoroutine(EggHatchCor());
                break;
            case BeeStage.Larvae:
                kOutline.sr = mLarvaeObj.GetComponent<SpriteRenderer>();
                break;
            case BeeStage.Pupa:
                kOutline.sr = mPupaObj.GetComponent<SpriteRenderer>();
                StartCoroutine(PupaConvertCor());
                break;
            case BeeStage.Bee:
                kOutline.sr = mBeeObj.GetComponent<SpriteRenderer>();
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

        kOutline.sr = GetCurrentStageObject().GetComponent<SpriteRenderer>();
        GetCurrentStageObject().SetActive(true);
    }

    public Sprite GetCurrentSprite()
    {
        return GetCurrentStageObject().GetComponent<SpriteRenderer>().sprite;
    }

    public void UpdateLevel(int _level)
    {
        kLevel = _level;
        mMaxHoney = Mng.play.UpdateUnit(new GameResAmount(0.04f*kLevel, GameResUnit.Milligram));
        mMaxNectar = Mng.play.UpdateUnit(new GameResAmount(0.04f*kLevel, GameResUnit.Milligram));
        mMaxPollen = Mng.play.UpdateUnit(new GameResAmount(2f*kLevel, GameResUnit.Milligram));
        mMaxWax = Mng.play.UpdateUnit(new GameResAmount(0.04f*kLevel, GameResUnit.Milligram));

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
    
        while(kExp >= 1)
        {
            kExp -= 1;
            UpdateLevel(kLevel + 1);
        }

        Mng.canvas.kBeeInfo.UpdateStat(this);
    }

    
    
    public void Feed()
    {
        bool isInvenEnough = Mng.play.kInventory.CheckIfEnoughResource(GameResType.Honey, mHoneyFeedAmount) && Mng.play.kInventory.CheckIfEnoughResource(GameResType.Pollen, mPollenFeedAmount);
        bool isBeeInvenEnough = Mng.play.CompareResourceAmounts(mCurrentHoney, mHoneyFeedAmount) && Mng.play.CompareResourceAmounts(mCurrentPollen, mPollenFeedAmount);

        if(isInvenEnough)
        {
            Mng.play.kInventory.SubtractFromInven(GameResType.Honey, mHoneyFeedAmount);
            Mng.play.kInventory.SubtractFromInven(GameResType.Pollen, mPollenFeedAmount);
            UpdateExp(0.25f);
        }
        else if(isBeeInvenEnough)
        {
            UpdateResourceAmount(GameResType.Honey, Mng.play.SubtractResourceAmounts(mCurrentHoney, mHoneyFeedAmount));
            UpdateResourceAmount(GameResType.Pollen, Mng.play.SubtractResourceAmounts(mCurrentPollen, mPollenFeedAmount));
            UpdateExp(0.25f);
        }
        else
        {
            Mng.canvas.DisplayWarning("Not enough food");
        }
        
        Mng.canvas.kBeeInfo.UpdateStat(this);
    }

    public void Feed(Bee _feeder)
    {
        _feeder.UpdateResourceAmount(GameResType.Honey, Mng.play.SubtractResourceAmounts(mCurrentHoney, mHoneyFeedAmount));
        _feeder.UpdateResourceAmount(GameResType.Pollen, Mng.play.SubtractResourceAmounts(mCurrentPollen, mPollenFeedAmount));
        UpdateExp(0.25f);
    }

    public void UpdateResourceAmount(GameResType _type, GameResAmount _amount)
    {
        switch(_type)
        {
            case GameResType.Honey:
                mCurrentHoney = _amount;
                break;
            case GameResType.Nectar:
                mCurrentNectar = _amount;
                break;
            case GameResType.Pollen:
                mCurrentPollen = _amount;
                break;
            case GameResType.Wax:
                mCurrentWax = _amount;
                break;
        }

        if(mCurrentPollen.amount > 0.1f)
        {
            kBeeAnimator.SetBool("hasPollen", true);
            kPollenParticle.Play();
        }
        else
        {
            kBeeAnimator.SetBool("hasPollen", false);
            kPollenParticle.Stop();
        }

        Mng.canvas.kBeeInfo.UpdateStat(this);
    }

    private void OnMouseDown()
    {
        if(PopupBase.IsTherePopup())
        {
            return;
        }

        if(Mng.play.kHive.mIsBuilding == true || Mng.play.kHive.mIsPlacingItem == true)
        {
            return;
        }

        if(Mng.canvas.kBeeInfo.gameObject.activeSelf || !PopupBase.IsTherePopup())
        {
            //Mng.canvas.kBeeInfo.Show();
            Mng.canvas.kQueen.Hide();
            Mng.canvas.kBeeInfo.gameObject.SetActive(true);
            Mng.play.kCamera.SetFollow(transform);
            Mng.canvas.kBeeInfo.SetBee(this);
        }
    }

    private void OnMouseOver()
    {
        if(PopupBase.IsTherePopup() || Mng.play.kHive.mIsBuilding) 
        {
            return;
        }

        Mng.play.kHive.mHoveredBee = this;
        kOutline.EnableOutline();
    }

    private void OnMouseExit()
    {
        if(Mng.play.kHive.mHoveredBee == this)
        {
            Mng.play.kHive.mHoveredBee = null;
        }

        kOutline.DisableOutline();
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
                mCanWork = true;
                StartCoroutine(CallDoJob());
                return;
            }

			Vector3 randomPos = randomHoneycomb.transform.position + Mng.play.SetZ((Vector3)UnityEngine.Random.insideUnitCircle*0.5f, 0);
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
                    UpdateThinking(BeeThinking.NoAvailableFlower);
                    mCanWork = false;
                    StartCoroutine(CallDoJob());
                    return;
                }

                UpdateThinking(BeeThinking.MovingToFlower);
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
                    UpdateThinking(BeeThinking.NoAvailablePollenStorage);
                    mAtTarget = false;
                    StartCoroutine(CallDoJob());
                    return;
                }

                UpdateResourceAmount(GameResType.Pollen, targetHoneyComb.StoreResource(GameResType.Pollen, mCurrentPollen));

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
                    UpdateThinking(BeeThinking.NoAvailableNectarStorage);
                    mAtTarget = false;
                    StartCoroutine(CallDoJob());
                    return;
                }

                UpdateResourceAmount(GameResType.Nectar, targetHoneyComb.StoreResource(GameResType.Nectar, mCurrentNectar));

                mAtTarget = false; 
                mTargetHoneycomb.Unlink();
                StartCoroutine(CallDoJob());
                return;
            }
            else
            {
                UpdateThinking(BeeThinking.None);
                mCanWork = false;
                StartCoroutine(CallDoJob());
                return;
            }
        }
        else if(kCurrentJob == Job.Build)
        {
            if (mAtTarget == false && mCurrentWax.amount == 0)
            {
                FetchWax();
                return;
            }
            else if (mCurrentWax.amount == 0 && mAtTarget == true && mTargetHoneycomb.IsLinked())
            {
                Honeycomb targetHoneyComb = mTargetHoneycomb.GetObject();

                if(!(targetHoneyComb.type == GameResType.Wax && Mng.play.IsAmountZero(targetHoneyComb.amount) == false && targetHoneyComb.kStructureType == StructureType.Storage))
                {   
                    UpdateThinking(BeeThinking.NoWaxInThisStorage);
                    mAtTarget = false;
                    StartCoroutine(CallDoJob());
                    return;
                }

                UpdateResourceAmount(GameResType.Wax, targetHoneyComb.FetchResource(GameResType.Wax, mCurrentWax, mMaxWax));
                mAtTarget = false; 

                mTargetHoneycomb.Unlink();
                StartCoroutine(CallDoJob());
                return;
            }
            else if(mCurrentWax.amount > 0 && mAtTarget == false)
            {
                LinkTo(PlayManager.Instance.kHive.FindBuildingHoneycomb());

                if(!mTargetHoneycomb.IsLinked()) 
                {
                    UpdateThinking(BeeThinking.NoBuildingStructure);
                    mCanWork = false;
                    StartCoroutine(CallDoJob());
                    return;
                }

                UpdateThinking(BeeThinking.MovingToBuild);
                StartCoroutine(GoToPos(mTargetHoneycomb.GetObject().pos));
                return;
            }
            else if(mCurrentWax.amount > 0 && mAtTarget == true)
            {
                if(!mTargetHoneycomb.IsLinked() || mTargetHoneycomb.GetObject().kStructureType != StructureType.Building || Vector3.Distance(mTargetHoneycomb.GetObject().pos, transform.position) > 0.1f)
                {
                    mCanWork = false;
                    StartCoroutine(CallDoJob());
                    return;
                }

                StartCoroutine(BuildStructure());
                return;
            }
            else
            {
                UpdateThinking(BeeThinking.None);
                mCanWork = false;
                StartCoroutine(CallDoJob());
                return;
            }
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
                    UpdateThinking(BeeThinking.NoPollenInThisStorage);
                    mAtTarget = false;
                    StartCoroutine(CallDoJob());
                    return;
                }

                UpdateResourceAmount(GameResType.Pollen, targetHoneyComb.FetchResource(GameResType.Pollen, mCurrentPollen, mMaxPollen));

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
                    UpdateThinking(BeeThinking.NoHoneyInThisStorage);
                    mAtTarget = false;
                    StartCoroutine(CallDoJob());
                    return;
                }

                UpdateResourceAmount(GameResType.Honey, targetHoneyComb.FetchResource(GameResType.Honey, mCurrentHoney, mMaxHoney));

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
                    UpdateThinking(BeeThinking.NoAvailableLarvae);
                    mCanWork = false;
                    StartCoroutine(CallDoJob());
                    return;
                }

                UpdateThinking(BeeThinking.MovingToLarvae);
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
                UpdateThinking(BeeThinking.None);
                mCanWork = false;
                StartCoroutine(CallDoJob());
                return;
            }
        }
    }

    private void StorePollen()
    {
        LinkTo(PlayManager.Instance.kHive.GetUsableStorage(GameResType.Pollen, false));

        if (!mTargetHoneycomb.IsLinked())
        {
            StoreNectar();
            return;
        }

        UpdateThinking(BeeThinking.MovingToStorage);
        StartCoroutine(GoToPos(mTargetHoneycomb.GetObject().pos));
    }

    private void StoreNectar()
    {
        LinkTo(PlayManager.Instance.kHive.GetUsableStorage(GameResType.Nectar, false));

        if (!mTargetHoneycomb.IsLinked())
        {
            UpdateThinking(BeeThinking.NoAvailableNectarStorage);
            mCanWork = false;
            StartCoroutine(CallDoJob());
            return;
        }

        UpdateThinking(BeeThinking.MovingToStorage);
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

        UpdateThinking(BeeThinking.MovingToStorage);
        StartCoroutine(GoToPos(mTargetHoneycomb.GetObject().pos));
    }

    private void FetchHoney()
    {
        LinkTo(PlayManager.Instance.kHive.GetFetchableStorage(GameResType.Honey));

        if (!mTargetHoneycomb.IsLinked())
        {
            UpdateThinking(BeeThinking.NoAvailableHoneyStorage);
            mCanWork = false;
            StartCoroutine(CallDoJob());
            return;
        }

        UpdateThinking(BeeThinking.MovingToStorage);
        StartCoroutine(GoToPos(mTargetHoneycomb.GetObject().pos));
    }

    private void FetchWax()
    {
        LinkTo(PlayManager.Instance.kHive.GetFetchableStorage(GameResType.Wax));

        if (!mTargetHoneycomb.IsLinked())
        {
            UpdateThinking(BeeThinking.NoAvailableWaxStorage);
            mCanWork = false;
            StartCoroutine(CallDoJob());
            return;
        }

        UpdateThinking(BeeThinking.MovingToStorage);
        StartCoroutine(GoToPos(mTargetHoneycomb.GetObject().pos));
    }

    private IEnumerator CollectFromFlower()
    {
        UpdateThinking(BeeThinking.CollectingFromFlower);
        yield return new WaitForSeconds(mFlowerCollectTime); //이동안 ui 표시
        mAtTarget = false;
        AddResource(mTargetFlowerSpot.GetObject().pollenAmount, mTargetFlowerSpot.GetObject().nectarAmount);

        mTargetFlowerSpot.Unlink();

        StartCoroutine(CallDoJob());
    }

    private IEnumerator FeedLarvae()
    {
        UpdateThinking(BeeThinking.Feeding);
        yield return new WaitForSeconds(0.5f); 

        var targetBee = mTargetBee.GetObject();

        yield return new WaitForSeconds(0.5f); 

        if(targetBee != null
        && Mng.play.CompareResourceAmounts(targetBee.mPollenFeedAmount, mCurrentPollen) == true 
        && Mng.play.CompareResourceAmounts(targetBee.mHoneyFeedAmount, mCurrentHoney) == true
        && targetBee.mCurStage == BeeStage.Larvae)
        {
            targetBee.Feed(this);
        }

        UpdateThinking(BeeThinking.None);
        mTargetBee.Unlink();
        mAtTarget = false;

        StartCoroutine(CallDoJob());
    }

    private IEnumerator BuildStructure()
    {
        UpdateThinking(BeeThinking.Building);

        yield return new WaitForSeconds(0.5f);

        var targetHoneycomb = mTargetHoneycomb.GetObject();

        if(targetHoneycomb != null
        && targetHoneycomb.kStructureType == StructureType.Building)
        {
            UpdateResourceAmount(GameResType.Wax, targetHoneycomb.UpdateWaxAmount(mCurrentWax));
            targetHoneycomb.PlayParticles();
        }

        UpdateThinking(BeeThinking.None);
        mTargetHoneycomb.Unlink();
        mAtTarget = false;

        StartCoroutine(CallDoJob());
    }

    public bool IsStorageFull(GameResType _type)
    {
        return Mng.play.IsSameAmount(GetCurAmount(_type), GetMaxAmount(_type));
    }

    public GameResAmount AddResource(GameResAmount _pollenAmount, GameResAmount _nectarAmount) //벌 저장공간에 이만큼 더하기
    {
        GameResAmount newPollenAmount = PlayManager.Instance.AddResourceAmounts(mCurrentPollen, _pollenAmount);
        GameResAmount newNectarAmount = PlayManager.Instance.AddResourceAmounts(mCurrentNectar, _nectarAmount);

        if(PlayManager.Instance.CompareResourceAmounts(mMaxPollen, newPollenAmount) == true)
        {
            UpdateResourceAmount(GameResType.Pollen, mMaxPollen);
            return Mng.play.SubtractResourceAmounts(newPollenAmount, mMaxPollen);
        }
        if (PlayManager.Instance.CompareResourceAmounts(mMaxNectar, newNectarAmount) == true)
        {
            UpdateResourceAmount(GameResType.Nectar, mMaxNectar);
            return Mng.play.SubtractResourceAmounts(newNectarAmount, mMaxNectar);
        }

        UpdateResourceAmount(GameResType.Pollen, newPollenAmount);
        UpdateResourceAmount(GameResType.Nectar, newNectarAmount);

        return new GameResAmount(0f, GameResUnit.Microgram);
    }

    public GameResAmount AddResource(GameResType _type, GameResAmount _amount) 
    {
        if(IsStorageFull(_type))
        {
            Mng.canvas.DisplayWarning("Bee's " + Mng.canvas.GetResourceTypeText(_type, false) + " storage is full");
        }

        switch(_type)
        {
            case GameResType.Pollen:
                GameResAmount newPollenAmount = PlayManager.Instance.AddResourceAmounts(mCurrentPollen, _amount);
                if(PlayManager.Instance.CompareResourceAmounts(mMaxPollen, newPollenAmount) == true)
                {
                    UpdateResourceAmount(GameResType.Pollen, mMaxPollen);
                    return Mng.play.SubtractResourceAmounts(newPollenAmount, mMaxPollen);
                }
                UpdateResourceAmount(GameResType.Pollen, newPollenAmount);
                break;
            case GameResType.Nectar:
                GameResAmount newNectarAmount = PlayManager.Instance.AddResourceAmounts(mCurrentNectar, _amount);
                if(PlayManager.Instance.CompareResourceAmounts(mMaxNectar, newNectarAmount) == true)
                {
                    UpdateResourceAmount(GameResType.Nectar, mMaxNectar);
                    return Mng.play.SubtractResourceAmounts(newNectarAmount, mMaxNectar);
                }
                UpdateResourceAmount(GameResType.Nectar, newNectarAmount);
                break;
            case GameResType.Honey:
                GameResAmount newHoneyAmount = PlayManager.Instance.AddResourceAmounts(mCurrentHoney, _amount);
                if(PlayManager.Instance.CompareResourceAmounts(mMaxHoney, newHoneyAmount) == true)
                {
                    UpdateResourceAmount(GameResType.Honey, mMaxHoney);
                    return Mng.play.SubtractResourceAmounts(newHoneyAmount, mMaxHoney);
                }
                UpdateResourceAmount(GameResType.Honey, newHoneyAmount);
                break;
            case GameResType.Wax:
                GameResAmount newWaxAmount = PlayManager.Instance.AddResourceAmounts(mCurrentWax, _amount);
                if(PlayManager.Instance.CompareResourceAmounts(mMaxHoney, newWaxAmount) == true)
                {
                    UpdateResourceAmount(GameResType.Wax, mMaxWax);
                    return Mng.play.SubtractResourceAmounts(newWaxAmount, mMaxWax);
                }
                UpdateResourceAmount(GameResType.Wax, newWaxAmount);
                break;
        }

        return new GameResAmount(0f, GameResUnit.Microgram);
    }

    private IEnumerator GoToPos(Vector3 _targetPos)
    {
        if (mCurStage != BeeStage.Bee)
            yield break;

        mAtTarget = false;

        float waitSec = 0.05f;

        WaitForSeconds sec = new WaitForSeconds(waitSec);

        if(_targetPos.x > transform.position.x) transform.localRotation = Quaternion.Euler(0, 180, 0);
        else transform.localRotation = Quaternion.Euler(0, 0, 0);

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

		UpdateResourceAmount(GameResType.Pollen, savedata.mCurrentPollen);
		UpdateResourceAmount(GameResType.Nectar, savedata.mCurrentNectar);
		UpdateResourceAmount(GameResType.Honey, savedata.mCurrentHoney);
		UpdateResourceAmount(GameResType.Wax, savedata.mCurrentWax);

		UpdateStage(mCurStage, true);
		UpdateLevel(kLevel);

        if (mCurStage != BeeStage.Bee)
            mFirst = false;
	}
}
