using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Uchu.Core
{
    public class UnlockedEmote
    {
        [Key]
        public long Id { get; set; }
        
        public int EmoteId { get; set; }

        public long CharacterId { get; set; }
        
        [JsonIgnore]
        public Character Character { get; set; }
    }
}