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

    public GameResAmount kMaxNectarAmount = new GameResAmount(10, GameResUnit.Milligram);
    public GameResAmount kMaxPollenAmount = new GameResAmount(200, GameResUnit.Milligram);
    public GameResAmount kMaxHoneyAmount = new GameResAmount(500, GameResUnit.Microgram);
    public GameResAmount kMaxWaxAmount = new GameResAmount(20, GameResUnit.Milligram);

    private bool mIsDropped = false;

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(1);
        mIsDropped = true;
    }

    public GameResAmount UpdateAmount(GameResAmount _amount)
    {
        amount = _amount;
        valueText.text = amount.amount + Mng.canvas.GetUnitText(_amount.unit);

        if(amount.amount == 0)
        {
            Destroy(gameObject);
        }
        
        GameResAmount maxAmount = new GameResAmount(0, GameResUnit.Microgram);

        switch(type)
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
                
                break;

        }
    }

    private void OnMouseDrag()
    {
        rb.position = Camera.main.ScreenToWorldPoint(Mng.play.SetZToZero(Input.mousePosition));
    }
}
