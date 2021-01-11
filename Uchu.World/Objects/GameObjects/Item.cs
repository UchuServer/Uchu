using System;
using System.Linq;
using System.Threading.Tasks;
using InfectedRose.Lvl;
using IronPython.Modules;
using Microsoft.EntityFrameworkCore;
using Uchu.Core;
using Uchu.Core.Client;

namespace Uchu.World
{
    [Unconstructed]
    public class Item : GameObject
    {
        public Item()
        {
            OnConsumed = new Event();
            Listen(OnDestroyed, () => Inventory?.RemoveItem(this));
        }

        /// <summary>
        /// Instantiates an item using saved information from the player (e.g. the count, slot and LDD)
        /// </summary>
        /// <param name="clientContext">The client context to fetch item info from</param>
        /// <param name="itemInstance">An item as fetched from the Uchu database to base this item on</param>
        /// <param name="owner">The owner (generally player) of this item</param>
        /// <param name="inventory">The inventory to add the item to</param>
        /// <returns>The instantiated item</returns>
        public static async Task<Item> Instantiate(CdClientContext clientContext, InventoryItem itemInstance,
            GameObject owner, Inventory inventory)
        {
            return await Instantiate(clientContext, owner, itemInstance.Lot, inventory, (uint)itemInstance.Count,
                (uint)itemInstance.Slot, LegoDataDictionary.FromString(itemInstance.ExtraInfo),
                itemInstance.Id, isEquipped: itemInstance.IsEquipped, isBound: itemInstance.IsBound);
        }

        /// <summary>
        /// Instantiates an item using static information.
        /// </summary>
        /// <param name="clientContext">The client context to fetch item information from</param>
        /// <param name="owner">The owner of this item</param>
        /// <param name="lot">The lot of this item</param>
        /// <param name="inventory">The inventory to add the item to, if left empty, this item will be left unmanaged</param>
        /// <param name="count">The count of the item to add</param>
        /// <param name="slot">The slot to add the item to</param>
        /// <param name="extraInfo">Optional LDD to set on the item</param>
        /// <param name="objectId">Explicit object Id for this item, generally only used for player instance items</param>
        /// <param name="rootItem">The root item this item is based on</param>
        /// <param name="isEquipped">Whether the game object has this item equipped or not</param>
        /// <param name="isBound">Whether the game object has bound this item or not</param>
        /// <returns>The instantiated item</returns>
        public static async Task<Item> Instantiate(CdClientContext clientContext, GameObject owner, Lot lot,
            Inventory inventory, uint count, uint slot = default, LegoDataDictionary extraInfo = default,
            ObjectId objectId = default, Item rootItem = default, bool isEquipped = false, bool isBound = false)
        {
            var itemTemplate = await clientContext.ObjectsTable.FirstOrDefaultAsync(
                o => o.Id == lot
            );

            var itemRegistryEntry = await clientContext.ComponentsRegistryTable.FirstOrDefaultAsync(
                r => r.Id == lot && r.Componenttype == (int)ComponentId.ItemComponent
            );

            if (itemTemplate == default || itemRegistryEntry == default)
                return null;

            // If no object Id is provided (for example for a NPC), generate a random one
            objectId = objectId == default ? ObjectId.Standalone : objectId;
            var instance = Instantiate<Item>(owner.Zone, itemTemplate.Name, objectId: objectId, lot: lot);

            // Try to find the slot at which this item should be inserted if no explicit slot is provided
            if (inventory != default && slot == default)
            {
                for (var index = 0; index < inventory.Size; index++)
                {
                    if (inventory.Items.All(i => i.Slot != index))
                        break;
                    slot++;
                }
            }

            // Set all the standard values
            instance.Settings = extraInfo ?? new LegoDataDictionary();
            instance.ItemComponent = await clientContext.ItemComponentTable.FirstAsync(
                i => i.Id == itemRegistryEntry.Componentid);
            instance.Owner = owner;
            instance.Count = count;
            instance.Slot = slot;
            instance.RootItem = rootItem;
            instance.IsBound = isBound;
            instance.IsEquipped = isEquipped;
            instance.IsPackage = instance.Lot.GetComponentId(ComponentId.PackageComponent) != default;
            instance.Inventory = inventory;
            
            var skills = await clientContext.ObjectSkillsTable.Where(
                s => s.ObjectTemplate == instance.Lot
            ).ToArrayAsync();

            instance.IsConsumable = skills.Any(
                s => s.CastOnType == (int) SkillCastType.OnConsumed
            );

            instance.Inventory?.AddItem(instance);

            return instance;
        }

        /// <summary>
        /// The CdClient item component that contains extra meta information about this item
        /// </summary>
        public ItemComponent ItemComponent { get; private set; }
        
        /// <summary>
        /// Event calls if the player consumes the item, 
        /// </summary>
        public Event OnConsumed { get; }
        
        /// <summary>
        /// GameObject that owns this item
        /// </summary>
        public GameObject Owner { get; private set; }
        
        /// <summary>
        /// The amount this item carries
        /// </summary>
        public uint Count { get; private set; }
        
        /// <summary>
        /// If this item is equipped
        /// </summary>
        public bool IsEquipped { get; set; }
        
        /// <summary>
        /// If this item is bound, e.g. it cannot be directly traded or sold, determined by the CdClient
        /// </summary>
        public bool IsBound { get; private set; }
        
        /// <summary>
        /// If this item is a package, for example a brick bag
        /// </summary>
        public bool IsPackage { get; private set; }
        
        /// <summary>
        /// If this item can be consumed
        /// </summary>
        public bool IsConsumable { get; private set; }
        
        /// <summary>
        /// An optional parent item this item is bound to, for example the trial faction gear proxy item that a trial
        /// faction gear item would belong to
        /// </summary>
        public Item RootItem { get; private set; }
        
        /// <summary>
        /// The inventory this item belongs to, can be null, signifying that this is a skeleton item that other
        /// items may be based on
        /// </summary>
        public Inventory Inventory { get; private set; }

        /// <summary>
        /// Returns the lots of the sub items of this item
        /// </summary>
        public Lot[] SubItemLots => !string.IsNullOrWhiteSpace(ItemComponent.SubItems)
            ? ItemComponent.SubItems.Replace(" ", "").Split(',')
                .Select(i => (Lot) int.Parse(i)).ToArray()
            : new Lot[] { };

        /// <summary>
        /// The slot this item inhabits.
        /// </summary>
        /// <remarks>
        /// Should only be set as a response to a client request.
        /// </remarks>
        public uint Slot { get; set; }

        /// <summary>
        /// The type of this item
        /// </summary>
        public ItemType ItemType => (ItemType) (ItemComponent.ItemType ?? (int) ItemType.Invalid);

        /// <summary>
        /// Equips the item in the owners inventory
        /// </summary>
        /// <param name="skipAllChecks">Pass skill all checks to the inventory component</param>
        public async Task EquipAsync(bool skipAllChecks = false)
        {
            if (ItemComponent.IsBOE ?? false)
                IsBound = true;

            if (Owner.TryGetComponent<InventoryComponent>(out var inventory))
            {
                await inventory.EquipItemAsync(this, skipAllChecks);
            }
        }

        /// <summary>
        /// Unequips the item from the owners inventory
        /// </summary>
        public async Task UnEquipAsync()
        {
            if (Owner.TryGetComponent<InventoryComponent>(out var inventory))
            {
                await inventory.UnEquipItemAsync(this);
            }
        }

        /// <summary>
        /// Uses a package item, like a brick bag, removing that item from inventory and adding the yield produced by
        /// that item to the inventory
        /// </summary>
        public async Task UseNonEquipmentItem()
        {
            if (!IsPackage)
                return;
            
            await OnConsumed.InvokeAsync();
            
            var container = AddComponent<LootContainerComponent>();
            await container.CollectDetailsAsync();

            if (Owner.TryGetComponent<InventoryManagerComponent>(out var inventory))
            {
                await inventory.RemoveLotAsync(Lot, 1);
                await using var clientContext = new CdClientContext();
                
                foreach (var loot in await container.GenerateLootYieldsAsync((Player)Owner))
                {
                    await inventory.AddLotAsync(clientContext, loot.Key, loot.Value);
                }
            }
        }
        
        /// <summary>
        /// Consumes this item, removes it from the bound inventory.
        /// </summary>
        public async Task ConsumeAsync()
        {
            if (Owner.TryGetComponent<MissionInventoryComponent>(out var missionInventory))
            {
                await missionInventory.UseConsumableAsync(Lot);
                if (!IsConsumable)
                    return;
            }
            
            if (Owner.TryGetComponent<InventoryManagerComponent>(out var inventory))
                await inventory.RemoveLotAsync(Lot, 1);
        }

        /// <summary>
        /// Increments the count of this item, if not silent also notifies the player
        /// </summary>
        /// <param name="amount">the amount to increment the count with</param>
        /// <param name="silent">whether to not notify the player</param>
        public async Task IncrementCountAsync(uint amount, bool silent = false)
        {
            await SetCountAsync(Count + amount, silent);
        }

        /// <summary>
        /// Decrements the count of this item, if not silent also notifies the player
        /// </summary>
        /// <param name="amount">the amount to decrement the count with</param>
        /// <param name="silent">whether to not notify the player</param>
        public async Task DecrementCountAsync(uint amount, bool silent = false)
        {
            await SetCountAsync(Count - amount, silent);
        }

        /// <summary>
        /// Sets the count of this item, preferred over updating <see cref="Count"/> directly as this also handles
        /// unequipping, player messaging and destroying of the item
        /// </summary>
        /// <param name="count">the count to set <see cref="Count"/> to</param>
        /// <param name="silent">whether to not notify the player</param>
        private async Task SetCountAsync(uint count, bool silent = false)
        {
            if (!silent && count >= Count)
            {
                MessageAddItem(count);
            }
            else if (!silent)
            {
                MessageRemoveItem(count);
            }

            Count = count;

            if (count > 0)
                return;

            if (IsEquipped)
                await UnEquipAsync();

            // Disassemble item
            if (Owner.TryGetComponent<InventoryManagerComponent>(out var inventory) 
                && Settings.TryGetValue("assemblyPartLOTs", out var list))
            {
                await using var clientContext = new CdClientContext();
                foreach (var part in (LegoDataList) list)
                {
                    await inventory.AddLotAsync(clientContext, (int) part, 1);
                }
            }

            Destroy(this);
        }

        /// <summary>
        /// Messages the player of the creation of this item, should only be sent once
        /// </summary>
        /// <remarks>
        /// If an item is already created, you can update its count with <see cref="IncrementCountAsync"/> or
        /// <see cref="DecrementCountAsync"/>.
        /// </remarks>
        public void MessageCreation()
        {
            if (Owner is Player player && Inventory != null)
            {
                player.Message(new AddItemToInventoryMessage
                {
                    Associate = player,
                    Item = this,
                    ItemLot = Lot,
                    Delta = Count,
                    Slot = (int) Slot,
                    InventoryType = (int) Inventory.InventoryType,
                    ShowFlyingLoot = true,
                    TotalItems = Count,
                    ExtraInfo = Settings
                });
            }
        }

        /// <summary>
        /// Messages the player that <c>count</c> items of this lot have been added
        /// </summary>
        /// <param name="count">The amount of items that have been added</param>
        private void MessageAddItem(uint count)
        {
            if (Owner is Player player && Inventory != null)
            {
                player.Message(new AddItemToInventoryMessage
                {
                    Associate = player,
                    Item = this,
                    ItemLot = Lot,
                    Delta = count - Count,
                    TotalItems = count,
                    InventoryType = (int) Inventory.InventoryType,
                    ExtraInfo = Settings,
                    Slot = (int) Slot,
                    ShowFlyingLoot = count != default
                });
            }
        }

        /// <summary>
        /// Messages the player that <c>count</c> items of this lot have been removed
        /// </summary>
        /// <param name="count">The amount of items that have been removed</param>
        private void MessageRemoveItem(uint count)
        {
            if (Owner is Player player && Inventory != null)
            {
                var message = new RemoveItemToInventoryMessage
                {
                    Associate = player,
                    Item = this,
                    Delta = (uint) Math.Abs((int) count - Count),
                    TotalItems = count,
                    InventoryType = Inventory.InventoryType,
                    ExtraInfo = null,
                    ItemType = (ItemType) (ItemComponent.ItemType ?? -1),
                    Confirmed = true,
                    DeleteItem = true,
                    OutSuccess = false,
                    ForceDeletion = true
                };
                
                player.Message(message);
            }
        }
    }
}