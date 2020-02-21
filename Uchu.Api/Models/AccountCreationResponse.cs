namespace Uchu.Api.Models
{
    public class AccountCreationResponse : BaseResponse
    {
        public long Id { get; set; }
        
        public string Username { get; set; }
        
        public string Hash { get; set; }
    }
}