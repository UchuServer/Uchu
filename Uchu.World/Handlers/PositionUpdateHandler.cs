using System.Linq;
using System.Threading.Tasks;
using RakDotNet;
using Uchu.Core;

namespace Uchu.World.Handlers
{
    public class PositionUpdateHandler : HandlerGroup
    {
        [PacketHandler]
        public async Task PositionHandler(PositionUpdatePacket packet, IRakConnection connection)
        {
            var player = Server.FindPlayer(connection);

            if (player?.Transform == default)
            {
                Logger.Warning($"{connection} is not logged in but sent a Position Update packet.");
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

            physics.Platform = default; //player.Zone.GameObjects.FirstOrDefault(g => g.ObjectId == packet.PlatformObjectId);

            physics.PlatformPosition = packet.PlatformPosition;

            Zone.SendSerialization(player, player.Zone.Players.Where(
                p => p != player
            ).ToArray());

            await player.OnPositionUpdate.InvokeAsync(player.Transform.Position, player.Transform.Rotation);
        }
    }
}