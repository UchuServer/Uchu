using System.Net;
using Uchu.Core;

namespace Uchu.Auth
{
    public class LoginHandler : HandlerGroupBase
    {
        [PacketHandler]
        public void LoginInfo(ClientLoginInfoPacket packet, IPEndPoint endpoint)
        {
            var key = Server.Cache.CreateSession(endpoint, 0);
        }
    }
}