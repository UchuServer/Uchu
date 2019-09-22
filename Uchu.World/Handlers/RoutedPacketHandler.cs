using System.Net;
using System.Threading.Tasks;
using RakDotNet;
using Uchu.Core;

namespace Uchu.World.Handlers
{
    public class RoutedPacketHandler : HandlerGroup
    {
        [PacketHandler]
        public async Task HandleRoutedPacket(ClientRoutedPacket packet, IRakConnection connection)
        {
            await Server.HandlePacketAsync(connection.EndPoint, packet.Packet, Reliability.ReliableOrdered);
        }
    }
}