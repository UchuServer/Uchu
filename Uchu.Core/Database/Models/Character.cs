using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Uchu.Core
{
    public class Character
    {
        public long CharacterId { get; set; }

        [MaxLength(33), Required]
        public string Name { get; set; }

        [MaxLength(33), Required]
        public string CustomName { get; set; }

        [Required]
        public bool NameRejected { get; set; } = false;

        [Required]
        public bool FreeToPlay { get; set; } = false;

        [Required]
        public long ShirtColor { get; set; }

        [Required]
        public long ShirtStyle { get; set; }

        [Required]
        public long PantsColor { get; set; }

        [Required]
        public long HairStyle { get; set; }

        [Required]
        public long HairColor { get; set; }

        [Required]
        public long Lh { get; set; }

        [Required]
        public long Rh { get; set; }

        [Required]
        public long EyebrowStyle { get; set; }

        [Required]
        public long EyeStyle { get; set; }

        [Required]
        public long MouthStyle { get; set; }

        [Required]
        public int LastZone { get; set; }

        [Required]
        public int LastInstance { get; set; } = 0;

        [Required]
        public long LastClone { get; set; } = 0;

        [Required]
        public long LastActivity { get; set; }

        public long UserId { get; set; }
        public User User { get; set; }

        public List<InventoryItem> Items { get; set; }
    }
}