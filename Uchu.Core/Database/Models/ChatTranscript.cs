using System;
using System.ComponentModel.DataAnnotations;

namespace Uchu.Core
{
    public class ChatTranscript
    {
        [Key]
        public long Id { get; set; }
        
        public DateTime SentTime { get; set; }
        
        public long Author { get; set; }
        
        public long Receiver { get; set; }
        
        public string Message { get; set; }
    }
}