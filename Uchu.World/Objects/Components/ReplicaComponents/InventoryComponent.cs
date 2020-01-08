using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using InfectedRose.Lvl;
using Microsoft.EntityFrameworkCore;
using RakDotNet.IO;
using Uchu.Core;
using Uchu.Core.Client;

namespace Uchu.World
{
    public class InventoryComponent : ReplicaComponent
    {
        public AsyncEvent<Item> OnEquipped { get; } = new AsyncEvent<Item>();
        
        public AsyncEvent<Item> OnUnEquipped { get; } = new AsyncEvent<Item>();
        
        public Dictionary<EquipLocation, InventoryItem> Items { get; set; } =
            new Dictionary<EquipLocation, InventoryItem>();

        private Dictionary<long, long[]> ProxyItems { get; set; } = new Dictionary<long, long[]>();
        
        public override ComponentId Id => ComponentId.InventoryComponent;

        protected InventoryComponent()
        {
            Listen(OnDestroyed, () =>
            {
                OnEquipped.Clear();
                OnUnEquipped.Clear();
            });
            
            Listen(OnStart, () =>
            {
                using var cdClient = new CdClientContext();
            
                var component = cdClient.ComponentsRegistryTable.FirstOrDefault(c =>
                    c.Id == GameObject.Lot && c.Componenttype == (int) ComponentId.InventoryComponent);

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

                    Debug.Assert(item.Itemid != null, "item.Itemid != null");
                    Debug.Assert(item.Count != null, "item.Count != null");
                    
                    Items.TryAdd(itemComponent.EquipLocation, new InventoryItem
                    {
                        InventoryItemId = IdUtilities.GenerateObjectId(),
                        Count = (long) item.Count,
                        LOT = (int) item.Itemid,
                        Slot = -1,
                        InventoryType = -1
                    });
                }
            });
        }

        public long EquipUnmanagedItem(Lot lot, uint count = 1, int slot = -1,
            InventoryType inventoryType = InventoryType.None)
        {
            using var cdClient = new CdClientContext();
            var cdClientObject = cdClient.ObjectsTable.FirstOrDefault(
                o => o.Id == lot
            );

            var itemRegistryEntry = lot.GetComponentId(ComponentId.ItemComponent);

            if (cdClientObject == default || itemRegistryEntry == default)
            {
                Logger.Error($"{lot} is not a valid item");
                return -1;
            }

            var itemComponent = cdClient.ItemComponentTable.First(
                i => i.Id == itemRegistryEntry
            );

            var id = IdUtilities.GenerateObjectId();
            
            Items[itemComponent.EquipLocation] = new InventoryItem
            {
                InventoryItemId = id,
                Count = count,
                Slot = slot,
                LOT = lot,
                InventoryType = (int) inventoryType
            };

            return id;
        }

        public async Task EquipItem(Item item, bool ignoreAllChecks = false)
        {
            if (item?.InventoryItem == null)
            {
                Logger.Error($"{item} is not a valid item");
                
                return;
            }

            var itemType = (ItemType) (item.ItemComponent.ItemType ?? (int) ItemType.Invalid);

            if (!ignoreAllChecks)
            {
                if (!As<Player>().GetComponent<ModularBuilderComponent>().IsBuilding)
                {
                    if (itemType == ItemType.Model || itemType == ItemType.LootModel || itemType == ItemType.Vehicle || item.Lot == 6086)
                    {
                        return;
                    }
                }
            }
            
            /*
             * Equip proxies
             */

            var proxies = await GenerateProxyItems(item);

            if (proxies?.Length > 0)
            {
                ProxyItems[item.ObjectId] = proxies;
            }

            Logger.Debug($"Equipping {item}");
            
            var items = Items.Select(i => (i.Key, i.Value)).ToArray();
            
            foreach (var (equipLocation, value) in items)
            {
                if (!equipLocation.Equals(item.ItemComponent.EquipLocation)) continue;
                
                var manager = GameObject.GetComponent<InventoryManagerComponent>();
                
                if (manager != default)
                {
                    var oldItem = manager.FindItem(value.InventoryItemId);

                    await UnEquipItem(oldItem);
                }
                else
                {
                    await UnEquipItem(value.InventoryItemId);
                }
            }
            
            await OnEquipped.InvokeAsync(item);

            Items[item.ItemComponent.EquipLocation] = item.InventoryItem;

            await ChangeEquippedSateOnPlayerAsync(item.ObjectId, true);

            GameObject.Serialize(GameObject);
        }

        public async Task UnEquipItem(Item item)
        {
            OnUnEquipped?.Invoke(item);
            
            if (item?.ObjectId <= 0) return;

            if (item != null) await UnEquipItem(item.ObjectId);
        }

        private async Task UnEquipItem(long id)
        {
            var (equipLocation, value) = Items.FirstOrDefault(i => i.Value.InventoryItemId == id);

            if (value == default)
            {
                //
                // It's quite common for the client to send un-equip requests for items that it uses or whatever.
                //
                return;
            }

            Items.Remove(equipLocation);

            await ChangeEquippedSateOnPlayerAsync(id, false);

            GameObject.Serialize(GameObject);

            if (ProxyItems.TryGetValue(id, out var proxies))
            {
                foreach (var proxy in proxies)
                {
                    await UnEquipItem(proxy);
                }
            }
        }

        private async Task ChangeEquippedSateOnPlayerAsync(long itemId, bool equipped)
        {
            if (!(GameObject is Player)) return;
                
            await using var ctx = new UchuContext();
            
            var inventoryItem = await ctx.InventoryItems.FirstOrDefaultAsync(i => i.InventoryItemId == itemId);
            
            // Check if it's a proxy or alike.
            if (inventoryItem == default) return;

            inventoryItem.IsEquipped = equipped;

            await ctx.SaveChangesAsync();
        }

        private async Task<long[]> GenerateProxyItems(Item item)
        {
            if (string.IsNullOrWhiteSpace(item?.ItemComponent?.SubItems)) return null;
            
            var proxies = item.ItemComponent.SubItems
                .Replace(" ", "")
                .Split(',')
                .Select(int.Parse);

            var list = new List<long>();

            await using var cdClient = new CdClientContext();

            foreach (Lot proxy in proxies)
            {
                var componentId = proxy.GetComponentId(ComponentId.ItemComponent);

                var component = await cdClient.ItemComponentTable.FirstOrDefaultAsync(i => i.Id == componentId);

                if (component == default) continue;

                list.Add(EquipUnmanagedItem(
                    proxy,
                    inventoryType: item.Inventory.InventoryType)
                );
            }

            return list.ToArray();
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

                var hasExtraData = !string.IsNullOrWhiteSpace(item.ExtraInfo);

                writer.WriteBit(hasExtraData);

                if (hasExtraData) writer.WriteLdfCompressed(LegoDataDictionary.FromString(item.ExtraInfo));

                writer.WriteBit(true);
            }

            writer.WriteBit(true);
            writer.Write<uint>(0);
        }
    }
}