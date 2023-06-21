using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnumDef;
using StructDef;
using ClassDef;

public class Bee : MonoBehaviour
{
    private bool isWorking = false;

    private bool mCanStorePollen = true;

    public Job kCurrentJob = Job.Collect;
    private CollectState mCurrentCollectState = CollectState.GoToFlower;

    private GameResAmount mCurrentPollen;
    private GameResAmount mCurrentNectar;

    private GameResAmount mMaxPollen = new GameResAmount(20, GameResUnit.Milligram);
    private GameResAmount mMaxNectar = new GameResAmount(20, GameResUnit.Milligram);

    private float mSpeed = 3f;
    private float mFlowerCollectTime = 2f; // 꽃에서 자원 모으는데 걸리는 시간
    
    private bool mAtTarget = false;

    FlowerSpot mTargetFlowerSpot;
    Honeycomb mTargetHoneycomb;

    private void Update()
    {
        StartCoroutine(ChangeJob());
    }

    private IEnumerator ChangeJob()
    {
        DoJob();

        while(isWorking == true)
        {
            yield return new WaitForSeconds(0.3f);
        }

        if(kCurrentJob == Job.Collect)
        {
            if(mCurrentNectar.amount == 0 && mCurrentPollen.amount == 0 && !mAtTarget) //없으면 꽃 찾아서 가기
            {
                mCurrentCollectState = CollectState.GoToFlower;
            }
            else if(mCurrentNectar.amount == 0 && mCurrentPollen.amount == 0 && mAtTarget) //꽃에 도착
            {
                mCanStorePollen = true;
                mCurrentCollectState = CollectState.CollectResource;
            }
            else if(mCurrentPollen.amount != 0 && mAtTarget == false && mCanStorePollen) // 자원이 생겼으니 저장하러 가기
            {
                mCurrentCollectState = CollectState.GoToPollen;
            }
            else if (mCurrentPollen.amount != 0 && mAtTarget == true) 
            {
                mCurrentCollectState = CollectState.StorePollen;
            }
            else if (mCurrentNectar.amount != 0 && mAtTarget == false) // 자원이 생겼으니 저장하러 가기
            {
                mCurrentCollectState = CollectState.GoToNectar;
            }
            else if (mCurrentNectar.amount != 0 && mAtTarget == true)
            {
                mCurrentCollectState = CollectState.StorePollen;
            }
        }
    }

    private void DoJob()
    {
        isWorking = true;

        if(kCurrentJob == Job.Collect)
        {
            switch(mCurrentCollectState)
            {
                case CollectState.GoToFlower:
                    mTargetFlowerSpot = PlayManager.Instance.kGarden.GetUsableFlowerSpot();

                    if(mTargetFlowerSpot == null) 
                    {
                        kCurrentJob = Job.Idle;
                        isWorking = false;
                        break;
                    }

                    mTargetFlowerSpot.isTarget = true;
                    StartCoroutine(GoToPos(mTargetFlowerSpot.pos));
                    isWorking = false;
                    break;
                case CollectState.CollectResource:
                    StartCoroutine(CollectFromFlower());
                    isWorking = false;
                    break;
                case CollectState.GoToPollen:
                    StorePollen();
                    break;
                case CollectState.StorePollen:
                    mCurrentPollen = mTargetHoneycomb.StoreResource(GameResType.Pollen, mCurrentPollen);
                    mAtTarget = false;
                    mTargetHoneycomb = null;
                    isWorking = false;
                    break;
                case CollectState.GoToNectar:
                    StoreNectar();
                    break;
                case CollectState.StoreNectar:
                    mCurrentNectar = mTargetHoneycomb.StoreResource(GameResType.Nectar, mCurrentNectar);
                    mAtTarget = false; 
                    mTargetHoneycomb = null;
                    break;
            }
        }
        if(kCurrentJob == Job.Idle)
        {
            print("idle");

            Vector3 randomPos = new Vector3(Random.Range(Mng.play.kHiveXBound.start, Mng.play.kHiveXBound.end), Random.Range(Mng.play.kHiveYBound.start, Mng.play.kHiveYBound.end), 0);
            StartCoroutine(GoToPos(randomPos));
        }
    }

    private void StorePollen()
    {
        mTargetHoneycomb = PlayManager.Instance.kHive.GetUsableHoneycomb(GameResType.Pollen);

        if (mTargetHoneycomb == null)
        {
            mCanStorePollen = false;
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
            kCurrentJob = Job.Idle;
            DoJob();
            return;
        }

        mTargetHoneycomb.isTarget = true;
        StartCoroutine(GoToPos(mTargetHoneycomb.pos));
    }

    private void AddResource(GameResAmount _pollenAmount, GameResAmount _nectarAmount) //벌 저장공간에 이만큼 더하기
    {
        GameResAmount newPollenAmount = PlayManager.Instance.AddResourceAmounts(mCurrentPollen, _pollenAmount);
        GameResAmount newNectarAmount = PlayManager.Instance.AddResourceAmounts(mCurrentNectar, _nectarAmount);

        if(PlayManager.Instance.CompareResourceAmounts(mMaxPollen, newPollenAmount) == true)
        {
            newPollenAmount = mMaxPollen;
        }
        if (PlayManager.Instance.CompareResourceAmounts(mMaxNectar, newNectarAmount) == true)
        {
            newNectarAmount = mMaxNectar;
        }

        mCurrentPollen = newPollenAmount;
        mCurrentNectar = newNectarAmount;

        print("current pollen: " + mCurrentPollen.amount);
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

        mAtTarget = true;

        if(mTargetHoneycomb != null)
        {
            mTargetHoneycomb.isTarget = false;
        }

        if(kCurrentJob == Job.Idle)
        {
            yield return new WaitForSeconds(Random.Range(3, 6));
        }

        isWorking = false;
    }

    private IEnumerator CollectFromFlower()
    {
        yield return new WaitForSeconds(mFlowerCollectTime); //이동안 ui 표시
        mAtTarget = false;
        AddResource(mTargetFlowerSpot.pollenAmount, mTargetFlowerSpot.nectarAmount);

        mTargetFlowerSpot.isTarget = false;

        DoJob();
    }
}
