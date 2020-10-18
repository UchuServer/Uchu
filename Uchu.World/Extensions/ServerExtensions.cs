using System.Linq;
using RakDotNet;
using Uchu.Core;

namespace Uchu.World
{
    public static class ServerExtensions
    {
        public static Player FindPlayer(this UchuServer @this, IRakConnection connection)
        {
            if (connection == null) return null;
            return ((WorldUchuServer) @this).Zones.SelectMany(z => z.Players).FirstOrDefault(
                player => player.Connection.EndPoint.Equals(connection.EndPoint)
            );
        }
    }
}