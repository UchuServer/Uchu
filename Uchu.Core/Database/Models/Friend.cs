using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Uchu.Core
{
    public class Friend
    {
        [Key]
        public int Id { get; set; }
        
        public bool IsAccepted { get; set; }
        
        public bool IsDeclined { get; set; }
        
        public bool IsBestFriend { get; set; }
        
        public bool RequestHasBeenSent { get; set; }
        
        public bool RequestingBestFriend { get; set; }
        
        public long FriendId { get; set; }
        
        [ForeignKey("FriendId")]
        public Character FriendOne { get; set; }
        
        public long FriendTwoId { get; set; }
        
        [ForeignKey("FriendTwoId")]
        public Character FriendTwo { get; set; }
    }
}