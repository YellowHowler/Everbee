using EnumDef;
using StructDef;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryBarPanel : MonoBehaviour
{
    int mSlotCount = 3;

    public Image[] kItemImages;
    public TMP_Text[] kItemTexts;
    public GameObject[] kSelectedObjs;

    public Sprite[] kResourceSprites;

    [HideInInspector] public GameResAmount[] mItemAmounts;
    [HideInInspector] public GameResType[] mItemTypes;

    private int mSelectedNum = 1;

    [HideInInspector] public int mHoveredNum = -1;

    void Start()
    {
        mItemAmounts = new GameResAmount[mSlotCount];
        mItemTypes = new GameResType[mSlotCount];

        SetSelected(1);
        UpdateSlots();
    }
    void Update()
    {
        
    }

    public GameResAmount GetMaxAmount(GameResType _type)
    {
        GameResAmount maxAmount = new GameResAmount(0, GameResUnit.Microgram);

        switch(_type)
        {
            case GameResType.Nectar:
                maxAmount = Mng.play.kHive.mMaxItemAmounts[1];
                break;
            case GameResType.Pollen:
                maxAmount = Mng.play.kHive.mMaxItemAmounts[2];
                break;
            case GameResType.Honey:
                maxAmount = Mng.play.kHive.mMaxItemAmounts[0];
                break;
            case GameResType.Wax:
                maxAmount = Mng.play.kHive.mMaxItemAmounts[3];
                break;
        }

        return maxAmount;
    }

    public bool CheckIfSlotUsable(int _num, GameResType _type)
    {
        if(mItemTypes[_num] == GameResType.Empty || mItemAmounts[_num].amount == 0)
        {
            return true;
        }
        if(mItemTypes[_num] == _type && Mng.play.CompareResourceAmounts(mItemAmounts[_num], GetMaxAmount(_type)) == true)
        {
            return true;
        }

        return false;
    }

    private void UpdateSlots()
    {
        for(int i = 0; i < mSlotCount; i++)
        {
            if(mItemAmounts[i].amount == 0) 
            {
                kItemImages[i].gameObject.SetActive(false);
                kItemTexts[i].gameObject.SetActive(false);
            }
            else
            {
                kItemImages[i].gameObject.SetActive(true);
                kItemTexts[i].gameObject.SetActive(true);

                kItemImages[i].sprite = kResourceSprites[(int)mItemTypes[i]];
                kItemTexts[i].text = Mng.canvas.GetAmountText(mItemAmounts[i]);
            }
        }
    }

    public void UpdateSlotAmount(int _num, GameResType _type, GameResAmount _amount)
    {
        mItemAmounts[_num] = _amount;
        mItemTypes[_num] = _type;

        UpdateSlots();
    }

    public void SetSelected(int _num)
    {
        mSelectedNum = _num;

        for(int i = 0; i < mSlotCount; i++)
        {
            kSelectedObjs[i].SetActive(false);
        }

        kSelectedObjs[mSelectedNum].SetActive(true);
    }

    public void SetHovered(int _num)
    {
        mHoveredNum = _num;
        kSelectedObjs[mHoveredNum].SetActive(true);
    }

    public void CancelHovered()
    {
        if(mHoveredNum != mSelectedNum)
        {
            kSelectedObjs[mHoveredNum].SetActive(false);
        }
        mHoveredNum = -1;
    }

    public void DropItem(int _num)
    {
        if(mSelectedNum != _num || (mItemAmounts[_num].amount == 0 || mItemTypes[_num] == GameResType.Empty))
        {
            return;
        }

        Vector3 spawnPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        spawnPos = new Vector3(spawnPos.x, spawnPos.y, 0);

        Item item = Instantiate(Mng.play.kHive.kItemObj, spawnPos, Quaternion.identity, Mng.play.kHive.kItems).GetComponent<Item>();
        item.UpdateType(mItemTypes[_num]);
        item.UpdateAmount(mItemAmounts[_num]);
        mItemTypes[_num] = GameResType.Empty;
        mItemAmounts[_num] = new GameResAmount(0f, GameResUnit.Microgram);

        UpdateSlots();
    }
}
