using System.ComponentModel.DataAnnotations;

namespace Uchu.Core
{
    public class Character
    {
        public long CharacterId { get; set; }

        [MaxLength(33), Required]
        public string Name { get; set; }

        public long UserId { get; set; }
        public User User { get; set; }
    }
}