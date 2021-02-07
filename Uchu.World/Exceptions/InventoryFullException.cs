using System;

namespace Uchu.World.Exceptions
{
    /// <summary>
    /// Exception thrown when trying to manipulate an inventory when it's full
    /// </summary>
    public class InventoryFullException : Exception
    {
        public InventoryFullException() 
            : base("Cannot add item to inventory: inventory is full")
        {
        }
    }
}