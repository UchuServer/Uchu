using System.Collections.Generic;
using System.Linq;
using RakDotNet.IO;
using Uchu.Core;
using Uchu.Core.CdClient;
using Uchu.World.Collections;
using Uchu.World.Parsers;

namespace Uchu.World
{
    [Essential]
    public class InventoryComponent : ReplicaComponent
    {
        public List<InventoryItem> Items { get; set; } = new List<InventoryItem>();

        public override ReplicaComponentsId Id => ReplicaComponentsId.Inventory;

        public override void FromLevelObject(LevelObject levelObject)
        {
            using (var ctx = new CdClientContext())
            {
                var component = ctx.ComponentsRegistryTable.FirstOrDefault(c =>
                    c.Id == levelObject.Lot && c.Componenttype == (int) ReplicaComponentsId.Inventory);

                var items = ctx.InventoryComponentTable.Where(i => i.Id == component.Componentid).ToArray();

                Items = new List<InventoryItem>();

                foreach (var item in items)
                {
                    Items.Add(new InventoryItem
                    {
                        InventoryItemId = Utils.GenerateObjectId(),
                        Count = (long) item.Count,
                        LOT = (int) item.Itemid,
                        Slot = -1,
                        InventoryType = -1
                    });
                }
            }
        }

        public void EquipItem(int lot, uint count = 1, int slot = -1, InventoryType inventoryType = InventoryType.None)
        {
            Items.Add(new InventoryItem
            {
                InventoryItemId = Utils.GenerateObjectId(),
                Count = count,
                Slot = slot,
                LOT = lot,
                InventoryType = (int) inventoryType
            });
        }

        public void UnEquipItem(int lot)
        {
            var items = Items.Where(i => i.LOT == lot).ToArray();

            if (items.Any())
            {
                Logger.Error($"{GameObject} does not have an item of lot: {lot} equipped");
                return;
            }

            foreach (var item in items)
            {
                Items.Remove(item);
            }
        }

        public void UnEquipItem(long id)
        {
            var item = Items.FirstOrDefault(i => i.InventoryItemId == id);

            if (item == default)
            {
                Logger.Error($"{GameObject} does not have an item of id: {id} equipped");
                return;
            }

            Items.Remove(item);
        }

        public override void Construct(BitWriter writer)
        {
            Serialize(writer);
        }

        public override void Serialize(BitWriter writer)
        {
            writer.WriteBit(true);

            writer.Write((uint) Items.Count);

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