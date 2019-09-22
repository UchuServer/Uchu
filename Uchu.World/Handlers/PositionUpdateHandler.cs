using System.Linq;
using System.Net;
using RakDotNet;
using Uchu.Core;

namespace Uchu.World.Handlers
{
    public class PositionUpdateHandler : HandlerGroup
    {
        [PacketHandler]
        public void HandlePositionUpdate(PositionUpdatePacket packet, IRakConnection connection)
        {
            var session = Server.SessionCache.GetSession(connection.EndPoint);

            var player = ((WorldServer) Server).Zones.First(
                z => z.ZoneId == (ZoneId) session.ZoneId
            ).Players.FirstOrDefault(
                p => p.Connection.Equals(connection)
            );

            if (ReferenceEquals(player, null))
            {
                Logger.Error($"{connection} is not logged in but sent a Position Update packet.");
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

            physics.Platform = player.Zone.GetGameObject(packet.PlatformObjectId);

            physics.PlatformPosition = packet.PlatformPosition;

            player.Zone.SendSerialization(player, player.Zone.Players.Where(
                p => p != player
            ).ToArray());
        }
    }
}