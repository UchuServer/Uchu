using System.Linq;
using System.Threading.Tasks;
using Uchu.Api;
using Uchu.Api.Models;
using Uchu.Core;
using Uchu.World.Social;

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

        /// <summary>
        /// Take in a zone ID, find that zone in this server's Zones, return whether it is fully loaded
        /// </summary>
        /// <param name="zone">Zone ID</param>
        /// <returns>Whether the zone has loaded all objects</returns>
        [ApiCommand("world/zoneStatus")]
        public object ZoneStatus(string zone)
        {
            var response = new ZoneStatusResponse();

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

            response.Loaded = zoneInstance.Loaded;

            return response;
        }

        [ApiCommand("world/saveAndKick")]
        public async Task<object> SaveAndKick()
        {
            var response = new BaseResponse();
            foreach (var zonePlayer in UchuServer.Zones.SelectMany(zoneInstance => zoneInstance.Players))
            {
                await zonePlayer.GetComponent<SaveComponent>().SaveAsync();
                zonePlayer.Message(new DisconnectNotifyPacket
                {
                    DisconnectId = DisconnectId.ServerShutdown
                });
            }

            response.Success = true;
            return response;
        }

        [ApiCommand("world/announce")]
        public async Task Announce(string title, string message)
        {
            foreach (var zone in UchuServer.Zones)
            {
                foreach (var zonePlayer in zone.Players)
                {
                    await UiHelper.AnnouncementAsync(zonePlayer, title, message);
                }
            }
        }
    }
}
