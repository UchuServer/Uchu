using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Uchu.Core
{
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    public class User
    {
        [Key]
        public long Id { get; set; }

        [MaxLength(33), Required] public string Username { get; set; }

        [MaxLength(60)] public string Password { get; set; }
        
        public bool Sso { get; set; }

        public bool Banned { get; set; }

        public string BannedReason { get; set; }
        
        public string CustomLockout { get; set; }

        public int GameMasterLevel { get; set; }
        
        public bool FreeToPlay { get; set; }

        public bool FirstTimeOnSubscription { get; set; }

        [Required] public int CharacterIndex { get; set; }

        public List<Character> Characters { get; set; }
    }
}