using RakDotNet;
using Uchu.Core.Collections;

namespace Uchu.Core
{
    public class InventoryComponent : ReplicaComponent
    {
        public InventoryItem[] Items { get; set; } = new InventoryItem[0];

        public override void Serialize(BitStream stream)
        {
            stream.WriteBit(true);

            stream.WriteUInt((uint) Items.Length);

            foreach (var item in Items)
            {
                stream.WriteLong(item.InventoryItemId);
                stream.WriteInt(item.LOT);

                stream.WriteBit(false);

                var stack = item.Count > 1;

                stream.WriteBit(stack);

                if (stack)
                    stream.WriteUInt((uint) item.Count);

                var hasSlot = item.Slot != -1;

                stream.WriteBit(hasSlot);

                if (hasSlot)
                    stream.WriteUShort((ushort) item.Slot);

                var hasInvtype = item.InventoryType != -1;

                stream.WriteBit(hasInvtype);

                if (hasInvtype)
                    stream.WriteUInt((uint) item.InventoryType);

                var hasExtra = !string.IsNullOrEmpty(item.ExtraInfo);

                stream.WriteBit(hasExtra);

                if (hasExtra)
                    stream.WriteLDFCompressed(LegoDataDictionary.FromString(item.ExtraInfo, ","));

                stream.WriteBit(true);
            }

            stream.WriteBit(true);
            stream.WriteUInt(0);
        }

        public override void Construct(BitStream stream)
        {
            Serialize(stream);
        }
    }
}