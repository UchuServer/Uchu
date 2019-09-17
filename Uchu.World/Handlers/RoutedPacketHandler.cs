using System.Net;
using RakDotNet;
using Uchu.Core;

namespace Uchu.World.Handlers
{
    public class RoutedPacketHandler : HandlerGroup
    {
        [PacketHandler(RunTask = true)]
        public void HandleRoutedPacket(ClientRoutedPacket packet, IPEndPoint endPoint)
        {
            Server.HandlePacket(endPoint, packet.Packet, Reliability.ReliableOrdered);
        }
    }
}