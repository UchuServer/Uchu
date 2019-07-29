using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Uchu.Core
{
    public class User
    {
        public long UserId { get; set; }

        [MaxLength(33), Required]
        public string Username { get; set; }

        [MaxLength(60), Required]
        public string Password { get; set; }
        
        public bool Banned { get; set; }
        
        public string BannedReason { get; set; }

        [Required]
        public int CharacterIndex { get; set; }

        public List<Character> Characters { get; set; }
    }
}