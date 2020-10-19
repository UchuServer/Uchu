using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RakDotNet;
using Uchu.Api.Models;
using Uchu.Core;
using static Uchu.Auth.ServerLoginInfoPacket;

namespace Uchu.Auth.Handlers
{
    public class LoginHandler : HandlerGroup
    {
        [PacketHandler]
        public async Task LoginRequestHandler(ClientLoginInfoPacket packet, IRakConnection connection)
        {   
            await using var ctx = new UchuContext();
            
            var info = new ServerLoginInfoPacket
            {
                CharacterInstanceAddress = UchuServer.Host,
                CharacterInstancePort = ushort.MaxValue,
                ChatInstanceAddress = UchuServer.Host,
                ChatInstancePort = 2004
            };

            var characterSpecification = await UchuServer.Api.RunCommandAsync<InstanceInfoResponse>(
                UchuServer.MasterApi, $"instance/basic?t={(int) ServerType.Character}"
            ).ConfigureAwait(false);

            if (!characterSpecification.Success)
            {
                Logger.Error(characterSpecification.FailedReason);
                
                info.LoginCode = LoginCode.InsufficientPermissions;
                info.Error = new ErrorMessage
                {
                    Message = "No character server instance is running. Please try again later."
                };
            }
            else
            {
                info.CharacterInstancePort = (ushort) characterSpecification.Info.Port;

                if (!await ctx.Users.AnyAsync(u => u.Username == packet.Username && !u.Sso))
                {
                    info.LoginCode = LoginCode.InsufficientPermissions;
                    info.Error = new ErrorMessage
                    {
                        Message = "We have no records of that Username and Password combination. Please try again."
                    };
                }
                else
                {
                    var user = await ctx.Users.SingleAsync(u => u.Username == packet.Username && !u.Sso);

                    if (user != null && BCrypt.Net.BCrypt.EnhancedVerify(packet.Password, user.Password))
                    {
                        if (user.Banned)
                        {
                            info.LoginCode = LoginCode.InsufficientPermissions;
                            info.Error = new ErrorMessage
                            {
                                Message = $"This account has been banned by an admin. Reason:\n{user.BannedReason ?? "Unknown"}"
                            };
                        }
                        else if (!string.IsNullOrWhiteSpace(user.CustomLockout))
                        {
                            info.LoginCode = LoginCode.InsufficientPermissions;
                            info.Error = new ErrorMessage
                            {
                                Message = user.CustomLockout
                            };
                        }
                        else
                        {
                            var key = UchuServer.SessionCache.CreateSession(user.Id);

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
                        info.Error = new ErrorMessage
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