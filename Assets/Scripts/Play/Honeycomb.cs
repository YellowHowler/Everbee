using EnumDef;
using StructDef;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using TMPro;

public class Honeycomb : MonoBehaviour
{ 
    public SpriteRenderer kSpriteRenderer;

    public Vector3 pos { get { return transform.position; } set { pos = value; } }

    public bool isTarget;

    public GameResType type; //���� �ڿ��� �����ϰ� �ִ���
    public GameResAmount amount = new GameResAmount(0f, GameResUnit.Microgram);
    public GameResAmount kMaxNectarAmount = new GameResAmount(10, GameResUnit.Milligram);
    public GameResAmount kMaxPollenAmount = new GameResAmount(200, GameResUnit.Milligram);
    public GameResAmount kMaxHoneyAmount = new GameResAmount(10, GameResUnit.Milligram);
    public GameResAmount kMaxWaxAmount = new GameResAmount(10, GameResUnit.Milligram);

    public Hive mHive { get; set; }

    public StructureType kStructureType = StructureType.None;

    public GameObject kEmptyObj;
    public GameObject kStorageObj;
    public GameObject kDryerObj;
    public GameObject kHoverObj;

    public GameObject kCanvas;

    public GameObject kTimerPanel;
    public GameObject kButtonPanel;

    private bool mIsOpen = false;
    private bool mIsConverting = false;

    public Sprite[] kDryerSprites;

    private void Start()
    {
        SetStructure(StructureType.None);

        kCanvas.SetActive(true);
        kCanvas.GetComponent<Canvas>().worldCamera = Camera.main;
        HideObjects();
    }

    private void HideObjects()
    {
        kHoverObj.SetActive(false);
        kTimerPanel.SetActive(false);
        kButtonPanel.SetActive(false);
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
        if(type == GameResType.Empty)
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
            transform.GetChild(i).gameObject.SetActive(_setActive);
        }

        kEmptyObj.gameObject.SetActive(true);
    }

    public bool SetStructure(StructureType _type) //true if build successful
    {
        switch (_type)
        {
            case StructureType.None:
                SetAllChildrenActive(false);
                break;
            case StructureType.Storage:
                if(kStructureType != StructureType.None) 
                {
                    return false;
                }
                SetAllChildrenActive(false);
                kStorageObj.SetActive(true);
                break;
            case StructureType.Dryer:
                if(kStructureType != StructureType.Storage) 
                {
                    return false;
                }
                SetAllChildrenActive(false);
                kStorageObj.SetActive(true);
                kDryerObj.SetActive(true);
                break;
        }

        kStructureType = _type;
        return true;
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

        GameResAmount retAmount = Mng.play.AddResourceAmounts(_amount, amount);

        if (Mng.play.CompareResourceAmounts(retAmount, maxAmount) == true) //������� �Ⱥ����ϸ� �״�� ����
        {
            amount = retAmount;
            Mng.play.AddResourceToStorage(_type, _amount);
            UpdateSprite();

            return new GameResAmount(0f, GameResUnit.Microgram);
        }

        retAmount = Mng.play.SubtractResourceAmounts(retAmount, maxAmount);
        Mng.play.AddResourceToStorage(_type, Mng.play.SubtractResourceAmounts(maxAmount, amount));
        amount = maxAmount;

        UpdateSprite();

        return retAmount;
    }

    public void UpdateAmount(GameResAmount _amount)
    {
        amount = _amount;
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

        if((int)maxAmount.unit != (int)amount.unit)
        {
            kSpriteRenderer.sprite = mHive.kHoneycombHoneySprites[0];
            return;
        }

        
        int spriteNum = (int)((amount.amount / maxAmount.amount) * (mHive.kHoneycombNectarSprites.Length - 1));
        if(spriteNum == 0 && amount.amount > 0) spriteNum = 1;

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

    private void ToggleDoor()
    {
        mIsOpen = !mIsOpen;

        if(mIsOpen)
        {
            kDryerObj.GetComponent<SpriteRenderer>().sprite = kDryerSprites[1];
            kCanvas.SetActive(true);
            kButtonPanel.SetActive(true);

            kTimerPanel.SetActive(false);
        }
        else
        {
            kDryerObj.GetComponent<SpriteRenderer>().sprite = kDryerSprites[0];
            kButtonPanel.SetActive(false);
        }
    }

    private void OnMouseEnter()
    {
        if(kStructureType == StructureType.None || Mng.canvas.kIsViewingMenu || Mng.play.kHive.mIsBuilding) 
        {
            return;
        }

        kHoverObj.SetActive(true);
    }

    private void OnMouseExit()
    {
        kHoverObj.SetActive(false);
    }

    private void OnMouseDown()
    {
        if(Mng.canvas.kIsViewingMenu == true)
        {
            return;
        }

        Hive hive = Mng.play.kHive;

        if(hive.mIsBuilding == true)
        {
            if(SetStructure(hive.mStructureType) == true)
            {
                hive.mIsBuilding = false;
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
        
        Item item = Instantiate(Mng.play.kHive.kItemObj, gameObject.transform.position, Quaternion.identity, Mng.play.kHive.kItems).GetComponent<Item>();
        item.UpdateType(type);
        UpdateAmount(item.UpdateAmount(amount));

        Mng.play.SubtractResourceFromStorage(item.type, item.amount);
    }

    private void OnMouseUp()
    {
        if(Mng.canvas.kIsViewingMenu == true)
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
                    CreateItem();
                    break;
                case StructureType.Dryer:
                    break;
            }
        }
    }

    public void NectarToHoney()
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

    private IEnumerator ConvertCor(int _time, GameResType _finType)
    {
        WaitForSeconds sec = new WaitForSeconds(1);

        for(int i = _time; i >= 0; i--)
        {
            kTimerPanel.GetComponentInChildren<TMP_Text>().text = Mng.play.GetTimeText(i);
            yield return sec;
        }

        UpdateType(_finType);
        mIsConverting = false;
    }   
}