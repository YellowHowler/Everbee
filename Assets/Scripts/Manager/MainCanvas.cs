using EnumDef;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MainCanvas : MonoBehaviour
{
    static public MainCanvas Instance;

    public UIResourceInfoPanel kResource;
    public BuildMenuPanel kBuild;
    public JobMenuPanel kJob;

    public Button[] kToggleButtons;

    [HideInInspector] public bool kIsViewingMenu = false;

    public GameObject kCancelBuildObj;  

    /*
    public UIIntroPanel kIntro;
    public UIPlayInfoPanel kPlayInfo;
    public UIMenuPanel kMenu;
    public UIResultPanel kResult;
    */
    private void Awake()
    {
        Instance = this;

        kResource = GetComponentInChildren<UIResourceInfoPanel>(true);
        kBuild = GetComponentInChildren<BuildMenuPanel>(true);
        kJob = GetComponentInChildren<JobMenuPanel>(true);
        
        EnableToggleButtons();
        kCancelBuildObj.SetActive(false);
    }

    public void DisableToggleButtons() 
    {
        for(int i = 0; i < kToggleButtons.Length; i++)
        {
            kToggleButtons[i].interactable = false;
        }
    }
    public void EnableToggleButtons() 
    {
        for(int i = 0; i < kToggleButtons.Length; i++)
        {
            kToggleButtons[i].interactable = true;
        }
    }

    public string GetUnitText(GameResUnit _unit)
    {
        if(_unit == GameResUnit.Microgram)
        {
            return "ug";
        }
        else if (_unit == GameResUnit.Milligram)
        {
            return "mg";
        }
        else if (_unit == GameResUnit.Gram)
        {
            return "g";
        }
        else
        {
            return "kg";
        }
    }

    public void ShowBuildCancel()
    {
        kCancelBuildObj.SetActive(true);
    }

    public void HideBuildCancel()
    {
        kCancelBuildObj.SetActive(false);
    }

    public void CancelBuild()
    {
        Mng.play.kHive.EndBuild();
    }
}
