using RakDotNet;
using RakDotNet.IO;
using Uchu.World.Collections;

namespace Uchu.World
{
    public class RemoveItemToInventoryMessage : GeneralGameMessage
    {
        public override ushort GameMessageId => 0xE6;
        
        public bool Confirmed { get; set; } = false;

        public bool DeleteItem { get; set; } = true;

        public bool OutSuccess { get; set; } = false;

        public int InventoryType { get; set; }
        
        public ItemType ItemType { get; set; }
        
        public LegoDataDictionary ExtraInfo { get; set; }

        public bool ForceDeletion { get; set; } = true;
        
        public long LootTypeSourceId { get; set; }
        
        public long ItemObjectId { get; set; }
        
        public Lot ItemLot { get; set; }
        
        public long RequestingObjId { get; set; }

        public uint StackCount { get; set; } = 1;
        
        public uint StackRemaining { get; set; }
        
        public long SubKey { get; set; }
        
        public long TradeId { get; set; }

        public override void SerializeMessage(BitWriter writer)
        {
            writer.Flag(Confirmed);
            writer.Flag(DeleteItem);
            writer.Flag(OutSuccess);

            if (writer.Flag(InventoryType != default))
            {
                writer.Write(InventoryType);
            }

            if (writer.Flag(ItemType != ItemType.Invalid))
            {
                writer.Write((int) ItemType);
            }
            
            if (writer.Flag(ExtraInfo != default))
            {
                writer.Write(ExtraInfo);
            }
            else
            {
                writer.Write<uint>(0);
            }

            writer.Flag(ForceDeletion);
            
            if (writer.Flag(LootTypeSourceId != -1))
            {
                writer.Write(LootTypeSourceId);
            }

            if (writer.Flag(ItemObjectId != -1))
            {
                writer.Write(ItemObjectId);
            }

            if (writer.Flag(ItemLot != -1))
            {
                writer.Write(ItemLot);
            }

            if (writer.Flag(RequestingObjId != -1))
            {
                writer.Write(RequestingObjId);
            }

            if (writer.Flag(StackCount != 1))
            {
                writer.Write(StackCount);
            }

            if (writer.Flag(StackRemaining != 0))
            {
                writer.Write(StackRemaining);
            }

            if (writer.Flag(SubKey != -1))
            {
                writer.Write(SubKey);
            }

            if (writer.Flag(TradeId != -1))
            {
                writer.Write(TradeId);
            }
        }

        public override void Deserialize(BitReader reader)
        {
            Confirmed = reader.Flag();
            DeleteItem = reader.Flag();
            OutSuccess = reader.Flag();

            if (reader.Flag())
            {
                InventoryType = reader.Read<int>();
            }

            if (reader.Flag())
            {
                ItemType = (ItemType) reader.Read<int>();
            }

            var len = reader.Read<uint>();
            if (len > 0)
            {
                var info = reader.ReadString((int) len, true);
                ExtraInfo = LegoDataDictionary.FromString(info);
            }

            ForceDeletion = reader.Flag();

            if (reader.Flag())
            {
                LootTypeSourceId = reader.Read<long>();
            }

            if (reader.Flag())
            {
                ItemObjectId = reader.Read<long>();
            }

            if (reader.Flag())
            {
                RequestingObjId = reader.Read<long>();
            }

            if (reader.Flag())
            {
                StackRemaining = reader.Read<uint>();
            }

            if (reader.Flag())
            {
                SubKey = reader.Read<long>();
            }

            if (reader.Flag())
            {
                TradeId = reader.Read<long>();
            }
        }
    }
}