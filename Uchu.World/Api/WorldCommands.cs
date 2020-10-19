using System.Linq;
using System.Threading.Tasks;
using Uchu.Api;
using Uchu.Api.Models;
using Uchu.Core;

namespace Uchu.World.Api
{
    public class WorldCommands
    {
        public WorldUchuServer UchuServer { get; }

        public WorldCommands(WorldUchuServer uchuServer)
        {
            UchuServer = uchuServer;
        }
        
        [ApiCommand("world/players")]
        public object ZonePlayers(string zone)
        {
            var response = new ZonePlayersResponse();

            if (!int.TryParse(zone, out var zoneId))
            {
                response.FailedReason = "invalid zone";

                return response;
            }

            var zoneInstance = UchuServer.Zones.FirstOrDefault(z => z.ZoneId == (ZoneId) zoneId);

            if (zoneInstance == default)
            {
                response.FailedReason = "not found";

                return response;
            }

            response.Success = true;
            
            response.MaxPlayers = (int) UchuServer.MaxPlayerCount; // TODO: Set

            response.Characters = zoneInstance.Players.Select(p => (long) p.Id).ToList();

            return response;
        }
    }
}