using System.Collections.Generic;

namespace Uchu.Api.Models
{
    public class CharacterListResponse : BaseResponse
    {
        public long UserId { get; set; }
        
        public List<string> Characters { get; set; }
    }
}