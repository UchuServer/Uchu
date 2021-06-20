using Uchu.Core;

namespace Uchu.World
{
    [ClientGameMessagePacketStruct]
    public struct MoveItemInInventoryMessage
    {
        public GameObject Associate { get; set; }
        public GameMessageId GameMessageId => GameMessageId.MoveItemInInventory;
        [Default(InventoryType.Invalid)]
        public InventoryType DestinationInventoryType { get; set; }
        public Item Item { get; set; }
        public InventoryType CurrentInventoryType { get; set; }
        public int ResponseCode { get; set; }
        public int NewSlot { get; set; }

        public MoveItemInInventoryMessage(GameObject associate = default, InventoryType destinationInventoryType = InventoryType.Invalid, Item item = default, InventoryType currentInventoryType = default, int responseCode = default, int newSlot = default)
        {
            Associate = associate;
            DestinationInventoryType = destinationInventoryType;
            Item = item;
            CurrentInventoryType = currentInventoryType;
            ResponseCode = responseCode;
            NewSlot = newSlot;
        }
    }
}