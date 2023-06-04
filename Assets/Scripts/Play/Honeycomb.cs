using EnumDef;
using StructDef;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Honeycomb : MonoBehaviour
{
    public SpriteRenderer kSpriteRenderer;

    public Vector3 pos { get { return transform.position; } set { pos = value; } }

    public bool isTarget;

    public GameResType type; //무슨 자원을 저장하고 있는지
    public GameResAmount amount = new GameResAmount(0f, GameResUnit.Microgram);
    public GameResAmount kMaxNectarAmount = new GameResAmount(40, GameResUnit.Microgram);
    public GameResAmount kMaxPollenAmount = new GameResAmount(20, GameResUnit.Milligram);
    public GameResAmount kMaxHoneyAmount = new GameResAmount(500, GameResUnit.Microgram);
    public GameResAmount kMaxWaxAmount = new GameResAmount(20, GameResUnit.Milligram);

    public Hive mHive { get; set; }

    private void Start()
    {
        kSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    public bool IsFull() //그 셀이 꽉 차있는지 확인
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

    private GameResAmount GetMaxAmount(GameResType _type)
    {
        GameResAmount maxAmount = new GameResAmount(0f, GameResUnit.Microgram);

        if (_type == GameResType.Nectar)
        {
            maxAmount = kMaxNectarAmount;
        }
        else if (_type == GameResType.Pollen)
        {
            maxAmount = kMaxPollenAmount;
        }
        else if (_type == GameResType.Honey)
        {
            maxAmount = kMaxHoneyAmount;
        }
        else if (_type == GameResType.Wax)
        {
            maxAmount = kMaxWaxAmount;
        }

        return maxAmount;
    }

    /// <summary> 파라미터 값만큼 저장, 저장공간이 부족한다면 부족한만큼 반환 </summary>
    public GameResAmount StoreResource(GameResType _type, GameResAmount _amount)
    {
        GameResAmount maxAmount = GetMaxAmount(_type);

        if (type == GameResType.Empty || type == _type)
        {
            type = _type;
        }
        else
        {
            UpdateSprite();
            return new GameResAmount(0f, GameResUnit.Microgram);
        }

        GameResAmount retAmount = PlayManager.Instance.AddResourceAmounts(_amount, amount);

        if (PlayManager.Instance.CompareResourceAmounts(retAmount, maxAmount) == true) //저장공간 안부족하면 그대로 저장
        {
            amount = retAmount;
            PlayManager.Instance.AddResourceToStorage(_type, _amount);
            UpdateSprite();

            return new GameResAmount(0f, GameResUnit.Microgram);
        }

        retAmount = PlayManager.Instance.SubtractResourceAmounts(retAmount, maxAmount);
        PlayManager.Instance.AddResourceToStorage(_type, PlayManager.Instance.SubtractResourceAmounts(maxAmount, amount));
        amount = maxAmount;

        UpdateSprite();

        return retAmount;
    }

    public void UpdateSprite()
    {
        GameResAmount maxAmount = GetMaxAmount(type);

        if((int)maxAmount.unit != (int)amount.unit)
        {
            kSpriteRenderer.sprite = mHive.kHoneycombHoneySprites[0];
            return;
        }

        
        int spriteNum = (int)((amount.amount / maxAmount.amount) * (mHive.kHoneycombNectarSprites.Length - 1));

        if (type == GameResType.Nectar)
        {
            kSpriteRenderer.sprite = mHive.kHoneycombNectarSprites[spriteNum];
        }
        else if (type == GameResType.Pollen)
        {
            kSpriteRenderer.sprite = mHive.kHoneycombPollenSprites[spriteNum];
        }
        else if (type == GameResType.Honey)
        {
            kSpriteRenderer.sprite = mHive.kHoneycombHoneySprites[spriteNum];
        }
        else if (type == GameResType.Wax)
        {
            kSpriteRenderer.sprite = mHive.kHoneycombWaxSprites[spriteNum];
        }
        else
        {
            kSpriteRenderer.sprite = mHive.kHoneycombHoneySprites[0];
        }
    }
}