using InfectedRose.Lvl;
using Uchu.Core;
using System.Numerics;

namespace Uchu.World
{
	[ServerGameMessagePacketStruct]
	public struct AddItemToInventoryClientSyncMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.AddItemToInventoryClientSync;
		public bool IsBound { get; set; }
		public bool IsBoundOnEquip { get; set; }
		public bool IsBoundOnPickup { get; set; }
		[Default]
		public LootType Source { get; set; }
		public LegoDataDictionary ExtraInfo { get; set; }
		public Lot ItemLot { get; set; }
		[Default]
		public GameObject Subkey { get; set; }
		[Default]
		public InventoryType InventoryType { get; set; }
		[Default(1)]
		public uint Delta { get; set; }
		[Default]
		public uint TotalItems { get; set; }
		public Item Item { get; set; }
		public Vector3 FlyingLootPosit { get; set; }
		public bool ShowFlyingLoot { get; set; }
		public int Slot { get; set; }

		public AddItemToInventoryClientSyncMessage(GameObject associate = default, bool isBound = default, bool isBoundOnEquip = default, bool isBoundOnPickup = default, LootType source = default, LegoDataDictionary extraInfo = default, Lot itemLot = default, GameObject subkey = default, InventoryType inventoryType = default, uint delta = 1, uint totalItems = default, Item item = default, Vector3 flyingLootPosit = default, bool showFlyingLoot = true, int slot = default)
		{
			this.Associate = associate;
			this.IsBound = isBound;
			this.IsBoundOnEquip = isBoundOnEquip;
			this.IsBoundOnPickup = isBoundOnPickup;
			this.Source = source;
			this.ExtraInfo = extraInfo;
			this.ItemLot = itemLot;
			this.Subkey = subkey;
			this.InventoryType = inventoryType;
			this.Delta = delta;
			this.TotalItems = totalItems;
			this.Item = item;
			this.FlyingLootPosit = flyingLootPosit;
			this.ShowFlyingLoot = showFlyingLoot;
			this.Slot = slot;
		}
	}
}