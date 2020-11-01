using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Uchu.Core
{
    [SuppressMessage("ReSharper", "CA1716")]
    public class Friend
    {
        [Key]
        public long Id { get; set; }
        
        public bool BestFriend { get; set; }
        
        public long FriendA { get; set; }
        
        public long FriendB { get; set; }
    }
}