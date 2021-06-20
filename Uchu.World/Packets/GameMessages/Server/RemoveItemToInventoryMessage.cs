using InfectedRose.Lvl;
using RakDotNet.IO;
using Uchu.Core;

namespace Uchu.World
{
    [ServerGameMessagePacketStruct]
    public struct RemoveItemFromInventoryMessage
    {
        public GameObject Associate { get; set; }
        public GameMessageId GameMessageId => GameMessageId.RemoveItemFromInventory;
        public bool Confirmed { get; set; }
        public bool DeleteItem { get; set; }
        public bool OutSuccess { get; set; }
        [Default(InventoryType.Invalid)]
        public InventoryType InventoryType { get; set; }
        [Default]
        public ItemType ItemType { get; set; }
        public LegoDataDictionary ExtraInfo { get; set; }
        public bool ForceDeletion { get; set; }
        [Default]
        public GameObject Source { get; set; }
        [Default]
        public Item Item { get; set; }
        [Default]
        public Lot ItemLot { get; set; }
        [Default]
        public GameObject Requesting { get; set; }
        [Default(1)]
        public uint Delta { get; set; }
        [Default]
        public uint TotalItems { get; set; }
        [Default]
        public long SubKey { get; set; }
        [Default]
        public long TradeId { get; set; }
        
        public RemoveItemFromInventoryMessage(GameObject associate = default, bool confirmed = false, bool deleteItem = true, bool outSuccess = false, InventoryType inventoryType = InventoryType.Invalid, ItemType itemType = default, LegoDataDictionary extraInfo = default, bool forceDeletion = true, GameObject source = default, Item item = default, Lot itemLot = default, GameObject requesting = default, uint delta = 1, uint totalItems = default, long subKey = -1, long tradeId = -1)
        {
            Associate = associate;
            Confirmed = confirmed;
            DeleteItem = deleteItem;
            OutSuccess = outSuccess;
            InventoryType = inventoryType;
            ItemType = itemType;
            ExtraInfo = extraInfo;
            ForceDeletion = forceDeletion;
            Source = source;
            Item = item;
            ItemLot = itemLot;
            Requesting = requesting;
            Delta = delta;
            TotalItems = totalItems;
            SubKey = subKey;
            TradeId = tradeId;
        }
    }
}