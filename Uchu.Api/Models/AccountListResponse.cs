using System.Collections.Generic;

namespace Uchu.Api.Models
{
    public class AccountListResponse : BaseResponse
    {
        public List<string> Accounts { get; set; }
    }
}