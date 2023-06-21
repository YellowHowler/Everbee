using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnumDef;
using StructDef;
using ClassDef;

public class Bee : MonoBehaviour
{
    public Job kCurrentJob = Job.Collect;

    private GameResAmount mCurrentPollen;
    private GameResAmount mCurrentNectar;

    private GameResAmount mMaxPollen = new GameResAmount(20, GameResUnit.Milligram);
    private GameResAmount mMaxNectar = new GameResAmount(20, GameResUnit.Milligram);

    private float mSpeed = 3f;
    private float mFlowerCollectTime = 2f; // 꽃에서 자원 모으는데 걸리는 시간
    
    private bool mAtTarget = false;

    FlowerSpot mTargetFlowerSpot;
    Honeycomb mTargetHoneycomb;

    private void Start()
    {
        DoJob();
    }

    void Update()
    {
      
    }

    private IEnumerator CallDoJob()
    {
        yield return new WaitForSeconds(0.3f);
        DoJob();
    }

    private void DoJob()
    {
        if(kCurrentJob == Job.Collect)
        {
            if(mCurrentNectar.amount == 0 && mCurrentPollen.amount == 0 && !mAtTarget) //없으면 꽃 찾아서 가기
            {
                mTargetFlowerSpot = PlayManager.Instance.kGarden.GetUsableFlowerSpot();

                if(mTargetFlowerSpot == null) 
                {
                    kCurrentJob = Job.Idle;
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
            else if (mCurrentPollen.amount != 0 && mAtTarget == true) 
            {
                mCurrentPollen = mTargetHoneycomb.StoreResource(GameResType.Pollen, mCurrentPollen);
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
            else if (mCurrentNectar.amount != 0 && mAtTarget == true)
            {
                mCurrentNectar = mTargetHoneycomb.StoreResource(GameResType.Nectar, mCurrentNectar);
                mAtTarget = false; 
                mTargetHoneycomb = null;
                StartCoroutine(CallDoJob());
                return;
            }
        }
        if(kCurrentJob == Job.Build)
        {
            kCurrentJob = Job.Idle;
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
            kCurrentJob = Job.Idle;
            StartCoroutine(CallDoJob());
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

        StartCoroutine(CallDoJob());
    }

    private IEnumerator CollectFromFlower()
    {
        yield return new WaitForSeconds(mFlowerCollectTime); //이동안 ui 표시
        mAtTarget = false;
        AddResource(mTargetFlowerSpot.pollenAmount, mTargetFlowerSpot.nectarAmount);

        mTargetFlowerSpot.isTarget = false;

        StartCoroutine(CallDoJob());
    }
}
