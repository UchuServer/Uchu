using System;
using System.Collections.Generic;

namespace Uchu.Api.Models
{
    public class MasterStatusResponse : BaseResponse
    {
        public List<Guid> Instances { get; set; }
        
        public bool CanHostAdditionWorldInstances { get; set; }
    }
}