using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RakDotNet;
using Uchu.Core;

namespace Uchu.Auth.Handlers
{
    public class LoginHandler : HandlerGroup
    {
        [PacketHandler]
        public async Task LoginRequestHandler(ClientLoginInfoPacket packet, IRakConnection connection)
        {
            await using var ctx = new UchuContext();
            
            var addresses = Server.GetAddresses();
            
            var address = connection.EndPoint.Address.ToString() == "127.0.0.1" ? "localhost" : addresses[0].ToString();

            var info = new ServerLoginInfoPacket
            {
                CharacterInstanceAddress = address,
                CharacterInstancePort = ushort.MaxValue,
                ChatInstanceAddress = address,
                ChatInstancePort = 2004
            };

            var characterSpecification = await ServerHelper.GetServerByType(ServerType.Character);

            if (characterSpecification == default)
            {
                info.LoginCode = LoginCode.InsufficientPermissions;
                info.Error = new ServerLoginInfoPacket.ErrorMessage
                {
                    Message = "No character server instance is running. Please try again later."
                };
            }
            else
            {
                info.CharacterInstancePort = (ushort) characterSpecification.Port;

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
                        else if (!string.IsNullOrWhiteSpace(user.CustomLockout))
                        {
                            info.LoginCode = LoginCode.InsufficientPermissions;
                            info.Error = new ServerLoginInfoPacket.ErrorMessage
                            {
                                Message = user.CustomLockout
                            };
                        }
                        else
                        {
                            var key = Server.SessionCache.CreateSession(user.UserId);

                            info.LoginCode = LoginCode.Success;
                            info.UserKey = key;

                            //
                            // I don't intend, nor do I see anyone else, using these.
                            // Except maybe FirstTimeOnSubscription for the fancy screen.
                            //
                            
                            info.FreeToPlay = user.FreeToPlay;
                            info.FirstLoginWithSubscription = user.FirstTimeOnSubscription;

                            //
                            // No longer the first time on subscription
                            //
                            
                            user.FirstTimeOnSubscription = false;

                            await ctx.SaveChangesAsync();
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
            }

            connection.Send(info);
        }
    }
}