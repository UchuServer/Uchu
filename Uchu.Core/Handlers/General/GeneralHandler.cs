using System.Net;
using RakDotNet;

namespace Uchu.Core.Handlers
{
    public class GlobalGeneral : HandlerGroup
    {
        [PacketHandler]
        public void Handshake(HandshakePacket packet, IPEndPoint endPoint)
        {
            if (packet.GameVersion != 171022)
            {
                Logger.Warning($"Handshake attempted with client of Game version: {packet.GameVersion}");
                return;
            }
            var port = Server.RakNetServer.Protocol == ServerProtocol.TcpUdp ? 21836 : 1001;

            Server.Send(new HandshakePacket
            {
                ConnectionType = Server.Port == port ? 0x01u : 0x04u
            }, endPoint);
        }
    }
}