using EnumDef;
using StructDef;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QueenBee : MonoBehaviour
{
    private float mSpeed = 2f;

    private Animator mAnimator;

    [HideInInspector] public GameResAmount mCurHoney = new GameResAmount(0f, GameResUnit.Microgram);
    [HideInInspector] public GameResAmount mCurPollen = new GameResAmount(0f, GameResUnit.Microgram);

    [HideInInspector] public QueenState mCurState = QueenState.Wander;

    public Slider kSlider;
    private float mEggTime = 5f;
    private bool mFirst = true;

    void Awake()
    {
        mAnimator = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        Mng.canvas.kQueen.UpdateSliders(mCurHoney, mCurPollen);

        //kSlider.gameObject.SetActive(false);

        // DoJob 이 Start 보다 먼저 불리기도 하기 때문에 Start 에서 mFirst = true 를 해주면 안된다.
        
        StartCoroutine(Wander());
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

    }

    public void AddResource(GameResType _type, GameResAmount _amount)
    {
        switch(_type)
        {
            case GameResType.Honey:
                mCurHoney = Mng.play.AddResourceAmounts(mCurHoney, _amount);
                break;

            case GameResType.Pollen:
                mCurPollen = Mng.play.AddResourceAmounts(mCurPollen, _amount);
                break;
        }
        Mng.canvas.kQueen.UpdateSliders(mCurHoney, mCurPollen);
    }

    private void OnMouseDown()
    {
        if(!PopupBase.IsTherePopup() && mCurState == QueenState.Wander && Mng.play.kHive.mIsBuilding == false)
        {
            Mng.canvas.kQueen.UpdateSliders(mCurHoney, mCurPollen);
            Mng.canvas.kQueen.Show();
            Mng.canvas.kQueen.mTargetQueen = this;
            Mng.canvas.ShowMenu();
        }
    }

    private IEnumerator Wander()
    {
        float waitSec = 0.05f;
        WaitForSeconds sec = new WaitForSeconds(waitSec);

        while (mCurState == QueenState.Wander)
        {
            var targetComb = Mng.play.kHive.GetRandomHoneycomb();
            if (targetComb != null)
            {
				Vector3 randomPos = targetComb.transform.position;

				if(mFirst)
                {
                    mFirst = false;
                    transform.position = randomPos;
                }
                else
                {
                    while(Vector3.Distance(transform.position, randomPos) > 0.005f)
                    {
                        transform.position = Vector3.MoveTowards(transform.position, randomPos, waitSec * mSpeed);
                        yield return sec;
                    }
                }
            }

            yield return new WaitForSeconds(Random.Range(3f, 7f));
        }
    }

    private IEnumerator GoToTarget(Honeycomb _target)
    {
        float waitSec = 0.05f;
        WaitForSeconds sec = new WaitForSeconds(waitSec);

        while(Vector3.Distance(transform.position, _target.pos) > 0.005f)
        {
            transform.position = Vector3.MoveTowards(transform.position, _target.pos, waitSec * mSpeed);
            yield return sec;
        }

        StartCoroutine(LayEgg(_target));
    }

    public void WaitForTargetHoneycomb()
    {
        mCurState = QueenState.WaitForTarget;
        StopCoroutine(Wander());

        Mng.play.kHive.GuideQueen(this);
    }

    public void SetTarget(Honeycomb _target)
    {
        mCurState = QueenState.GoToTarget;
        Mng.play.kHive.mGuidingQueen = false;
        Mng.canvas.DisableToggleButtons();
        StartCoroutine(GoToTarget(_target));
    }

    private IEnumerator LayEgg(Honeycomb _target)
    {
        mCurHoney = Mng.play.SubtractResourceAmounts(mCurHoney, Mng.play.kHive.mQueenHoneyNeed);
        mCurPollen = Mng.play.SubtractResourceAmounts(mCurPollen, Mng.play.kHive.mQueenPollenNeed);
        Mng.canvas.kQueen.UpdateSliders(mCurHoney, mCurPollen);

        Mng.canvas.FillSlider(kSlider, mEggTime);
        yield return new WaitForSeconds(mEggTime+1.5f);

        _target.PlaceEgg();
        
        mCurState = QueenState.Wander;
        StartCoroutine(Wander());
    }
}
