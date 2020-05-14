using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Uchu.Core
{
    public class CharacterFlag
    {
        [Key]
        public long Id { get; set; }
        
        public int Flag { get; set; }
        
        public long CharacterId { get; set; }
        
        [ForeignKey(nameof(CharacterId))]
        public Character Character { get; set; }
    }
}