namespace Uchu.Api.Models
{
    public class AccountVerifyResponse : BaseResponse
    {
        public string Username { get; set; }

        public bool VerifiedPassword { get; set; }

        public string Key { get; set; }
    }
}