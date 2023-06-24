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

    GameResAmount[] mItemAmounts;
    GameResType[] mItemTypes;

    private int mSelectedNum = 1;

    void Start()
    {
        mItemAmounts = new GameResAmount[mSlotCount];
        mItemTypes = new GameResType[mSlotCount];

        SetSelected(1);
    }
    void Update()
    {
        
    }

    private void UpdateSlots()
    {
        for(int i = 0; i < mSlotCount; i++)
        {
            kItemImages[i].sprite = kResourceSprites[(int)mItemTypes[i]];
            kItemTexts[i].text = Mng.canvas.GetAmountText(mItemAmounts[i]);
        }
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

    public void DropItem(int _num)
    {
        if(mSelectedNum != _num)
        {
            return;
        }

        /*
        Item item = Instantiate(Mng.play.kHive.kItemObj, gameObject.transform.position, Quaternion.identity, Mng.play.kHive.kItems).GetComponent<Item>();
        item.UpdateType(type);
        UpdateAmount(item.UpdateAmount(amount));

        Mng.play.SubtractResourceFromStorage(item.type, item.amount);
        */
    }
}
