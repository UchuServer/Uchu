using System;
using System.ComponentModel.DataAnnotations;

namespace Uchu.Core
{
    public class SessionCache
    {
        [Key]
        public Guid Id { get; set; }
        
        public string Key { get; set; }
        
        public long CharacterId { get; set; } = -1;
        
        public long UserId { get; set; }
        
        public int ZoneId { get; set; } = -1;
    }
}