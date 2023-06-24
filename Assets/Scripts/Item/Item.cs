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
    public GameResType type;

    public GameResAmount kMaxNectarAmount;
    public GameResAmount kMaxPollenAmount;
    public GameResAmount kMaxHoneyAmount;
    public GameResAmount kMaxWaxAmount;

    private bool mIsDropped = false;

    [HideInInspector] public bool mCanMerge = true;

    private void Awake()
    {
        kMaxNectarAmount = Mng.play.kHive.mMaxItemAmounts[0];
        kMaxPollenAmount = Mng.play.kHive.mMaxItemAmounts[1];
        kMaxHoneyAmount = Mng.play.kHive.mMaxItemAmounts[2];
        kMaxWaxAmount = Mng.play.kHive.mMaxItemAmounts[3];
    }

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(1);
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

    public GameResAmount UpdateAmount(GameResAmount _amount)
    {
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
                UpdateAmount(storeHoneycomb.StoreResource(type, amount));
                break;
            case StructureType.Dryer:
                if(storeHoneycomb.IsUsable(type) == false)
                {
                    return;
                }
                UpdateAmount(storeHoneycomb.StoreResource(type, amount));
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

        print("item collide");

        Item colItem = col.gameObject.GetComponent<Item>();

        if(colItem.type != type)
        {
            return;
        }    

        if(mCanMerge == true)
        {
            colItem.mCanMerge = false;

            GameResAmount sumAmount = Mng.play.AddResourceAmounts(colItem.amount, amount);
            
            GameResAmount maxAmount = GetMaxAmount(type);
            if(Mng.play.CompareResourceAmounts(maxAmount, sumAmount))
            {
                Item item = Instantiate(Mng.play.kHive.kItemObj, transform.position, Quaternion.identity, Mng.play.kHive.kItems).GetComponent<Item>();
                item.UpdateType(type);
                item.UpdateAmount(Mng.play.SubtractResourceAmounts(sumAmount, maxAmount));
                amount = maxAmount;
                return;
            }

            UpdateAmount(sumAmount);
            return;
        }

        Destroy(gameObject);
    }
}
