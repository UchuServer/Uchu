using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Xml.Serialization;
using Microsoft.EntityFrameworkCore;
using RakDotNet;
using ServiceStack;
using Uchu.Core;
using Uchu.Core.Collections;
using Uchu.Core.IO.Compression;

namespace Uchu.World
{
    public class WorldHandler : HandlerGroupBase
    {
        public XmlSerializer Serializer { get; }

        public WorldHandler()
        {
            Serializer = new XmlSerializer(typeof(XmlData));
        }

        [PacketHandler]
        public async Task SessionInfo(SessionInfoPacket packet, IPEndPoint endpoint)
        {
            var session = Server.SessionCache.GetSession(endpoint);

            using (var ctx = new UchuContext())
            {
                var character = await ctx.Characters.FindAsync(session.CharacterId);

                var zoneId = (ushort) character.LastZone;

                if (zoneId == 0)
                    zoneId = 1000;

                var zone = await Server.ZoneParser.ParseAsync(ZoneParser.Zones[zoneId]);

                Server.Send(new WorldInfoPacket
                {
                    ZoneId = (ZoneId) zoneId,
                    Instance = 0,
                    Clone = 0,
                    Checksum = Utils.GetChecksum((ZoneId) zoneId),
                    Position = zone.SpawnPosition
                }, endpoint);
            }
        }

        [PacketHandler(RunTask = true)]
        public void PositionUpdate(PositionUpdatePacket packet, IPEndPoint endpoint)
        {
            var session = Server.SessionCache.GetSession(endpoint);
            var world = Server.Worlds[(ZoneId) session.ZoneId];
            var charObj = world.GetObject(session.CharacterId);
            var comp = (ControllablePhysicsComponent) charObj.Components.First(c => c is ControllablePhysicsComponent);

            comp.HasPosition = true;
            comp.Position = packet.Position;
            comp.Rotation = packet.Rotation;
            comp.IsOnGround = packet.IsOnGround;
            comp.NegativeAngularVelocity = packet.NegativeAngularVelocity;
            comp.Velocity = packet.Velocity;
            comp.AngularVelocity = packet.AngularVelocity;
            comp.PlatformObjectId = packet.PlatformObjectId;
            comp.PlatformPosition = packet.PlatformPosition;

            world.UpdateObject(charObj);
        }

        [PacketHandler]
        public async Task LoadComplete(ClientLoadCompletePacket packet, IPEndPoint endpoint)
        {
            var session = Server.SessionCache.GetSession(endpoint);

            using (var ctx = new UchuContext())
            {
                var character = await ctx.Characters.Include(c => c.Items).Include(c => c.User).Include(c => c.Missions)
                    .ThenInclude(m => m.Tasks).SingleAsync(c => c.CharacterId == session.CharacterId);

                var zoneId = (ushort) character.LastZone;

                if (zoneId == 0)
                {
                    zoneId = 1000;
                    character.LastZone = 1000;

                    await ctx.SaveChangesAsync();
                }

                Server.SessionCache.SetZone(endpoint, (ZoneId) zoneId);

                var zone = await Server.ZoneParser.ParseAsync(ZoneParser.Zones[zoneId]);

                if (!Server.Worlds.ContainsKey(packet.ZoneId))
                {
                    var wrld = new Core.World(Server);

                    await wrld.InitializeAsync(zone);

                    Server.Worlds[packet.ZoneId] = wrld;
                }

                var world = Server.Worlds[packet.ZoneId];

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
                                    LOT = i.LOT,
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
                                    LOT = i.LOT,
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
                                    LOT = i.LOT,
                                    ObjectId = i.InventoryItemId,
                                    Equipped = i.IsEquipped ? 1 : 0,
                                    Bound = i.IsBound ? 1 : 0,
                                    ExtraInfo = i.ExtraInfo != null ? new ExtraInfoNode
                                    {
                                        ModuleAssemblyInfo = "0:" + LegoDataDictionary.FromString(i.ExtraInfo)["assemblyPartLOTs"].ToString()
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
                                    LOT = i.LOT,
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

                using (var ms = new MemoryStream())
                using (var writer = new StreamWriter(ms, Encoding.UTF8))
                {
                    Serializer.Serialize(writer, xmlData);

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

                    /*foreach (var b in xml)
                    {
                        Console.Write((char) b);
                    }

                    Console.WriteLine();*/

                    Server.Send(new DetailedUserInfoPacket {LDF = ldf}, endpoint);

                    world.SpawnPlayer(character, endpoint);

                    if (character.LandingByRocket)
                    {
                        character.LandingByRocket = false;
                        character.Rocket = null;

                        await ctx.SaveChangesAsync();
                    }

                    Server.Send(new DoneLoadingObjectsMessage {ObjectId = character.CharacterId}, endpoint);
                    Server.Send(new PlayerReadyMessage {ObjectId = character.CharacterId}, endpoint);
                }
            }
        }
    }
}