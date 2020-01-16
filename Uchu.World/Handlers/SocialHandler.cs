using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RakDotNet;
using Uchu.Core;
using Uchu.World.Social;

namespace Uchu.World.Handlers
{
    public class SocialHandler : HandlerGroup
    {
        //
        // TODO: Move all of this to a component
        //
        
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
            
            var character = await ctx.Characters.Include(c => c.User)
                .FirstAsync(c => c.CharacterId == player.ObjectId);
            
            var response = await Server.HandleCommandAsync(
                message.Message,
                player,
                (GameMasterLevel) character.User.GameMasterLevel
            );

            if (!string.IsNullOrWhiteSpace(response))
            {
                player.SendChatMessage(response);
            }
            else
            {
                if (((WorldServer) Server).Whitelist.CheckPhrase(message.Message).Any()) return;
                
                foreach (var zonePlayer in player.Zone.Players)
                {
                    zonePlayer.SendChatMessage(message.Message, PlayerChatChannel.Normal, player);
                }
            }
        }

        [PacketHandler]
        public async Task AddFriendRequestHandler(AddFriendRequestPacket packet, IRakConnection connection)
        {
            var session = Server.SessionCache.GetSession(connection.EndPoint);
            var zone = ((WorldServer) Server).Zones.FirstOrDefault(z => z.ZoneInfo.LuzFile.WorldId == session.ZoneId);

            if (zone == default)
            {
                Logger.Error($"Invalid ZoneId for {connection}");
                return;
            }

            var player = zone.Players.First(p => p.Connection.Equals(connection));

            await using var ctx = new UchuContext();
            
            var friend = ctx.Characters.FirstOrDefault(c => c.Name == packet.PlayerName);

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

            var character = ctx.Characters.First(c => c.CharacterId == session.CharacterId);

            var relations = ctx.Friends.Where(f =>
                f.FriendId == character.CharacterId || f.FriendTwoId == character.CharacterId
            ).ToArray();

            foreach (var relation in relations)
            {
                if (!relation.IsAccepted || relation.IsDeclined) continue;
                if (relation.FriendId == friend.CharacterId || relation.FriendTwoId == friend.CharacterId)
                {
                    if (!relation.IsBestFriend && packet.IsRequestingBestFriend) continue;
                        
                    player.Message(new NotifyFriendRequestResponsePacket
                    {
                        PlayerName = packet.PlayerName,
                        Response = ServerFriendRequestResponse.AlreadyFriends
                    });
                        
                    return;
                }
            }

            Logger.Information($"Sending friend request from {player.Name} to {packet.PlayerName}!");

            var invite = relations.FirstOrDefault(relation =>
                relation.FriendId == friend.CharacterId || relation.FriendTwoId == friend.CharacterId
            );

            if (invite == default)
            {
                invite = new Friend
                {
                    FriendId = character.CharacterId,
                    FriendTwoId = friend.CharacterId
                };

                ctx.Friends.Add(invite);
            }
            else
            {
                invite.RequestingBestFriend = true;
            }

            // Friend one is sender;
            invite.FriendId = character.CharacterId;
            invite.FriendTwoId = friend.CharacterId;

            var friendPlayer = zone.Players.FirstOrDefault(p => p.ObjectId == invite.FriendTwoId);

            Logger.Information($"{friendPlayer} is getting a friend request from {player.Name}!");

            invite.RequestHasBeenSent = false;

            if (!ReferenceEquals(friendPlayer, null))
            {
                player.Message(new NotifyFriendRequestPacket
                {
                    FriendName = player.Name,
                    IsBestFriendRequest = packet.IsRequestingBestFriend
                });

                invite.RequestHasBeenSent = true;

                Logger.Information($"Friend request sent to {friendPlayer} from {player}");
            }

            await ctx.SaveChangesAsync();
        }

        [PacketHandler]
        public void FriendsListRequestHandler(GetFriendListPacket packet,IRakConnection connection)
        {
            var session = Server.SessionCache.GetSession(connection.EndPoint);
            var zone = ((WorldServer) Server).Zones.FirstOrDefault(z => (int) z.ZoneId == session.ZoneId);

            if (zone == default)
            {
                Logger.Error($"Invalid ZoneId for {connection}");
                return;
            }

            using var ctx = new UchuContext();
            var character = ctx.Characters.First(c => c.CharacterId == session.CharacterId);

            var relations = ctx.Friends.Where(f =>
                f.FriendId == character.CharacterId || f.FriendTwoId == character.CharacterId
            ).ToArray();

            var friends = new List<FriendListPacket.Friend>();
            foreach (var characterFriend in relations)
            {
                if (!characterFriend.IsAccepted) continue;

                var friendId = characterFriend.FriendTwoId == character.CharacterId
                    ? characterFriend.FriendId
                    : characterFriend.FriendTwoId;

                var friend = ctx.Characters.First(c => c.CharacterId == friendId);

                var player = zone.Players.FirstOrDefault(p => p.ObjectId == friend.CharacterId);
                friends.Add(new FriendListPacket.Friend
                {
                    IsBestFriend = characterFriend.IsBestFriend,
                    IsFreeToPlay = friend.FreeToPlay,
                    IsOnline = player != default,
                    PlayerId = player?.ObjectId ?? -1,
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
            var session = Server.SessionCache.GetSession(connection.EndPoint);
            var zone = ((WorldServer) Server).Zones.FirstOrDefault(z => (int) z.ZoneId == session.ZoneId);

            if (zone == default)
            {
                Logger.Error($"Invalid ZoneId for {connection}");
                return;
            }

            await using var ctx = new UchuContext();
            var thisCharacter = ctx.Characters.First(c => c.CharacterId == session.CharacterId);
            var friendCharacter = ctx.Characters.First(c => c.Name == packet.FriendName);

            var relations = ctx.Friends.Where(f =>
                f.FriendTwoId == thisCharacter.CharacterId
            ).ToArray();

            foreach (var characterFriend in relations.Where(c => !c.IsAccepted))
            {
                characterFriend.RequestHasBeenSent = true;

                var player = zone.Players.FirstOrDefault(p => p.ObjectId == friendCharacter.CharacterId);
                switch (packet.Response)
                {
                    case ClientFriendRequestResponse.Accepted:
                        characterFriend.IsAccepted = true;
                        characterFriend.IsBestFriend = characterFriend.RequestingBestFriend;
                        break;
                    case ClientFriendRequestResponse.Declined:
                        characterFriend.IsDeclined = true;
                        break;
                    case ClientFriendRequestResponse.InviteWindowClosed:
                        return;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                var senderPlayer = zone.Players.FirstOrDefault(p => p.Name == packet.FriendName);

                if (senderPlayer != default)
                {
                    senderPlayer.Connection.Send(new NotifyFriendRequestResponsePacket
                    {
                        IsBestFriend = characterFriend.IsBestFriend,
                        IsFreeToPlay = friendCharacter.FreeToPlay,
                        IsPlayerOnline = player != default,
                        PlayerId = player?.ObjectId ?? -1,
                        PlayerName = friendCharacter.Name,
                        ZoneId = (ZoneId) friendCharacter.LastZone,
                        WorldClone = (uint) friendCharacter.LastClone,
                        WorldInstance = (ushort) friendCharacter.LastInstance,
                        Response = packet.Response == ClientFriendRequestResponse.Accepted
                            ? ServerFriendRequestResponse.Accepted
                            : ServerFriendRequestResponse.Declined
                    });

                    FriendsListRequestHandler(null, senderPlayer.Connection);
                }

                FriendsListRequestHandler(null, connection);

                await ctx.SaveChangesAsync();

                if (characterFriend.IsDeclined)
                    await RemoveFriendHandler(new RemoveFriendPacket {FriendName = packet.FriendName}, connection);
            }
        }

        [PacketHandler]
        public async Task RemoveFriendHandler(RemoveFriendPacket packet, IRakConnection connection)
        {
            var session = Server.SessionCache.GetSession(connection.EndPoint);
            var zone = ((WorldServer) Server).Zones.FirstOrDefault(z => (int) z.ZoneId == session.ZoneId);

            if (zone == default)
            {
                Logger.Error($"Invalid ZoneId for {connection}");
                return;
            }

            await using var ctx = new UchuContext();
            var character = ctx.Characters.First(c => c.CharacterId == session.CharacterId);

            var relations = ctx.Friends.Where(f =>
                f.FriendId == character.CharacterId || f.FriendTwoId == character.CharacterId &&
                f.FriendOne.Name == packet.FriendName || f.FriendTwo.Name == packet.FriendName
            ).ToArray();

            foreach (var friend in relations)
            {
                connection.Send(new RemoveFriendResponsePacket
                {
                    FriendName = packet.FriendName,
                    Success = true
                });

                ctx.Friends.Remove(friend);
            }

            await ctx.SaveChangesAsync();
        }

        [PacketHandler]
        public void TeamInviteHandler(TeamInvitePacket packet, IRakConnection connection)
        {
            var session = Server.SessionCache.GetSession(connection.EndPoint);
            var zone = ((WorldServer) Server).Zones.FirstOrDefault(z => (int) z.ZoneId == session.ZoneId);

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
        public void TeamInviteResponseHandler(TeamInviteResponse packet, IPEndPoint endPoint)
        {
            var session = Server.SessionCache.GetSession(endPoint);
            var zone = ((WorldServer) Server).Zones.FirstOrDefault(z => (int) z.ZoneId == session.ZoneId);

            if (zone == default)
            {
                Logger.Error($"Invalid ZoneId for {endPoint}");
                return;
            }

            var player = zone.Players.First(p => p.Connection.EndPoint.Equals(endPoint));
            var author = zone.Players.First(p => p.ObjectId == packet.InviterObjectId);

            Logger.Information($"{player} responded to {author}'s team invite with Declined: {packet.IsDeclined}");

            author.GetComponent<TeamPlayerComponent>().MessageAddPlayer(player);

            var playerTeam = player.GetComponent<TeamPlayerComponent>();
            playerTeam.MessageAddPlayer(author);
            playerTeam.MessageSetLeader(author);
        }

        [PacketHandler]
        public void CheckWhitelistRequestHandler(CheckWhitelistRequestPacket packet, IRakConnection connection)
        {
            var player = Server.FindPlayer(connection);
            
            if (player == default) return;
            
            Logger.Information(
                $"Checking whitelist for [{packet.ChatMode}:{packet.ChatChannel}]: {packet.PrivateReceiver} | [{packet.ChatMessageLength}] {packet.ChatMessage}"
            );

            var redact = ((WorldServer) Server).Whitelist.CheckPhrase(packet.ChatMessage);
            
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