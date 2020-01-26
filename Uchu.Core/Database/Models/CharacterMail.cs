using System;
using System.ComponentModel.DataAnnotations;

namespace Uchu.Core
{
    public class CharacterMail
    {
        [Key]
        public long Id { get; set; }
        
        public string Subject { get; set; }
        
        public string Body { get; set; }
        
        public int AttachmentLot { get; set; }
        
        public ushort AttachmentCount { get; set; }
        
        public ulong AttachmentCurrency { get; set; }
        
        public DateTime ExpirationTime { get; set; }
        
        public DateTime SentTime { get; set; }
        
        public bool Read { get; set; }
        
        public long RecipientId { get; set; }
        
        public long AuthorId { get; set; }
    }
}