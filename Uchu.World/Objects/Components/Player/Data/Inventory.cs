using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Uchu.Core;

namespace Uchu.World
{
    public class Inventory
    {
        private readonly List<Item> _items;
        
        public readonly InventoryType InventoryType;
        public readonly InventoryManager Manager;

        // TODO: Network & Store in DB
        public uint Size = 20;
        
        public Inventory(InventoryType inventoryType, InventoryManager manager)
        {
            InventoryType = inventoryType;
            Manager = manager;

            using (var ctx = new UchuContext())
            {
                var playerCharacter = ctx.Characters
                    .Include(c => c.Items)
                    .First(c => c.CharacterId == manager.Player.ObjectId);

                var inventoryItems = playerCharacter.Items
                    .Where(item => (InventoryType) item.InventoryType == inventoryType)
                    .ToList();

                _items = inventoryItems.Select(
                    i => Item.Instantiate(i.InventoryItemId, this)
                ).Where(item => !ReferenceEquals(item, default)).ToList();

                foreach (var item in _items)
                {
                    Object.Start(item);

                    Logger.Information($"\t-> {item}");

                    item.OnDestroyed += () => { _items.Remove(item); };
                }
            }
        }

        public IReadOnlyCollection<Item> Items => Array.AsReadOnly(_items.ToArray());

        public Item this[uint slot] => Items.FirstOrDefault(i => i.Slot == slot);

        public Item this[long id] => Items.FirstOrDefault(i => i.ObjectId == id);

        public void ManageItem(Item item)
        {
            _items.Add(item);
            Logger.Debug($"Item {item} is now managed.");
        }
    }
}