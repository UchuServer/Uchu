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
        public long Count { get; set; } = 1;

        [Required]
        public bool IsBound { get; set; } = false;

        [Required]
        public bool IsEquipped { get; set; } = false;

        [Required]
        public int InventoryType { get; set; } = 0;

        public string ExtraInfo { get; set; } = null;

        public long CharacterId { get; set; }
        public Character Character { get; set; }
    }
}