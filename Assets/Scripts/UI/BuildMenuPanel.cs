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
        gameObject.SetActive(false);
    }

    public void OnBuildBtnClick()
    {
        gameObject.SetActive(false);
        Mng.play.kHive.SetDrawBuild(StructureType.Storage);

    }
}
