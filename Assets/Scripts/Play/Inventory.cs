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
		public GameResType type = GameResType.Empty;
		public GameResAmount amount = new GameResAmount(0f,GameResUnit.Microgram);

		// 세이브/로드 관련
		[Serializable]
		public class CSaveData
		{
			public GameResType type = GameResType.Empty;
			public GameResAmount amount = new GameResAmount(0f,GameResUnit.Microgram);
		}

		public void ExportTo(CSaveData savedata)
		{
			savedata.type = type;
			savedata.amount = amount;
		}

		public void ImportFrom(CSaveData savedata)
		{
			type = savedata.type;
			amount = savedata.amount;
		}
	}

	public List<ItemSlot> mItemSlots { get; private set; } = new List<ItemSlot>();

	public void Init(int numSlots)
	{
		mItemSlots.Clear();

		for(int i=0; i<numSlots; ++i)
			mItemSlots.Add(new ItemSlot());
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
		foreach(var slotsave in savedata.itemSlots)
		{
			var slot = new ItemSlot();
			slot.ImportFrom(slotsave);
			mItemSlots.Add(slot);
		}

		var invenUI = MainCanvas.Instance.kInven;

		invenUI.SetSelected(1);
		invenUI.UpdateSlots();
	}
}
