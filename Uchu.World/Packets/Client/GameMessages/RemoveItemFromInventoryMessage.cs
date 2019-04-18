using RakDotNet;
using Uchu.Core;
using Uchu.Core.Collections;

namespace Uchu.World
{
    public class RemoveItemFromInventoryMessage : ClientGameMessage
    {
        /*
         * TODO: Implement.
         */
        
        public override ushort GameMessageId => 0x00e6;

        public bool Confirmed { get; set; } = false;

        public bool DeleteItem { get; set; } = true;

        public bool OutSuccess { get; set; } = false;

        public InventoryType InventoryType { get; set; } = InventoryType.Items;
        
        public ItemType ItemType { get; set; }
        
        public LegoDataDictionary ExtraInfo { get; set; }

        public bool ForceDeletion { get; set; } = true;
        
        public long LootTypeSourceID { get; set; }
        
        public long ObjID { get; set; }
        
        public int LOT { get; set; }
        
        public long RequestingObjID { get; set; }

        public uint StackCount { get; set; } = 1;
        
        public uint StackRemaining { get; set; }
        
        public long SubKey { get; set; }
        
        public long TradeID { get; set; }
        
        public override void Deserialize(BitStream stream)
        {
            Confirmed = stream.ReadBit();
            DeleteItem = stream.ReadBit();
            OutSuccess = stream.ReadBit();

            if (stream.ReadBit())
            {
                InventoryType = (InventoryType) stream.ReadInt32();
            }

            if (stream.ReadBit())
            {
                ItemType = (ItemType) stream.ReadInt32();
            }

            var len = stream.ReadUInt32();
            if (len > 0)
            {
                var info = stream.ReadString((int) len, true);
                ExtraInfo = LegoDataDictionary.FromString(info);
            }

            ForceDeletion = stream.ReadBit();

            if (stream.ReadBit())
            {
                LootTypeSourceID = stream.ReadInt64();
            }

            if (stream.ReadBit())
            {
                ObjID = stream.ReadInt64();
            }

            if (stream.ReadBit())
            {
                LOT = stream.ReadInt32();
            }

            if (stream.ReadBit())
            {
                RequestingObjID = stream.ReadInt64();
            }

            if (stream.ReadBit())
            {
                StackCount = stream.ReadUInt32();
            }

            if (stream.ReadBit())
            {
                StackRemaining = stream.ReadUInt32();
            }

            if (stream.ReadBit())
            {
                SubKey = stream.ReadInt64();
            }

            if (stream.ReadBit())
            {
                TradeID = stream.ReadInt64();
            }
        }
    }
}