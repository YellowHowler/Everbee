using EnumDef;
using StructDef;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Honeycomb : MonoBehaviour
{ 
    public SpriteRenderer kSpriteRenderer;

    public Vector3 pos { get { return transform.position; } set { pos = value; } }

    public bool isTarget;

    public GameResType type; //���� �ڿ��� �����ϰ� �ִ���
    public GameResAmount amount = new GameResAmount(0f, GameResUnit.Microgram);
    public GameResAmount kMaxNectarAmount = new GameResAmount(40, GameResUnit.Microgram);
    public GameResAmount kMaxPollenAmount = new GameResAmount(20, GameResUnit.Milligram);
    public GameResAmount kMaxHoneyAmount = new GameResAmount(500, GameResUnit.Microgram);
    public GameResAmount kMaxWaxAmount = new GameResAmount(20, GameResUnit.Milligram);

    public Hive mHive { get; set; }

    public StructureType kStructType = StructureType.None;
    private void Start()
    {
        SetStructure(StructureType.None);
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

    private void SetAllChildrenActive(bool _setActive)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(_setActive);
        }

        transform.GetChild(0).gameObject.SetActive(true);
    }

    public void SetStructure(StructureType _type)
    {
        switch (_type)
        {
            case StructureType.None:
                SetAllChildrenActive(false);
                break;
            case StructureType.Storage:
                SetAllChildrenActive(false);
                transform.GetChild(1).gameObject.SetActive(true);
                break;
            case StructureType.Dry:
                SetAllChildrenActive(false);
                break;
        }

        kStructType = _type;
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

    /// <summary> �Ķ���� ����ŭ ����, ��������� �����Ѵٸ� �����Ѹ�ŭ ��ȯ </summary>
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

        if (PlayManager.Instance.CompareResourceAmounts(retAmount, maxAmount) == true) //������� �Ⱥ����ϸ� �״�� ����
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

    private void OnDrawGizmos()
    {
        /*
        Vector3 top = transform.position + Vector3.up * 0.825f;
        Vector3 topleft = transform.position + Vector3.up * 0.4125f + Vector3.left * 0.825f;
        Vector3 bottomleft = transform.position + Vector3.down* 0.4125f + Vector3.left * 0.825f;
        
        Vector3 bottom = transform.position + Vector3.down * 0.825f;
        Vector3 bottomright = transform.position + Vector3.down * 0.4125f + Vector3.right * 0.825f;
        Vector3 topright = transform.position + Vector3.up * 0.4125f + Vector3.right * 0.825f;

        Gizmos.DrawLine(top, topleft);
        Gizmos.DrawLine(topleft, bottomleft);
        Gizmos.DrawLine(bottomleft, bottom);

        Gizmos.DrawLine(bottom, bottomright);
        Gizmos.DrawLine(bottomright, topright);
        Gizmos.DrawLine(topright, top);
        */

        //Gizmos.DrawSphere(transform.position, 0.825f);

        if (mHive != null)
        {
            if (mHive.kIsDrawHoneycombName == true)
                Handles.Label(transform.position, name);

            if (mHive.kIsDrawHoneycombShape == true)
                Gizmos.DrawWireSphere(transform.position, Mng.play.kHive.mHoneycombRadiusY);
        }
    }

    private void OnMouseDown()
    {
        Hive hive = Mng.play.kHive;

        if(hive.mIsBuilding == true)
        {
            
        }
    }
}