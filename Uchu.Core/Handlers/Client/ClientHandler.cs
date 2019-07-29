using System.Net;

namespace Uchu.Core.Handlers
{
    public class ClientHandler : HandlerGroup
    {
        [PacketHandler]
        public void ValidateClient(SessionInfoPacket packet, IPEndPoint endPoint)
        {
            if (!Server.SessionCache.IsKey(packet.SessionKey))
            {
                Server.DisconnectClient(endPoint, DisconnectId.InvalidSessionKey);
                Logger.Warning($"{endPoint} attempted to connect with an invalid session key");
                return;
            }
            
            Server.SessionCache.RegisterKey(endPoint, packet.SessionKey);
        }
    }
}