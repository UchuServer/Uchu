using System.Linq;
using System.Net;
using Uchu.Core;

namespace Uchu.World.Handlers
{
    public class PositionUpdateHandler : HandlerGroup
    {
        [PacketHandler(RunTask = true)]
        public void HandlePositionUpdate(PositionUpdatePacket packet, IPEndPoint endPoint)
        {
            var session = Server.SessionCache.GetSession(endPoint);

            var player = ((WorldServer) Server).Zones.Where(z => z.ZoneInfo.ZoneId == session.ZoneId)
                .SelectMany(z => z.Players)
                .FirstOrDefault(p => p.EndPoint.Equals(endPoint));

            if (ReferenceEquals(player, null))
            {
                Logger.Error($"{endPoint} is not logged in but sent a Position Update packet.");
                return;
            }
            
            //
            // The server is a slave to the position update packets it gets from the client right now.
            //
            
            player.Transform.Position = packet.Position;
            player.Transform.Rotation = packet.Rotation;

            var physics = player.GetComponent<ControllablePhysicsComponent>();

            physics.HasPosition = true;
            
            physics.IsOnGround = packet.IsOnGround;
            physics.NegativeAngularVelocity = packet.NegativeAngularVelocity;

            physics.HasVelocity = packet.HasVelocity;
            
            physics.Velocity = packet.Velocity;

            physics.HasAngularVelocity = packet.HasAngularVelocity;

            physics.AngularVelocity = packet.AngularVelocity;
            
            physics.Platform = player.Zone.GameObjects.Find(o => o.ObjectId == packet.PlatformObjectId);
            physics.PlatformPosition = packet.PlatformPosition;
            
            player.Update();
        }
    }
}