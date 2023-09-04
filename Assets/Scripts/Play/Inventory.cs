using System;
using System.Collections.Generic;
using UnityEngine;
using EnumDef;
using StructDef;

public class Inventory: MonoBehaviour
{
	private List<Item> mItems = new List<Item>();

	[Serializable]
	public class ItemSlot
	{
		public int num;
		public GameResType type = GameResType.Empty;
		public int typeInt = 0;
		public GameResAmount amount = new GameResAmount(0f,GameResUnit.Microgram);

		public ItemSlot(int _num)
		{
			num = _num;
		}

		// 세이브/로드 관련
		[Serializable]
		public class CSaveData
		{
			public int num;
			public GameResType type = GameResType.Empty;
			public int typeInt = 0;
			public GameResAmount amount = new GameResAmount(0f,GameResUnit.Microgram);
		}

		public void ExportTo(CSaveData savedata)
		{
			savedata.num = num;
			savedata.type = type;
			savedata.typeInt = typeInt;
			savedata.amount = amount;
		}

		public void ImportFrom(CSaveData savedata)
		{
			num = savedata.num;
			type = savedata.type;
			typeInt = savedata.typeInt;
			amount = savedata.amount;
		}
	}

	private InventoryBarPanel mPanel;
	public List<ItemSlot> mItemSlots { get; private set; } = new List<ItemSlot>();

	public void Init(int numSlots)
	{
		mPanel = Mng.canvas.kInven;

		mItemSlots.Clear();

		for(int i=0; i<numSlots; ++i)
			mItemSlots.Add(new ItemSlot(i));
	}

	public void UpdateSlotAmount(int _num, GameResType _type, GameResAmount _amount)
    {
		var slot = mItemSlots[_num];

		slot.type = _type;
		slot.amount = _amount;

        mPanel.UpdateSlots();
    }

	public void UpdateSlotAmount(int _num, GameResType _type, int _typeInt, GameResAmount _amount)
    {
		var slot = mItemSlots[_num];

		slot.type = _type;
		slot.typeInt = _typeInt;
		slot.amount = _amount;

        mPanel.UpdateSlots();
    }

	public void AddSlotAmount(int _num, GameResType _type, GameResAmount _amount)
    {
        UpdateSlotAmount(_num, _type, Mng.play.AddResourceAmounts(_amount, mItemSlots[_num].amount));
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

	public int GetAvailableSlot(GameResType _type)
    {
		int slotNum = mItemSlots.Count;

        for(int i = 0; i < slotNum; i++)
        {
            if(CheckIfSameType(i, _type))
            {
                return i;
            }
        }

        for(int i = 0; i < slotNum; i++)
        {
            if(CheckIfSlotUsable(i, _type))
            {
                return i;
            }
        }

        return -1;
    }

	public GameResAmount GetTotalAmount(GameResType _type) //인벤에 이 resource 가 총 얼마나 있는지
	{
		int slotNum = mItemSlots.Count;
		GameResAmount retAmount = new GameResAmount(0f, GameResUnit.Microgram);

		for(int i = 0; i < slotNum; i++)
        {
			if(mItemSlots[i].type != _type) continue;
			
            retAmount = Mng.play.AddResourceAmounts(retAmount, mItemSlots[i].amount);
        }

		return retAmount;
	}

	public bool CheckIfEnoughResource(GameResType _type, GameResAmount _amount)
	{
		return Mng.play.CompareResourceAmounts(_amount, GetTotalAmount(_type));
	}

	public bool SubtractFromInven(GameResType _type, GameResAmount _amount)
	{
		int slotNum = mItemSlots.Count;
		GameResAmount subNeedAmount = _amount;

		for(int i = 0; i < slotNum; i++)
		{
			if(mItemSlots[i].type != _type) continue;

			GameResAmount slotAmount = mItemSlots[i].amount;
			
			if(Mng.play.CompareResourceAmounts(slotAmount, subNeedAmount))
			{
				subNeedAmount = Mng.play.SubtractResourceAmounts(subNeedAmount, slotAmount);
				UpdateSlotAmount(i, _type, new GameResAmount(0f, GameResUnit.Microgram));
			}
			else
			{
				UpdateSlotAmount(i, _type, Mng.play.SubtractResourceAmounts(slotAmount, subNeedAmount));
				subNeedAmount = new GameResAmount(0f, GameResUnit.Microgram);
				break;
			}
		}

		return Mng.play.IsSameAmount(new GameResAmount(0f, GameResUnit.Microgram), subNeedAmount);
	}

	public bool CheckIfEmpty(int _num)
	{
		var slot = mItemSlots[_num];

		return Mng.play.IsAmountZero(slot.amount);
	}
    public bool CheckIfSameType(int _num, GameResType _type)
    {
        var slot = mItemSlots[_num];
        
        if(slot.amount.amount >= 0.1f && slot.type == _type && Mng.play.CompareResourceAmounts(slot.amount, GetMaxAmount(_type)) == true)
        {
            return true;
        }

        return false;
    }

    public bool CheckIfSlotUsable(int _num, GameResType _type)
    {
        var slot = mItemSlots[_num];

        if(slot.type == GameResType.Empty || slot.amount.amount == 0)
        {
            return true;
        }
        if(slot.type == _type && Mng.play.CompareResourceAmounts(slot.amount, GetMaxAmount(_type)) == true)
        {
            return true;
        }

        return false;
    }

	public Item CreateItem(GameResType _type, GameResAmount _amount, Vector3 position)
	{
		if(_amount.amount == 0)
		{
			return null;
		}

		Item item = GameObject.Instantiate(Mng.play.kHive.kItemObj,position, Quaternion.identity, Mng.play.kHive.kItems).GetComponent<Item>();
		mItems.Add(item);

		return item;
	}

	public void RemoveItem(Item item)
	{
		mItems.Remove(item);
		GameObject.Destroy(item.gameObject);
	}


	// 세이브/로드 관련
	[Serializable]
	public class CSaveData
	{
		public List<Item.CSaveData> Items = new List<Item.CSaveData>();
		public List<ItemSlot.CSaveData> itemSlots = new List<ItemSlot.CSaveData>();
	}

	public void ExportTo(CSaveData savedata)
	{
		savedata.Items.Clear();
		foreach(var item in mItems)
		{
			var itemsave = new Item.CSaveData();
			item.ExportTo(itemsave);
			savedata.Items.Add(itemsave);
		}

		savedata.itemSlots.Clear();
		foreach(var slot in mItemSlots)
		{
			var slotsave = new ItemSlot.CSaveData();
			slot.ExportTo(slotsave);
			savedata.itemSlots.Add(slotsave);
		}
	}

	public void ImportFrom(CSaveData savedata)
	{
		foreach(var item in mItems)
			GameObject.Destroy(item.gameObject);
		mItems.Clear();

		Vector3 creationPos = new Vector3(0, -3, 0);
		foreach(var itemsave in savedata.Items)
		{
			var item = CreateItem(itemsave.type, itemsave.amount, creationPos);
			if (item != null)
				item.ImportFrom(itemsave);
		}

		mItemSlots.Clear();

		int i = 0;
		foreach(var slotsave in savedata.itemSlots)
		{
			var slot = new ItemSlot(i);
			slot.ImportFrom(slotsave);
			mItemSlots.Add(slot);
			i++;
		}

		var invenUI = MainCanvas.Instance.kInven;

		invenUI.SetSelected(1);
		invenUI.UpdateSlots();
	}
}
