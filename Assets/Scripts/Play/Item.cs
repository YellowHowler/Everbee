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

    private void Update()
    {
        transform.position = Camera.main.ScreenToWorldPoint(Mng.play.SetZ(Input.mousePosition, 0));

        if(Input.GetMouseButtonUp(0))
        {
            if(mIsDropped == false)
            {
                return;
            }

            InventoryBarPanel inven = Mng.canvas.kInven;

            if(inven.mHoveredNum != -1 && (inven.CheckIfSlotUsable(inven.mHoveredNum, type)))
            {
                GameResAmount sumAmount = Mng.play.AddResourceAmounts(Mng.play.kInventory.mItemSlots[inven.mHoveredNum].amount, amount);
                if(Mng.play.CompareResourceAmounts(sumAmount, GetMaxAmount(type)) == true)
                {
                    inven.UpdateSlotAmount(inven.mHoveredNum, type, sumAmount);
                    UpdateAmount(type, new GameResAmount(0, GameResUnit.Microgram));
                }
                else
                {
                    inven.UpdateSlotAmount(inven.mHoveredNum, type, GetMaxAmount(type));
                    UpdateAmount(type, Mng.play.SubtractResourceAmounts(sumAmount, GetMaxAmount(type)));
                }
                
                return;
            }

            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Mng.play.SetZ(Input.mousePosition, 0));

            Bee storeBee = Mng.play.kBees.GetBeeFromPos(mousePos);
            if(storeBee != null)
            {
                if(storeBee.mCurStage == BeeStage.Egg || storeBee.mCurStage == BeeStage.Pupa)
                {
                    return;
                }
                UpdateAmount(type, storeBee.AddResource(type, amount));
                CancelPlace();
            }

            Honeycomb storeHoneycomb = Mng.play.kHive.GetHoneycombFromPos(mousePos);
            if(storeHoneycomb != null)
            {
                switch(storeHoneycomb.kStructureType)
                {
                    case StructureType.Storage:
                        if(storeHoneycomb.IsUsable(type) == false)
                        {
                            CancelPlace();
                            return;
                        }
                        UpdateAmount(type, storeHoneycomb.StoreResource(type, amount));
                        break;

                    case StructureType.Dryer:
                        if(storeHoneycomb.IsUsable(type) == false || storeHoneycomb.mIsOpen == false)
                        {
                            CancelPlace();
                            return;
                        }
                        UpdateAmount(type, storeHoneycomb.StoreResource(type, amount));
                        break;
                    case StructureType.Coalgulate:
                        if(storeHoneycomb.IsUsable(type) == false || storeHoneycomb.mIsOpen == false)
                        {
                            CancelPlace();
                            return;
                        }
                        UpdateAmount(type, storeHoneycomb.StoreResource(type, amount));
                        break;
                    default:
                        CancelPlace();
                        return;
                }
            }

            CancelPlace();
        }
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
        int placeSlot = mPrevSlot;

        InventoryBarPanel inven = Mng.canvas.kInven;

        if(placeSlot == -1 || (inven.CheckIfSlotUsable(mPrevSlot, type)))
        {
            placeSlot = Mng.canvas.kInven.GetAvailableSlot(type);
            
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
