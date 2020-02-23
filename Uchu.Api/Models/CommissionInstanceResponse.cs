using System;

namespace Uchu.Api.Models
{
    public class CommissionInstanceResponse : BaseResponse
    {
        public Guid Id { get; set; }
        
        public int ApiPort { get; set; }
    }
}