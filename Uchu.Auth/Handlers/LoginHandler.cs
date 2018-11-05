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
            var user = await Database.GetUserAsync(packet.Username) ??
                       await Database.CreateUserAsync(packet.Username, packet.Password);

            var info = new ServerLoginInfoPacket
            {
                WorldInstanceAddress = "127.0.0.1",
                WorldInstancePort = 2002,
                ChatInstanceAddress = "127.0.0.1",
                ChatInstancePort = 2003
            };

            if (BCrypt.Net.BCrypt.EnhancedVerify(packet.Password, user.Password))
            {
                var key = Server.Cache.CreateSession(endpoint, user.UserId);

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