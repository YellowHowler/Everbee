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

    public void UpdateText(GameResType _type, GameResAmount _amount)
    {
        string newText = _amount.amount.ToString("#.00") + kMainCanvas.GetUnitText(_amount.unit);

        if(_type == GameResType.Pollen)
        {
            kPollenText.text = newText;
        }
        else if (_type == GameResType.Nectar)
        {
            kNectarText.text = newText;
        }
        else if (_type == GameResType.Wax)
        {
            kWaxText.text = newText;
        }
        else if (_type == GameResType.Honey)
        {
            kHoneyText.text = newText;
        }
    }
}
