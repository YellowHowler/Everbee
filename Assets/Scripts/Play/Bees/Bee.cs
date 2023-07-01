using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnumDef;
using StructDef;
using ClassDef;
using TMPro;

public class Bee : MonoBehaviour
{
    [HideInInspector] public int kLevel;
    [HideInInspector] public float kExp = 0; // 0~1 

    [HideInInspector] public GameResAmount mFeedAmount = new GameResAmount(0.8f, GameResUnit.Microgram);
    private int mPupaLevel = 4;

    [HideInInspector] public Job kCurrentJob = Job.Collect;

    [HideInInspector] public GameResAmount mCurrentPollen = new GameResAmount(0, GameResUnit.Microgram);
    [HideInInspector] public GameResAmount mCurrentNectar = new GameResAmount(0, GameResUnit.Microgram);
    [HideInInspector] public GameResAmount mCurrentHoney = new GameResAmount(0, GameResUnit.Microgram);
    [HideInInspector] public GameResAmount mCurrentWax = new GameResAmount(0, GameResUnit.Microgram);

    [HideInInspector] public GameResAmount mMaxPollen = new GameResAmount(20, GameResUnit.Milligram);
    [HideInInspector] public GameResAmount mMaxNectar = new GameResAmount(1, GameResUnit.Milligram);
    [HideInInspector] public GameResAmount mMaxHoney = new GameResAmount(1, GameResUnit.Milligram);
    [HideInInspector] public GameResAmount mMaxWax = new GameResAmount(1, GameResUnit.Milligram);

    private float mSpeed = 3f;
    private float mFlowerCollectTime = 2f; // 꽃에서 자원 모으는데 걸리는 시간
    
    private bool mAtTarget = false;

    [HideInInspector] public bool mIsTarget = false;

    FlowerSpot mTargetFlowerSpot;
    Honeycomb mTargetHoneycomb;
    Bee mTargetBee;

    private bool mCanWork = true;

    private void Start()
    {
        UpdateLevel(8); // 나중에 수정예정
        DoJob();
    }

    void Update()
    {
      
    }

<<<<<<< HEAD
<<<<<<< HEAD
=======
=======
>>>>>>> parent of 9ba245c (Revert "-")
    public void UpdateJob(Job _job)
    {
        kCurrentJob = _job;

        Mng.canvas.kBeeInfo.UpdateStat(gameObject);
    }

    public void UpdateStage(BeeStage _stage)
    {
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
                DoJob();
                break;
        }

        UpdateStageSprite(_stage);
        Mng.canvas.kBeeInfo.UpdateStat(gameObject);
    }

    public void UpdateStageSprite(BeeStage _stage)
    {
        mEggObj.SetActive(false);
        mLarvaeObj.SetActive(false);
        mPupaObj.SetActive(false);
        mBeeObj.SetActive(false);

        switch(_stage)
        {
            case BeeStage.Egg:
                mEggObj.SetActive(true);
                break;
            case BeeStage.Larvae:
                mLarvaeObj.SetActive(true);
                break;
            case BeeStage.Pupa:
                mPupaObj.SetActive(true);
                break;
            case BeeStage.Bee:
                mBeeObj.SetActive(true);
                break;
        }
    }

>>>>>>> parent of 9ba245c (Revert "-")
    public void UpdateLevel(int _level)
    {
        kLevel = _level;
        mMaxHoney = Mng.play.UpdateUnit(new GameResAmount(0.2f*kLevel, GameResUnit.Milligram));
        mMaxNectar = Mng.play.UpdateUnit(new GameResAmount(2f*kLevel, GameResUnit.Milligram));
        mMaxHoney = Mng.play.UpdateUnit(new GameResAmount(0.2f*kLevel, GameResUnit.Milligram));
        mMaxHoney = Mng.play.UpdateUnit(new GameResAmount(0.2f*kLevel, GameResUnit.Milligram));

        mFeedAmount = Mng.play.UpdateUnit(new GameResAmount(Mathf.Clamp(kLevel * 0.1f, 0, 1) * 0.15f*kLevel, GameResUnit.Milligram));
        
        Mng.canvas.kBeeInfo.UpdateStat(gameObject);
    }

<<<<<<< HEAD
=======
    private IEnumerator EggHatchCor()
    {
        WaitForSeconds sec = new WaitForSeconds(1);

        mConvertTime = mEggHatchTime;
        for(int i = 0; i < mEggHatchTime; i++)
        {
            mConvertTime--;
            Mng.canvas.kBeeInfo.UpdateStat(gameObject);
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
            Mng.canvas.kBeeInfo.UpdateStat(gameObject);
            yield return sec;
        }
        UpdateStage(BeeStage.Bee);
    }

>>>>>>> parent of 9ba245c (Revert "-")
    public void UpdateExp(float _exp)
    {
        kExp += _exp;
        Mng.canvas.kBeeInfo.UpdateStat(gameObject);

        while(kExp >= 1)
        {
            kExp -= 1;
            UpdateLevel(kLevel + 1);
        }
    }

    public void Feed()
    {
        mCurrentHoney = Mng.play.SubtractResourceAmounts(mCurrentHoney, mFeedAmount);
        UpdateExp(0.25f);
    }

    private void OnMouseDown()
    {
        if(Mng.canvas.kIsViewingMenu == false)
        {
            Mng.canvas.kBeeInfo.gameObject.SetActive(true);
            Mng.play.kCamera.SetFollow(transform);
            Mng.canvas.kBeeInfo.SetObject(gameObject);
        }
    }

    private IEnumerator CallDoJob()
    {
        yield return new WaitForSeconds(0.3f);
        DoJob();
    }

    private void DoJob()
    {
        if(mLevel < mPupaLevel) 
        {
            return;
        }

        if(mCanWork == false)
        {
            //mCanWork = true;
            print("idle");

            Vector3 randomPos = new Vector3(Random.Range(Mng.play.kHiveXBound.start, Mng.play.kHiveXBound.end), Random.Range(Mng.play.kHiveYBound.start, Mng.play.kHiveYBound.end), 0);
            StartCoroutine(GoToPos(randomPos));
        }
        else if(kCurrentJob == Job.Collect)
        {
            if(mCurrentNectar.amount == 0 && mCurrentPollen.amount == 0 && !mAtTarget) //없으면 꽃 찾아서 가기
            {
                mTargetFlowerSpot = PlayManager.Instance.kGarden.GetUsableFlowerSpot();

                if(mTargetFlowerSpot == null) 
                {
                    mCanWork = false;
                    StartCoroutine(CallDoJob());
                    return;
                }

                mTargetFlowerSpot.isTarget = true;
                StartCoroutine(GoToPos(mTargetFlowerSpot.pos));
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
            else if (mCurrentPollen.amount != 0 && mAtTarget == true && mTargetHoneycomb != null) 
            {
                if(!(mTargetHoneycomb.IsUsable(GameResType.Pollen) == true && mTargetHoneycomb.kStructureType == StructureType.Storage))
                {   
                    mAtTarget = false;
                    StartCoroutine(CallDoJob());
                    return;
                }

                mCurrentPollen = mTargetHoneycomb.StoreResource(GameResType.Pollen, mCurrentPollen);
                
                Mng.canvas.kBeeInfo.UpdateStat(gameObject);
                mAtTarget = false;
                mTargetHoneycomb = null;
                StartCoroutine(CallDoJob());
                return;
            }
            else if (mCurrentNectar.amount != 0 && mAtTarget == false) // 자원이 생겼으니 저장하러 가기
            {
                StoreNectar();
                return;
            }
            else if (mCurrentNectar.amount != 0 && mAtTarget == true && mTargetHoneycomb != null)
            {
                if(!(mTargetHoneycomb.IsUsable(GameResType.Nectar) == true && mTargetHoneycomb.kStructureType == StructureType.Storage))
                {   
                    mAtTarget = false;
                    StartCoroutine(CallDoJob());
                    return;
                }

                mCurrentNectar = mTargetHoneycomb.StoreResource(GameResType.Nectar, mCurrentNectar);
                Mng.canvas.kBeeInfo.UpdateStat(gameObject);
                mAtTarget = false; 
                mTargetHoneycomb = null;
                StartCoroutine(CallDoJob());
                return;
            }
            else
            {
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
            if (mAtTarget == false && (mCurrentHoney.amount == 0 && mCurrentPollen.amount == 0))
            {
                FetchPollen();
                return;
            }
            else if (mCurrentPollen.amount == 0 && mAtTarget == true && mTargetHoneycomb != null)
            {
                if(!((mTargetHoneycomb.type == GameResType.Pollen && mTargetHoneycomb.amount.amount > 0) && mTargetHoneycomb.kStructureType == StructureType.Storage))
                {   
                    mAtTarget = false;
                    StartCoroutine(CallDoJob());
                    return;
                }

                mCurrentPollen = mTargetHoneycomb.FetchResource(GameResType.Pollen, mCurrentPollen, mMaxPollen);
                Mng.canvas.kBeeInfo.UpdateStat(gameObject);
                mAtTarget = false; 
                mTargetHoneycomb = null;
                StartCoroutine(CallDoJob());
                return;
            }
            else if (mCurrentHoney.amount == 0 && mAtTarget == false)
            {
                FetchHoney();
            }
            else if (mCurrentHoney.amount == 0 && mAtTarget == true && mTargetHoneycomb != null)
            {
                if(!((mTargetHoneycomb.type == GameResType.Honey && mTargetHoneycomb.amount.amount > 0) && mTargetHoneycomb.kStructureType == StructureType.Storage))
                {   
                    mAtTarget = false;
                    StartCoroutine(CallDoJob());
                    return;
                }

                mCurrentHoney = mTargetHoneycomb.FetchResource(GameResType.Honey, mCurrentHoney, mMaxHoney);
                Mng.canvas.kBeeInfo.UpdateStat(gameObject);
                mAtTarget = false; 
                mTargetHoneycomb = null;
                StartCoroutine(CallDoJob());
                return;
            }
            else if(mCurrentHoney.amount > 0 && mCurrentPollen.amount > 0 && mAtTarget == false)
            {
                mTargetBee = PlayManager.Instance.kBees.FindLarvae();

                if(mTargetBee == null) 
                {
                    mCanWork = false;
                    StartCoroutine(CallDoJob());
                    return;
                }

                mTargetBee.mIsTarget = true;
                StartCoroutine(GoToPos(mTargetBee.gameObject.transform.position));
                return;
            }
            else if(mCurrentHoney.amount > 0 && mCurrentPollen.amount > 0 && mAtTarget == true)
            {
                if(mTargetBee == null || mTargetBee.mCurStage == BeeStage.Larvae || Vector3.Distance(mTargetBee.gameObject.transform.position, transform.position) > 0.1f)
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
                mCanWork = false;
                StartCoroutine(CallDoJob());
                return;
            }
        }
    }

    private void StorePollen()
    {
        mTargetHoneycomb = PlayManager.Instance.kHive.GetUsableHoneycomb(GameResType.Pollen);

        if (mTargetHoneycomb == null)
        {
            StoreNectar();
            return;
        }

        mTargetHoneycomb.isTarget = true;
        StartCoroutine(GoToPos(mTargetHoneycomb.pos));
    }

    private void StoreNectar()
    {
        mTargetHoneycomb = PlayManager.Instance.kHive.GetUsableHoneycomb(GameResType.Nectar);

        if (mTargetHoneycomb == null)
        {
            mCanWork = false;
            StartCoroutine(CallDoJob());
            return;
        }

        mTargetHoneycomb.isTarget = true;
        StartCoroutine(GoToPos(mTargetHoneycomb.pos));
    }

<<<<<<< HEAD
<<<<<<< HEAD
=======
=======
>>>>>>> parent of 9ba245c (Revert "-")
    private void FetchPollen()
    {
        mTargetHoneycomb = PlayManager.Instance.kHive.GetHoneycombOfType(GameResType.Pollen);

        if (mTargetHoneycomb == null)
        {
            FetchHoney();
            return;
        }

        mTargetHoneycomb.isTarget = true;
        StartCoroutine(GoToPos(mTargetHoneycomb.pos));
    }

    private void FetchHoney()
    {
        mTargetHoneycomb = PlayManager.Instance.kHive.GetHoneycombOfType(GameResType.Honey);

        if (mTargetHoneycomb == null)
        {
            mCanWork = false;
            StartCoroutine(CallDoJob());
            return;
        }

        mTargetHoneycomb.isTarget = true;
        StartCoroutine(GoToPos(mTargetHoneycomb.pos));
    }

    private IEnumerator CollectFromFlower()
    {
        yield return new WaitForSeconds(mFlowerCollectTime); //이동안 ui 표시
        mAtTarget = false;
        AddResource(mTargetFlowerSpot.pollenAmount, mTargetFlowerSpot.nectarAmount);
        Mng.canvas.kBeeInfo.UpdateStat(gameObject);

        mTargetFlowerSpot.isTarget = false;

        StartCoroutine(CallDoJob());
    }

    private IEnumerator FeedLarvae()
    {
        yield return new WaitForSeconds(0.5f); 

        mCurrentHoney = mTargetBee.AddResource(GameResType.Honey, mCurrentHoney);
        mCurrentPollen = mTargetBee.AddResource(GameResType.Pollen, mCurrentPollen);
        Mng.canvas.kBeeInfo.UpdateStat(gameObject);

        yield return new WaitForSeconds(0.5f); 

        if(Mng.play.CompareResourceAmounts(mTargetBee.mPollenFeedAmount, mTargetBee.mCurrentPollen) == true && Mng.play.CompareResourceAmounts(mTargetBee.mHoneyFeedAmount, mTargetBee.mCurrentHoney) == true)
        {
            mTargetBee.Feed();
        }

        mTargetBee.mIsTarget = false;
        mAtTarget = false;

        StartCoroutine(CallDoJob());
    }

<<<<<<< HEAD
>>>>>>> parent of 9ba245c (Revert "-")
=======
>>>>>>> parent of 9ba245c (Revert "-")
    public GameResAmount AddResource(GameResAmount _pollenAmount, GameResAmount _nectarAmount) //벌 저장공간에 이만큼 더하기
    {
        GameResAmount newPollenAmount = PlayManager.Instance.AddResourceAmounts(mCurrentPollen, _pollenAmount);
        GameResAmount newNectarAmount = PlayManager.Instance.AddResourceAmounts(mCurrentNectar, _nectarAmount);

        if(PlayManager.Instance.CompareResourceAmounts(mMaxPollen, newPollenAmount) == true)
        {
            mCurrentPollen = mMaxPollen;
            Mng.canvas.kBeeInfo.UpdateStat(gameObject);
            return Mng.play.SubtractResourceAmounts(newPollenAmount, mMaxPollen);
        }
        if (PlayManager.Instance.CompareResourceAmounts(mMaxNectar, newNectarAmount) == true)
        {
            mCurrentNectar = mMaxNectar;
            Mng.canvas.kBeeInfo.UpdateStat(gameObject);
            return Mng.play.SubtractResourceAmounts(newNectarAmount, mMaxNectar);
        }

        mCurrentPollen = newPollenAmount;
        mCurrentNectar = newNectarAmount;

        Mng.canvas.kBeeInfo.UpdateStat(gameObject);

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
                    Mng.canvas.kBeeInfo.UpdateStat(gameObject);
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
                Mng.canvas.kBeeInfo.UpdateStat(gameObject);
                break;
            case GameResType.Honey:
                GameResAmount newHoneyAmount = PlayManager.Instance.AddResourceAmounts(mCurrentHoney, _amount);
                if(PlayManager.Instance.CompareResourceAmounts(mMaxHoney, newHoneyAmount) == true)
                {
                    mCurrentHoney = mMaxNectar;
                    return Mng.play.SubtractResourceAmounts(newHoneyAmount, mMaxHoney);
                }
                mCurrentHoney = newHoneyAmount;
                Mng.canvas.kBeeInfo.UpdateStat(gameObject);
                break;
            case GameResType.Wax:
                GameResAmount newWaxAmount = PlayManager.Instance.AddResourceAmounts(mCurrentWax, _amount);
                if(PlayManager.Instance.CompareResourceAmounts(mMaxHoney, newWaxAmount) == true)
                {
                    mCurrentWax = mMaxWax;
                    return Mng.play.SubtractResourceAmounts(newWaxAmount, mMaxWax);
                }
                mCurrentWax = newWaxAmount;
                Mng.canvas.kBeeInfo.UpdateStat(gameObject);
                break;
        }

        Mng.canvas.kBeeInfo.UpdateStat(gameObject);
        return new GameResAmount(0f, GameResUnit.Microgram);
    }

    private IEnumerator GoToPos(Vector3 _targetPos)
    {
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

        if(mTargetHoneycomb != null)
        {
            mTargetHoneycomb.isTarget = false;
        }

        if(mCanWork == false)
        {
            yield return new WaitForSeconds(Random.Range(3, 6));
        }

        mCanWork = true;
        StartCoroutine(CallDoJob());
    }

    private IEnumerator CollectFromFlower()
    {
        yield return new WaitForSeconds(mFlowerCollectTime); //이동안 ui 표시
        mAtTarget = false;
        AddResource(mTargetFlowerSpot.pollenAmount, mTargetFlowerSpot.nectarAmount);
        Mng.canvas.kBeeInfo.UpdateStat(gameObject);

        mTargetFlowerSpot.isTarget = false;

        StartCoroutine(CallDoJob());
    }
}
