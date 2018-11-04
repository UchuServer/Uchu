using System.Net;

namespace Uchu.Core.Handlers
{
    public class HandshakeHandler : HandlerGroupBase
    {
        [PacketHandler]
        public void Handshake(HandshakePacket packet, IPEndPoint endpoint)
        {
            Server.Send(new HandshakePacket {ConnectionType = Server.Port == 1001 ? 0x01u : 0x04u}, endpoint);
        }
    }
}