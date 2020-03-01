namespace Uchu.Api.Models
{
    public class AccountAdminResponse : BaseResponse
    {
        public string Username { get; set; }
        
        public int Level { get; set; }
    }
}