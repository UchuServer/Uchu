using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace Uchu.Core
{
    public class InventoryItem
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long Id { get; set; }

        [Required]
        public int Lot { get; set; }

        [Required]
        public int Slot { get; set; }

        [Required]
        public long Count { get; set; } = 1;

        [Required]
        public bool IsBound { get; set; }

        [Required]
        public bool IsEquipped { get; set; }

        [Required]
        public int InventoryType { get; set; }

        public string ExtraInfo { get; set; }
        
        public long ParentId { get; set; }

        public long CharacterId { get; set; }
        
        [JsonIgnore]
        public Character Character { get; set; }
    }
}