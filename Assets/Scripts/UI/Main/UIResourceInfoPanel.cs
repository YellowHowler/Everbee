using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using StructDef;
using EnumDef;

public class UIResourceInfoPanel : MonoBehaviour
{
    public MainCanvas kMainCanvas;

    public TMP_Text[] kResourceTexts;

    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < kResourceTexts.Length; ++i)
        {
            kResourceTexts[i].text = "0ug";
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateText()
    {
        for(int i = 0; i < kResourceTexts.Length; i++)
        {
            kResourceTexts[i].text = Mng.play.kStorageResourceAmounts[i].amount.ToString("#.00") + kMainCanvas.GetUnitText(Mng.play.kStorageResourceAmounts[i].unit);
        }
    }
}
