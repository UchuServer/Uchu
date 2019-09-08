using System.Numerics;
using RakDotNet;
using RakDotNet.IO;
using Uchu.World.Collections;

namespace Uchu.World
{
    public class AddItemToInventoryMessage : ServerGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.AddItemToInventoryClientSync;
        
        public bool IsBound { get; set; }
        
        public bool IsBoundOnEquip { get; set; }
        
        public bool IsBoundOnPickup { get; set; }
        
        public Lot Source { get; set; } = -1;
        
        public LegoDataDictionary ExtraInfo { get; set; }
        
        public Lot ItemLot { get; set; }
        
        public long SubKey { get; set; } = -1;
        
        public int Inventory { get; set; } = -1;
        
        public uint Count { get; set; } = 1;
        
        public uint TotalItems { get; set; }

        public long ItemObjectId { get; set; } = -1;
        
        public Vector3 FlyingLootPosition { get; set; } = Vector3.Zero;
        
        public bool ShowFlyingLoot { get; set; } = true;
        
        public int Slot { get; set; }

        public override void SerializeMessage(BitWriter writer)
        {
            writer.WriteBit(IsBound);
            writer.WriteBit(IsBoundOnEquip);
            writer.WriteBit(IsBoundOnPickup);

            var hasSource = Source != -1;
            writer.WriteBit(hasSource);
            if (hasSource) writer.Write(Source);

            if (ExtraInfo != null)
            {
                var ldf = ExtraInfo.ToString();

                writer.Write((uint) ldf.Length);

                if (ldf.Length > 0)
                {
                    writer.WriteString(ldf, ldf.Length, true);

                    writer.Write<byte>(0);
                    writer.Write<byte>(0);
                }
            }
            else
            {
                writer.Write<uint>(0);
            }

            writer.Write(ItemLot);

            var hasSubKey = SubKey != -1;
            writer.WriteBit(hasSubKey);
            if (hasSubKey) writer.Write(SubKey);
            
            //
            // The defaults are not worth calculating.
            //

            writer.WriteBit(true);
            writer.Write(Inventory);

            writer.WriteBit(true);
            writer.Write(Count);
            
            writer.WriteBit(true);
            writer.Write(TotalItems);
            
            writer.Write(ItemObjectId);

            writer.Write(FlyingLootPosition);
            
            writer.WriteBit(ShowFlyingLoot);

            writer.Write(Slot);
        }
    }
}