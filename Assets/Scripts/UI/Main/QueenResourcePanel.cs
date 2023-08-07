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

    public Button kQueenBtn;

    public Slider kHoneySlider;
    public Slider kPollenSlider;

    public TMP_Text kHoneyText;
    public TMP_Text kPollenText;

    public Button kEggButton;
    
    void Start()
    {
    }

    void Update()
    {
        
    }

	override public void ProcessEscapeKey()
	{
		OnQueenResourceBgClick();
		base.ProcessEscapeKey();
	}

	public void QueenLayEgg()
    {
        mTargetQueen.WaitForTargetHoneycomb();
        OnQueenResourceBgClick();
    }

    public void UpdateSliders(GameResAmount _honeyAmount, GameResAmount _pollenAmount)
    {
        GameResAmount honeyNeed = Mng.play.kHive.mQueenHoneyNeed;
        GameResAmount pollenNeed = Mng.play.kHive.mQueenPollenNeed;

        kHoneySlider.value = Mng.play.GetResourcePercent(_honeyAmount, honeyNeed) / 100f;
        kPollenSlider.value = Mng.play.GetResourcePercent(_pollenAmount, pollenNeed) / 100f;

        kHoneyText.text = Mng.canvas.GetAmountText(_honeyAmount) + "/" + Mng.canvas.GetAmountText(honeyNeed);
        kPollenText.text = Mng.canvas.GetAmountText(_pollenAmount) + "/" + Mng.canvas.GetAmountText(pollenNeed);

        if(Mng.play.CompareResourceAmounts(honeyNeed, _honeyAmount) == true && Mng.play.CompareResourceAmounts(pollenNeed, _pollenAmount) == true)
        {
            kEggButton.enabled = true;
        }
        else
        {
            kEggButton.enabled = false;
        }
    }

    public void OnQueenResourceBgClick()
    {
        kQueenBtn.interactable = true;
        Mng.canvas.HideMenu();
        Hide();
    }
}
