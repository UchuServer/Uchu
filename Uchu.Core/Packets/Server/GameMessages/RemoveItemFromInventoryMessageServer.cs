using RakDotNet;
using Uchu.Core.Collections;

namespace Uchu.Core
{
    public class RemoveItemFromInventoryMessageServer : ServerGameMessage
    {
        public override ushort GameMessageId => 0x00e6;

        public bool Confirmed { get; set; } = false;

        public bool DeleteItem { get; set; } = true;

        public bool OutSuccess { get; set; } = false;

        public InventoryType InventoryType { get; set; } = InventoryType.Items;

        public ItemType ItemType { get; set; } = ItemType.Invalid;

        public LegoDataDictionary ExtraInfo { get; set; } = null;

        public bool ForceDeletion { get; set; } = true;

        public long LootTypeSourceID { get; set; } = -1;

        public long ObjID { get; set; } = -1;

        public int LOT { get; set; } = -1;

        public long RequestingObjID { get; set; } = -1;

        public uint StackCount { get; set; } = 1;

        public uint StackRemaining { get; set; } = 0;

        public long SubKey { get; set; } = -1;

        public long TradeID { get; set; } = -1;

        public override void SerializeMessage(BitStream stream)
        {
            stream.WriteBit(Confirmed);
            stream.WriteBit(DeleteItem);
            stream.WriteBit(OutSuccess);

            stream.WriteBit(InventoryType != InventoryType.Items);
            if (InventoryType != InventoryType.Items)
            {
                stream.WriteInt32((int) InventoryType);
            }

            stream.WriteBit(ItemType != ItemType.Invalid);
            if (ItemType != ItemType.Invalid)
            {
                stream.WriteInt32((int) ItemType);
            }

            if (ExtraInfo != null)
            {
                stream.WriteSerializable(ExtraInfo);
            }
            else
            {
                stream.WriteUInt32(0);
            }

            /*
            stream.WriteUInt32((uint) (ExtraInfo?.ToString().Length ?? 0));
            if (ExtraInfo != null && ExtraInfo?.ToString().Length > 0)
            {
                stream.WriteString(ExtraInfo.ToString(), ExtraInfo.ToString().Length, true);
            }
            */

            stream.WriteBit(ForceDeletion);

            stream.WriteBit(LootTypeSourceID != -1);
            if (LootTypeSourceID != -1)
            {
                stream.WriteInt64(LootTypeSourceID);
            }

            stream.WriteBit(ObjID != -1);
            if (ObjID != -1)
            {
                stream.WriteInt64(ObjID);
            }

            stream.WriteBit(LOT != -1);
            if (LOT != -1)
            {
                stream.WriteInt32(LOT);
            }

            stream.WriteBit(RequestingObjID != -1);
            if (RequestingObjID != -1)
            {
                stream.WriteInt64(RequestingObjID);
            }

            stream.WriteBit(StackCount != 1);
            if (StackCount != 1)
            {
                stream.WriteUInt32(StackCount);
            }

            stream.WriteBit(StackRemaining != 0);
            if (StackRemaining != 0)
            {
                stream.WriteUInt32(StackRemaining);
            }

            stream.WriteBit(SubKey != -1);
            if (SubKey != -1)
            {
                stream.WriteInt64(SubKey);
            }

            stream.WriteBit(TradeID != -1);
            if (TradeID != -1)
            {
                stream.WriteInt64(TradeID);
            }
        }
    }
}