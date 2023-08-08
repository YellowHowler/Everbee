using EnumDef;
using StructDef;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HoneycombBuildPanel : MonoBehaviour
{
    public Slider kWaxSlider;
    public TMP_Text kWaxText;

    public void UpdateUI(GameResAmount _curWax, GameResAmount _needWax)
    {
        kWaxSlider.value = Mng.play.GetResourcePercent(_curWax, _needWax)/100;
        kWaxText.text = Mng.canvas.GetAmountRatioText(_curWax, _needWax);
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
