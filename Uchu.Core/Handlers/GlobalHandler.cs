using System;
using RakDotNet;
using System.Net;

namespace Uchu.Core
{
    public class GlobalHandler : HandlerGroupBase
    {
        [PacketHandler]
        public void Handshake(HandshakePacket packet, IPEndPoint endpoint)
        {
            var port = Server.RakNetServer.Protocol == ServerProtocol.RakNet ? 1001 : 21836;

            Server.Send(new HandshakePacket
            {
                ConnectionType = Server.Port == port ? 0x01u : 0x04u
            }, endpoint);
            
        }

        [PacketHandler]
        public void ValidateClient(SessionInfoPacket packet, IPEndPoint endpoint)
        {
            if (!Server.SessionCache.IsKey(packet.UserKey))
            {
                Server.DisconnectClient(endpoint, DisconnectId.InvalidSessionKey);
                return;
            }
            
            Console.WriteLine($"Registered Key for {endpoint} [{packet.UserKey}]");
            
            Server.SessionCache.RegisterKey(endpoint, packet.UserKey);
        }
    }
}