using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Uchu.Core;
using Uchu.Core.Client;

namespace Uchu.World
{
    /// <summary>
    /// An inventory of a certain type (f.e. bricks, items), that contains a list of items.
    /// </summary>
    /// <remarks>
    /// These are generally not used stand-alone but combined using a <see cref="InventoryManagerComponent"/> as there
    /// are many different types of inventories (bricks, items, etc).
    /// </remarks>
    public class Inventory
    {
        #region constructors
        /// <summary>
        /// Initializes a new inventory for a given type
        /// </summary>
        /// <param name="inventoryType">The type of inventory to initialize</param>
        /// <param name="managerComponent">The manager of this inventory</param>
        internal Inventory(InventoryType inventoryType, InventoryManagerComponent managerComponent)
        {
            InventoryType = inventoryType;
            ManagerComponent = managerComponent;

            if (managerComponent.GameObject.TryGetComponent<CharacterComponent>(out var character))
            {
                Size = inventoryType != InventoryType.Items ? 1000 : character.InventorySize;
            }
        }
        #endregion constructors
        
        #region properties
        /// <summary>
        /// The items in this inventory
        /// </summary>
        private readonly List<Item> _items = new List<Item>();
        
        /// <summary>
        /// The type of this inventory
        /// </summary>
        public InventoryType InventoryType { get; }
        
        /// <summary>
        /// The manager of this inventory
        /// </summary>
        public InventoryManagerComponent ManagerComponent { get; }

        /// <summary>
        /// The current max size of the inventory
        /// </summary>
        private int _size;
        
        /// <summary>
        /// The current max size of the inventory, for <see cref="InventoryType"/> items, this is also maintained in
        /// the database.
        /// </summary>
        public int Size
        {
            get => InventoryType == InventoryType.Items 
                   && ManagerComponent.GameObject.TryGetComponent<CharacterComponent>(out var character) 
                ? character.InventorySize : _size;
            set
            {
                if (InventoryType == InventoryType.Items
                    && ManagerComponent.GameObject.TryGetComponent<CharacterComponent>(out var character))
                    character.InventorySize = value;
                
                _size = value;

                ((Player) ManagerComponent.GameObject).Message(new SetInventorySizeMessage
                {
                    Associate = ManagerComponent.GameObject,
                    InventoryType = InventoryType,
                    Size = Size
                });
            }
        }

        /// <summary>
        /// The items in this inventory, returned as safe read only array
        /// </summary>
        public IEnumerable<Item> Items => Array.AsReadOnly(_items.ToArray());
        
        #endregion properties

        #region operators

        public Item this[uint slot] => Items.FirstOrDefault(i => i.Slot == slot);

        public Item this[long id] => Items.FirstOrDefault(i => i.Id == id);

        #endregion operators

        #region methods

        /// <summary>
        /// Takes a list of database items of a player and uses those to initialize the inventory
        /// </summary>
        /// <param name="clientContext">The client context to load extra item info from</param>
        /// <param name="items">The items to initialize the inventory with </param>
        public async Task LoadItems(CdClientContext clientContext, IEnumerable<InventoryItem> items)
        {
            var initializeTasks = items.Select(async i =>
            {
                var item = await Item.Instantiate(clientContext, i, ManagerComponent.GameObject, this);
                if (item != default)
                {
                    Object.Start(item);
                    AddItem(item);
                }
            });
            
            await Task.WhenAll(initializeTasks);
        }
        
        /// <summary>
        /// Adds an item to the inventory
        /// </summary>
        /// <param name="item">The item to add</param>
        public void AddItem(Item item)
        {
            _items.Add(item);
        }
        
        /// <summary>
        /// Removes an item from the inventory
        /// </summary>
        /// <param name="item">The item to remove</param>
        public void RemoveItem(Item item)
        {
            _items.Remove(item);
        }
        
        #endregion methods
    }
}