using System;
using System.Collections.Generic;
using System.Linq;
using Uchu.Core;

namespace Uchu.World
{
    public class Inventory
    {
        public readonly InventoryType InventoryType;

        public readonly InventoryManager Manager;

        public IReadOnlyCollection<Item> Items => Array.AsReadOnly(_items.ToArray());

        private readonly List<Item> _items;
        
        public Inventory(InventoryType inventoryType, InventoryManager manager)
        {
            InventoryType = inventoryType;
            Manager = manager;

            using (var ctx = new UchuContext())
            {
                var items = ctx.InventoryItems.Where(
                    i => i.CharacterId == manager.Player.ObjectId && i.InventoryType == (int) inventoryType
                );

                _items = items.Select(
                    i => Item.Instantiate(i.InventoryItemId, this)
                ).Where(i => i != default).ToList();
            }
        }

        public Item this[uint slot]
        {
            get => Items.FirstOrDefault(s => s.Slot == slot);
            set => value.Slot = slot;
        }
    }
}