using EnumDef;
using StructDef;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using TMPro;

public class Honeycomb : MonoBehaviour
{ 
    public SpriteRenderer kSpriteRenderer;

    public Vector3 pos { get { return transform.position; } set { transform.position = value; } }
    public CTargetLink<Honeycomb, Bee> mTargetBee;

    public GameResType type; //���� �ڿ��� �����ϰ� �ִ���
    public GameResAmount amount = new GameResAmount(0f, GameResUnit.Microgram);

    public Hive mHive { get; set; }

    public StructureType kStructureType;

    [HideInInspector] public StructureType mStartStructureType = StructureType.None;

    private StructureType mTargetStructureType = StructureType.None;
    private StructureType mPrevStructureType = StructureType.None;
    private GameResAmount mBuildNeedWaxAmount = new GameResAmount(0f, GameResUnit.Microgram);
    private GameResAmount mCurWaxAmount = new GameResAmount(0f, GameResUnit.Microgram);

    public GameObject kEmptyObj;
    public GameObject kStorageObj;
    public GameObject kDryerObj;
    public GameObject kHatchteryObj;
    public GameObject kCoalgulateObj;
    public GameObject kHoverObj;
    public GameObject kBuildObj;

    public Animator kDryerAni;

    public GameObject kCanvas;

    public GameObject kTimerPanel;
    public GameObject kButtonPanel;
    public GameObject kBuildPanel;

    [HideInInspector] public bool mIsOpen = false;
    private bool mIsConverting = false;

    public Sprite[] kDryerSprites;
    public Sprite[] kCoalgulateSprites;

    public ParticleSystem kParticle;


    private void Awake()
    {
        mTargetBee = new CTargetLink<Honeycomb, Bee>(this);

		kCanvas.SetActive(true);
		kCanvas.GetComponent<Canvas>().worldCamera = Camera.main;

        kParticle.Stop();
        HideObjects();
    }

    private GameResAmount GetMaxHoneyAmount() { return Mng.play.kHive.mMaxItemAmounts[0]; }
    private GameResAmount GetMaxNectarAmount() { return Mng.play.kHive.mMaxItemAmounts[1]; }
    private GameResAmount GetMaxPollenAmount() { return Mng.play.kHive.mMaxItemAmounts[2]; }
    private GameResAmount GetMaxWaxAmount() { return Mng.play.kHive.mMaxItemAmounts[3]; }

	public void InitDefault()
    {
        SetStructure(mStartStructureType, true);
	}

	private void HideObjects()
    {
        kEmptyObj.SetActive(false);
        kHoverObj.SetActive(false);
        kTimerPanel.SetActive(false);
        kButtonPanel.SetActive(false);
        kBuildObj.SetActive(false);
    }

    public bool IsFull() //�� ���� �� ���ִ��� Ȯ��
    {
        GameResAmount maxAmount = GetMaxAmount(type);

        if (amount.unit != maxAmount.unit)
        {
            return (int)amount.unit >= (int)maxAmount.unit;
        }
        else
        {
            return (int)amount.amount >= (int)maxAmount.amount;
        }
    }

    public bool IsUsable(GameResType _type)
    {
        if(amount.amount < 0.1f)
        {
            type = GameResType.Empty;
        }

        if(type == GameResType.Empty || amount.amount == 0f)
        {
            return true;
        }
        if(type == _type && !IsFull()) 
        {
            return true;
        }

        return false;
    }

    private void SetAllChildrenActive(bool _setActive)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject child = transform.GetChild(i).gameObject;
            if (child.tag == "HoneycombSprite") child.SetActive(_setActive);
        }

        kParticle.gameObject.SetActive(true);
        kCanvas.SetActive(true);
    }

    public Sprite GetSpriteOfStructure(StructureType _type)
    {
        switch (_type)
        {
            case StructureType.None:
                return null;
            case StructureType.Storage:
                return kStorageObj.GetComponent<SpriteRenderer>().sprite;
            case StructureType.Dryer:
                return kDryerObj.GetComponent<SpriteRenderer>().sprite;
			case StructureType.Coalgulate:
				return kCoalgulateObj.GetComponent<SpriteRenderer>().sprite;
			case StructureType.Hatchtery:
                return kHatchteryObj.GetComponent<SpriteRenderer>().sprite;
            default:
                return null;
        }
    }

    public void ExpandHoneycombs()
    {
        Hive hive = Mng.play.kHive;

        for(int i = 1; i <= 6; i++)
        {
            hive.AddNewHoneycomb(hive.GetHexagonPos(pos, (HoneycombDirection)i), false);
        }
    }

    public bool SetStructure(StructureType _type, bool forced) //true if build successful
    {
        kBuildPanel.SetActive(false);
        kBuildObj.SetActive(false);

        switch (_type)
        {
            case StructureType.None:
                SetAllChildrenActive(false);
                break;
            case StructureType.Building:
                SetStructure(mPrevStructureType, forced);
                PrepareBuildStructure(mTargetStructureType);
                break;
            case StructureType.Storage:
                SetAllChildrenActive(false);
                kStorageObj.SetActive(true);

                if (!forced)    // forced 는 첫 시작할 때 및 로딩할 때이므로 forced == true 이면 expand 하지 않음
                    ExpandHoneycombs();
                break;
            case StructureType.Dryer:
                SetAllChildrenActive(false);
                kStorageObj.SetActive(true);
                kDryerObj.SetActive(true);
                break;
			case StructureType.Coalgulate:
				SetAllChildrenActive(false);
				kStorageObj.SetActive(true);
				kCoalgulateObj.SetActive(true);
				break;
			case StructureType.Hatchtery:
                SetAllChildrenActive(false);
                kHatchteryObj.SetActive(true);
                break;
        }

        kStructureType = _type;

        return true;
    }

    public bool PrepareBuildStructure(StructureType _type)
    {
        mCurWaxAmount = new GameResAmount(0f, GameResUnit.Microgram);
        mBuildNeedWaxAmount = Mng.play.kHive.mWaxCosts[_type];

        switch (_type)
        {
            case StructureType.None:
                break;
            case StructureType.Storage:
                if(kStructureType != StructureType.None) 
                {
                    Mng.canvas.DisplayWarning("Must place on empty space");
                    return false;
                }
                break;
            case StructureType.Dryer:
                if(kStructureType != StructureType.Storage) 
                {
                    Mng.canvas.DisplayWarning("Must upgrade from storage");
                    return false;
                }
                if(Mng.play.IsAmountZero(amount) == false)
                {
                    Mng.canvas.DisplayWarning("Storage must be empty");
                    return false;
                }
                break;
			case StructureType.Coalgulate:
				if(kStructureType != StructureType.Storage)
				{
                    Mng.canvas.DisplayWarning("Must upgrade from storage");
					return false;
				}
                if(Mng.play.IsAmountZero(amount) == false)
                {
                    Mng.canvas.DisplayWarning("Storage must be empty");
                    return false;
                }
				break;
			case StructureType.Hatchtery:
                if(kStructureType != StructureType.Storage)
				{
                    Mng.canvas.DisplayWarning("Must upgrade from storage");
					return false;
				}
                if(Mng.play.IsAmountZero(amount) == false)
                {
                    Mng.canvas.DisplayWarning("Storage must be empty");
                    return false;
                }
                break;
        }

        kCanvas.SetActive(true);
        kBuildPanel.SetActive(true);
        kBuildObj.SetActive(true);
        kBuildObj.GetComponent<SpriteRenderer>().sprite = GetSpriteOfStructure(_type);

        HoneycombBuildPanel buildPanel = kBuildPanel.GetComponent<HoneycombBuildPanel>();

        mPrevStructureType = kStructureType;
        kStructureType = StructureType.Building;
        mTargetStructureType = _type;
        
        UpdateWaxAmount(new GameResAmount(0, GameResUnit.Microgram));
        buildPanel.UpdateUI(mCurWaxAmount, mBuildNeedWaxAmount);

        return true;
    }

    public GameResAmount UpdateWaxAmount(GameResAmount _amount)
    {
        mCurWaxAmount = Mng.play.AddResourceAmounts(_amount, mCurWaxAmount);
        
        GameResAmount retAmount = new GameResAmount(0f, GameResUnit.Microgram);

        if(Mng.play.CompareResourceAmounts(mBuildNeedWaxAmount, mCurWaxAmount) || Mng.play.IsSameAmount(mBuildNeedWaxAmount, mCurWaxAmount))
        {
            retAmount = Mng.play.SubtractResourceAmounts(mCurWaxAmount, mBuildNeedWaxAmount);
            mCurWaxAmount = mBuildNeedWaxAmount;

            SetStructure(mTargetStructureType, false);
        }

        HoneycombBuildPanel buildPanel = kBuildPanel.GetComponent<HoneycombBuildPanel>();
        buildPanel.UpdateUI(mCurWaxAmount, mBuildNeedWaxAmount);

        return retAmount;
    }

    private GameResAmount GetMaxAmount(GameResType _type)
    {
        GameResAmount maxAmount = new GameResAmount(0f, GameResUnit.Microgram);

        if (_type == GameResType.Nectar)
        {
            maxAmount = GetMaxNectarAmount();
        }
        else if (_type == GameResType.Pollen)
        {
            maxAmount = GetMaxPollenAmount();
        }
        else if (_type == GameResType.Honey)
        {
            maxAmount = GetMaxHoneyAmount();
        }
        else if (_type == GameResType.Wax)
        {
            maxAmount = GetMaxWaxAmount();
        }

        return maxAmount;
    }

    /// <summary> �Ķ���� ����ŭ ����, ��������� �����Ѵٸ� �����Ѹ�ŭ ��ȯ </summary>
    public GameResAmount StoreResource(GameResType _type, GameResAmount _amount)
    {
        GameResAmount maxAmount = GetMaxAmount(_type);

        if (type == GameResType.Empty || type == _type)
        {
            UpdateType(_type);
        }
        else
        {
            UpdateSprite();
            return new GameResAmount(0f, GameResUnit.Microgram);
        }

        GameResAmount retAmount = Mng.play.AddResourceAmounts(_amount, amount);

        if (Mng.play.CompareResourceAmounts(retAmount, maxAmount) == true) 
        {
            UpdateAmount(retAmount);

            return new GameResAmount(0f, GameResUnit.Microgram);
        }

        retAmount = Mng.play.SubtractResourceAmounts(retAmount, maxAmount);
        
        UpdateAmount(maxAmount);

        return retAmount;
    }

    public GameResAmount FetchResource(GameResType _type, GameResAmount _amount, GameResAmount _maxAmount)
    {
        GameResAmount retAmount = new GameResAmount(0f, GameResUnit.Microgram);

        if(Mng.play.CompareResourceAmounts(_maxAmount, amount) == true)
        {
            UpdateAmount(Mng.play.SubtractResourceAmounts(amount, _maxAmount));

            return _maxAmount;
        }

        retAmount = amount;

        Mng.play.SubtractResourceFromStorage(_type, amount);
        UpdateAmount(new GameResAmount(0f, GameResUnit.Microgram));

        return retAmount;
    }

    public void UpdateAmount(GameResAmount _amount)
    {
        if(Mng.play.CompareResourceAmounts(amount, _amount))
        {
            Mng.play.AddResourceToStorage(type, Mng.play.SubtractResourceAmounts(_amount, amount));
        }
        else
        {
            Mng.play.SubtractResourceFromStorage(type, Mng.play.SubtractResourceAmounts(amount, _amount));
        }

        amount = _amount;

        if(amount.amount < 0.1f) 
        {
            UpdateType(GameResType.Empty);
        }
        
        UpdateSprite();
    }

    public void UpdateType(GameResType _type)
    {
        type = _type;
        UpdateSprite();
    }

    public void UpdateSprite()
    {
        GameResAmount maxAmount = GetMaxAmount(type);

        int spriteNum = Mathf.Clamp((int)(Mng.play.GetResourcePercent(amount, maxAmount)/100 * (mHive.kHoneycombNectarSprites.Length - 1)), 1, mHive.kHoneycombNectarSprites.Length - 1);

        switch(type)
        {
            case GameResType.Empty:
                kSpriteRenderer.sprite = mHive.kHoneycombHoneySprites[0];
                break;
            case GameResType.Nectar:
                kSpriteRenderer.sprite = mHive.kHoneycombNectarSprites[spriteNum];
                break;
            case GameResType.Pollen:
                kSpriteRenderer.sprite = mHive.kHoneycombPollenSprites[spriteNum];
                break;
            case GameResType.Honey:
                kSpriteRenderer.sprite = mHive.kHoneycombHoneySprites[spriteNum];
                break;
            case GameResType.Wax:
                kSpriteRenderer.sprite = mHive.kHoneycombWaxSprites[spriteNum];
                break;
            default:
                break;
        }
    }

    private void OnDrawGizmos()
    {
    #if UNITY_EDITOR
        if (mHive != null)
        {
            if (mHive.kIsDrawHoneycombName == true)
                Handles.Label(transform.position, name);

            if (mHive.kIsDrawHoneycombShape == true)
                Gizmos.DrawWireSphere(transform.position, Mng.play.kHive.mHoneycombRadiusY);
        }
    #endif
    }

    private void ToggleDoor()
    {
        mIsOpen = !mIsOpen;

        if(mIsOpen)
        {
            switch(kStructureType)
            {
                case StructureType.Dryer:
					kDryerAni.SetBool("isOpen", mIsOpen);
                    break;

				case StructureType.Coalgulate:
					kCoalgulateObj.GetComponent<SpriteRenderer>().sprite = kCoalgulateSprites[1];
					break;
			}

			kCanvas.SetActive(true);
            kButtonPanel.SetActive(true);
            kTimerPanel.SetActive(false);
        }
        else
        {
			switch(kStructureType)
			{
				case StructureType.Dryer:
					kDryerObj.GetComponent<SpriteRenderer>().sprite = kDryerSprites[0];
					break;

				case StructureType.Coalgulate:
					kCoalgulateObj.GetComponent<SpriteRenderer>().sprite = kCoalgulateSprites[0];
					break;
			}

            kButtonPanel.SetActive(false);
        }
    }

    private void OnMouseEnter()
    {
        if(kStructureType == StructureType.None || PopupBase.IsTherePopup() || Mng.play.kHive.mIsBuilding) 
        {
            return;
        }

        Mng.play.kHive.mHoveredHoneycomb = this;
        kHoverObj.SetActive(true);
    }

    private void OnMouseExit()
    {
        if(Mng.play.kHive.mHoveredHoneycomb == this)
        {
            Mng.play.kHive.mHoveredHoneycomb = null;
        }
        kHoverObj.SetActive(false);
    }

    private void OnMouseDown()
    {
        if(PopupBase.IsTherePopup())
        {
            return;
        }

        Hive hive = Mng.play.kHive;

        if(hive.mIsPlacingItem == true)
        {
            return;
        }

        if(hive.mIsBuilding == true)
        {
            if(PrepareBuildStructure(hive.mStructureType) == true)
            {
                Mng.canvas.EndBuild();
            }
        }
        else
        {
            switch (kStructureType)
            {
                case StructureType.None:
                    break;
                case StructureType.Storage:

                    break;
                case StructureType.Dryer:
				case StructureType.Coalgulate:
					if(mIsConverting == true)
                    {
                        return;
                    }
                    ToggleDoor();
                    break;
            }
        }
    }

    public void CreateItem()
    {
        if(amount.amount == 0) 
        {
            return;
        }
        
        Item item = Mng.play.kInventory.CreateItem(type, amount, gameObject.transform.position);
        if (item != null)
        {
            UpdateAmount(item.UpdateAmount(type, amount));

            Mng.play.SubtractResourceFromStorage(item.type, item.amount);
            print(item.amount.amount);
        }
    }

    public void StoreItemInInven()
    {
        if(amount.amount == 0) 
        {
            return;
        }

        Inventory inven = Mng.play.kInventory;

        int targetSlot = inven.GetAvailableSlot(type);

        if(targetSlot == -1)
        {
            Mng.canvas.DisplayWarning("No available inventory slot");
            return;
        }

        GameResAmount sumAmount = Mng.play.AddResourceAmounts(inven.mItemSlots[targetSlot].amount, amount);

        if(Mng.play.CompareResourceAmounts(sumAmount, GetMaxAmount(type)) == true)
        {
            inven.UpdateSlotAmount(targetSlot, type, sumAmount);
            UpdateAmount(new GameResAmount(0, GameResUnit.Microgram));
        }
        else
        {
            inven.UpdateSlotAmount(targetSlot, type, GetMaxAmount(type));
            UpdateAmount(Mng.play.SubtractResourceAmounts(sumAmount, GetMaxAmount(type)));
        }
        
        return;
    }

    private void OnMouseUp()
    {
        Hive hive = Mng.play.kHive;

        if(PopupBase.IsTherePopup())
        {
            return;
        }

        if(hive.mIsPlacingItem == true)
        {
            return;
        }

        if(Mng.play.kHive.mIsBuilding == true)
        {

        }
        else
        {
            switch (kStructureType)
            {
                case StructureType.None:
                    break;
                case StructureType.Storage:
                    StoreItemInInven();
                    break;
                case StructureType.Dryer:
                    break;
            }
        }
    }

    public void ConvertResource()
    {
        if (kStructureType == StructureType.Dryer)
        {
            if(type == GameResType.Nectar && amount.amount > 0)
            {
                kButtonPanel.SetActive(false);
                kTimerPanel.SetActive(true);
                ToggleDoor();

                mIsConverting = true;

                StartCoroutine(ConvertCor(10, GameResType.Honey));
            }
            else if(amount.amount == 0)
            {
                Mng.canvas.DisplayWarning("Honeycomb is empty");
            }
            else if(type != GameResType.Nectar)
            {
                Mng.canvas.DisplayWarning("Dryer can only convert nectar");
            }
        }
        else if (kStructureType == StructureType.Coalgulate)
        {
			if(type == GameResType.Honey && amount.amount > 0)
			{
				kButtonPanel.SetActive(false);
				kTimerPanel.SetActive(true);
				ToggleDoor();

				mIsConverting = true;

				StartCoroutine(ConvertCor(10, GameResType.Wax));
			}
			else if(amount.amount == 0)
			{
				Mng.canvas.DisplayWarning("Honeycomb is empty");
			}
			else if(type != GameResType.Honey)
			{
				Mng.canvas.DisplayWarning("Coalgulate can only convert honey");
			}
		}
	}

    public void PlayParticles()
    {
        StartCoroutine(PlayParticlesCor());
    }
    private IEnumerator ConvertCor(int _time, GameResType _finType)
    {
        kDryerAni.SetBool("isConverting", true);

        WaitForSeconds sec = new WaitForSeconds(1);

        for(int i = _time; i >= 0; i--)
        {
            kTimerPanel.GetComponentInChildren<TMP_Text>().text = Mng.play.GetTimeText(i);
            yield return sec;
        }

        kTimerPanel.gameObject.SetActive(false);

        UpdateType(_finType);

        Mng.play.kHive.RecountAllResources();

        mIsConverting = false;
        kDryerAni.SetBool("isConverting", false);
    }   

    private IEnumerator PlayParticlesCor()
    {
        kParticle.Play();
        yield return new WaitForSeconds(0.5f);
        kParticle.Stop();
    }
    
    public void PlaceEgg()
    {
        UpdateType(GameResType.Egg);
        Mng.play.kBees.CreateBee(Mng.play.SetZ(transform.position, 0), 0, BeeStage.Egg);
    }


    // 세이브/로드 관련
    [Serializable]
    public class CSaveData
    {
		public Vector3 pos;

		public GameResType type;
		public GameResAmount amount;

		public StructureType kStructureType;

        public StructureType mTargetStructureType;
        public StructureType mPrevStructureType;
        public GameResAmount mBuildNeedWaxAmount;
        public GameResAmount mCurWaxAmount;
	}

    public void ExportTo(CSaveData savedata)
    {
        savedata.pos = pos;
        savedata.type = type;
        savedata.amount = amount;
        savedata.kStructureType = kStructureType;

        savedata.mTargetStructureType = mTargetStructureType;
        savedata.mPrevStructureType = mPrevStructureType;
        savedata.mBuildNeedWaxAmount = mBuildNeedWaxAmount;
        savedata.mCurWaxAmount = mCurWaxAmount;
    }

    public void ImportFrom(CSaveData savedata)
    {
        pos = savedata.pos;
        UpdateType(savedata.type);
        UpdateAmount(savedata.amount);
        kStructureType = savedata.kStructureType;

        mTargetStructureType = savedata.mTargetStructureType;
        mPrevStructureType = savedata.mPrevStructureType;
        mBuildNeedWaxAmount = savedata.mBuildNeedWaxAmount;
        mCurWaxAmount = savedata.mCurWaxAmount;

        SetStructure(kStructureType, true);
    }
}