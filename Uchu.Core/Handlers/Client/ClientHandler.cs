using System.Threading.Tasks;
using RakDotNet;

namespace Uchu.Core.Handlers
{
    public class ClientHandler : HandlerGroup
    {
        [PacketHandler]
        public async Task ValidateClient(SessionInfoPacket packet, IRakConnection connection)
        {
            await Server.ValidateUserAsync(connection, packet.Username, packet.SessionKey).ConfigureAwait(false);
        }
    }
}