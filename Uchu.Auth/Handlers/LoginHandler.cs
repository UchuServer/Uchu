using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RakDotNet;
using Uchu.Core;

namespace Uchu.Auth.Handlers
{
    public class LoginHandler : HandlerGroup
    {
        public const int CloseDelay = 5000;

        [PacketHandler]
        public async Task LoginRequestHandler(ClientLoginInfoPacket packet, IRakConnection connection)
        {
            using (var ctx = new UchuContext())
            {
                var addresses = NetworkInterface.GetAllNetworkInterfaces()
                    .Where(i => (i.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 ||
                                 i.NetworkInterfaceType == NetworkInterfaceType.Ethernet) &&
                                i.OperationalStatus == OperationalStatus.Up)
                    .SelectMany(i => i.GetIPProperties().UnicastAddresses).Select(a => a.Address)
                    .Where(a => a.AddressFamily == AddressFamily.InterNetwork).ToArray();

                var address = connection.EndPoint.Address.ToString() == "127.0.0.1" ? "localhost" : addresses[0].ToString();

                var info = new ServerLoginInfoPacket
                {
                    CharacterInstanceAddress = address,
                    CharacterInstancePort = 2002,
                    ChatInstanceAddress = address,
                    ChatInstancePort = 2004
                };

                if (!await ctx.Users.AnyAsync(u => u.Username == packet.Username))
                {
                    info.LoginCode = LoginCode.InsufficientPermissions;
                    info.Error = new ServerLoginInfoPacket.ErrorMessage
                    {
                        Message = "We have no records of that Username and Password combination. Please try again."
                    };
                }
                else
                {
                    var user = await ctx.Users.SingleAsync(u => u.Username == packet.Username);

                    if (user != null && BCrypt.Net.BCrypt.EnhancedVerify(packet.Password, user.Password))
                    {
                        if (user.Banned)
                        {
                            info.LoginCode = LoginCode.InsufficientPermissions;
                            info.Error = new ServerLoginInfoPacket.ErrorMessage
                            {
                                Message = $"This account has been banned by an admin. Reason:\n{user.BannedReason ?? "Unknown"}"
                            };
                        }
                        else
                        {
                            var key = Server.SessionCache.CreateSession(user.UserId);

                            info.LoginCode = LoginCode.Success;
                            info.UserKey = key;
                        }
                    }
                    else
                    {
                        info.LoginCode = LoginCode.InsufficientPermissions;
                        info.Error = new ServerLoginInfoPacket.ErrorMessage
                        {
                            Message = "We have no records of that Username and Password combination. Please try again."
                        };
                    }
                }

                connection.Send(info);

                if (info.LoginCode == LoginCode.Success)
                {
                    await Task.Delay(CloseDelay);

                    await connection.CloseAsync();
                }
            }
        }
    }
}