using System.Numerics;
using RakDotNet;
using RakDotNet.IO;
using Uchu.World.Collections;

namespace Uchu.World
{
    public class AddItemToInventoryMessage : ServerGameMessage
    {
        public override ushort GameMessageId => 0xE3;
        
        public bool IsBound { get; set; }
        
        public bool IsBoundOnEquip { get; set; }
        
        public bool IsBoundOnPickup { get; set; }
        
        public int Source { get; set; } = -1;
        
        public LegoDataDictionary ExtraInfo { get; set; }
        
        public int ItemLot { get; set; }
        
        public long SubKey { get; set; } = -1;
        
        public int Inventory { get; set; } = -1;
        
        public uint ItemCount { get; set; } = 1;
        
        public uint TotalItems { get; set; }
        
        public long ItemObjectId { get; set; }
        
        public Vector3 FlyingLootPosition { get; set; } = Vector3.Zero;
        
        public bool ShowFlyingLoot { get; set; } = true;
        
        public int Slot { get; set; }

        public override void SerializeMessage(BitWriter writer)
        {
            writer.Flag(IsBound);
            writer.Flag(IsBoundOnEquip);
            writer.Flag(IsBoundOnPickup);

            if (writer.Flag(Source != -1))
            {
                writer.Write(Source);
            }

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

            if (writer.Flag(SubKey != -1))
            {
                writer.Write(SubKey);
            }

            if (writer.Flag(Inventory != -1))
            {
                writer.Write(Inventory);
            }

            if (writer.Flag(ItemCount != 1))
            {
                writer.Write(ItemCount);
            }

            if (writer.Flag(TotalItems != 0))
            {
                writer.Write(TotalItems);
            }
            
            writer.Write(ItemObjectId);

            writer.Write(FlyingLootPosition);

            writer.Flag(ShowFlyingLoot);

            writer.Write(Slot);
        }
    }
}