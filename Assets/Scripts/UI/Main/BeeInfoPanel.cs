using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EnumDef;
using StructDef;
using ClassDef;
using TMPro;

public class BeeInfoPanel : PopupBase
{
    public Image kImage;

    public TMP_Text kLevelText;
    public TMP_Text kNameText;
    public Slider kLevelSlider;

    public GameObject kJobPanel;
    public GameObject kStoragePanel;
    public GameObject kTimerPanel;

    public Slider[] kStorageSliders;
    public TMP_Text[] kStorageTexts;

    public TMP_Text kFeedTextHoney;
    public TMP_Text kFeedTextPollen;
    public Button kFeedButton;
    public GameObject kFeedButtonCover;

    public Sprite[] kJobIcons;
    public Image kJobImage;
    public TMP_Text kJobText;
    public TMP_Text kThinkingText;

    public TMP_Text kTimerText;

    [HideInInspector] public Bee mTargetBee;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(mTargetBee)
        {
            kImage.sprite = mTargetBee.GetCurrentSprite();
        }
    }

/*
	override public void ProcessEscapeKey()
	{
		PlayerCamera.Instance.StopFollow();
		base.ProcessEscapeKey();
	}
    */

	public void SetStat(Bee _bee)
    {
        mTargetBee = _bee;

        kNameText.text = mTargetBee.mCurStage.ToString();
        kLevelText.text = "Level " + mTargetBee.kLevel;
        kLevelSlider.value = mTargetBee.kExp;

        if(mTargetBee.mCurStage != BeeStage.Bee)
        {   
            kFeedButton.gameObject.SetActive(false);
            kStoragePanel.SetActive(false);
            kJobPanel.SetActive(false);
            kTimerPanel.SetActive(true);
            kStoragePanel.SetActive(false);

            kTimerText.text = Mng.play.GetTimeText(mTargetBee.mConvertTime);
        }
        else
        {
            kFeedButton.gameObject.SetActive(true);
            kStoragePanel.SetActive(true);
            kTimerPanel.SetActive(false);
            kJobPanel.SetActive(false);
            kStoragePanel.SetActive(true);

            kFeedTextHoney.text = Mng.canvas.GetAmountText(mTargetBee.mHoneyFeedAmount);
            kFeedTextPollen.text = Mng.canvas.GetAmountText(mTargetBee.mPollenFeedAmount);

            bool isInvenEnough = Mng.play.kInventory.CheckIfEnoughResource(GameResType.Honey, mTargetBee.mHoneyFeedAmount) && Mng.play.kInventory.CheckIfEnoughResource(GameResType.Pollen, mTargetBee.mPollenFeedAmount);
            bool isBeeInvenEnough = Mng.play.CompareResourceAmounts(mTargetBee.mHoneyFeedAmount, mTargetBee.mCurrentHoney) && Mng.play.CompareResourceAmounts(mTargetBee.mPollenFeedAmount, mTargetBee.mCurrentPollen);

            if(isInvenEnough || isBeeInvenEnough)
            {
                kFeedButtonCover.SetActive(false);
                kFeedButton.enabled = true;
            }
            else
            {   
                kFeedButtonCover.SetActive(true);
                kFeedButton.enabled = false;
            }
        }

        if(mTargetBee.mCurStage == BeeStage.Bee)
        {
            kJobPanel.SetActive(true);

            switch(mTargetBee.kCurrentJob)
            {
                case Job.Collect:
                    kJobText.text = "Collecting";
                    kJobImage.sprite = kJobIcons[0];
                    break;
                case Job.Build:
                    kJobText.text = "Building";
                    kJobImage.sprite = kJobIcons[1];
                    break;
                case Job.Feed:
                    kJobText.text = "Feeding";
                    kJobImage.sprite = kJobIcons[2];
                    break;
                default:
                    kJobText.text = "Idle";
                    break;
            }

            kThinkingText.text = Mng.canvas.GetBeeThinkingText(mTargetBee.Thinking);

            kStorageSliders[0].value = Mng.play.GetResourcePercent(mTargetBee.mCurrentHoney, mTargetBee.mMaxHoney)/100;
            kStorageTexts[0].text = Mng.canvas.GetAmountRatioText(mTargetBee.mCurrentHoney, mTargetBee.mMaxHoney);

            kStorageSliders[1].value = Mng.play.GetResourcePercent(mTargetBee.mCurrentNectar, mTargetBee.mMaxNectar)/100;
            kStorageTexts[1].text = Mng.canvas.GetAmountRatioText(mTargetBee.mCurrentNectar, mTargetBee.mMaxNectar);

            kStorageSliders[2].value = Mng.play.GetResourcePercent(mTargetBee.mCurrentPollen, mTargetBee.mMaxPollen)/100;
            kStorageTexts[2].text = Mng.canvas.GetAmountRatioText(mTargetBee.mCurrentPollen, mTargetBee.mMaxPollen);
            
            kStorageSliders[3].value = Mng.play.GetResourcePercent(mTargetBee.mCurrentWax, mTargetBee.mMaxWax)/100;
            kStorageTexts[3].text = Mng.canvas.GetAmountRatioText(mTargetBee.mCurrentWax, mTargetBee.mMaxWax);
        }
    }

    public void SetBee(Bee _bee)
    {
        mTargetBee = _bee;

        SetStat(_bee);
    }

    public void UpdateStat(Bee _bee)
    {
        if(mTargetBee != _bee)
        {
            return;
        }

        SetStat(_bee);
    }

    public void FeedBee()
    {
        mTargetBee.Feed();
    }
}
