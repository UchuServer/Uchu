using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Uchu.Core
{
    public class GuildInvite
    {
        [Key]
        public long Id { get; set; }
        
        public long SenderId { get; set; }
        
        public long RecipientId { get; set; }
        
        public long GuildId { get; set; }
        
        [ForeignKey(nameof(GuildId))]
        public Guild Guild { get; set; }
    }
}