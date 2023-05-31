using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnumDef;
using StructDef;
using ClassDef;

public class Bee : MonoBehaviour
{
    private Job mCurrentJob;

    private GameResAmount mCurrentPollen;
    private GameResAmount mCurrentNectar;

    private GameResAmount mMaxPollen;
    private GameResAmount mMaxNectar;

    private float mSpeed = 2f;
    private float mFlowerCollectTime = 2f; // 꽃에서 자원 모으는데 걸리는 시간
    
    private bool mAtTarget = false;

    FlowerSpot mTargetFlowerSpot;
    Honeycomb mTargetHoneycomb;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private void DoJob()
    {
        if(mCurrentJob == Job.Collect)
        {
            if(mCurrentNectar.amount == 0 && mCurrentPollen.amount == 0 && !mAtTarget) //없으면 꽃 찾아서 가기
            {
                mTargetFlowerSpot = PlayManager.Instance.kGarden.GetUsableFlowerSpot();

                if(mTargetFlowerSpot == null) 
                {
                    mCurrentJob = Job.Idle;
                    DoJob();
                    return;
                }

                mTargetFlowerSpot.isTarget = true;
                StartCoroutine(GoToPos(mTargetFlowerSpot.pos));
                return;
            }
            else if(mCurrentNectar.amount == 0 && mCurrentPollen.amount == 0 && mAtTarget) //꽃에 도착
            {
                StartCoroutine(CollectFromFlower());
            }
            else if(mCurrentPollen.amount != 0 && mAtTarget == false) // 자원이 생겼으니 저장하러 가기
            {
                mTargetHoneycomb = PlayManager.Instance.kHive.GetUsableHoneycomb(GameResType.Pollen);

                if (mTargetHoneycomb == null)
                {
                    mCurrentJob = Job.Idle;
                    DoJob();
                    return;
                }

                mTargetHoneycomb.isTarget = true;
                StartCoroutine(GoToPos(mTargetHoneycomb.pos));
            }
        }
        if(mCurrentJob == Job.Idle)
        {
            
        }
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
    }

    private IEnumerator GoToPos(Vector3 _targetPos)
    {
        float waitSec = 0.1f;

        WaitForSeconds sec = new WaitForSeconds(waitSec);

        while(Vector3.Distance(transform.position, _targetPos) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, _targetPos, waitSec * mSpeed);
            yield return sec;
        }

        mAtTarget = true;

        if(mTargetHoneycomb != null)
        {
            mTargetHoneycomb.isTarget = false;
        }

        DoJob();
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
