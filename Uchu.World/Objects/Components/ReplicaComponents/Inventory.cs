using RakDotNet.IO;
using Uchu.Core;
using Uchu.World.Collections;

namespace Uchu.World
{
    [Essential]
    public class Inventory : ReplicaComponent
    {
        public InventoryItem[] Items { get; set; } = new InventoryItem[0];

        public override ReplicaComponentsId Id => ReplicaComponentsId.Inventory;
        
        public override void Construct(BitWriter writer)
        {
            Serialize(writer);
        }

        public override void Serialize(BitWriter writer)
        {
            writer.WriteBit(true);

            writer.Write((uint) Items.Length);

            foreach (var item in Items)
            {
                writer.Write(item.InventoryItemId);
                writer.Write(item.LOT);

                writer.WriteBit(false);

                var stack = item.Count > 1;

                writer.WriteBit(stack);

                if (stack) writer.Write((uint) item.Count);

                var hasSlot = item.Slot != -1;

                writer.WriteBit(hasSlot);

                if (hasSlot) writer.Write((ushort) item.Slot);

                var hasInventoryType = item.InventoryType != -1;

                writer.WriteBit(hasInventoryType);

                if (hasInventoryType) writer.Write((uint) item.InventoryType);

                var hasExtraData = !string.IsNullOrEmpty(item.ExtraInfo);

                writer.WriteBit(hasExtraData);

                if (hasExtraData) writer.WriteLdfCompressed(LegoDataDictionary.FromString(item.ExtraInfo, ","));

                writer.WriteBit(true);
            }

            writer.WriteBit(true);
            writer.Write<uint>(0);
        }
    }
}