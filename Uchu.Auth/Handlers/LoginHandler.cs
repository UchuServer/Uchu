using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Uchu.Core;

namespace Uchu.Auth
{
    public class LoginHandler : HandlerGroupBase
    {
        [PacketHandler]
        public async Task LoginInfo(ClientLoginInfoPacket packet, IPEndPoint endpoint)
        {
            using (var ctx = new UchuContext())
            {
                var addresses = NetworkInterface.GetAllNetworkInterfaces()
                    .Where(i => (i.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 ||
                                 i.NetworkInterfaceType == NetworkInterfaceType.Ethernet) &&
                                i.OperationalStatus == OperationalStatus.Up)
                    .SelectMany(i => i.GetIPProperties().UnicastAddresses).Select(a => a.Address)
                    .Where(a => a.AddressFamily == AddressFamily.InterNetwork).ToArray();

                var address = endpoint.Address.ToString() == "127.0.0.1" ? "127.0.0.1" : addresses[0].ToString();

                var info = new ServerLoginInfoPacket
                {
                    CharacterInstanceAddress = address,
                    CharacterInstancePort = 2002,
                    ChatInstanceAddress = address,
                    ChatInstancePort = 2004
                };

                if (!await ctx.Users.AnyAsync(u => u.Username == packet.Username))
                {
                    info.LoginCode = LoginCode.InvalidLogin;
                }
                else
                {
                    var user = await ctx.Users.SingleAsync(u => u.Username == packet.Username);

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
                }

                Server.Send(info, endpoint);
            }
        }
    }
}