using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnumDef;
using StructDef;

public class Bee : MonoBehaviour
{
    private Job mCurrentJob;

    private float mCurrentNectar; //현재 꽃꿀을 얼마나 갖고 있는지
    private Unit mCurrentNectarUnit;
    private float mCurrentPollen;
    private Unit mCurrentPollenUnit;

    private float mMaxNectar; //자원을 얼마큼씩 가질 수 있는지
    private Unit mMaxNectarUnit;
    private float mMaxPollen;
    private Unit mMaxPollenUnit;

    private float mSpeed = 2f;
    private float mFlowerCollectTime = 2f; // 꽃에서 자원 모으는데 걸리는 시간
    
    private bool mAtTarget = false;

    FlowerSpot mTargetFlowerSpot;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private void DoJob()
    {
        if(mCurrentJob == Job.collect)
        {
            if(mCurrentNectar == 0 && mCurrentPollen == 0 && !mAtTarget) //없으면 꽃 찾아서 가기
            {
                mTargetFlowerSpot = PlayManager.Instance.mGarden.GetEmptyFlowerSpot();

                if(mTargetFlowerSpot.ind == -1) 
                {
                    mCurrentJob = Job.idle;
                    DoJob();
                    return;
                }

                StartCoroutine(GoToPos(mTargetFlowerSpot.pos));
                return;
            }
            else if(mCurrentNectar == 0 && mCurrentPollen == 0 && mAtTarget) //꽃에 도착
            {
                StartCoroutine(CollectFromFlower());
            }
        }
    }

    private void AddResource(float _pollenAmount, Unit _pollenUnit, float _nectarAmount, Unit _nectarUnit) //벌 저장공간에 이만큼 더하기
    {

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
        DoJob();
    }

    private IEnumerator CollectFromFlower()
    {
        yield return new WaitForSeconds(mFlowerCollectTime);
        mAtTarget = false;
        AddResource(1, Unit.microgram, 1, Unit.microgram);

        DoJob();
    }
}
