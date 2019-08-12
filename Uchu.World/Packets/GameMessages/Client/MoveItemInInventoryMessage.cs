using RakDotNet.IO;
using Uchu.Core;

namespace Uchu.World
{
    public class MoveItemInInventoryMessage : ClientGameMessage
    {
        public override ushort GameMessageId => 0XE0;

        public InventoryType DestinationInventoryType { get; set; } = InventoryType.Invalid;
        
        public long ItemId { get; set; }
        
        public InventoryType CurrentInventoryType { get; set; }
        
        public int ResponseCode { get; set; }
        
        public int NewSlot { get; set; }
        
        public override void Deserialize(BitReader reader)
        {
            if (reader.ReadBit())
            {
                DestinationInventoryType = (InventoryType) reader.Read<int>();
            }

            ItemId = reader.Read<long>();

            CurrentInventoryType = (InventoryType) reader.Read<int>();

            ResponseCode = reader.Read<int>();

            NewSlot = reader.Read<int>();
        }
    }
}