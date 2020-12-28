using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using InfectedRose.Lvl;
using IronPython.Modules;
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

        /// <summary>
        /// Returns the total sum of items a game object has of a certain lot.
        /// </summary>
        /// <param name="lot">The lot to calculate the count for</param>
        /// <returns>The total count of a certain lot in the inventory</returns>
        private uint InventoryCount(Lot lot)
        {
            return (uint) Items.Where(i => i.Lot == lot).Sum(i => i.Count);
        }

        #endregion

        /// <summary>
        /// Adds a new lot to the inventory. The lot will automatically instantiated as the required amount of items
        /// based on the provided count and the stack size of that item.
        /// </summary>
        /// <param name="clientContext">The client context to fetch the item info from</param>
        /// <param name="lot">The lot to add to the inventory</param>
        /// <param name="count">The count of the lot we want to add to the inventory</param>
        /// <param name="settings">Optional <c>LegoDataDictionary</c> to instantiate the item with</param>
        public async Task AddLotAsync(CdClientContext clientContext, Lot lot, uint count,
            LegoDataDictionary settings = default)
        {
            // First create an unmanaged skeleton of this item. Updating stacks will be handled later on
            var itemSkeleton = await Item.Instantiate(clientContext, GameObject, lot, default, count, 
                extraInfo: settings);
            
            if (itemSkeleton.ItemComponent.ItemType == default)
                throw new InvalidOperationException("Could not add item: cannot determine inventory type.");
            
            // Get the proper inventory for this game object, if it has no inventory of that type yet, create it
            var inventoryType = ((ItemType) itemSkeleton.ItemComponent.ItemType).GetInventoryType();
            if (!_inventories.TryGetValue(inventoryType, out var inventory))
            {
                inventory = new Inventory(inventoryType, this);
                _inventories[inventoryType] = inventory;
            }

            // Some items have no stack size, like bricks
            var stackSize = itemSkeleton.ItemComponent.StackSize ?? int.MaxValue;
            
            // Fill stacks
            lock (_lock)
            {
                // Fill all old stacks
                foreach (var item in inventory.Items.Where(i => i.Lot == lot && i.Count < stackSize))
                {
                    var toAdd = (uint) Min(stackSize, (int) itemSkeleton.Count,
                        (int) (stackSize - item.Count));

                    item.Count += toAdd;
                    itemSkeleton.Count -= toAdd;

                    // Exit if no new stacks are required to hold the lot
                    if (itemSkeleton.Count <= 0)
                        return;
                }
                
                // Create new stacks
                while (itemSkeleton.Count > 0)
                {
                    var toAdd = (uint) Min(stackSize, (int) itemSkeleton.Count);
                    
                    var item = await Item.Instantiate(clientContext, itemSkeleton.Owner, itemSkeleton.Lot, inventory, 
                        toAdd, extraInfo: itemSkeleton.Settings);
                    Start(item);
                    
                    // TODO: Message player of lot addition
                    
                    itemSkeleton.Count -= toAdd;
                }
            }
            
            // Complete all the collectible tasks for this game object
            if (GameObject.TryGetComponent<MissionInventoryComponent>(out var missionInventory))
            {
                for (var i = 0; i < itemSkeleton.Count; i++)
                {
                    await missionInventory.ObtainItemAsync(lot);
                }
            }
            
            var _ = Task.Run(() =>
            {
                OnLotAdded.Invoke(lot, itemSkeleton.Count);
            });
        }

        /// <summary>
        /// Removes all occurrences of a certain lot from the inventory
        /// </summary>
        /// <param name="lot">The lot to remove</param>
        /// <param name="silent">Whether to notify the game object of the updated item counts</param>
        public Task RemoveAllAsync(Lot lot, bool silent = false)
        {
            return RemoveLotAsync(lot, InventoryCount(lot), silent);
        }

        /// <summary>
        /// Removes <c>count</c> items of a certain lot from inventory of the GameObject.
        /// </summary>
        /// <param name="lot">The lot of the item to remove</param>
        /// <param name="count">The number of items to remove</param>
        /// <param name="silent">Whether to send the updated count to the game object or not</param>
        /// <exception cref="InvalidOperationException">If the passed item does not have an item component</exception>
        public async Task RemoveLotAsync(Lot lot, uint count, bool silent = false)
        {
            await OnLotRemoved.InvokeAsync(lot, count);
            var itemsToRemove = Items.Where(i => i.Lot == lot).ToList();

            // Sort to make sure we remove from the stacks with the lowest count first.
            itemsToRemove.Sort((i1, i2) => (int) (i1.Count - i2.Count));
            foreach (var itemToRemove in itemsToRemove)
            {
                var amountToRemove = (uint) Min((int) count, (int) itemToRemove.Count);
                if (!silent)
                {
                    itemToRemove.Count -= amountToRemove;
                }
                else
                {
                    var storedCount = itemToRemove.Count - amountToRemove;
                    
                    var _ = Task.Run(async () =>
                    {
                        await itemToRemove.SetCountSilentAsync(storedCount);
                    });
                }

                count -= amountToRemove;
                if (count != default)
                    continue;

                return;
            }
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