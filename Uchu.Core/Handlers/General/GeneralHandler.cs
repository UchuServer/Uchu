using RakDotNet;

namespace Uchu.Core.Handlers
{
    public class GlobalGeneral : HandlerGroup
    {
        [PacketHandler]
        public void Handshake(HandshakePacket packet, IRakConnection connection)
        {
            if (packet.GameVersion != 171022)
            {
                Logger.Warning($"Handshake attempted with client of Game version: {packet.GameVersion}");
                return;
            }
            
            const int port = 21836;

            connection.Send(new HandshakePacket
            {
                ConnectionType = Server.Port == port ? 0x01u : 0x04u,
                Address = Server.Host
            });
        }
    }
}