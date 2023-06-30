using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EnumDef;
using StructDef;
using ClassDef;
using TMPro;

public class BeeInfoPanel : MonoBehaviour
{
    public TMP_Text kLevelText;
    public Slider kLevelSlider;

    public GameObject kJobPanel;
    public GameObject kStoragePanel;
    public GameObject kTimerPanel;

    public Slider[] kStorageSliders;
    public TMP_Text[] kStorageTexts;

    public TMP_Text kFeedTextHoney;
    public TMP_Text kFeedTextPollen;
    public Button kFeedButton;

    public TMP_Text kTimerText;

    [HideInInspector] public GameObject mTargetObj;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetStat(GameObject _obj)
    {
        Bee targetBee = _obj.GetComponent<Bee>();


        kLevelText.text = "Level " + targetBee.kLevel;
        kLevelSlider.value = targetBee.kExp;

        if(targetBee.mCurStage == BeeStage.Egg || targetBee.mCurStage == BeeStage.Pupa)
        {   
            kFeedButton.gameObject.SetActive(false);
            kStoragePanel.SetActive(false);
            kJobPanel.SetActive(false);
            kTimerPanel.SetActive(true);

            kTimerText.text = Mng.play.GetTimeText(targetBee.mConvertTime);
        }
        
        else if(targetBee.mCurStage == BeeStage.Bee || targetBee.mCurStage == BeeStage.Larvae)
        {
            kFeedButton.gameObject.SetActive(true);
            kStoragePanel.SetActive(false);
            kTimerPanel.SetActive(false);
            kJobPanel.SetActive(false);

            kFeedTextHoney.text = Mng.canvas.GetAmountText(targetBee.mHoneyFeedAmount);
            kFeedTextHoney.text = Mng.canvas.GetAmountText(targetBee.mPollenFeedAmount);

            if(Mng.play.CompareResourceAmounts(targetBee.mCurrentHoney, targetBee.mHoneyFeedAmount) == true && Mng.play.CompareResourceAmounts(targetBee.mCurrentPollen, targetBee.mPollenFeedAmount) == true)
            {
                kFeedButton.enabled = false;
            }
            else
            {   
                kFeedButton.enabled = true;
            }
        }

        if(targetBee.mCurStage == BeeStage.Bee)
        {
            kStoragePanel.SetActive(true);
            kJobPanel.SetActive(true);

            kStorageSliders[0].value = Mng.play.GetResourcePercent(targetBee.mCurrentHoney, targetBee.mMaxHoney)/100;
            kStorageTexts[0].text = Mng.canvas.GetAmountRatioText(targetBee.mCurrentHoney, targetBee.mMaxHoney);

            kStorageSliders[1].value = Mng.play.GetResourcePercent(targetBee.mCurrentNectar, targetBee.mMaxNectar)/100;
            kStorageTexts[1].text = Mng.canvas.GetAmountRatioText(targetBee.mCurrentNectar, targetBee.mMaxNectar);

            kStorageSliders[2].value = Mng.play.GetResourcePercent(targetBee.mCurrentPollen, targetBee.mMaxPollen)/100;
            kStorageTexts[2].text = Mng.canvas.GetAmountRatioText(targetBee.mCurrentPollen, targetBee.mMaxPollen);
            
            kStorageSliders[3].value = Mng.play.GetResourcePercent(targetBee.mCurrentWax, targetBee.mMaxWax)/100;
            kStorageTexts[3].text = Mng.canvas.GetAmountRatioText(targetBee.mCurrentWax, targetBee.mMaxWax);
        }
    }

    public void SetObject(GameObject _obj)
    {
        mTargetObj = _obj;

        SetStat(_obj);
    }

    public void UpdateStat(GameObject _obj)
    {
        if(mTargetObj != _obj)
        {
            return;
        }

        SetStat(_obj);
    }

    public void FeedBee()
    {
        mTargetObj.GetComponent<Bee>().Feed();
    }
}
