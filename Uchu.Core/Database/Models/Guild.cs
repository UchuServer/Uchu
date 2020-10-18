using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Uchu.Core
{
    public class Guild
    {
        [Key]
        public long Id { get; set; }
        
        public string Name { get; set; }
        
        public long CreatorId { get; set; }
        
        public List<GuildInvite> Invites { get; }
    }
}