using EnumDef;
using StructDef;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using TMPro;

public class Item : MonoBehaviour
{
    [SerializeField] SpriteRenderer itemSprite;
    [SerializeField] TMP_Text valueText;
    [SerializeField] Sprite[] itemSprites;

    public GameResType type = GameResType.Honey;
    public GameResAmount amount = new GameResAmount(0f, GameResUnit.Microgram);
    public float XPosition { get { return transform.localPosition.x; } set { Vector3 pos = transform.localPosition; pos.x = value; transform.localPosition = pos; } }

    private bool mIsDropped = false;

    private List<GameObject> mTouchingObjs;

    private GameObject mQueen;
    private bool mIsTouchingQueen = false;

    [HideInInspector] public bool mCanMerge = true;

    [HideInInspector] public ItemLoc mPrevLoc = ItemLoc.None;
    [HideInInspector] public int mPrevSlot = -1;

    private void Awake()
    {
        mTouchingObjs = new List<GameObject>();

        Mng.play.kHive.mIsPlacingItem = true;
        Mng.play.kHive.mPlaceItem = this;
    }

    private IEnumerator Start()
    {
        if(transform.position.y < Mng.play.kHive.mFloorY)
        {
            transform.position = new Vector3(transform.position.x, Mng.play.kHive.mFloorY, 0);
        }

        yield return new WaitForSeconds(0.1f);
        mIsDropped = true;
    }

    public void InitProperties(GameResType _type, GameResAmount _amount, ItemLoc _prevLoc, int _prevSlot)
    {
        UpdateAmount(_type, _amount);
        mPrevLoc = _prevLoc;
        mPrevSlot = _prevSlot;
    }

    private void Update()
    {
        transform.position = Camera.main.ScreenToWorldPoint(Mng.play.SetZ(Input.mousePosition, 0));

        if(Input.GetMouseButtonUp(0))
        {
            if(mIsDropped == false)
            {
                return;
            }

            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Mng.play.SetZ(Input.mousePosition, 0));
            
            if(PutInInven())
            {
                return;
            }
            if(PutInConvert())
            {
                return;
            }
            if(GiveToQueen())
            {
                return;
            }
            if(GiveToBee())
            {
                return;
            }
            if(GiveToHoneycomb())
            {
                return;
            }
            if(GiveToFlower())
            {
                return;
            }

            CancelPlace();
        }
    }

    public bool PutInConvert()
    {
        ResourceConvertPanel convertPanel = Mng.canvas.kConvert;

        if(convertPanel.mIsPrevHover == false || convertPanel.IsPrevUsable(type) == false)
            return false;
        
        UpdateAmount(type, convertPanel.AddPrevAmount(type, amount));
        CancelPlace();

        return true;
    }

    public bool PutInInven()
    {
        InventoryBarPanel invenPanel = Mng.canvas.kInven;
        Inventory inven = Mng.play.kInventory;

        if(invenPanel.mHoveredNum != -1 && (inven.CheckIfSlotUsable(invenPanel.mHoveredNum, type)))
        {
            GameResAmount sumAmount = Mng.play.AddResourceAmounts(Mng.play.kInventory.mItemSlots[invenPanel.mHoveredNum].amount, amount);
            if(Mng.play.CompareResourceAmounts(sumAmount, GetMaxAmount(type)) == true)
            {
                inven.UpdateSlotAmount(invenPanel.mHoveredNum, type, sumAmount);
                UpdateAmount(type, new GameResAmount(0, GameResUnit.Microgram));
                CancelPlace();
            }
            else
            {
                Mng.play.kInventory.UpdateSlotAmount(invenPanel.mHoveredNum, type, GetMaxAmount(type));
                UpdateAmount(type, Mng.play.SubtractResourceAmounts(sumAmount, GetMaxAmount(type)));
                CancelPlace();
            }

            return true;
        }

        return false;
    }

    public bool GiveToBee()
    {
        Bee storeBee = Mng.play.kHive.mHoveredBee;

        if(storeBee != null)
        {
            if(storeBee.mCurStage == BeeStage.Egg || storeBee.mCurStage == BeeStage.Pupa || storeBee.IsStorageFull(type))
            {
                return false;
            }
            Mng.play.kHive.PlayGiveParticles(storeBee.gameObject.transform);
            UpdateAmount(type, storeBee.AddResource(type, amount));
            CancelPlace();
            return true;
        }

        return false;
    }

    public bool GiveToQueen()
    {
        if(type != GameResType.Pollen && type != GameResType.Honey)
        {
            return false;
        }

        QueenBee storeQueen = Mng.play.kHive.mHoveredQueenBee;

        if(storeQueen != null)
        {
            storeQueen.AddResource(type, amount);
            Mng.play.kHive.PlayGiveParticles(storeQueen.gameObject.transform);
            UpdateAmount(type, new GameResAmount(0f, GameResUnit.Microgram));
            return true;
        }

        return false;
    }

    public bool GiveToHoneycomb()
    {
        Honeycomb storeHoneycomb = Mng.play.kHive.mHoveredHoneycomb;

        if(storeHoneycomb != null)
        {
            switch(storeHoneycomb.kStructureType)
            {
                case StructureType.Storage:
                    if(storeHoneycomb.IsUsable(type) == false)
                    {
                        CancelPlace();
                        return false;
                    }
                    break;

                case StructureType.Dryer:
                    if(storeHoneycomb.IsUsable(type) == false || storeHoneycomb.mIsOpen == false)
                    {
                        CancelPlace();
                        return false;
                    }
                    break;
                case StructureType.Coalgulate:
                    if(storeHoneycomb.IsUsable(type) == false || storeHoneycomb.mIsOpen == false)
                    {
                        CancelPlace();
                        return false;
                    }
                    
                    break;
                case StructureType.Building:
                    break;
                default:
                    CancelPlace();
                    return false;
            }


            if(storeHoneycomb.kStructureType == StructureType.Building)
            {
                Mng.play.kHive.PlayGiveParticles(storeHoneycomb.gameObject.transform);
                UpdateAmount(type, storeHoneycomb.UpdateWaxAmount(amount));
            }
            else
            {
                UpdateAmount(type, storeHoneycomb.StoreResource(type, amount));
            }
            CancelPlace();
            return true;
        }

        return false;
    }

    public bool GiveToFlower()
    {
        if(type != GameResType.Pollen)
        {
            return false;
        }

        Flower storeFlower = Mng.play.kGarden.mHoveredFlower;
        
        if(storeFlower != null)
        {
            if(storeFlower.stage != FlowerStage.Flower)
            {
                return false;
            }

            UpdateAmount(type, storeFlower.AddPollenAmount(amount, true));
            CancelPlace();
            return true;
        }

        return false;
    }

   
	private GameResAmount GetMaxHoneyAmount() { return Mng.play.kHive.mMaxItemAmounts[0]; }
	private GameResAmount GetMaxNectarAmount() { return Mng.play.kHive.mMaxItemAmounts[1]; }
	private GameResAmount GetMaxPollenAmount() { return Mng.play.kHive.mMaxItemAmounts[2]; }
	private GameResAmount GetMaxWaxAmount() { return Mng.play.kHive.mMaxItemAmounts[3]; }

	public GameResAmount GetMaxAmount(GameResType _type)
    {
        GameResAmount maxAmount = new GameResAmount(0, GameResUnit.Microgram);

        switch(_type)
        {
            case GameResType.Nectar:
                maxAmount = GetMaxNectarAmount();
                break;
            case GameResType.Pollen:
                maxAmount = GetMaxPollenAmount();
                break;
            case GameResType.Honey:
                maxAmount = GetMaxHoneyAmount();
                break;
            case GameResType.Wax:
                maxAmount = GetMaxWaxAmount();
                break;
        }

        return maxAmount;
    }

    public GameResAmount UpdateAmount(GameResType _type, GameResAmount _amount)
    {
        UpdateType(_type);

        amount = _amount;
        valueText.text = Mng.canvas.GetAmountText(amount);

        if(amount.amount == 0)
        {
            Mng.play.kHive.mIsPlacingItem = false;
            Mng.play.kInventory.RemoveItem(this);
        }
        
        GameResAmount maxAmount = GetMaxAmount(type);

        if(Mng.play.CompareResourceAmounts(maxAmount, _amount) == true)
        {
            amount = maxAmount;
            return Mng.play.SubtractResourceAmounts(_amount, maxAmount);
        }

        return new GameResAmount(0, GameResUnit.Microgram);
    }

    public void UpdateType(GameResType _type)
    {
        type = _type;
        itemSprite.sprite = itemSprites[(int)_type];
    }

    private GameObject GetTopTouchingObj()
    {
        int queenInd = -1;
        int beeInd = -1;

        foreach(GameObject obj in mTouchingObjs)
        {
            if(obj.GetComponent<QueenBee>() != null)
            {
                queenInd = mTouchingObjs.IndexOf(obj);
            }
            if(obj.GetComponent<Bee>()!= null)
            {
                beeInd = mTouchingObjs.IndexOf(obj);
            }
        }

        if(queenInd!= -1)
        {
            return mTouchingObjs[queenInd];
        }
        else if(beeInd != -1)
        {
            return mTouchingObjs[beeInd];
        }
        else
        {
            return null;
        }
    }

    private void OnMouseUp()
    {
        
	}

    public void CancelPlace()
    {
        switch(mPrevLoc)
        {
            case ItemLoc.InvenSlot:
                int placeSlot = mPrevSlot;

                Inventory inven = Mng.play.kInventory;

                if(placeSlot == -1 || (inven.CheckIfSlotUsable(mPrevSlot, type)))
                {
                    placeSlot = inven.GetAvailableSlot(type);
                    
                    if(placeSlot == -1)
                    {
                        Mng.play.kHive.mIsPlacingItem = false;
                        Destroy(gameObject);
                    }
                }

                GameResAmount sumAmount = Mng.play.AddResourceAmounts(Mng.play.kInventory.mItemSlots[mPrevSlot].amount, amount);

                if(Mng.play.CompareResourceAmounts(sumAmount, GetMaxAmount(type)) == true)
                {
                    inven.UpdateSlotAmount(mPrevSlot, type, sumAmount);
                    UpdateAmount(type, new GameResAmount(0, GameResUnit.Microgram));
                }
                else
                {
                    inven.UpdateSlotAmount(mPrevSlot, type, GetMaxAmount(type));
                    UpdateAmount(type, Mng.play.SubtractResourceAmounts(sumAmount, GetMaxAmount(type)));
                }

                break;
            case ItemLoc.ConvertPrevSlot:
                Mng.canvas.kConvert.AddPrevAmount(type, amount);
                break;
            case ItemLoc.ConvertResSlot:
                Mng.canvas.kConvert.AddResAmount(type, amount);
                break;
        }
        
        Mng.play.kHive.mIsPlacingItem = false;
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if(col.gameObject.tag != "Item" || mIsDropped == false)
        {
            return;
        }

        GameResAmount maxAmount = GetMaxAmount(type);

        if(Mng.play.IsSameAmount(amount, maxAmount) == true)
        {
            return;
        }
    
        Item colItem = col.gameObject.GetComponent<Item>();

        if(mCanMerge == true)
        {
            colItem.mCanMerge = false;
            if(colItem.type != type)
            {
                return;
            }    

            colItem.mCanMerge = false;

            GameResAmount sumAmount = Mng.play.AddResourceAmounts(colItem.amount, amount);
            
            if(Mng.play.CompareResourceAmounts(maxAmount, sumAmount))
            {
                Item item = Instantiate(Mng.play.kHive.kItemObj, transform.position, Quaternion.identity, Mng.play.kHive.kItems).GetComponent<Item>();
                item.UpdateAmount(type, Mng.play.SubtractResourceAmounts(sumAmount, maxAmount));
                UpdateAmount(type, maxAmount);
            }
            else
            {
                UpdateAmount(type, sumAmount);
            }
            
            col.gameObject.SetActive(false);
            Mng.play.kInventory.RemoveItem(this);
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if(col.gameObject.tag == "Honeycomb")
        {
            return;
        }

        print(col.gameObject.name);
        mTouchingObjs.Add(col.gameObject);
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if(col.gameObject.tag == "Honeycomb")
        {
            return;
        }

        mTouchingObjs.Remove(col.gameObject);
    }

	// 세이브/로드 관련
	[Serializable]
	public class CSaveData
	{
		public float XPosition;
		public GameResType type = GameResType.Honey;
		public GameResAmount amount = new GameResAmount(0f,GameResUnit.Microgram);
	}

	public void ExportTo(CSaveData savedata)
	{
		savedata.XPosition = XPosition;
        savedata.type = type;
		savedata.amount = amount;
	}

	public void ImportFrom(CSaveData savedata)
	{
		XPosition = savedata.XPosition;
		type = savedata.type;
		amount = savedata.amount;

        UpdateAmount(type, amount);
	}
}
