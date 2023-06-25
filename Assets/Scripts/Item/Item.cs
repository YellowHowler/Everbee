using EnumDef;
using StructDef;
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

    public Rigidbody2D rb;

    public GameResAmount amount = new GameResAmount(0f, GameResUnit.Microgram);
    public GameResType type = GameResType.Honey;

    public GameResAmount kMaxNectarAmount;
    public GameResAmount kMaxPollenAmount;
    public GameResAmount kMaxHoneyAmount;
    public GameResAmount kMaxWaxAmount;

    private bool mIsDropped = false;

    private GameObject mQueen;
    private bool mIsTouchingQueen = false;

    [HideInInspector] public bool mCanMerge = true;

    private void Awake()
    {
        kMaxNectarAmount = Mng.play.kHive.mMaxItemAmounts[1];
        kMaxPollenAmount = Mng.play.kHive.mMaxItemAmounts[2];
        kMaxHoneyAmount = Mng.play.kHive.mMaxItemAmounts[0];
        kMaxWaxAmount = Mng.play.kHive.mMaxItemAmounts[3];
    }

    private IEnumerator Start()
    {
        if(transform.position.y < Mng.play.kHive.mFloorY)
        {
            transform.position = new Vector3(transform.position.x, Mng.play.kHive.mFloorY, 0);
        }

        yield return new WaitForSeconds(0.1f);
        mIsDropped = true;

        StartCoroutine(DestroyCor());
    }

    private IEnumerator DestroyCor()
    {
        yield return new WaitForSeconds(300);
        Destroy(gameObject);
    }

    public GameResAmount GetMaxAmount(GameResType _type)
    {
        GameResAmount maxAmount = new GameResAmount(0, GameResUnit.Microgram);

        switch(_type)
        {
            case GameResType.Nectar:
                maxAmount = kMaxNectarAmount;
                break;
            case GameResType.Pollen:
                maxAmount = kMaxPollenAmount;
                break;
            case GameResType.Honey:
                maxAmount = kMaxHoneyAmount;
                break;
            case GameResType.Wax:
                maxAmount = kMaxWaxAmount;
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
            Destroy(gameObject);
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

    private void OnMouseDown()
    {
        rb.velocity = Vector3.zero;
        rb.gravityScale = 0;
    }

    private void OnMouseUp()
    {
        rb.velocity = Vector3.zero;
        rb.gravityScale = 1;

        if(mIsDropped == false)
        {
            return;
        }

        InventoryBarPanel inven = Mng.canvas.kInven;
        if(inven.mHoveredNum != -1 && (inven.CheckIfSlotUsable(inven.mHoveredNum, type)))
        {
            GameResAmount sumAmount = Mng.play.AddResourceAmounts(inven.mItemAmounts[inven.mHoveredNum], amount);
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

        if(mIsTouchingQueen == true)
        {
            mQueen.GetComponent<QueenBee>().AddResource(type, amount);
            Destroy(gameObject);
        }

        if(transform.position.y < Mng.play.kHive.mFloorY)
        {
            transform.position = new Vector3(transform.position.x, Mng.play.kHive.mFloorY, 0);
        }

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Mng.play.SetZToZero(Input.mousePosition));

        Honeycomb storeHoneycomb = Mng.play.kHive.GetHoneycombFromPos(mousePos);

        if(storeHoneycomb == null)
        {
            return;
        }

        switch(storeHoneycomb.kStructureType)
        {
            case StructureType.Storage:
                if(storeHoneycomb.IsUsable(type) == false)
                {
                    return;
                }
                UpdateAmount(type, storeHoneycomb.StoreResource(type, amount));
                break;
            case StructureType.Dryer:
                if(storeHoneycomb.IsUsable(type) == false)
                {
                    return;
                }
                UpdateAmount(type, storeHoneycomb.StoreResource(type, amount));
                break;
        }
    }

    private void OnMouseDrag()
    {
        rb.position = Camera.main.ScreenToWorldPoint(Mng.play.SetZToZero(Input.mousePosition));
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
            Destroy(col.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if(col.gameObject.tag == "QueenBee")
        {
            mQueen = col.gameObject;
            mIsTouchingQueen = true;
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if(col.gameObject.tag == "QueenBee")
        {
            mIsTouchingQueen = false;
        }
    }
}
