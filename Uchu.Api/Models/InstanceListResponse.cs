using System.Collections.Generic;

namespace Uchu.Api.Models
{
    public class InstanceListResponse : BaseResponse
    {
        public List<InstanceInfo> Instances { get; set; }
    }
}