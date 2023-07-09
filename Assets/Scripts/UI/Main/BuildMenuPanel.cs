using EnumDef;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildMenuPanel : PopupBase
{
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    override public void ProcessEscapeKey()
    {
        OnBuildMenuBgClick();
        base.ProcessEscapeKey();
    }

    public void OnBuildMenuBgClick()
    {
        Mng.canvas.HideMenu();
        Hide();
    }

    public void OnBuildBtnClick(int _type)
    {
        Hide();
        Mng.canvas.kInven.gameObject.SetActive(false);
        Mng.play.kHive.SetDrawBuild((StructureType)_type);
    }
}
