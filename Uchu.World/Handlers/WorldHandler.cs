using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
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
            var character = await Database.GetCharacterAsync(session.CharacterId);
            var zoneId = (ushort) character.LastZone;

            if (zoneId == 0)
                zoneId = 1000;

            Server.SessionCache.SetZone(endpoint, (ZoneId) zoneId);

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
                    zoneId = 1000;

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
                            Progress = mission.Tasks.Select(t => new MissionProgressNode {Value = t.Values.Count}).ToArray()
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
                                Type = 0
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
                        CurrentImagination = character.MaximumImagination
                    }
                };

                var replica = new ReplicaPacket
                {
                    ObjectId = character.CharacterId,
                    LOT = 1,
                    Name = character.Name,
                    Created = 0,
                    Components = new IReplicaComponent[]
                    {
                        new ControllablePhysicsComponent
                        {
                            HasPosition = true,
                            Position = zone.SpawnPosition,
                            Rotation = zone.SpawnRotation
                        },
                        new DestructibleComponent(),
                        new StatsComponent
                        {
                            HasStats = true,
                            CurrentArmor = (uint) character.CurrentArmor,
                            MaxArmor = (uint) character.MaximumArmor,
                            CurrentHealth = (uint) character.CurrentHealth,
                            MaxHealth = (uint) character.MaximumHealth,
                            CurrentImagination = (uint) character.CurrentImagination,
                            MaxImagination = (uint) character.CurrentImagination
                        },
                        new CharacterComponent
                        {
                            Level = (uint) character.Level,
                            Character = character
                        },
                        new InventoryComponent
                        {
                            Items = character.Items.ToArray()
                        },
                        new ScriptComponent(),
                        new SkillComponent(),
                        new RenderComponent(),
                        new Component107()
                    }
                };

                using (var ms = new MemoryStream())
                using (var writer = new StreamWriter(ms, Encoding.UTF8))
                {
                    Serializer.Serialize(writer, xmlData);

                    var bytes = ms.ToArray();

                    var ldf = new LegoDataDictionary
                    {
                        ["accountId"] = session.UserId,
                        ["objid", 9] = character.CharacterId,
                        ["name"] = character.Name,
                        ["template"] = 1,
                        ["xmlData"] = bytes
                    };

                    var temp = new BitStream();

                    temp.WriteSerializable(ldf);

                    var length = temp.BaseBuffer.Length;

                    var compressed = await Zlib.CompressBytesAsync(temp.BaseBuffer, CompressionLevel.Fastest);

                    Server.Send(new DetailedUserInfoPacket
                    {
                        UncompressedSize = (uint) length,
                        Data = compressed
                    }, endpoint);

                    world.ReplicaManager.AddConnection(endpoint);
                    world.SpawnObject(replica);

                    Server.Send(new DoneLoadingObjectsPacket {ObjectId = character.CharacterId}, endpoint);
                    Server.Send(new PlayerReadyPacket {ObjectId = character.CharacterId}, endpoint);
                }
            }
        }
    }
}