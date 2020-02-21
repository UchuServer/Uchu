namespace Uchu.Api.Models
{
    public class BaseResponse
    {
        public bool Success { get; set; }
        
        public string FailedReason { get; set; }
    }
}