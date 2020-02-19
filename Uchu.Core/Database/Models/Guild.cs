using System.Collections.Generic;

namespace Uchu.Core
{
    public class Guild
    {
        public long Id { get; set; }
        
        public string Name { get; set; }
        
        public long CreatorId { get; set; }
        
        public List<GuildInvite> Invites { get; set; }
    }
}