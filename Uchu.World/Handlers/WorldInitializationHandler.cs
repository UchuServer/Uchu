using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Microsoft.EntityFrameworkCore;
using Uchu.Core;
using Uchu.World.Collections;
using Uchu.World.Social;

namespace Uchu.World.Handlers
{
    public class WorldInitializationHandler : HandlerGroup
    {
        public XmlSerializer Serializer { get; } = new XmlSerializer(typeof(XmlData));
        
        [PacketHandler]
        public async Task ValidateClient(SessionInfoPacket packet, IPEndPoint endPoint)
        {
            if (!Server.SessionCache.IsKey(packet.SessionKey))
            {
                Server.DisconnectClient(endPoint, DisconnectId.InvalidSessionKey);
                Logger.Warning($"{endPoint} attempted to connect with an invalid session key");
                return;
            }
            
            Server.SessionCache.RegisterKey(endPoint, packet.SessionKey);
            
            var session = Server.SessionCache.GetSession(endPoint);

            using (var ctx = new UchuContext())
            {
                var character = await ctx.Characters.FindAsync(session.CharacterId);

                if (character == null)
                {
                    Logger.Warning($"{endPoint} attempted to connect to world with an invalid character {session.CharacterId}");

                    Server.Send(new DisconnectNotifyPacket
                    {
                        DisconnectId = DisconnectId.CharacterCorruption
                    }, endPoint);
                    
                    return;
                }

                var zoneId = (ZoneId) character.LastZone;

                if (zoneId == ZoneId.VentureExplorerCinematic) zoneId = ZoneId.VentureExplorer;
                
                Server.Send(new WorldInfoPacket
                {
                    ZoneId = zoneId,
                    Checksum = Zone.GetChecksum(zoneId),
                    SpawnPosition = (await ((WorldServer) Server).GetZone(zoneId)).ZoneInfo.SpawnPosition
                }, endPoint);
            }
        }

        [PacketHandler]
        public async Task ClientLoadComplete(ClientLoadCompletePacket packet, IPEndPoint endPoint)
        {
            var session = Server.SessionCache.GetSession(endPoint);

            using (var ctx = new UchuContext())
            {
                var character = await ctx.Characters
                .Include(c => c.Items)
                .Include(c => c.User)
                .Include(c => c.Missions)
                .ThenInclude(m => m.Tasks).SingleAsync(c => c.CharacterId == session.CharacterId);

                var zoneId = (ZoneId) character.LastZone;
                
                if (zoneId == ZoneId.VentureExplorerCinematic)
                {
                    zoneId = ZoneId.VentureExplorer;
                    character.LastZone = (int) zoneId;

                    await ctx.SaveChangesAsync();
                }

                Server.SessionCache.SetZone(endPoint, zoneId);

                // Zone should already be initialized at this point.
                var zone = await ((WorldServer) Server).GetZone(zoneId);
                
                var completed = new List<CompletedMissionNode>();
                var missions = new List<MissionNode>();

                foreach (var mission in character.Missions)
                {
                    if (mission.State == (int) MissionState.Completed)
                    {
                        completed.Add(new CompletedMissionNode
                        {
                            CompletionCount = mission.CompletionCount,
                            LastCompletion = mission.LastCompletion,
                            MissionId = mission.MissionId
                        });
                    }
                    else
                    {
                        missions.Add(new MissionNode
                        {
                            MissionId = mission.MissionId,
                            Progress = mission.Tasks.OrderBy(t => t.TaskId)
                                .Select(t => new MissionProgressNode {Value = t.Values.Count}).ToArray()
                        });
                    }
                }

                var xmlData = new XmlData
                {
                    Inventory = new InventoryNode
                    {
                        ItemContainers = new[]
                        {
                            new ItemContainerNode
                            {
                                Type = 0,
                                Items = character.Items.Where(i => i.InventoryType == 0).Select(i => new ItemNode
                                {
                                    Count = (int) i.Count,
                                    Slot = i.Slot,
                                    Lot = i.LOT,
                                    ObjectId = i.InventoryItemId,
                                    Equipped = i.IsEquipped ? 1 : 0,
                                    Bound = i.IsBound ? 1 : 0
                                }).ToArray()
                            },
                            new ItemContainerNode
                            {
                                Type = 2,
                                Items = character.Items.Where(i => i.InventoryType == 2).Select(i => new ItemNode
                                {
                                    Count = (int) i.Count,
                                    Slot = i.Slot,
                                    Lot = i.LOT,
                                    ObjectId = i.InventoryItemId,
                                    Equipped = i.IsEquipped ? 1 : 0,
                                    Bound = i.IsBound ? 1 : 0
                                }).ToArray()
                            },
                            new ItemContainerNode
                            {
                                Type = 5,
                                Items = character.Items.Where(i => i.InventoryType == 5).Select(i => new ItemNode
                                {
                                    Count = (int) i.Count,
                                    Slot = i.Slot,
                                    Lot = i.LOT,
                                    ObjectId = i.InventoryItemId,
                                    Equipped = i.IsEquipped ? 1 : 0,
                                    Bound = i.IsBound ? 1 : 0,
                                    ExtraInfo = i.ExtraInfo != null ? new ExtraInfoNode
                                    {
                                        ModuleAssemblyInfo = "0:" + LegoDataDictionary.FromString(i.ExtraInfo)["assemblyPartLOTs"]
                                    } : null
                                }).ToArray()
                            },
                            new ItemContainerNode
                            {
                                Type = 7,
                                Items = character.Items.Where(i => i.InventoryType == 7).Select(i => new ItemNode
                                {
                                    Count = (int) i.Count,
                                    Slot = i.Slot,
                                    Lot = i.LOT,
                                    ObjectId = i.InventoryItemId,
                                    Equipped = i.IsEquipped ? 1 : 0,
                                    Bound = i.IsBound ? 1 : 0
                                }).ToArray()
                            }
                        }
                    },
                    Character = new CharacterNode
                    {
                        AccountId = character.User.UserId,
                        Currency = character.Currency,
                        FreeToPlay = character.FreeToPlay ? 1 : 0,
                        UniverseScore = character.UniverseScore
                    },
                    Level = new LevelNode
                    {
                        Level = character.Level
                    },
                    Missions = new MissionsNode
                    {
                        CompletedMissions = completed.ToArray(),
                        CurrentMissions = missions.ToArray()
                    },
                    Minifigure = new MinifigureNode
                    {
                        EyebrowStyle = character.EyebrowStyle,
                        EyeStyle = character.EyeStyle,
                        HairColor = character.HairColor,
                        HairStyle = character.HairStyle,
                        PantsColor = character.PantsColor,
                        Lh = character.Lh,
                        MouthStyle = character.MouthStyle,
                        Rh = character.Rh,
                        ShirtColor = character.ShirtColor
                    },
                    Stats = new DestNode
                    {
                        MaximumArmor = character.MaximumArmor,
                        CurrentArmor = character.CurrentArmor,
                        MaximumHealth = character.MaximumHealth,
                        CurrentHealth = character.CurrentHealth,
                        MaximumImagination = character.MaximumImagination,
                        CurrentImagination = character.CurrentImagination
                    }
                };

                var ms = new MemoryStream();
                
                using (var writer = new StreamWriter(ms, Encoding.UTF8))
                {
                    Serializer.Serialize(writer, xmlData);
                }

                var bytes = ms.ToArray();
                var xml = new byte[bytes.Length - 3];

                Buffer.BlockCopy(bytes, 3, xml, 0, bytes.Length - 3);

                var ldf = new LegoDataDictionary
                {
                    ["accountId"] = session.UserId,
                    ["objid", 9] = character.CharacterId,
                    ["name"] = character.Name,
                    ["template"] = 1,
                    ["xmlData"] = xml
                };

                Server.Send(new DetailedUserInfoPacket {Data = ldf}, endPoint);

                var player = Player.Create(character, endPoint, zone);

                if (character.LandingByRocket)
                {
                    character.LandingByRocket = false;
                    character.Rocket = null;

                    await ctx.SaveChangesAsync();
                }

                player.Message(new DoneLoadingObjectsMessage {Associate = player});
                player.Message(new PlayerReadyMessage {Associate = player});

                var relations = ctx.Friends.Where(f =>
                    f.FriendTwoId == character.CharacterId
                ).ToArray();
                
                foreach (var friend in relations.Where(f => !f.RequestHasBeenSent))
                {
                    Server.Send(new NotifyFriendRequestPacket
                    {
                        FriendName = (await ctx.Characters.SingleAsync(c => c.CharacterId == friend.FriendTwoId)).Name,
                        IsBestFriendRequest = friend.RequestingBestFriend
                    }, endPoint);
                }
            }
        }
    }
}