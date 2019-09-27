using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RakDotNet.IO;
using Uchu.Core;
using Uchu.Core.CdClient;
using Uchu.World.Collections;
using Uchu.World.Parsers;

namespace Uchu.World
{
    public class InventoryComponent : ReplicaComponent
    {
        public Dictionary<EquipLocation, InventoryItem> Items { get; set; } =
            new Dictionary<EquipLocation, InventoryItem>();

        public override ReplicaComponentsId Id => ReplicaComponentsId.Inventory;

        public override void FromLevelObject(LevelObject levelObject)
        {
            using (var cdClient = new CdClientContext())
            {
                var component = cdClient.ComponentsRegistryTable.FirstOrDefault(c =>
                    c.Id == levelObject.Lot && c.Componenttype == (int) ReplicaComponentsId.Inventory);

                var items = cdClient.InventoryComponentTable.Where(i => i.Id == component.Componentid).ToArray();

                Items = new Dictionary<EquipLocation, InventoryItem>();

                foreach (var item in items)
                {
                    var cdClientObject = cdClient.ObjectsTable.FirstOrDefault(
                        o => o.Id == item.Itemid
                    );

                    var itemRegistryEntry = cdClient.ComponentsRegistryTable.FirstOrDefault(
                        r => r.Id == item.Itemid && r.Componenttype == 11
                    );

                    if (cdClientObject == default || itemRegistryEntry == default)
                    {
                        Logger.Error($"{item.Itemid} is not a valid item");
                        continue;
                    }

                    var itemComponent = cdClient.ItemComponentTable.First(
                        i => i.Id == itemRegistryEntry.Componentid
                    );

                    Items.TryAdd(itemComponent.EquipLocation, new InventoryItem
                    {
                        InventoryItemId = IdUtils.GenerateObjectId(),
                        Count = (long) item.Count,
                        LOT = (int) item.Itemid,
                        Slot = -1,
                        InventoryType = -1
                    });
                }
            }
        }

        public void EquipUnmanagedItem(Lot lot, uint count = 1, int slot = -1,
            InventoryType inventoryType = InventoryType.None)
        {
            using (var cdClient = new CdClientContext())
            {
                var cdClientObject = cdClient.ObjectsTable.FirstOrDefault(
                    o => o.Id == lot
                );

                var itemRegistryEntry = lot.GetComponentId(ReplicaComponentsId.Item);

                if (cdClientObject == default || itemRegistryEntry == default)
                {
                    Logger.Error($"{lot} is not a valid item");
                    return;
                }

                var itemComponent = cdClient.ItemComponentTable.First(
                    i => i.Id == itemRegistryEntry
                );

                Items.Add(itemComponent.EquipLocation, new InventoryItem
                {
                    InventoryItemId = IdUtils.GenerateObjectId(),
                    Count = count,
                    Slot = slot,
                    LOT = lot,
                    InventoryType = (int) inventoryType
                });
            }
        }

        public void EquipItem(Item item)
        {
            Logger.Debug($"Equipping {item}");
            
            var items = Items.Select(i => (i.Key, i.Value)).ToArray();
            foreach (var (equipLocation, value) in items)
                if (equipLocation.Equals(item.ItemComponent.EquipLocation))
                    UnEquipItem(value.InventoryItemId);

            Items.Add(item.ItemComponent.EquipLocation, item.InventoryItem);

            Task.Run(async () => { await ChangeEquippedSateOnPlayerAsync(item.ObjectId, true); });

            GameObject.Serialize(GameObject);
        }

        public void UnEquipItem(Item item)
        {
            UnEquipItem(item.ObjectId);
        }

        public void UnEquipItem(long id)
        {
            var (equipLocation, value) = Items.FirstOrDefault(i => i.Value.InventoryItemId == id);

            if (value == default)
            {
                Logger.Error($"{GameObject} does not have an item of id: {id} equipped");
                return;
            }

            Items.Remove(equipLocation);

            Task.Run(async () => { await ChangeEquippedSateOnPlayerAsync(id, false); });

            GameObject.Serialize(GameObject);
        }

        private async Task ChangeEquippedSateOnPlayerAsync(long itemId, bool equipped)
        {
            if (As<Player>() != null)
                using (var ctx = new UchuContext())
                {
                    var inventoryItem = await ctx.InventoryItems.FirstAsync(i => i.InventoryItemId == itemId);

                    inventoryItem.IsEquipped = equipped;

                    await ctx.SaveChangesAsync();
                }
        }

        public override void Construct(BitWriter writer)
        {
            Serialize(writer);
        }

        public override void Serialize(BitWriter writer)
        {
            writer.WriteBit(true);

            writer.Write((uint) Items.Count);

            foreach (var (_, item) in Items)
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