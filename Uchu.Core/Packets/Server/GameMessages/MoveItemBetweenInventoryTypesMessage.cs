using RakDotNet;

namespace Uchu.Core
{
    public class MoveItemBetweenInventoryTypesMessage : ServerGameMessage
    {
        public override ushort GameMessageId => 0x0445;

        public InventoryType CurrentType { get; set; }
        
        public InventoryType NewType { get; set; }

        public long ItemId { get; set; }

        public bool ShowFlyingLot { get; set; }
        
        public uint StackCount { get; set; }
        
        public int LOT { get; set; }
        
        public override void SerializeMessage(BitStream stream)
        {
            stream.WriteInt32((int) CurrentType);
            stream.WriteInt32((int) NewType);
            
            stream.WriteInt64(ItemId);
            
            stream.WriteBit(ShowFlyingLot);
            
            stream.WriteUInt32(StackCount);
            stream.WriteInt32(LOT);
        }
    }
}