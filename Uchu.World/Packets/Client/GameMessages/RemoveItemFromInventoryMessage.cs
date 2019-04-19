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
        
        public InventoryType InventoryType { get; set; }
        
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
            base.Deserialize(stream);
        }
    }
}