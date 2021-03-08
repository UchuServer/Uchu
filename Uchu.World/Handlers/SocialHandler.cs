using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RakDotNet;
using Uchu.Core;

namespace Uchu.World.Handlers
{
    public class SocialHandler : HandlerGroup
    {
        public static string[] ClientCommands = { "/quit", "/exit", "/logoutcharacter", "/camp", "/logoutaccount", "/logout", "/say", "/s",
            "/whisper", "/w", "/tell", "/team", "/t", "/location", "/locate", "/loc", "/faq", "/faqs", "/shop", "/store", "/minigames", "/forums",
            "/thumbsup", "/thumb", "/thumb", "/victory", "/backflip", "/clap", "/cringe", "/cry", "/dance", "/gasp", "/giggle", "/talk", "/salute",
            "/shrug", "/sigh", "/wave", "/why", "/thanks", "/yes", "/addfriend", "/removefriend", "/addignore", "/removeignore", "/recommendedperfoptions",
            "/perfoptionslow", "/perfoptionsmid", "/perfoptionshigh", "/invite", "/tinvite", "/teaminvite", "/inviteteam", "/leaveteam", "/leave", "/tleave",
            "/teamleave", "/setloot", "/tloot", "/tsetloot", "/teamsetloot", "/kickplayer", "/tkick", "/kick", "/tkickplayer", "/teamkickplayer", "/leader",
            "/setleader", "/tleader", "/tsetleader", "/teamsetleader", "/cancelqueue" };

        [PacketHandler]
        public async Task ParseChatMessageHandler(ParseChatMessage message, Player player)
        {
            try
            {
                await player.Zone.OnChatMessage.InvokeAsync(player, message.Message);
            }
            catch
            {
                // Something went wrong with event
            }
            
            await using var ctx = new UchuContext();

            var character = await ctx.Characters.Include(c => c.User).FirstAsync(
                c => c.Id == player.Id
            );

            if (message.Message.StartsWith('/') && !ClientCommands.Contains(message.Message.Split(" ").ElementAt(0)))
            {

                //Logger.Debug($"message.Message: {message.Message}");

                if (ClientCommands.Contains(message.Message.Split(" ").ElementAt(0)))
                    return;

                var response = await UchuServer.HandleCommandAsync(
                    message.Message,
                    player,
                    (GameMasterLevel) character.User.GameMasterLevel
                );

                if (!string.IsNullOrWhiteSpace(response))
                {
                    player.SendChatMessage(response, PlayerChatChannel.Normal);
                }
                
                var CommandTranscript = new ChatTranscript
                {
                    Author = character.Id,
                    Message = message.Message,
                    Receiver = 0,
                    SentTime = DateTime.Now
                };

                await ctx.ChatTranscript.AddAsync(CommandTranscript);

                await ctx.SaveChangesAsync();
                
                return;
            }

            Console.WriteLine($"Message: {message.Message}");
            
            if (((WorldUchuServer) UchuServer).Whitelist.CheckPhrase(message.Message).Any())
                return;
            
            var transcript = new ChatTranscript
            {
                Author = character.Id,
                Message = message.Message,
                Receiver = 0,
                SentTime = DateTime.Now
            };

            await ctx.ChatTranscript.AddAsync(transcript);

            await ctx.SaveChangesAsync();
            
            foreach (var zonePlayer in player.Zone.Players)
            {
                zonePlayer.SendChatMessage(message.Message, PlayerChatChannel.Normal, player);
            }
        }

        [PacketHandler]
        public async Task AddFriendRequestHandler(AddFriendRequestPacket packet, IRakConnection connection)
        {
            var session = UchuServer.SessionCache.GetSession(connection.EndPoint);
            var zone = ((WorldUchuServer) UchuServer).Zones.FirstOrDefault(z => z.ZoneId == session.ZoneId);

            if (zone == default)
            {
                Logger.Error($"Invalid ZoneId for {connection}");
                return;
            }

            var player = zone.Players.First(p => p.Connection.Equals(connection));

            await using var ctx = new UchuContext();
            
            var friend = await ctx.Characters.FirstOrDefaultAsync(c => c.Name == packet.PlayerName);

            if (friend == default)
            {
                Logger.Information(
                    $"{player.Name} is trying to be friends with a none existent player {packet.PlayerName}"
                );
                    
                player.Message(new NotifyFriendRequestResponsePacket
                {
                    PlayerName = packet.PlayerName,
                    Response = ServerFriendRequestResponse.InvalidName
                });
                    
                return;
            }

            var character = ctx.Characters.First(c => c.Id == session.CharacterId);

            var alreadyFriends = await ctx.Friends.Where(f =>
                f.FriendA == character.Id || f.FriendB == character.Id
            ).AnyAsync();

            if (alreadyFriends)
            {
                player.Message(new NotifyFriendRequestResponsePacket
                {
                    PlayerName = packet.PlayerName,
                    Response = ServerFriendRequestResponse.AlreadyFriends
                });
                
                return;
            }
            
            var isPending = await ctx.FriendRequests.Where(f =>
                f.Sender == character.Id && f.Receiver == friend.Id
            ).AnyAsync();

            if (isPending)
            {
                player.Message(new NotifyFriendRequestResponsePacket
                {
                    PlayerName = packet.PlayerName,
                    Response = ServerFriendRequestResponse.PendingApproval
                });
                
                return;
            }
            
            var request = new FriendRequest
            {
                Sender = character.Id,
                Receiver = friend.Id,
                BestFriend = packet.IsRequestingBestFriend
            };

            await ctx.FriendRequests.AddAsync(request);
        }

        [PacketHandler]
        public async Task FriendsListRequestHandler(GetFriendListPacket packet, IRakConnection connection)
        {
            var session = UchuServer.SessionCache.GetSession(connection.EndPoint);
            var zone = ((WorldUchuServer) UchuServer).Zones.FirstOrDefault(z => (int) z.ZoneId == session.ZoneId);

            if (zone == default)
            {
                Logger.Error($"Invalid ZoneId for {connection}");
                return;
            }

            await using var ctx = new UchuContext();
            var character =  await ctx.Characters.FirstAsync(c => c.Id == session.CharacterId);

            var relations = await ctx.Friends.Where(f =>
                f.FriendA == character.Id || f.FriendB == character.Id
            ).ToArrayAsync();

            var friends = new List<FriendListPacket.Friend>();
            
            foreach (var characterFriend in relations)
            {
                var friendId = characterFriend.FriendA == character.Id
                    ? characterFriend.FriendB
                    : characterFriend.FriendA;

                var friend = ctx.Characters.First(c => c.Id == friendId);

                var player = zone.Players.FirstOrDefault(p => p.Id == friend.Id);
                
                friends.Add(new FriendListPacket.Friend
                {
                    IsBestFriend = characterFriend.BestFriend,
                    IsFreeToPlay = friend.FreeToPlay,
                    IsOnline = player != default,
                    PlayerId = player?.Id ?? (ObjectId) (-1),
                    PlayerName = friend.Name,
                    ZoneId = (ZoneId) friend.LastZone,
                    WorldClone = (uint) friend.LastClone,
                    WorldInstance = (ushort) friend.LastInstance
                });
            }

            connection.Send(new FriendListPacket
            {
                Friends = friends.ToArray()
            });
        }

        [PacketHandler]
        public async Task AddFriendResponseHandler(AddFriendResponsePacket packet, IRakConnection connection)
        {
            var session = UchuServer.SessionCache.GetSession(connection.EndPoint);
            var zone = ((WorldUchuServer) UchuServer).Zones.FirstOrDefault(z => (int) z.ZoneId == session.ZoneId);

            if (zone == default)
            {
                Logger.Error($"Invalid ZoneId for {connection}");
                return;
            }

            await using var ctx = new UchuContext();
            
            var character = await ctx.Characters.FirstAsync(c => c.Id == session.CharacterId);
            
            var friend = await ctx.Characters.FirstAsync(c => c.Name == packet.FriendName);
            
            var player = zone.Players.First(p => p.Connection.Equals(connection));
            
            if (friend == default)
            {
                player.Message(new NotifyFriendRequestResponsePacket
                {
                    PlayerName = packet.FriendName,
                    Response = ServerFriendRequestResponse.InvalidName
                });
                
                return;
            }

            var request = await ctx.FriendRequests.Where(f =>
                f.Receiver == character.Id
            ).FirstOrDefaultAsync();

            if (request == default)
            {
                player.Message(new NotifyFriendRequestResponsePacket
                {
                    PlayerName = packet.FriendName,
                    Response = ServerFriendRequestResponse.UnknownError
                });
                
                return;
            }

            var declined = packet.Response == ClientFriendRequestResponse.Declined ||
                           packet.Response == ClientFriendRequestResponse.InviteWindowClosed;

            if (declined)
            {
                ctx.FriendRequests.Remove(request);

                await ctx.SaveChangesAsync();

                player.Message(new NotifyFriendRequestResponsePacket
                {
                    PlayerName = packet.FriendName,
                    Response = ServerFriendRequestResponse.Declined
                });
                
                return;
            }

            var relation = new Friend
            {
                FriendA = request.Sender,
                FriendB = request.Receiver,
                BestFriend = request.BestFriend
            };

            ctx.FriendRequests.Remove(request);

            await ctx.Friends.AddAsync(relation);

            await FriendsListRequestHandler(null, connection);

            player.Message(new NotifyFriendRequestResponsePacket
            {
                PlayerName = packet.FriendName,
                Response = ServerFriendRequestResponse.Accepted
            });
        }

        [PacketHandler]
        public async Task RemoveFriendHandler(RemoveFriendPacket packet, IRakConnection connection)
        {
            var session = UchuServer.SessionCache.GetSession(connection.EndPoint);
            var zone = ((WorldUchuServer) UchuServer).Zones.FirstOrDefault(z => (int) z.ZoneId == session.ZoneId);

            if (zone == default)
            {
                Logger.Error($"Invalid ZoneId for {connection}");
                return;
            }
            
            await using var ctx = new UchuContext();
            
            var character = await ctx.Characters.FirstAsync(c => c.Id == session.CharacterId);
            
            var friend = await ctx.Characters.FirstAsync(c => c.Name == packet.FriendName);
            
            var player = zone.Players.First(p => p.Connection.Equals(connection));
            
            if (friend == default)
            {
                player.Message(new RemoveFriendResponsePacket
                {
                    FriendName = packet.FriendName,
                    Success = false
                });
                
                return;
            }

            var relation = await ctx.Friends.FirstOrDefaultAsync(
                f => f.FriendA == character.Id || f.FriendB == character.Id
            );

            ctx.Friends.Remove(relation);

            await ctx.SaveChangesAsync();
        }

        [PacketHandler]
        public void TeamInviteHandler(TeamInvitePacket packet, IRakConnection connection)
        {
            var session = UchuServer.SessionCache.GetSession(connection.EndPoint);
            var zone = ((WorldUchuServer) UchuServer).Zones.FirstOrDefault(z => (int) z.ZoneId == session.ZoneId);

            if (zone == default)
            {
                Logger.Error($"Invalid ZoneId for {connection}");
                return;
            }

            var player = zone.Players.First(p => p.Connection.Equals(connection));
            var invitedPlayer = zone.Players.First(p => p.Name == packet.InvitedPlayer);

            invitedPlayer.Message(new NotifyTeamInvitePacket
            {
                Sender = player
            });
        }

        [PacketHandler]
        public void TeamInviteResponseHandler(TeamInviteResponse packet, IRakConnection connection)
        {
            var player = UchuServer.FindPlayer(connection);
            var author = player.Zone.Players.First(p => p.Id == packet.InviterObjectId);

            Logger.Information($"{player} responded to {author}'s team invite with Declined: {packet.IsDeclined}");

            author.GetComponent<TeamPlayerComponent>().MessageAddPlayer(player);

            var playerTeam = player.GetComponent<TeamPlayerComponent>();
            playerTeam.MessageAddPlayer(author);
            playerTeam.MessageSetLeader(author);
        }

        [PacketHandler]
        public void CheckWhitelistRequestHandler(CheckWhitelistRequestPacket packet, IRakConnection connection)
        {
            var player = UchuServer.FindPlayer(connection);
            
            if (player == default) return;
            
            Logger.Information(
                $"Checking whitelist for [{packet.ChatMode}:{packet.ChatChannel}]: {packet.PrivateReceiver} | [{packet.ChatMessageLength}] {packet.ChatMessage}"
            );

            var redact = ((WorldUchuServer) UchuServer).Whitelist.CheckPhrase(packet.ChatMessage);
            
            player.Message(new ChatModerationResponsePacket
            {
                RequestAccepted = redact.Length == default,
                ChatChannel = packet.ChatChannel,
                ChatMode = packet.ChatMode,
                UnacceptedRanges = redact
            });
        }
    }
}
