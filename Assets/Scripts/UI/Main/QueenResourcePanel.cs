using EnumDef;
using StructDef;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QueenResourcePanel : PopupBase
{
    [HideInInspector] public QueenBee mTargetQueen;

    public Image kImage;

    public Button kQueenBtn;

    public Slider kHoneySlider;
    public Slider kPollenSlider;

    public TMP_Text kHoneyText;
    public TMP_Text kPollenText;

    public Button kEggButton;
    public GameObject kEggButtonCover;
    
    void Start()
    {
    }

    void Update()
    {
        if(mTargetQueen)
        {
            kQueenBtn.interactable = false;
            kImage.sprite = mTargetQueen.GetCurrentSprite();
        }
    }

    /*
	override public void ProcessEscapeKey()
	{
		OnQueenResourceBgClick();
		base.ProcessEscapeKey();
	}
    */

	public void QueenLayEgg()
    {
        mTargetQueen.WaitForTargetHoneycomb();
        Hide();
    }

    public void UpdateSliders(GameResAmount _honeyAmount, GameResAmount _pollenAmount)
    {
        GameResAmount honeyNeed = Mng.play.kHive.mQueenHoneyNeed;
        GameResAmount pollenNeed = Mng.play.kHive.mQueenPollenNeed;

        kHoneySlider.value = Mng.play.GetResourcePercent(_honeyAmount, honeyNeed) / 100f;
        kPollenSlider.value = Mng.play.GetResourcePercent(_pollenAmount, pollenNeed) / 100f;

        kHoneyText.text = Mng.canvas.GetAmountText(_honeyAmount) + "\n/\n" + Mng.canvas.GetAmountText(honeyNeed);
        kPollenText.text = Mng.canvas.GetAmountText(_pollenAmount) + "\n/\n" + Mng.canvas.GetAmountText(pollenNeed);

        bool isQueenInvenEnough = Mng.play.CompareResourceAmounts(honeyNeed, _honeyAmount) && Mng.play.CompareResourceAmounts(pollenNeed, _pollenAmount);

        if(isQueenInvenEnough && Mng.play.kHive.mActiveQueenBee.mCurState == QueenState.Wander)
        {
            kEggButton.enabled = true;
            kEggButtonCover.SetActive(false);
        }
        else
        {
            kEggButton.enabled = false;
            kEggButtonCover.SetActive(true);
        }
    }

    public void Hide()
    {
        mTargetQueen = null;

        if(Mng.play.kHive.mIsBuilding == false)
            kQueenBtn.interactable = true;
        base.Hide();        
    }
}
