using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using InfectedRose.Lvl;
using Microsoft.EntityFrameworkCore;
using Uchu.Core;
using Uchu.Core.Client;

namespace Uchu.World
{
    public class InventoryManagerComponent : Component
    {
        private readonly Dictionary<InventoryType, Inventory> _inventories = new Dictionary<InventoryType, Inventory>();

        private object _lock;

        public Event<Lot, uint> OnLotAdded { get; } = new Event<Lot, uint>();

        public Event<Lot, uint> OnLotRemoved { get; } = new Event<Lot, uint>();

        protected InventoryManagerComponent()
        {
            Listen(OnStart, () =>
            {
                _lock = new object();

                foreach (var value in Enum.GetValues(typeof(InventoryType)))
                {
                    var id = (InventoryType) value;
                    Logger.Debug($"Loading {id} inventory.");
                    _inventories.Add(id, new Inventory(id, this));
                }
            });

            Listen(OnDestroyed, () =>
            {
                OnLotAdded.Clear();
                OnLotRemoved.Clear();
                
                foreach (var item in _inventories.Values.SelectMany(inventory => inventory.Items))
                    Destroy(item);
            });
        }

        /// <summary>
        /// If this is a sub item, it returns the root item of this item. If this item is a root item, it returns itself.
        /// </summary>
        /// <param name="item">The item to find the root item for</param>
        /// <returns>The root item of this item if it is a sub item, the same item otherwise</returns>
        public Item GetRootItem(Item item) => Items.FirstOrDefault(i => i.Id == item.ParentId) ?? item;

        public Inventory this[InventoryType inventoryType] => _inventories[inventoryType];

        public Inventory[] Inventories => _inventories.Values.ToArray();

        public Item[] Items => _inventories.Values.SelectMany(i => i.Items).ToArray();

        #region Find Item

        public Item FindItem(long id)
        {
            using var ctx = new UchuContext();
            var item = ctx.InventoryItems.FirstOrDefault(
                i => i.Id == id && i.CharacterId == GameObject.Id
            );

            if (item == default)
            {
                Logger.Error($"{id} is not an item on {GameObject}");
                return null;
            }

            var managedItem = _inventories[(InventoryType) item.InventoryType][id];

            if (managedItem == null) Logger.Error($"{item.Id} is not managed on {GameObject}");

            return managedItem;
        }
        
        public Item FindItem(Lot lot)
        {
            return _inventories.Values.Select(
                inventory => inventory.Items.FirstOrDefault(i => i.Lot == lot)
            ).FirstOrDefault(item => item != default);
        }

        public Item FindItem(Lot lot, InventoryType inventoryType)
        {
            return _inventories[inventoryType].Items.FirstOrDefault(i => i.Lot == lot);
        }
        
        public bool TryFindItem(Lot lot, out Item item)
        {
            item = FindItem(lot);

            return item != default;
        }

        public bool TryFindItem(Lot lot, InventoryType inventoryType, out Item item)
        {
            item = FindItem(lot, inventoryType);

            return item != default;
        }
        
        public Item[] FindItems(Lot lot)
        {
            return _inventories.Values.SelectMany(
                inventory => inventory.Items.Where(i => i.Lot == lot)
            ).ToArray();
        }

        public Item[] FindItems(Lot lot, InventoryType inventoryType)
        {
            return _inventories[inventoryType].Items.Where(i => i.Lot == lot).ToArray();
        }

        public uint CountItems(Lot lot)
        {
            return (uint) Items.Sum(item => item.Count);
        }

        #endregion
        
        public async Task AddItemAsync(CdClientContext clientContext, Lot lot, uint count,
            LegoDataDictionary extraInfo = default)
        {
            var componentId = await clientContext.ComponentsRegistryTable.FirstOrDefaultAsync(
                r => r.Id == lot && r.Componenttype == (int) ComponentId.ItemComponent
            );

            if (componentId == default)
            {
                Logger.Error($"{lot} does not have a Item component");
                return;
            }

            var component = await clientContext.ItemComponentTable.FirstOrDefaultAsync(
                i => i.Id == componentId.Componentid
            );

            if (component?.ItemType == null)
            {
                Logger.Error(
                    $"{lot} has a corrupted component registry." +
                    $"There is no Item component of Id: {componentId.Componentid}"
                );
                return;
            }

            await AddItemAsync(lot, count, ((ItemType) component.ItemType).GetInventoryType(), extraInfo);
        }

        public async Task AddItemAsync(int lot, uint count, InventoryType inventoryType,
            LegoDataDictionary extraInfo = default)
        {
            if (!(GameObject is Player player))
                return;

            var itemCount = count;
            var _ = Task.Run(() =>
            {
                OnLotAdded.Invoke(lot, itemCount);
            });

            if (!_inventories.TryGetValue(inventoryType, out var inventory))
            {
                inventory = new Inventory(inventoryType, this);
                _inventories[inventoryType] = inventory;
            }

            // Bricks and alike does not have a stack limit.
            // TODO: Get stack size from item
            var stackSize = 1;
            if (stackSize == default)
                stackSize = int.MaxValue;

            if (GameObject.TryGetComponent<MissionInventoryComponent>(out var missionInventory))
            {
                for (var i = 0; i < count; i++)
                {
                    await missionInventory.ObtainItemAsync(lot);
                }
            }
            
            // Fill stacks
            lock (_lock)
            {
                foreach (var item in inventory.Items.Where(i => i.Lot == lot && i.Settings.Count == default 
                                                           && i.Count == stackSize))
                {
                    var toAdd = (uint) Min(stackSize, (int) count, (int) (stackSize - item.Count));

                    item.Count += toAdd;
                    count -= toAdd;

                    if (count <= 0)
                        return;
                }
                
                // Create new stacks
                var toCreate = count;
                while (toCreate != default)
                {
                    var toAdd = (uint) Min(stackSize, (int) toCreate);
                    var item = Item.Instantiate(lot, inventory, toAdd, extraInfo);
                    Start(item);
                    toCreate -= toAdd;
                }
            }
        }

        public Task RemoveAllAsync(Lot lot, bool silent = false)
        {
            return RemoveItemAsync(lot, CountItems(lot), silent);
        }
        
        public async Task RemoveItemAsync(Lot lot, uint count, bool silent = false)
        {
            if (count == default) return;
            
            await using var cdClient = new CdClientContext();
            
            var componentId = await cdClient.ComponentsRegistryTable.FirstOrDefaultAsync(
                r => r.Id == lot && r.Componenttype == (int) ComponentId.ItemComponent
            );

            if (componentId == default)
            {
                Logger.Error($"{lot} does not have a Item component");
                return;
            }

            var component = await cdClient.ItemComponentTable.FirstOrDefaultAsync(
                i => i.Id == componentId.Componentid
            );

            if (component == default)
            {
                Logger.Error(
                    $"{lot} has a corrupted component registry. There is no Item component of Id: {componentId.Componentid}"
                );
                
                return;
            }

            Debug.Assert(component.ItemType != null, "component.ItemType != null");

            RemoveItem(lot, count, ((ItemType) component.ItemType).GetInventoryType(), silent);
        }

        public void RemoveItem(int lot, uint count, InventoryType inventoryType, bool silent = false)
        {
            OnLotRemoved.Invoke(lot, count);

            using var cdClient = new CdClientContext();
            
            var componentId = cdClient.ComponentsRegistryTable.FirstOrDefault(
                r => r.Id == lot && r.Componenttype == (int) ComponentId.ItemComponent
            );

            if (componentId == default)
            {
                Logger.Error($"{lot} does not have a Item component");
                return;
            }

            var component = cdClient.ItemComponentTable.FirstOrDefault(
                i => i.Id == componentId.Componentid
            );

            if (component == default)
            {
                Logger.Error(
                    $"{lot} has a corrupted component registry. There is no Item component of Id: {componentId.Componentid}"
                );
                return;
            }

            var items = _inventories[inventoryType].Items.Where(i => i.Lot == lot).ToList();

            //
            // Sort to make sure we remove from the stacks with the lowest count first.
            //

            items.Sort((i1, i2) => (int) (i1.Count - i2.Count));

            foreach (var item in items)
            {
                var toRemove = (uint) Min((int) count, (int) item.Count);

                if (!silent)
                {
                    item.Count -= toRemove;
                }
                else
                {
                    var storedCount = item.Count - toRemove;
                    
                    var _ = Task.Run(async () =>
                    {
                        await item.SetCountSilentAsync(storedCount);
                    });
                }

                count -= toRemove;

                if (count != default) continue;

                return;
            }

            Logger.Error(
                $"Trying to remove {lot} x {count} when {GameObject} does not have that many of {lot} in their {inventoryType} inventory"
            );
        }

        public async Task MoveItemsBetweenInventoriesAsync(Item item, Lot lot, uint count, InventoryType source, InventoryType destination, bool silent = false)
        {
            if (item?.Settings != null)
            {
                if (count != 1 || item.Count != 1)
                {
                    Logger.Error($"Invalid special item {item}");
                    return;
                }
                
                Destroy(item);

                await AddItemAsync(item.Lot, count, destination, item.Settings);
                
                return;
            }
            
            RemoveItem(item?.Lot ?? lot, count, source, silent);

            await AddItemAsync(item?.Lot ?? lot, count, destination);
        }
        
        private static int Min(params int[] nums)
        {
            return nums.Min();
        }
    }
}