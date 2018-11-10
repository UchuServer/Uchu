using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using RakDotNet;
using Uchu.Core;
using Uchu.Core.Collections;
using Uchu.Core.IO.Compression;

namespace Uchu.World
{
    public class WorldHandler : HandlerGroupBase
    {
        public XmlSerializer Serializer { get; }
        public ReplicaManager ReplicaManager { get; set; } // temp

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
            if (ReplicaManager == null)
                ReplicaManager = Server.CreateReplicaManager();

            var session = Server.SessionCache.GetSession(endpoint);
            var character = await Database.GetCharacterAsync(session.CharacterId);

            var zoneId = (ushort) character.LastZone;

            if (zoneId == 0)
                zoneId = 1000;

            var zone = await Server.ZoneParser.ParseAsync(ZoneParser.Zones[zoneId]);

            var xmlData = new XmlData
            {
                Inventory = new InventoryNode
                {
                    Bags = new[]
                    {
                        new BagNode
                        {
                            Slots = 0,
                            Type = 0
                        }
                    },
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
                    Currency = 0
                },
                Level = new LevelNode
                {
                    Level = 1
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
                    new StatsComponent(),
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

                var bytes = new byte[writer.BaseStream.Length];

                await writer.BaseStream.ReadAsync(bytes, 0, (int) writer.BaseStream.Length);

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

                ReplicaManager.AddConnection(endpoint);
                ReplicaManager.SendConstruction(replica);

                Server.Send(new DoneLoadingObjectsPacket {ObjectId = character.CharacterId}, endpoint);
                Server.Send(new PlayerReadyPacket {ObjectId = character.CharacterId}, endpoint);
            }
        }
    }
}