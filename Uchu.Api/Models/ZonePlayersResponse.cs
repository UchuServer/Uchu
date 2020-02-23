using System.Collections.Generic;

namespace Uchu.Api.Models
{
    public class ZonePlayersResponse : BaseResponse
    {
        public List<long> Characters { get; set; }
        
        public int MaxPlayers { get; set; }
    }
}