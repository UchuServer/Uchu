using System.Threading.Tasks;
using RakDotNet;

namespace Uchu.Core.Handlers
{
    public class ClientHandler : HandlerGroup
    {
        [PacketHandler]
        public void ValidateClient(SessionInfoPacket packet, IRakConnection connection)
        {
            if (!Server.SessionCache.IsKey(packet.SessionKey))
            {
                Task.Run(connection.CloseAsync);
                Logger.Warning($"{connection} attempted to connect with an invalid session key");
                return;
            }
            
            Server.SessionCache.RegisterKey(connection.EndPoint, packet.SessionKey);
        }
    }
}