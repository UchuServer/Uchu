using System.ComponentModel.DataAnnotations;

namespace Uchu.Core
{
    public class Friend
    {
        [Key]
        public long Id { get; set; }
        
        public bool BestFriend { get; set; }
        
        public long FriendA { get; set; }
        
        public long FriendB { get; set; }
    }
}