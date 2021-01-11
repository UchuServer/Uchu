using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InfectedRose.Lvl;
using Microsoft.EntityFrameworkCore;
using RakDotNet.IO;
using Uchu.Core;
using Uchu.Core.Client;

namespace Uchu.World
{
    /// <summary>
    /// Represents the equipped inventory of a GameObject, not to be confused with the <see cref="InventoryManagerComponent"/>
    /// which represents the entire inventory of a GameObject. Also note that <see cref="InventoryComponent"/> is a
    /// replica component that was available in live while <see cref="InventoryManagerComponent"/> is a new component
    /// for efficient handling of the entire inventory of a game object.
    /// </summary>
    /// <remarks>
    /// In case of a player, the <see cref="InventoryManagerComponent"/> has to be loaded first, as only then this
    /// component can equip all the items from the inventory manager.
    /// It's possible for this component to add new items to the <see cref="InventoryManagerComponent"/> as the manager
    /// component might contain a root item that on equip, loads new items that listen to the root item. Take for example
    /// the trial faction kit: once the kit is equipped, it loads in all the trial gear of the kit. The inventory
    /// manager however only contains the faction kit item initially, this component then handles the logic of adding and
    /// removing all the sub items on equip and unequip.
    /// </remarks>
    public class InventoryComponent : ReplicaComponent
    {
        /// <summary>
        /// Component Id of this component
        /// </summary>
        public override ComponentId Id => ComponentId.InventoryComponent;
        
        /// <summary>
        /// Mapping of items and their equipped slots on the game object
        /// </summary>
        public Dictionary<EquipLocation, Item> Items { get; }
        
        /// <summary>
        /// Called when a new item is equipped
        /// </summary>
        public Event<Item> OnEquipped { get; }
        
        /// <summary>
        /// Called when an item is unequipped
        /// </summary>
        public Event<Item> OnUnEquipped { get; }

        protected InventoryComponent()
        {
            Items = new Dictionary<EquipLocation, Item>();
            OnEquipped = new Event<Item>();
            OnUnEquipped = new Event<Item>();

            Listen(OnDestroyed, () =>
            {
                OnEquipped.Clear();
                OnUnEquipped.Clear();
            });
            
            Listen(OnStart, async () =>
            {
                await LoadAsync();
            });
        }

        /// <summary>
        /// Loads the inventory for the game object, if this object has a <see cref="InventoryManagerComponent"/>, the
        /// equipped items from that inventory will be used, otherwise the CdClient inventory will be used.
        /// </summary>
        private async Task LoadAsync()
        {
            // If this is a player, load the items from the uchu database, otherwise load the pre-defined
            // items from the cd client
            if (GameObject.TryGetComponent<InventoryManagerComponent>(out var inventoryComponent))
            {
                foreach (var item in inventoryComponent.Items.Where(i => i.IsEquipped))
                {
                    await EquipAsync(item);
                }
            }
            else
            {
                await using var clientContext = new CdClientContext();
                
                var component = clientContext.ComponentsRegistryTable.FirstOrDefault(c =>
                    c.Id == GameObject.Lot && c.Componenttype == (int) ComponentId.InventoryComponent);
                var clientItems = clientContext.InventoryComponentTable
                    .Where(i => i.Id == component.Componentid && i.Itemid != default).ToArray();

                foreach (var clientItem in clientItems)
                {
                    if (clientItem.Itemid != default)
                    {
                        var lot = (Lot) clientItem.Itemid;
                        var itemComponent = clientContext.ItemComponentTable.First(
                            i => i.Id == lot.GetComponentId(ComponentId.ItemComponent));

                        if (itemComponent.ItemType != default)
                        {
                            var item = await Item.Instantiate(clientContext, GameObject, lot, default,
                                (uint)(clientItem.Count ?? 1));
                            Items[(EquipLocation) itemComponent.EquipLocation] = item;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Updates a slot in the inventory, swapping a previous item for a new one
        /// </summary>
        /// <param name="slot">The slot to equip an item to</param>
        /// <param name="item">The item to place in the slot</param>
        private async Task UpdateSlotAsync(Item item)
        {
            var equipLocation = item.ItemComponent.EquipLocation;
            if (Items.TryGetValue(equipLocation, out var previouslyEquippedItem))
            {
                await UnEquipAsync(previouslyEquippedItem);
            }

            Items[equipLocation] = item;
        }

        /// <summary>
        /// Gets the slot an object is potentially located at
        /// </summary>
        /// <param name="itemId">The object id to find the equiplocation for</param>
        /// <returns>The equip location if the object could be found, default otherwise</returns>
        private EquipLocation GetSlot(ObjectId itemId) => 
            Items.FirstOrDefault(i => i.Value == itemId) is {} keyValue 
                ? keyValue.Key
                : default;
        
        /// <summary>
        /// Whether the game object has this lot equipped
        /// </summary>
        /// <param name="lot">The lot to find items for</param>
        /// <returns><c>true</c> if the game object has this lot equipped</returns>
        public bool HasEquipped(Lot lot) => Items.Any(i => i.Value.Lot == lot);

        /// <summary>
        /// Equips an item for a game object, updating the equipped items list. Should be used at run-time, not at
        /// loading time as it also calls events and serializes the game object.
        /// </summary>
        /// <param name="item">The item to load for the player</param>
        /// <param name="ignoreAllChecks">Disables item checks, which checks for example if a player is building</param>
        public async Task EquipItemAsync(Item item, bool ignoreAllChecks = false)
        {
            var itemType = (ItemType) (item.ItemComponent.ItemType ?? (int) ItemType.Invalid);

            // TODO: What does this do?
            if (!ignoreAllChecks 
                && GameObject is Player player 
                && player.TryGetComponent<ModularBuilderComponent>(out var modularBuilderComponent)
                && !modularBuilderComponent.IsBuilding
                && (itemType == ItemType.Model || itemType == ItemType.LootModel || itemType == ItemType.Vehicle 
                    || item.Lot == 6086))
            {
                return;
            }
            
            await EquipAsync(item);
            GameObject.Serialize(GameObject);
            await OnEquipped.InvokeAsync(item);
        }
        
        /// <summary>
        /// Internal item equipping logic, also equips all sub items for this item.
        /// </summary>
        /// <param name="item">The item to equip</param>
        private async Task EquipAsync(Item item)
        {
            await UpdateSlotAsync(item);
            item.IsEquipped = true;

            // Update all the new skills the player gets from this item
            var skills = GameObject.TryGetComponent<SkillComponent>(out var skillComponent);
            if (skills)
                await skillComponent.MountItem(item);

            // Finally mount all the sub items for this item
            var subItems = await GenerateSubItemsAsync(item);
            foreach (var subItem in subItems)
            {
                await UpdateSlotAsync(subItem);
                subItem.IsEquipped = true;
                
                if (skills)
                    await skillComponent.MountItem(subItem);
            }
        }

        /// <summary>
        /// Unequips an item for a player.
        /// </summary>
        /// <remarks>Should be used at run-time instead of directly calling <see cref="UnEquipAsync"/> as this
        /// also calls unequip events</remarks>
        /// <param name="item">The item to unequip</param>
        public async Task UnEquipItemAsync(Item item)
        {
            await OnUnEquipped.InvokeAsync(item);
            
            if (item?.Id <= 0)
                return;
            
            if (item != null)
            {
                await UnEquipAsync(item);
            }

            GameObject.Serialize(GameObject);
        }
        
        /// <summary>
        /// Unequips an item by unequipping it or its root item, if available.
        /// </summary>
        /// <param name="item">The item to unequip or unequip the root item for</param>
        private async Task UnEquipAsync(Item item)
        {
            if (GameObject.TryGetComponent<InventoryManagerComponent>(out var inventory) 
                && inventory.GetRootItem(item) is {} rootItem)
            {
                if (GameObject.TryGetComponent<SkillComponent>(out var skillComponent))
                    await skillComponent.DismountItemAsync(item);
                
                Items.Remove(GetSlot(rootItem.Id));
                rootItem.IsEquipped = false;

                foreach (var subItem in GetSubItems(rootItem))
                {
                    var subSlot = GetSlot(subItem.Id);
                    if (Items.TryGetValue(subSlot, out var equippedSubItem))
                    {
                        if (skillComponent != default)
                            await skillComponent.DismountItemAsync(equippedSubItem);
                        Items.Remove(subSlot);
                    }
                    
                    subItem.IsEquipped = false;
                }
            }
        }

        /// <summary>
        /// Generates all the item objects for the sub item lot list provided by the given item.
        /// </summary>
        /// <param name="item">The item to get the sub items for</param>
        /// <returns>The list of sub items of this item</returns>
        private async Task<Item[]> GenerateSubItemsAsync(Item item)
        {
            var subItems = new List<Item>();

            if (GameObject.TryGetComponent<InventoryManagerComponent>(out var inventory))
            {
                await using var clientContext = new CdClientContext();
                
                // Make sure that all sub items are available in the inventory, if not: add them.
                foreach (var subItemLot in item.SubItemLots)
                {
                    var subItem = inventory.FindItem(subItemLot) 
                                  ?? await Item.Instantiate(clientContext, GameObject, subItemLot, item.Inventory, 1);
                    subItems.Add(subItem);
                }
            }

            return subItems.ToArray();
        }

        /// <summary>
        /// Gets all the sub items of an item by fetching them from the game object inventory
        /// </summary>
        /// <param name="item">The object to get the sub items for</param>
        /// <returns>A list of all the sub items for this item</returns>
        private Item[] GetSubItems(Item item)
        {
            if (GameObject.TryGetComponent<InventoryManagerComponent>(out var inventory))
            {
                return inventory.Items.Where(i => i.RootItem?.Id == item.Id).ToArray();
            }

            return new Item[] { };
        }

        /// <summary>
        /// Constructs the component
        /// </summary>
        /// <param name="writer">The writer to serialize to</param>
        public override void Construct(BitWriter writer)
        {
            Serialize(writer);
        }

        /// <summary>
        /// Serializes the component
        /// </summary>
        /// <param name="writer">The writer to serialize to</param>
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

                if (item.ItemComponent == default)
                {
                    writer.WriteBit(false);
                }
                else
                {
                    var info = item.Settings.ToString();
                    if (writer.Flag(!string.IsNullOrWhiteSpace(info)))
                    {
                        writer.WriteLdfCompressed(LegoDataDictionary.FromString(info));
                    }
                }
                writer.WriteBit(true);
            }
            writer.WriteBit(false);
        }
    }
}