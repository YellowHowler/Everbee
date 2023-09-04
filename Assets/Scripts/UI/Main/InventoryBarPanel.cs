using EnumDef;
using StructDef;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryBarPanel : MonoBehaviour
{
    public Image[] kItemImages;
    public TMP_Text[] kItemTexts;
    public GameObject[] kSelectedObjs;

    public Sprite[] kResourceSprites;

    private int mSelectedNum = 1;

    [HideInInspector] public int mHoveredNum = -1;

    private int mSlotNum;

    public void Init()
    {
        mSlotNum = kItemImages.Length;

        SetSelected(1);
        UpdateSlots();
    }
    void Update()
    {
        
    }

    public void UpdateSlots()
    {
        var inventory = Mng.play.kInventory;

        for(int i = 0; i < mSlotNum; i++)
        {
            var slot = inventory.mItemSlots[i];

            if(slot.amount.amount == 0) 
            {
                kItemImages[i].gameObject.SetActive(false);
                kItemTexts[i].gameObject.SetActive(false);
            }
            else
            {
                kItemImages[i].gameObject.SetActive(true);
                kItemTexts[i].gameObject.SetActive(true);

                if(Mng.play.IsBaseResource(slot.type))
                {
                    kItemImages[i].sprite = kResourceSprites[(int)slot.type];
                    kItemTexts[i].text = Mng.canvas.GetAmountText(slot.amount);
                }
                else if (slot.type == GameResType.Seed)
                {
                    kItemImages[i].sprite = Mng.canvas.kSeedSprites[slot.typeInt];
                    kItemTexts[i].text = "";
                }
            }
        }
    }

    public void SetSelected(int _num)
    {
        StartCoroutine(SetSelectedCor(_num));
    }

    private IEnumerator SetSelectedCor(int _num)
    {
        yield return new WaitForSeconds(0.1f);

        mSelectedNum = _num;

        for(int i = 0; i < kSelectedObjs.Length; i++)
        {
            kSelectedObjs[i].SetActive(false);
        }

        kSelectedObjs[mSelectedNum].SetActive(true);
        kSelectedObjs[mSelectedNum].GetComponent<Image>().color = Color.white;
    }

    public void SetHovered(int _num)
    {
        mHoveredNum = _num;

        for(int i = 0; i < kSelectedObjs.Length; i++)
        {
            kSelectedObjs[i].SetActive(false);
        }

        kSelectedObjs[mSelectedNum].SetActive(true);
        kSelectedObjs[mHoveredNum].SetActive(true);
        
        if(mHoveredNum != mSelectedNum)
            kSelectedObjs[mHoveredNum].GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);
        else    
            kSelectedObjs[mSelectedNum].GetComponent<Image>().color = Color.white;
    }

    public void CancelHovered()
    {
        mHoveredNum = -1;

        for(int i = 0; i < kSelectedObjs.Length; i++)
        {
            kSelectedObjs[i].SetActive(false);
        }

        kSelectedObjs[mSelectedNum].SetActive(true);
        kSelectedObjs[mSelectedNum].GetComponent<Image>().color = Color.white;
    }

    public void DropItem(int _num)
    {
		var inventory = Mng.play.kInventory;
		var slot = inventory.mItemSlots[_num];

		if(mSelectedNum != _num || (slot.amount.amount == 0 || slot.type == GameResType.Empty))
        {
            return;
        }

        Mng.canvas.SpawnItemAtMousePos(slot.type, slot.amount, ItemLoc.InvenSlot, _num);
        Mng.play.kInventory.UpdateSlotAmount(_num, GameResType.Empty, new GameResAmount(0f, GameResUnit.Microgram));
    }
}
