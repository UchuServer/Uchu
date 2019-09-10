using System.Numerics;
using RakDotNet;
using RakDotNet.IO;
using Uchu.World.Collections;

namespace Uchu.World
{
    public class AddItemToInventoryMessage : ServerGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.AddItemToInventoryClientSync;
        
        public bool IsBound { get; set; } = false;
        
        public bool IsBoundOnEquip { get; set; } = false;
        
        public bool IsBoundOnPickup { get; set; } = false;
        
        public int Source { get; set; } = -1;
        
        public LegoDataDictionary ExtraInfo { get; set; } = null;
        
        public int ItemLot { get; set; }
        
        public long SubKey { get; set; } = -1;
        
        public int InventoryType { get; set; } = -1;
        
        public uint ItemCount { get; set; } = 1;
        
        public uint TotalItems { get; set; } = 0;
        
        public long ItemObjectId { get; set; }
        
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

            if (hasSource)
                writer.Write(Source);

            if (ExtraInfo != null)
            {
                var ldf = ExtraInfo.ToString();

                writer.Write((uint) ldf.Length);

                if (ldf.Length > 0)
                {
                    writer.WriteString(ldf, ldf.Length, true);

                    writer.Write((byte) 0);
                    writer.Write((byte) 0);
                }
            }
            else
            {
                writer.Write(0u);
            }

            writer.Write(ItemLot);

            var hasSubKey = SubKey != -1;

            writer.WriteBit(hasSubKey);

            if (hasSubKey)
                writer.Write(SubKey);

            var hasInvType = InventoryType != -1;

            writer.WriteBit(hasInvType);

            if (hasInvType)
                writer.Write(InventoryType);

            var hasCount = ItemCount != 1;

            writer.WriteBit(hasCount);

            if (hasCount)
                writer.Write(ItemCount);

            var hasTotal = TotalItems != 0;

            writer.WriteBit(hasTotal);

            if (hasTotal)
                writer.Write(TotalItems);

            writer.Write(ItemObjectId);

            writer.Write(FlyingLootPosition.X);
            writer.Write(FlyingLootPosition.Y);
            writer.Write(FlyingLootPosition.Z);

            writer.WriteBit(ShowFlyingLoot);

            writer.Write(Slot);
        }
    }
}