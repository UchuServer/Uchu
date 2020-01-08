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
        private InventoryComponent _inventoryComponent;

        private object _lock;

        public AsyncEvent<Lot, uint> OnLotAdded { get; } = new AsyncEvent<Lot, uint>();

        public AsyncEvent<Lot, uint> OnLotRemoved { get; } = new AsyncEvent<Lot, uint>();

        protected InventoryManagerComponent()
        {
            Listen(OnStart, () =>
            {
                _inventoryComponent = GameObject.GetComponent<InventoryComponent>();
                
                _lock = new object();

                foreach (var value in Enum.GetValues(typeof(InventoryType)))
                {
                    var id = (InventoryType) value;

                    Logger.Information($"[{id}]");

                    _inventories.Add(id, new Inventory(id, this));
                }
            });

            Listen(OnDestroyed, () =>
            {
                OnLotAdded.Clear();
                
                OnLotRemoved.Clear();
                
                foreach (var item in _inventories.Values.SelectMany(inventory => inventory.Items)) Destroy(item);
            });
        }

        public Inventory this[InventoryType inventoryType] => _inventories[inventoryType];

        #region Find Item

        public Item FindItem(long id)
        {
            using var ctx = new UchuContext();
            var item = ctx.InventoryItems.FirstOrDefault(
                i => i.InventoryItemId == id && i.CharacterId == GameObject.ObjectId
            );

            if (item == default)
            {
                Logger.Error($"{id} is not an item on {GameObject}");
                return null;
            }

            var managedItem = _inventories[(InventoryType) item.InventoryType][id];

            if (managedItem == null) Logger.Error($"{item.InventoryItemId} is not managed on {GameObject}");

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

        #endregion
        
        public async Task AddItemAsync(Lot lot, uint count, LegoDataDictionary extraInfo = default)
        {
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

            AddItem(lot, count, ((ItemType) component.ItemType).GetInventoryType(), extraInfo);
        }

        public void AddItem(int lot, uint count, InventoryType inventoryType, LegoDataDictionary extraInfo = default)
        {
            var inventory = _inventories[inventoryType];
            
            OnLotAdded.Invoke(lot, count);

            // The math here cannot be executed in parallel
            lock (_lock)
            {
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

                var stackSize = component.StackSize ?? 1;
                    
                // Bricks and alike does not have a stack limit.
                if (stackSize == default) stackSize = int.MaxValue;

                //
                // Fill stacks
                //

                foreach (var item in inventory.Items.Where(i => i.Lot == lot))
                {
                    if (item.Count == stackSize) continue;

                    var toAdd = (uint) Min(stackSize, (int) count, (int) (stackSize - item.Count));

                    item.Count += toAdd;

                    count -= toAdd;

                    if (count <= 0) return;
                }

                //
                // Create new stacks
                //

                var toCreate = count;

                while (toCreate != default)
                {
                    var toAdd = (uint) Min(stackSize, (int) toCreate);

                    var item = Item.Instantiate(lot, inventory, toAdd, extraInfo);

                    Start(item);

                    toCreate -= toAdd;
                }
            }
            
            //
            // Update quest tasks
            //

            var questInventory = GameObject.GetComponent<MissionInventoryComponent>();

            for (var i = 0; i < count; i++)
            {
                questInventory.UpdateObjectTask(MissionTaskType.ObtainItem, lot);
            }

        }

        public async Task RemoveItemAsync(Lot lot, uint count, bool silent = false)
        {
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
            // The math here cannot be executed in parallel
            lock (_lock)
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

                    if (!silent) item.Count -= toRemove;
                    else
                    {
                        var storedCount = count;
                        Task.Run(async () => { await item.SetCountSilentAsync(storedCount); });
                    }

                    count -= toRemove;

                    if (count != default) continue;

                    return;
                }

                Logger.Error(
                    $"Trying to remove {lot} x {count} when {GameObject} does not have that many of {lot} in their {inventoryType} inventory"
                );
            }
        }

        public void MoveItemsBetweenInventories(Item item, Lot lot, uint count, InventoryType source, InventoryType destination, bool silent = false)
        {
            if (item?.Settings != null)
            {
                if (count != 1 || item.Count != 1)
                {
                    Logger.Error($"Invalid special item {item}");
                    return;
                }
                
                Destroy(item);

                AddItem(item.Lot, count, destination, item.Settings);
                
                return;
            }
            
            RemoveItem(item?.Lot ?? lot, count, source, silent);

            AddItem(item?.Lot ?? lot, count, destination);
        }
        
        private static int Min(params int[] nums)
        {
            return nums.Min();
        }
    }
}