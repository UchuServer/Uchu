using System;
using System.Threading.Tasks;
using RakDotNet;
using Uchu.Core.Resources;

namespace Uchu.Core.Handlers
{
    public class ClientHandler : HandlerGroup
    {
        [PacketHandler]
        public async Task ValidateClient(SessionInfoPacket packet, IRakConnection connection)
        {
            if (packet == null)
                throw new ArgumentNullException(nameof(packet), 
                    ResourceStrings.ClientHandler_ValidateClient_PacketNullException);
            
            await UchuServer.ValidateUserAsync(connection, packet.Username, packet.SessionKey)
                .ConfigureAwait(false);
        }
    }
}