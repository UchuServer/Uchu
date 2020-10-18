using System;
using System.Threading.Tasks;
using RakDotNet;

namespace Uchu.Core.Handlers
{
    public class ClientHandler : HandlerGroup
    {
        [PacketHandler]
        public async Task ValidateClient(SessionInfoPacket packet, IRakConnection connection)
        {
            if (packet == null)
                throw new ArgumentNullException(nameof(packet), "Received null packet in validate client");
            
            await Server.ValidateUserAsync(connection, packet.Username, packet.SessionKey)
                .ConfigureAwait(false);
        }
    }
}