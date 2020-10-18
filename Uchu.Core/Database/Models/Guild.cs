using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Uchu.Core
{
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    public class Guild
    {
        [Key]
        public long Id { get; set; }
        
        public string Name { get; set; }
        
        public long CreatorId { get; set; }
        
        public List<GuildInvite> Invites { get; set; }
    }
}