using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using InfectedRose.Lvl;
using IronPython.Modules;
using Microsoft.EntityFrameworkCore;
using Uchu.Core;
using Uchu.Core.Client;
using Uchu.Core.Resources;

namespace Uchu.World
{
    /// <summary>
    /// Component that handles the actual inventories of a game object, not to be confused with <see cref="InventoryComponent"/>
    /// which represents the equipped inventory of a game object.
    /// </summary>
    public class InventoryManagerComponent : Component
    {
        #region properties
        
        /// <summary>
        /// Contains all the inventories this game object has, indexed by inventory type
        /// </summary>
        private readonly Dictionary<InventoryType, Inventory> _inventories = new Dictionary<InventoryType, Inventory>();
        
        /// <summary>
        /// Event called when a lot is added
        /// </summary>
        public Event<Lot, uint> OnLotAdded { get; } = new Event<Lot, uint>();

        /// <summary>
        /// Event called when a lot is removed
        /// </summary>
        public Event<Lot, uint> OnLotRemoved { get; } = new Event<Lot, uint>();

        /// <summary>
        /// Dictionary of lots and locks that allows to lock a single lot in the inventory
        /// </summary>
        /// <remarks>
        /// Use <see cref="GetLotLock"/> to interact with this dictionary to ensure proper updates of keys.
        /// </remarks>
        private Dictionary<int, SemaphoreSlim> LotLock { get; } = new Dictionary<int, SemaphoreSlim>();

        /// <summary>
        /// All inventories in this inventory manager
        /// </summary>
        public Inventory[] Inventories => _inventories.Values.ToArray();

        /// <summary>
        /// All the items located in all inventories
        /// </summary>
        public Item[] Items => _inventories.Values.SelectMany(i => i.Items).ToArray();
        #endregion properties

        #region constructors
        protected InventoryManagerComponent()
        {
            Listen(OnStart, async () =>
            {
                await using var uchuContext = new UchuContext();
                await using var clientContext = new CdClientContext();
                
                var playerCharacter = uchuContext.Characters
                    .Include(c => c.Items)
                    .First(c => c.Id == GameObject.Id);

                var inventoryTypesToCreate = playerCharacter.Items
                    .Select(i => (InventoryType) i.InventoryType)
                    .Distinct();
                
                // Load all the already owned items of the player
                foreach (var inventoryType in inventoryTypesToCreate)
                {
                    
                    Logger.Debug($"Loading {inventoryType} inventory.");
                    var inventory = new Inventory(inventoryType, this);
                    
                    await inventory.LoadItems(clientContext, playerCharacter.Items
                        .Where(item => item.ParentId == ObjectId.Invalid 
                                       && (InventoryType) item.InventoryType == inventoryType));
                    
                    _inventories.Add(inventoryType, inventory);
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
        #endregion constructors
        
        #region operators
        
        public Inventory this[InventoryType inventoryType] => _inventories.TryGetValue(inventoryType, out var inventory) 
            ? inventory : default;

        #endregion operators

        #region methods
        
        #region finditem

        /// <summary>
        /// Finds and returns the first item of a certain lot throughout all inventories
        /// </summary>
        /// <param name="lot">The lot of the item to look for</param>
        /// <returns>The item retrieved from the entire inventory</returns>
        public Item FindItem(Lot lot)
        {
            return _inventories.Values.Select(
                inventory => inventory.Items.FirstOrDefault(i => i.Lot == lot)
            ).FirstOrDefault(item => item != default);
        }

        /// <summary>
        /// Finds and returns the first item of a certain lot in a certain inventory
        /// </summary>
        /// <param name="lot">The lot of the item to look for</param>
        /// <param name="inventoryType">The inventory type to look in for the item</param>
        /// <returns>The item retrieved from the given inventory</returns>
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
        
        /// <summary>
        /// Finds all the items of a certain lot, in all inventories
        /// </summary>
        /// <param name="lot">The lot to look for</param>
        /// <returns>List of all found items</returns>
        public Item[] FindItems(Lot lot) => _inventories.Values.SelectMany(
            inventory => inventory.Items.Where(i => i.Lot == lot)
            ).ToArray();

        /// <summary>
        /// Finds all the items of a certain lot from a certain inventory
        /// </summary>
        /// <param name="lot">The lot to find items for</param>
        /// <param name="inventoryType">The inventory type to look in</param>
        /// <returns>List of all found items</returns>
        public Item[] FindItems(Lot lot, InventoryType inventoryType) =>
            _inventories[inventoryType].Items.Where(i => i.Lot == lot).ToArray();
        
        /// <summary>
        /// If this is a sub item, it returns the root item of this item. If this item is a root item, it returns itself.
        /// </summary>
        /// <param name="item">The item to find the root item for</param>
        /// <returns>The root item of this item if it is a sub item, the same item otherwise</returns>
        public Item GetRootItem(Item item) => Items.FirstOrDefault(i => i == item.RootItem) ?? item;
        
        #endregion finditem
        
        /// <summary>
        /// Returns the lock for a lot, allowing locking a single lot in the inventory
        /// </summary>
        /// <param name="lot">The lot to acquire the lock for</param>
        /// <returns>The lock for the lot</returns>
        private SemaphoreSlim GetLotLock(Lot lot)
        {
            if (!LotLock.ContainsKey(lot.Id))
                LotLock[lot.Id] = new SemaphoreSlim(1, 1);
            return LotLock[lot.Id];
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

        /// <summary>
        /// Checks if a lot is a faction token proxy and tries to assign the proper faction token according to a
        /// possibly related character component. In case there's no character component for the GameObject or the
        /// lot is isn't a faction token proxy, nothing is done.
        /// </summary>
        /// <param name="lot">The lot to check</param>
        /// <returns>Whether the lot is valid for use, in case of a faction token proxy this means that the proxy
        /// could be properly replaced with a faction token</returns>
        private bool HandleFactionToken(ref Lot lot)
        {
            // If this is not a faction token or the game object doesn't have a character, there's nothing to check
            if (lot != Lot.FactionTokenProxy) return true;
            if (!GameObject.TryGetComponent<CharacterComponent>(out var character)) return true;

            var possibleLots = new List<Lot>();
            
            if (character.IsAssembly) possibleLots.Add(Lot.AssemblyFactionToken);
            if (character.IsParadox) possibleLots.Add(Lot.ParadoxFactionToken);
            if (character.IsSentinel) possibleLots.Add(Lot.SentinelFactionToken);
            if (character.IsVentureLeague) possibleLots.Add(Lot.VentureFactionToken);

            // If this is a character with no valid factions, don't drop anything
            if (possibleLots.Count == 0)
                return false;
            
            // Generally this will return the same faction token
            // but for characters with multiple factions this equally distributes the drops
            lot = possibleLots.Count == 1 
                ? possibleLots[0] 
                : possibleLots[new Random().Next(0, possibleLots.Count)];
            
            return true;
        }
        
        /// <summary>
        /// Adds a new lot to the inventory. The lot will automatically instantiated as the required amount of items
        /// based on the provided count and the stack size of that item.
        /// </summary>
        /// <param name="clientContext">The client context to fetch the item info from</param>
        /// <param name="lot">The lot to add to the inventory</param>
        /// <param name="count">The count of the lot we want to add to the inventory</param>
        /// <param name="inventoryType">Optional explicit inventory type to add this lot to, if not provided this will be
        /// implicitly retrieved from the item lot, generally used for vendor buy back</param>
        /// <param name="settings">Optional <c>LegoDataDictionary</c> to instantiate the item with</param>
        public async Task AddLotAsync(CdClientContext clientContext, Lot lot, uint count, 
            LegoDataDictionary settings = default, InventoryType inventoryType = default)
        {
            // For players, the faction token proxy needs to be replaced with an actual faction token
            // If this wasn't possible, exit
            if (!HandleFactionToken(ref lot))
                return;
            
            // First create an unmanaged skeleton of this item. Updating stacks will be handled later on
            var itemSkeleton = await Item.Instantiate(clientContext, GameObject, lot, default, count, 
                extraInfo: settings);
            
            if (itemSkeleton == null)
                return;
            
            if (itemSkeleton.ItemComponent.ItemType == default)
                throw new InvalidOperationException("Could not add item: cannot determine inventory type.");

            inventoryType = inventoryType != default 
                ? inventoryType 
                : ((ItemType) itemSkeleton.ItemComponent.ItemType).GetInventoryType();
            
            // Get the proper inventory for this game object, if it has no inventory of that type yet, create it
            if (!_inventories.TryGetValue(inventoryType, out var inventory))
            {
                inventory = new Inventory(inventoryType, this);
                _inventories[inventoryType] = inventory;
            }

            // Some items have no stack size, like bricks
            var stackSize = itemSkeleton.ItemComponent.StackSize ?? int.MaxValue;

            // Acquire the lock for a single lot to update the lot in the inventory
            var lotLock = GetLotLock(itemSkeleton.Lot);
            await lotLock.WaitAsync();

            try
            {
                // Fill all old stacks
                foreach (var item in inventory.Items.Where(i => i.Lot == lot && i.Count < stackSize))
                {
                    var toAdd = (uint) Min(stackSize, (int) itemSkeleton.Count, 
                        (int) (stackSize - item.Count));

                    await item.IncrementCountAsync(toAdd);
                    await itemSkeleton.DecrementCountAsync(toAdd, true);

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
                    
                    item.MessageCreation();
                    await itemSkeleton.DecrementCountAsync(toAdd);
                }
            }
            finally
            {
                lotLock.Release();
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
            return RemoveLotAsync(lot, InventoryCount(lot), silent: silent);
        }

        /// <summary>
        /// Removes <c>count</c> items of a certain lot from inventory of the GameObject.
        /// </summary>
        /// <param name="lot">The lot of the item to remove</param>
        /// <param name="count">The number of items to remove</param>
        /// <param name="inventoryType">Optional explicit inventory type to remove items from, generally used for
        ///     vendor buyback</param>
        /// <param name="silent">Whether to send the updated count to the game object or not</param>
        /// <exception cref="InvalidOperationException">If the passed item does not have an item component</exception>
        public async Task RemoveLotAsync(Lot lot, uint count, InventoryType inventoryType = default,
            bool silent = false)
        {
            var lotLock = GetLotLock(lot);
            await lotLock.WaitAsync();

            try
            {
                // Sort to make sure we remove from the stacks with the lowest count first.
                var itemsToRemove = (inventoryType == default ? FindItems(lot) : FindItems(lot, inventoryType)).ToList();
                itemsToRemove.Sort((i1, i2) => (int) (i1.Count - i2.Count));

                foreach (var itemToRemove in itemsToRemove)
                {
                    var amountToRemove = (uint) Min((int) count, (int) itemToRemove.Count);
                    await itemToRemove.DecrementCountAsync(amountToRemove, silent);

                    count -= amountToRemove;
                    if (count != default)
                        continue;

                    return;
                }

                await OnLotRemoved.InvokeAsync(lot, count);
            }
            finally
            {
                lotLock.Release();
            }
        }

        /// <summary>
        /// Moves an item from one inventory to another inventory, generally used for vendor buy back
        /// </summary>
        /// <param name="item">The item to move</param>
        /// <param name="lot">The lot of the item to move (if no item is provided)</param>
        /// <param name="count">The amount of items to move</param>
        /// <param name="source">The source inventory to move from</param>
        /// <param name="destination">The destination inventory to move to</param>
        /// <param name="silent">Whether to send inventory update messages or not</param>
        public async Task MoveItemsBetweenInventoriesAsync(Item item, Lot lot, uint count, InventoryType source,
            InventoryType destination, bool silent = false)
        {
            await using var clientContext = new CdClientContext();
            
            if (item?.Settings != null)
            {
                if (count != 1 || item.Count != 1)
                {
                    Logger.Error($"Invalid special item {item}");
                    return;
                }
                
                Destroy(item);
                await AddLotAsync(clientContext, item.Lot, count, item.Settings, destination);
                
                return;
            }
            
            await RemoveLotAsync(item?.Lot ?? lot, count, source, silent);
            await AddLotAsync(clientContext, item?.Lot ?? lot, count, inventoryType: destination);
        }
        
        #endregion methods
        
        private static int Min(params int[] nums)
        {
            return nums.Min();
        }
    }
}