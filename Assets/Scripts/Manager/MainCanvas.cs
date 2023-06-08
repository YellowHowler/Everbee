using EnumDef;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MainCanvas : MonoBehaviour
{
    static public MainCanvas Instance;

    public UIResourceInfoPanel kResource;
    public BuildMenuPanel kBuild;
    public JobMenuPanel kJob;

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
        /*
        kIntro = GetComponentInChildren<UIIntroPanel>(true);
        kPlayInfo = GetComponentInChildren<UIPlayInfoPanel>(true);
        kMenu = GetComponentInChildren<UIMenuPanel>(true);
        kResult = GetComponentInChildren<UIResultPanel>(true);

        kPlayInfo.gameObject.SetActive(false);
        kMenu.gameObject.SetActive(false);
        kResult.gameObject.SetActive(false);
        */
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
}
