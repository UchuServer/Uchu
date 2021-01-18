using System;

namespace Uchu.World.Exceptions
{
    public class InventorySlotOccupiedException : Exception
    {
        public InventorySlotOccupiedException()
            : base("Cannot add item to inventory: slot is already occupied by an item")
        {
        }
    }
}