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

    public Slider[] kStorageSliders;
    public TMP_Text[] kStorageTexts;

    public Button kFeedButton;

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
        switch(_obj.tag)
        {
            case "Bee":
                Bee targetBee = _obj.GetComponent<Bee>();

                if(Mng.play.CompareResourceAmounts(targetBee.mCurrentHoney, targetBee.mFeedAmount) == true)
                {
                    kFeedButton.enabled = false;
                }
                else
                {
                    kFeedButton.enabled = true;
                }

                kStorageSliders[0].value = Mng.play.GetResourcePercent(targetBee.mCurrentHoney, targetBee.mMaxHoney)/100;
                kStorageTexts[0].text = Mng.canvas.GetAmountRatioText(targetBee.mCurrentHoney, targetBee.mMaxHoney);

                kStorageSliders[1].value = Mng.play.GetResourcePercent(targetBee.mCurrentNectar, targetBee.mMaxNectar)/100;
                kStorageTexts[1].text = Mng.canvas.GetAmountRatioText(targetBee.mCurrentNectar, targetBee.mMaxNectar);

                kStorageSliders[2].value = Mng.play.GetResourcePercent(targetBee.mCurrentPollen, targetBee.mMaxPollen)/100;
                kStorageTexts[2].text = Mng.canvas.GetAmountRatioText(targetBee.mCurrentPollen, targetBee.mMaxPollen);
                
                kStorageSliders[3].value = Mng.play.GetResourcePercent(targetBee.mCurrentWax, targetBee.mMaxWax)/100;
                kStorageTexts[3].text = Mng.canvas.GetAmountRatioText(targetBee.mCurrentWax, targetBee.mMaxWax);

                kLevelText.text = "Level " + targetBee.kLevel;
                kLevelSlider.value = targetBee.kExp;

                break;
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
}
