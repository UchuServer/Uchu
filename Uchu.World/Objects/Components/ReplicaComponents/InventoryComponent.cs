using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InfectedRose.Lvl;
using Microsoft.EntityFrameworkCore;
using RakDotNet.IO;
using Uchu.Core;
using Uchu.Core.Client;
using Uchu.World.Client;

namespace Uchu.World
{
    public class InventoryComponent : ReplicaComponent
    {
        public override ComponentId Id => ComponentId.InventoryComponent;
        
        public Dictionary<EquipLocation, EquippedItem> Items { get; }
        
        public Event<Item> OnEquipped { get; }
        
        public Event<Item> OnUnEquipped { get; }

        protected InventoryComponent()
        {
            Items = new Dictionary<EquipLocation, EquippedItem>();
            
            OnEquipped = new Event<Item>();
            
            OnUnEquipped = new Event<Item>();

            Listen(OnDestroyed, () =>
            {
                OnEquipped.Clear();
                OnUnEquipped.Clear();
            });
            
            Listen(OnStart, () =>
            {
                if (GameObject is Player) return;
                
                var component = ClientCache.GetTable<ComponentsRegistry>().FirstOrDefault(c =>
                    c.Id == GameObject.Lot && c.Componenttype == (int) ComponentId.InventoryComponent);

                var items = ClientCache.GetTable<Core.Client.InventoryComponent>().Where(i => i.Id == component.Componentid).ToArray();

                foreach (var item in items)
                {
                    if (item.Itemid == default) continue;
                    
                    var lot = (Lot) item.Itemid;

                    var componentId = lot.GetComponentId(ComponentId.ItemComponent);

                    var info = ClientCache.GetTable<ItemComponent>().First(i => i.Id == componentId);
                    
                    var location = (EquipLocation) info.EquipLocation;
                    
                    Items[location] = new EquippedItem
                    {
                        Id = ObjectId.Standalone,
                        Lot = lot
                    };
                }
            });
        }

        private async Task UpdateSlotAsync(EquipLocation slot, EquippedItem item)
        {
            if (Items.TryGetValue(slot, out var previous))
            {
                var id = await FindRootAsync(previous.Id);

                await UnEquipAsync(id);
            }

            Items[slot] = item;
        }

        private EquipLocation FindSlot(ObjectId id)
        {
            var reference = Items.FirstOrDefault(i => i.Value.Id == id);

            return reference.Key;
        }

        public async Task EquipAsync(EquippedItem item)
        {
            var componentId = item.Lot.GetComponentId(ComponentId.ItemComponent);

            var info = (await ClientCache.GetTableAsync<ItemComponent>()).First(i => i.Id == componentId);
            
            var location = (EquipLocation) info.EquipLocation;

            await UpdateSlotAsync(location, item);

            var skills = GameObject.TryGetComponent<SkillComponent>(out var skillComponent);

            if (skills)
            {
                await skillComponent.MountItemAsync(item.Lot);
            }

            await UpdateEquipState(item.Id, true);

            var proxies = await GenerateProxiesAsync(item.Id);

            foreach (var proxy in proxies)
            {
                var instance = await proxy.FindItemAsync();

                var lot = (Lot) instance.Lot;
                
                componentId = lot.GetComponentId(ComponentId.ItemComponent);

                info = (await ClientCache.GetTableAsync<ItemComponent>()).FirstOrDefault(i => i.Id == componentId);
            
                if (info == default) continue;
                
                location = (EquipLocation) info.EquipLocation;
                
                await UpdateSlotAsync(location, new EquippedItem
                {
                    Id = proxy,
                    Lot = lot
                });

                await UpdateEquipState(proxy, true);
                
                if (skills)
                {
                    await skillComponent.MountItemAsync(lot);
                }
            }
        }

        public bool HasEquipped(Lot lot)
        {
            return Items.Any(i => i.Value.Lot == lot);
        }

        private async Task UnEquipAsync(ObjectId id)
        {
            id = await FindRootAsync(id);

            var slot = FindSlot(id);

            if (!Items.TryGetValue(slot, out var info)) return;

            var skills = GameObject.TryGetComponent<SkillComponent>(out var skillComponent);
            
            if (skills)
            {
                await skillComponent.DismountItemAsync(info.Lot);
            }

            await UpdateEquipState(id, false);

            Items.Remove(slot);

            var proxies = await FindProxiesAsync(id);

            foreach (var proxy in proxies)
            {
                slot = FindSlot(proxy);

                if (Items.TryGetValue(slot, out info))
                {
                    if (skills)
                    {
                        await skillComponent.DismountItemAsync(info.Lot);
                    }

                    Items.Remove(slot);
                }

                await UpdateEquipState(proxy, false);
            }

            await ClearProxiesAsync(id);
        }

        public async Task<bool> EquipItemAsync(Item item, bool ignoreAllChecks = false)
        {
            var itemType = (ItemType) (item.ItemComponent.ItemType ?? (int) ItemType.Invalid);

            if (!ignoreAllChecks)
            {
                if (GameObject is Player player)
                {
                    if (!player.GetComponent<ModularBuilderComponent>().IsBuilding)
                    {
                        if (itemType == ItemType.Model || itemType == ItemType.LootModel ||
                            itemType == ItemType.Vehicle || item.Lot == 6086)
                        {
                            return false;
                        }
                    }
                }
            }
            
            await OnEquipped.InvokeAsync(item);

            await MountItemAsync(item.Id);

            GameObject.Serialize(GameObject);

            return true;
        }

        public async Task UnEquipItemAsync(Item item)
        {
            await OnUnEquipped.InvokeAsync(item);
            
            if (item?.Id <= 0) return;

            if (item != null)
            {
                await UnMountItemAsync(item.Id);
            }

            GameObject.Serialize(GameObject);
        }

        private static async Task<Lot[]> ParseProxyItemsAsync(Lot item)
        {
            var itemInfo = (await ClientCache.GetTableAsync<ItemComponent>()).FirstOrDefault(
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

        private static async Task<ObjectId> FindRootAsync(ObjectId id)
        {
            var item = await id.FindItemAsync();

            if (item == default) return ObjectId.Invalid;

            if (item.ParentId == ObjectId.Invalid) return id;

            return item.ParentId;
        }

        private static async Task<ObjectId[]> GenerateProxiesAsync(ObjectId id)
        {
            var item = await id.FindItemAsync();

            if (item == default) return new ObjectId[0];

            var proxies = await ParseProxyItemsAsync(item.Lot);
            
            await using var ctx = new UchuContext();

            var references = new ObjectId[proxies.Length];

            for (var index = 0; index < proxies.Length; index++)
            {
                var proxy = proxies[index];
                
                var instance = await ctx.InventoryItems.FirstOrDefaultAsync(
                    i => i.ParentId == id && i.Lot == proxy
                ).ConfigureAwait(false);

                if (instance == default)
                {
                    instance = new InventoryItem
                    {
                        Id = ObjectId.Standalone,
                        Lot = proxy,
                        Count = 0,
                        Slot = -1,
                        InventoryType = (int) InventoryType.Hidden,
                        CharacterId = item.CharacterId,
                        ParentId = id
                    };

                    await ctx.InventoryItems.AddAsync(instance);

                    await ctx.SaveChangesAsync();
                }

                references[index] = instance.Id;
            }

            return references;
        }

        private static async Task<ObjectId[]> FindProxiesAsync(ObjectId id)
        {
            await using var ctx = new UchuContext();

            var proxies = await ctx.InventoryItems.Where(
                i => i.ParentId == id
            ).ToArrayAsync().ConfigureAwait(false);

            return proxies.Select(i => (ObjectId) i.Id).ToArray();
        }

        private static async Task ClearProxiesAsync(ObjectId id)
        {
            await using var ctx = new UchuContext();

            var proxies = await ctx.InventoryItems.Where(
                i => i.ParentId == id
            ).ToArrayAsync().ConfigureAwait(false);

            foreach (var proxy in proxies)
            {
                ctx.InventoryItems.Remove(proxy);
            }

            await ctx.SaveChangesAsync();
        }

        private static async Task UpdateEquipState(ObjectId id, bool state)
        {
            await using var ctx = new UchuContext();

            var item = await ctx.InventoryItems.FirstOrDefaultAsync(i => i.Id == id);

            item.IsEquipped = state;

            await ctx.SaveChangesAsync();
        }

        private async Task MountItemAsync(ObjectId id)
        {
            var root = await id.FindItemAsync();
            
            await EquipAsync(new EquippedItem
            {
                Id = id,
                Lot = root.Lot
            });
        }

        private async Task UnMountItemAsync(ObjectId id)
        {
            var root = await FindRootAsync(id);

            await UnEquipAsync(root);
        }

        public override void Construct(BitWriter writer)
        {
            Serialize(writer);
        }

        public override void Serialize(BitWriter writer)
        {
            writer.WriteBit(true);

            var items = Items.Values.ToArray();

            writer.Write((uint) items.Length);

            foreach (var item in items)
            {
                writer.Write(item.Id);
                writer.Write(item.Lot);

                writer.WriteBit(false);

                writer.WriteBit(false);

                writer.WriteBit(false);

                writer.WriteBit(false);

                var info = item.Id.FindItem();

                if (info == default)
                {
                    writer.WriteBit(false);
                }
                else
                {
                    if (writer.Flag(!string.IsNullOrWhiteSpace(info.ExtraInfo)))
                    {
                        writer.WriteLdfCompressed(LegoDataDictionary.FromString(info.ExtraInfo));
                    }
                }

                writer.WriteBit(true);
            }

            writer.WriteBit(false);
        }
    }
}