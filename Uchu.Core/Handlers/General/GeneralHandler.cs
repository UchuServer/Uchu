using System;
using RakDotNet;
using Uchu.Core.Resources;

namespace Uchu.Core.Handlers
{
    public class GlobalGeneral : HandlerGroup
    {
        [PacketHandler]
        public void Handshake(HandshakePacket packet, IRakConnection connection)
        {
            if (packet == null)
                throw new ArgumentNullException(nameof(packet), 
                    ResourceStrings.GeneralHandler_Handshake_PacketNullException);
            
            if (packet.GameVersion != 171022)
            {
                Logger.Warning($"Handshake attempted with client of Game version: {packet.GameVersion}");
                return;
            }
            
            // TODO: Use resource / setting
            const int port = 21836;

            connection.Send(new HandshakePacket
            {
                ConnectionType = UchuServer.Port == port ? 0x01u : 0x04u,
                Address = UchuServer.Host
            });
        }
    }
}