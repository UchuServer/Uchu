using System.Net;
using RakDotNet;
using Uchu.Core;

namespace Uchu.World.Handlers
{
    public class RoutedPacketHandler : HandlerGroup
    {
        [PacketHandler(RunTask = true)]
        public void HandleRoutedPacket(ClientRoutedPacket packet, IRakConnection connection)
        {
            Server.HandlePacket(connection.EndPoint, packet.Packet, Reliability.ReliableOrdered);
        }
    }
}