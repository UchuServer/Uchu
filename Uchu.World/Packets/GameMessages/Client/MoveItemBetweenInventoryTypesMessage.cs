using Uchu.Core;

namespace Uchu.World
{
	[ClientGameMessagePacketStruct]
	public struct MoveItemBetweenInventoryTypesMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.MoveItemBetweenInventoryTypes;
		public InventoryType SourceInventory { get; set; }
		public InventoryType DestinationInventory { get; set; }
		public Item Item { get; set; }
		public bool ShowFlyingLoot { get; set; }
		[Default(1)]
		public uint StackCount { get; set; }
		[Default]
		public Lot Lot { get; set; }

		public MoveItemBetweenInventoryTypesMessage(GameObject associate = default, InventoryType sourceInventory = default, InventoryType destinationInventory = default, Item item = default, bool showFlyingLoot = true, uint stackCount = 1, Lot lot = default)
		{
			this.Associate = associate;
			this.SourceInventory = sourceInventory;
			this.DestinationInventory = destinationInventory;
			this.Item = item;
			this.ShowFlyingLoot = showFlyingLoot;
			this.StackCount = stackCount;
			this.Lot = lot;
		}
	}
}