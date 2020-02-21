using Uchu.Core;

namespace Uchu.Api.Models
{
    public class AccountAdminResponse : BaseResponse
    {
        public string Username { get; set; }
        
        public GameMasterLevel Level { get; set; }
    }
}