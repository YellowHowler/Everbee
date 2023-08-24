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

    public Sprite[] kResourceSprites;
    public Sprite kEmptySprite;

    public TMP_Text kWarningTxt;

    public UIResourceInfoPanel kResource;
    public BuildMenuPanel kBuild;
    public JobMenuPanel kJob;
    public InventoryBarPanel kInven;
    public QueenResourcePanel kQueen;
    public BeeInfoPanel kBeeInfo;
    public MenuTogglePanel kMenuToggle;
    public ResourceConvertPanel kConvert;
    public LHS.CLHSDialogUI kDialoguePopup;

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
        kMenuToggle = GetComponentInChildren<MenuTogglePanel>(true);
        kConvert = GetComponentInChildren<ResourceConvertPanel>(true);

        kResource.gameObject.SetActive(true);
        kInven.gameObject.SetActive(true);

        kBuild.gameObject.SetActive(false);
        kJob.gameObject.SetActive(false);
        kQueen.gameObject.SetActive(false);
        kBeeInfo.gameObject.SetActive(false);
        
        EnableToggleButtons();
        kCancelBuildObj.SetActive(false);
        kWarningTxt.gameObject.SetActive(false);

        kDialoguePopup.Init();
        kDialoguePopup.gameObject.SetActive(false);
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

    public Sprite GetResourceTypeIcon(GameResType _type)
    {
        switch(_type)
        {
            case GameResType.Honey:
                return kResourceSprites[0];
            case GameResType.Nectar:
                return kResourceSprites[1];
            case GameResType.Pollen:
                return kResourceSprites[2];
            case GameResType.Wax:
                return kResourceSprites[3];
            case GameResType.Empty:
                return kEmptySprite;
            default:
                return kEmptySprite;
        }
    }

    public void SpawnItemAtMousePos(GameResType _type, GameResAmount _amount, ItemLoc _prevLoc, int _prevSlot)
    {
        Vector3 spawnPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        spawnPos = new Vector3(spawnPos.x, spawnPos.y, 0);

        Item item = Instantiate(Mng.play.kHive.kItemObj, Camera.main.ScreenToWorldPoint(Input.mousePosition), Quaternion.identity, Mng.play.kHive.kItems).GetComponent<Item>();
        item.InitProperties(_type, _amount, _prevLoc, _prevSlot);

        Mng.play.kHive.mPlaceItem = item;
        Mng.play.kHive.mIsPlacingItem = true;
    }

    public string GetResourceTypeText(GameResType _type, bool _isStart)
    {
        if(_isStart == false)
        {
            switch(_type)
            {
                case GameResType.Honey:
                    return "honey";
                case GameResType.Nectar:
                    return "nectar";
                case GameResType.Pollen:
                    return "pollen";
                case GameResType.Wax:
                    return "wax";
                default:
                    return "";
            }
        }
        else
        {
            switch(_type)
            {
                case GameResType.Honey:
                    return "Honey";
                case GameResType.Nectar:
                    return "Nectar";
                case GameResType.Pollen:
                    return "Pollen";
                case GameResType.Wax:
                    return "Wax";
                default:
                    return "";
            }
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
        return _amount.amount.ToString("#.0") + Mng.canvas.GetUnitText(_amount.unit);
    }

    public string GetAmountRatioText(GameResAmount _amount, GameResAmount _maxAmount)
    {
        return GetAmountText(_amount) + " / " + GetAmountText(_maxAmount);
    }

    public string GetBeeThinkingText(BeeThinking _thinking)
    {
        switch(_thinking)
        {
            case BeeThinking.None:                       return  ""; 

            case BeeThinking.MovingToFlower:             return  "Moving to flower"; 
            case BeeThinking.MovingToStorage:            return  "Moving to storage"; 
            case BeeThinking.MovingToLarvae:             return  "Moving to larvae"; 
            case BeeThinking.MovingToBuild :             return  "Moving to build"; 

            case BeeThinking.CollectingFromFlower:       return  "Collecting from flower"; 
            case BeeThinking.Feeding:                    return  "Feeding"; 
            case BeeThinking.Building:                   return  "Building"; 

            case BeeThinking.NoAvailableFlower:          return  "No available flower"; 
            case BeeThinking.NoAvailableNectarStorage:   return  "No available nectar storage"; 
            case BeeThinking.NoAvailablePollenStorage:   return  "No available pollen storage"; 
            case BeeThinking.NoAvailableHoneyStorage:    return  "No available honey storage"; 
            case BeeThinking.NoAvailableWaxStorage:      return  "No available wax storage"; 
            case BeeThinking.NoAvailableLarvae:          return  "No available larvae"; 
            case BeeThinking.NoBuildingStructure:        return  "No current build"; 


            case BeeThinking.NoPollenInThisStorage:      return  "No pollen in target storage"; 
            case BeeThinking.NoHoneyInThisStorage:       return  "No honey in target storage"; 
            case BeeThinking.NoWaxInThisStorage:         return  "No wax in target storage"; 

            default:                                     return "";
        }

        return "";
    }

    public void ShowBuildCancel()
    {
        kCancelBuildObj.SetActive(true);
    }

    public void HideBuildCancel()
    {
        kCancelBuildObj.SetActive(false);
    }

    public void EndBuild()
    {
        kInven.gameObject.SetActive(true);
        Mng.play.kHive.mIsBuilding = false;
        Mng.play.kHive.mMouseOverBuildCancel = false;
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
        Mng.play.kHive.mIsBuilding = false;
        if( Mng.play.kHive.mIsPlacingItem == true)
        {
            Mng.play.kHive.mPlaceItem.CancelPlace();
        }
        kBeeInfo.Hide();
    }

    public void HideMenu()
    {
    }
}
