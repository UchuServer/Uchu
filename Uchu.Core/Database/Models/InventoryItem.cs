using System.ComponentModel.DataAnnotations;

namespace Uchu.Core
{
    public class InventoryItem
    {
        public int InventoryItemId { get; set; }

        [Required]
        public long LOT { get; set; }

        public long CharacterId { get; set; }
        public Character Character { get; set; }
    }
}