using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using StructDef;
using EnumDef;

public class UIResourceInfoPanel : MonoBehaviour
{
    public MainCanvas kMainCanvas;

    public TMP_Text kHoneyText;
    public TMP_Text kNectarText;
    public TMP_Text kPollenText;
    public TMP_Text kWaxText;

    // Start is called before the first frame update
    void Start()
    {
        kHoneyText.text = "0ug";
        kNectarText.text = "0ug";
        kPollenText.text = "0ug";
        kWaxText.text = "0ug";
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateText()
    {
        kPollenText.text = Mng.play.kStoragePollenAmount.amount.ToString("#.00") + kMainCanvas.GetUnitText(Mng.play.kStoragePollenAmount.unit);
        kNectarText.text = Mng.play.kStorageNectarAmount.amount.ToString("#.00") + kMainCanvas.GetUnitText(Mng.play.kStorageNectarAmount.unit);
        kHoneyText.text = Mng.play.kStorageHoneyAmount.amount.ToString("#.00") + kMainCanvas.GetUnitText(Mng.play.kStorageHoneyAmount.unit);
        kWaxText.text = Mng.play.kStorageWaxAmount.amount.ToString("#.00") + kMainCanvas.GetUnitText(Mng.play.kStorageWaxAmount.unit);
    }
}
