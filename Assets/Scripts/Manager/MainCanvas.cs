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
    static public MainCanvas Instance { get; private set; }

    public TMP_Text kWarningTxt;

    public UIResourceInfoPanel kResource;
    public BuildMenuPanel kBuild;
    public JobMenuPanel kJob;
    public InventoryBarPanel kInven;
    public QueenResourcePanel kQueen;
    public BeeInfoPanel kBeeInfo;

    public Button[] kToggleButtons;

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
        kInven = GetComponentInChildren<InventoryBarPanel>(true);
        kQueen = GetComponentInChildren<QueenResourcePanel>(true);
        kBeeInfo = GetComponentInChildren<BeeInfoPanel>(true);

        kResource.gameObject.SetActive(true);
        kInven.gameObject.SetActive(true);

        kBuild.gameObject.SetActive(false);
        kJob.gameObject.SetActive(false);
        kQueen.gameObject.SetActive(false);
        kBeeInfo.gameObject.SetActive(false);
        
        EnableToggleButtons();
        kCancelBuildObj.SetActive(false);
        kWarningTxt.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        Instance = null;
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

    public string GetAmountRatioText(GameResAmount _amount, GameResAmount _maxAmount)
    {
        return GetAmountText(_amount) + " / " + GetAmountText(_maxAmount);
    }

    public void ShowBuildCancel()
    {
        kCancelBuildObj.SetActive(true);
    }

    public void HideBuildCancel()
    {
        kCancelBuildObj.SetActive(false);
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

        yield return new WaitForSeconds(0.4f);

        while (kWarningTxt.color.a > 0.0f)
        {
            kWarningTxt.color = new Color(kWarningTxt.color.r, kWarningTxt.color.g, kWarningTxt.color.b, kWarningTxt.color.a - (Time.deltaTime / 1.2f));
            yield return null;
        }

        kWarningTxt.gameObject.SetActive(false);

        mIsFading = false;
    }

    public void FillSlider(Slider _slider, float _time)
    {   
        StartCoroutine(FillSliderCor(_slider, _time));
    }

    private IEnumerator FillSliderCor(Slider _slider, float _time)
    {
        _slider.gameObject.SetActive(true);

        float timeInterval = 0.1f;
        WaitForSeconds sec = new WaitForSeconds(timeInterval);
        
        for(float i = 0; i < _time; i += timeInterval)
        {
            _slider.value = i/_time;
            yield return sec;
        }

        _slider.gameObject.SetActive(false);
    }

    public void ShowMenu()
    {
        kBeeInfo.Hide();
    }

    public void HideMenu()
    {
    }
}
