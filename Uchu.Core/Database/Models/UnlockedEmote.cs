using System.ComponentModel.DataAnnotations;

namespace Uchu.Core
{
    public class UnlockedEmote
    {
        [Key]
        public long Id { get; set; }
        
        public int EmoteId { get; set; }

        public long CharacterId { get; set; }
        
        public Character Character { get; set; }
    }
}