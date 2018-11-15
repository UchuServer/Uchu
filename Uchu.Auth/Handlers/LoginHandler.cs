using System;
using System.Net;
using System.Threading.Tasks;
using Uchu.Core;

namespace Uchu.Auth
{
    public class LoginHandler : HandlerGroupBase
    {
        [PacketHandler]
        public async Task LoginInfo(ClientLoginInfoPacket packet, IPEndPoint endpoint)
        {
            var user = await Database.GetUserAsync(packet.Username);

            var address = endpoint.Address.ToString() == "127.0.0.1" ? "127.0.0.1" : "192.168.1.109";

            var info = new ServerLoginInfoPacket
            {
                CharacterInstanceAddress = address,
                CharacterInstancePort = 2002,
                ChatInstanceAddress = address,
                ChatInstancePort = 2004
            };

            if (user != null && BCrypt.Net.BCrypt.EnhancedVerify(packet.Password, user.Password))
            {
                var key = Server.SessionCache.CreateSession(endpoint, user.UserId);

                info.LoginCode = LoginCode.Success;
                info.UserKey = key;
            }
            else
            {
                info.LoginCode = LoginCode.InvalidLogin;
            }

            Server.Send(info, endpoint);
        }
    }
}