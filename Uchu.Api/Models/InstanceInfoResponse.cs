namespace Uchu.Api.Models
{
    public class InstanceInfoResponse : BaseResponse
    {
        public bool Hosting { get; set; }
        
        public InstanceInfo Info { get; set; }
    }
}