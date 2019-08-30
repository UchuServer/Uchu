using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Uchu.Core;
using Uchu.World.Social;

namespace Uchu.World.Handlers
{
    public class SocialHandler : HandlerGroup
    {
        [PacketHandler]
        public async Task ParseChatMessageHandler(ParseChatMessage message, Player player)
        {
            if (!message.Message.StartsWith('/'))
                return;
            
            var command = message.Message.Remove(0, 1);

            var response = await ((WorldServer) Server).AdminCommand(command, player);

            Server.Send(new ChatMessagePacket
            {
                Message = $"{response}\0"
            }, player.EndPoint);
        }

        [PacketHandler]
        public async Task AddFriendRequestHandler(AddFriendRequestPacket packet, IPEndPoint endPoint)
        {
            var session = Server.SessionCache.GetSession(endPoint);
            var zone = ((WorldServer) Server).Zones.FirstOrDefault(z => z.ZoneInfo.ZoneId == session.ZoneId);

            if (zone == default)
            {
                Logger.Error($"Invalid ZoneId for {endPoint}");
                return;
            }
            
            var player = zone.Players.First(p => p.EndPoint.Equals(endPoint));

            using (var ctx = new UchuContext())
            {
                var friend = ctx.Characters.FirstOrDefault(c => c.Name == packet.PlayerName);

                if (friend == default)
                {
                    Logger.Information(
                        $"{player.Name} is trying to be friends with a none existent player {packet.PlayerName}"
                    );
                    Server.Send(new NotifyFriendRequestResponsePacket
                    {
                        PlayerName = packet.PlayerName,
                        Response = ServerFriendRequestResponse.InvalidName
                    }, endPoint);
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
                        Server.Send(new NotifyFriendRequestResponsePacket
                        {
                            PlayerName = packet.PlayerName,
                            Response = ServerFriendRequestResponse.AlreadyFriends
                        }, endPoint);
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
                    Server.Send(new NotifyFriendRequestPacket
                    {
                        FriendName = player.Name,
                        IsBestFriendRequest = packet.IsRequestingBestFriend
                    }, friendPlayer.EndPoint);

                    invite.RequestHasBeenSent = true;

                    Logger.Information($"Friend request sent to {friendPlayer} from {player}");
                }

                await ctx.SaveChangesAsync();
            }
        }

        [PacketHandler(RunTask = true)]
        public void FriendsListRequestHandler(GetFriendListPacket packet, IPEndPoint endPoint)
        {
            var session = Server.SessionCache.GetSession(endPoint);
            var zone = ((WorldServer) Server).Zones.FirstOrDefault(z => z.ZoneInfo.ZoneId == session.ZoneId);
            
            if (zone == default)
            {
                Logger.Error($"Invalid ZoneId for {endPoint}");
                return;
            }

            using (var ctx = new UchuContext())
            {
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
                        PlayerId = player == default ? -1 : player.ObjectId,
                        PlayerName = friend.Name,
                        ZoneId = (ZoneId) friend.LastZone,
                        WorldClone = (uint) friend.LastClone,
                        WorldInstance = (ushort) friend.LastInstance
                    });
                }

                Server.Send(new FriendListPacket
                {
                    Friends = friends.ToArray()
                }, endPoint);
            }
        }

        [PacketHandler]
        public async Task AddFriendResponseHandler(AddFriendResponsePacket packet, IPEndPoint endPoint)
        {
            var session = Server.SessionCache.GetSession(endPoint);
            var zone = ((WorldServer) Server).Zones.FirstOrDefault(z => z.ZoneInfo.ZoneId == session.ZoneId);
            
            if (zone == default)
            {
                Logger.Error($"Invalid ZoneId for {endPoint}");
                return;
            }
            
            using (var ctx = new UchuContext())
            {
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
                        Server.Send(new NotifyFriendRequestResponsePacket
                        {
                            IsBestFriend = characterFriend.IsBestFriend,
                            IsFreeToPlay = friendCharacter.FreeToPlay,
                            IsPlayerOnline = player != default,
                            PlayerId = player == default ? -1 : player.ObjectId,
                            PlayerName = friendCharacter.Name,
                            ZoneId = (ZoneId) friendCharacter.LastZone,
                            WorldClone = (uint) friendCharacter.LastClone,
                            WorldInstance = (ushort) friendCharacter.LastInstance,
                            Response = packet.Response == ClientFriendRequestResponse.Accepted
                                ? ServerFriendRequestResponse.Accepted
                                : ServerFriendRequestResponse.Declined
                        }, senderPlayer.EndPoint);

                        FriendsListRequestHandler(null, senderPlayer.EndPoint);
                    }

                    FriendsListRequestHandler(null, endPoint);
                    
                    await ctx.SaveChangesAsync();

                    if (characterFriend.IsDeclined)
                        await RemoveFriendHandler(new RemoveFriendPacket {FriendName = packet.FriendName}, endPoint);
                }
            }
        }

        [PacketHandler]
        public async Task RemoveFriendHandler(RemoveFriendPacket packet, IPEndPoint endPoint)
        {
            var session = Server.SessionCache.GetSession(endPoint);
            var zone = ((WorldServer) Server).Zones.FirstOrDefault(z => z.ZoneInfo.ZoneId == session.ZoneId);
            
            if (zone == default)
            {
                Logger.Error($"Invalid ZoneId for {endPoint}");
                return;
            }
            
            using (var ctx = new UchuContext())
            {
                var character = ctx.Characters.First(c => c.CharacterId == session.CharacterId);
                
                var relations = ctx.Friends.Where(f =>
                    f.FriendId == character.CharacterId || f.FriendTwoId == character.CharacterId &&
                    f.FriendOne.Name == packet.FriendName || f.FriendTwo.Name == packet.FriendName
                ).ToArray();

                foreach (var friend in relations)
                {
                    Server.Send(new RemoveFriendResponsePacket
                    {
                        FriendName = packet.FriendName,
                        Success = true
                    }, endPoint);
                    
                    ctx.Friends.Remove(friend);
                }

                await ctx.SaveChangesAsync();
            }
        }

        [PacketHandler(RunTask = true)]
        public void TeamInviteHandler(TeamInvitePacket packet, IPEndPoint endPoint)
        {
            var session = Server.SessionCache.GetSession(endPoint);
            var zone = ((WorldServer) Server).Zones.FirstOrDefault(z => z.ZoneInfo.ZoneId == session.ZoneId);

            if (zone == default)
            {
                Logger.Error($"Invalid ZoneId for {endPoint}");
                return;
            }
            
            var player = zone.Players.First(p => p.EndPoint.Equals(endPoint));
            var invitedPlayer = zone.Players.First(p => p.Name == packet.InvitedPlayer);

            Server.Send(new NotifyTeamInvitePacket
            {
                Sender = player
            }, invitedPlayer.EndPoint);
        }

        [PacketHandler(RunTask = true)]
        public void TeamInviteResponseHandler(TeamInviteResponse packet, IPEndPoint endPoint)
        {
            var session = Server.SessionCache.GetSession(endPoint);
            var zone = ((WorldServer) Server).Zones.FirstOrDefault(z => z.ZoneInfo.ZoneId == session.ZoneId);

            if (zone == default)
            {
                Logger.Error($"Invalid ZoneId for {endPoint}");
                return;
            }
            
            var player = zone.Players.First(p => p.EndPoint.Equals(endPoint));
            var author = zone.Players.First(p => p.ObjectId == packet.InviterObjectId);
            
            Logger.Information($"{player} responded to {author}'s team invite with Declined: {packet.IsDeclined}");

            author.GetComponent<TeamPlayer>().MessageAddPlayer(player);
            
            var playerTeam = player.GetComponent<TeamPlayer>();
            playerTeam.MessageAddPlayer(author);
            playerTeam.MessageSetLeader(author);
        }
    }
}