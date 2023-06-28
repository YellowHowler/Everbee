using EnumDef;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildMenuPanel : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void OnBuildMenuBgClick()
    {
        Mng.canvas.HideMenu();
        
        gameObject.SetActive(false);
    }

    public void OnBuildBtnClick(int _type)
    {
        gameObject.SetActive(false);
        Mng.canvas.kInven.gameObject.SetActive(false);
        Mng.play.kHive.SetDrawBuild((StructureType)_type);
    }
}
