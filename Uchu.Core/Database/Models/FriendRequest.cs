using System.ComponentModel.DataAnnotations;

namespace Uchu.Core
{
    public class FriendRequest
    {
        [Key]
        public long Id { get; set; }
        
        public bool Sent { get; set; }
        
        public bool BestFriend { get; set; }
        
        public long Sender { get; set; }
        
        public long Receiver { get; set; }
    }
}