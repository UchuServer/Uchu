using RakDotNet.IO;
using Uchu.Core;

namespace Uchu.World
{
    public class MoveItemBetweenInventoryTypesMessage : ClientGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.MoveItemBetweenInventoryTypes;

        public InventoryType SourceInventory { get; set; }
        
        public InventoryType DestinationInventory { get; set; }
        
        public Item Item { get; set; }
        
        public bool ShowFlyingLot { get; set; }

        public uint StackCount { get; set; } = 1;
        
        public Lot Lot { get; set; }
        
        public override void Deserialize(BitReader reader)
        {
            SourceInventory = (InventoryType) reader.Read<int>();
            DestinationInventory = (InventoryType) reader.Read<int>();

            Item = reader.ReadGameObject<Item>(Associate.Zone);

            ShowFlyingLot = reader.ReadBit();

            if (reader.ReadBit()) StackCount = reader.Read<uint>();

            if (reader.ReadBit()) Lot = reader.Read<Lot>();
        }
    }
}