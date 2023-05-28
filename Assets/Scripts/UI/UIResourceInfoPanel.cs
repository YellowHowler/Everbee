using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIResourceInfoPanel : MonoBehaviour
{
    public TMP_Text kText;    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetText(string _txt)
    {
        kText.text = _txt;
    }
}
