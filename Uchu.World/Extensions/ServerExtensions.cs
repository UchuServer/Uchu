using System.Linq;
using RakDotNet;
using Uchu.Core;

namespace Uchu.World
{
    public static class ServerExtensions
    {
        public static Player FindPlayer(this Server @this, IRakConnection connection)
        {
            var session = @this.SessionCache.GetSession(connection.EndPoint);

            var world = ((WorldServer) @this).Zones.FirstOrDefault(z => z.ZoneId == (ZoneId) session.ZoneId);

            return world?.Players.FirstOrDefault(p => p.Connection.EndPoint.Equals(connection.EndPoint));
        }
    }
}