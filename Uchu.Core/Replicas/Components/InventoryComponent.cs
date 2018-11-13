using RakDotNet;

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

                stream.WriteBit(false);
                // stream.WriteUInt(4);

                stream.WriteBit(false);

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