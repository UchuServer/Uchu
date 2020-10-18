using System.Threading.Tasks;
using RakDotNet;
using Uchu.Core;

namespace Uchu.World.Handlers
{
    public class RoutedPacketHandler : HandlerGroup
    {
        [PacketHandler]
        public async Task RoutedHandler(ClientRoutedPacket packet, IRakConnection connection)
        {
            await UchuServer.HandlePacketAsync(connection.EndPoint, packet.Packet, Reliability.ReliableOrdered);
        }
    }
}