using EnumDef;
using StructDef;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainCanvas : MonoBehaviour
{
    static public MainCanvas Instance;

    public TMP_Text kWarningTxt;

    public UIResourceInfoPanel kResource;
    public BuildMenuPanel kBuild;
    public JobMenuPanel kJob;

    public Button[] kToggleButtons;

    [HideInInspector] public bool kIsViewingMenu = false;

    public GameObject kCancelBuildObj;  

    private bool mIsFading = false;

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
        kWarningTxt.gameObject.SetActive(false);
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

    public string GetAmountText(GameResAmount _amount)
    {
        return _amount.amount.ToString("#.00") + Mng.canvas.GetUnitText(_amount.unit);
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

    public void DisplayWarning(string _warning)
    {   
        kWarningTxt.gameObject.SetActive(true);
        kWarningTxt.text = _warning;

        if(mIsFading == false)
        {
            StartCoroutine(FadeTextCor());
        }
        else
        {
            kWarningTxt.color = new Color(1, 1, 1, 1);
        }
    }

    private IEnumerator FadeTextCor()
    {
        mIsFading = true;

        kWarningTxt.color = new Color(1, 1, 1, 1);

        while (kWarningTxt.color.a > 0.0f)
        {
            kWarningTxt.color = new Color(kWarningTxt.color.r, kWarningTxt.color.g, kWarningTxt.color.b, kWarningTxt.color.a - (Time.deltaTime / 1.2f));
            yield return null;
        }

        kWarningTxt.gameObject.SetActive(false);

        mIsFading = false;
    }
}
