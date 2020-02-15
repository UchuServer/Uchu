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
            
            Listen(OnStart, async () =>
            {
                await using var cdClient = new CdClientContext();
                
                var component = cdClient.ComponentsRegistryTable.FirstOrDefault(c =>
                    c.Id == GameObject.Lot && c.Componenttype == (int) ComponentId.InventoryComponent);

                var items = cdClient.InventoryComponentTable.Where(i => i.Id == component.Componentid).ToArray();

                Items = new Dictionary<EquipLocation, InventoryItem>();

                foreach (var item in items)
                {
                    Debug.Assert(item.Itemid != null, "item.Itemid != null");
                    Debug.Assert(item.Count != null, "item.Count != null");

                    await MountItemAsync(item.Itemid ?? 0, IdUtilities.GenerateObjectId());
                }
            });
        }

        public async Task EquipItemAsync(Item item, bool ignoreAllChecks = false)
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

            Logger.Debug($"Equipping {item}");

            await OnEquipped.InvokeAsync(item);

            await MountItemAsync(item.Lot, item.ObjectId, false, item.Settings);

            await ChangeEquippedSateOnPlayerAsync(item.ObjectId, true);

            GameObject.Serialize(GameObject);
        }

        public async Task UnEquipItemAsync(Item item)
        {
            await OnUnEquipped.InvokeAsync(item);
            
            if (item?.ObjectId <= 0) return;

            if (item != null)
            {
                await MountItemAsync(item.Lot, item.ObjectId, true);
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

        private static async Task<Lot[]> ParseProxyItemsAsync(Lot item)
        {
            await using var ctx = new CdClientContext();

            var itemInfo = await ctx.ItemComponentTable.FirstOrDefaultAsync(
                i => i.Id == item.GetComponentId(ComponentId.ItemComponent)
            );

            if (itemInfo == default) return new Lot[0];

            if (string.IsNullOrWhiteSpace(itemInfo.SubItems)) return new Lot[0];
            
            var proxies = itemInfo.SubItems
                .Replace(" ", "")
                .Split(',')
                .Select(i => (Lot) int.Parse(i));
            
            return proxies.ToArray();
        }

        public async Task<long> MountItemAsync(Lot inventoryItem, long id, bool unEquip = false, LegoDataDictionary settings = default)
        {
            if (Zone.TryGetGameObject<Item>(id, out var itemInstance))
            {
                itemInstance.Equipped = !unEquip;
            }
            
            await using var ctx = new CdClientContext();

            var itemInfo = await ctx.ItemComponentTable.FirstOrDefaultAsync(
                i => i.Id == inventoryItem.GetComponentId(ComponentId.ItemComponent)
            );

            if (itemInfo == default) return -1;

            var location = (EquipLocation) itemInfo.EquipLocation;
            
            var skills = GameObject.TryGetComponent<SkillComponent>(out var skillComponent);
            
            if (Items.TryGetValue(location, out var oldItem) && oldItem != default && skills)
            {
                foreach (var (key, proxyItems) in ProxyItems)
                {
                    if (!proxyItems.Contains(oldItem.InventoryItemId)) continue;
                    
                    var item = Items.Values.Where(v => v != default).FirstOrDefault(
                        v => v.InventoryItemId == key
                    );

                    if (item == default) goto equipItem;

                    await MountItemAsync(item.LOT, item.InventoryItemId, true);
                    
                    goto equipItem;
                }
                
                await skillComponent.DismountItemAsync(oldItem.LOT);
                
                if (ProxyItems.TryGetValue(oldItem.InventoryItemId, out var values))
                {
                    ProxyItems.Remove(oldItem.InventoryItemId);
                    
                    foreach (var value in values)
                    {
                        var item = Items.Values.Where(v => v != default).FirstOrDefault(
                            v => v.InventoryItemId == value
                        );
                        
                        if (item == default) continue;

                        await MountItemAsync(item.LOT, item.InventoryItemId, true);
                    }
                }
            }
            
            equipItem:

            if (unEquip)
            {
                Items[location] = default;
                
                GameObject.Serialize(GameObject);

                return -1;
            }
            
            var inventoryType = ((ItemType) (itemInfo.ItemType ?? 0)).GetInventoryType();

            Items[location] = new InventoryItem
            {
                LOT = inventoryItem,
                Count = 1,
                ExtraInfo = settings?.ToString(),
                InventoryItemId = id,
                InventoryType = (int) inventoryType,
                Slot = -1
            };

            GameObject.Serialize(GameObject);

            if (!skills || inventoryItem == default) return -1;

            await skillComponent.MountItemAsync(inventoryItem);

            /*
             * Equip proxies
             */

            var additionalItems = await ParseProxyItemsAsync(inventoryItem);

            if (additionalItems.Length <= 0) return id;
            
            var proxies = new List<long>();
            
            foreach (var proxy in additionalItems)
            {
                var proxyId = await MountItemAsync(proxy, IdUtilities.GenerateObjectId());

                proxies.Add(proxyId);
            }

            ProxyItems[id] = proxies.ToArray();

            return id;
        }

        public override void Construct(BitWriter writer)
        {
            Serialize(writer);
        }

        public override void Serialize(BitWriter writer)
        {
            writer.WriteBit(true);

            var items = Items.Values.Where(k => k != default).ToList();

            writer.Write((uint) items.Count);

            foreach (var item in items)
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