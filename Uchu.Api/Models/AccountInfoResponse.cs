using Uchu.Core;

namespace Uchu.Api.Models
{
    public class AccountInfoResponse : BaseResponse
    {
        public long Id { get; set; }
        
        public string Username { get; set; }
        
        public string Hash { get; set; }
        
        public bool Banned { get; set; }
        
        public string BannedReason { get; set; }
        
        public GameMasterLevel Level { get; set; }
    }
}