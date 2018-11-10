using System.ComponentModel.DataAnnotations;

namespace Uchu.Core
{
    public class InventoryItem
    {
        public long InventoryItemId { get; set; }

        [Required]
        public int LOT { get; set; }

        [Required]
        public int Slot { get; set; }

        [Required]
        public long Count { get; set; }

        public long CharacterId { get; set; }
        public Character Character { get; set; }
    }
}