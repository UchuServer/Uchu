namespace Uchu.Api.Models
{
    public class AccountBanResponse : BaseResponse
    {
        public string Username { get; set; }
        
        public string BannedReason { get; set; }
    }
}