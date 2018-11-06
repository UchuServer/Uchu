using System.Net;

namespace Uchu.Core
{
    public class GlobalHandler : HandlerGroupBase
    {
        [PacketHandler]
        public void Handshake(HandshakePacket packet, IPEndPoint endpoint)
        {
            Server.Send(new HandshakePacket
            {
                ConnectionType = Server.Port == 1001 ? 0x01u : 0x04u
            }, endpoint);
        }

        [PacketHandler]
        public void ValidateClient(SessionInfoPacket packet, IPEndPoint endpoint)
        {
            var session = Server.Cache.GetSession(endpoint);

            if (session == null || packet.UserKey != session.Key)
            {
                Server.DisconnectClient(endpoint, DisconnectId.InvalidSessionKey);
            }
        }
    }
}