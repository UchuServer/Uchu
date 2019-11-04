using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Microsoft.EntityFrameworkCore;
using RakDotNet;
using Uchu.Core;
using Uchu.World.Collections;
using Uchu.World.Social;

namespace Uchu.World.Handlers
{
    public class WorldInitializationHandler : HandlerGroup
    {
        public XmlSerializer Serializer { get; } = new XmlSerializer(typeof(XmlData));

        [PacketHandler]
        public async Task ValidateClientHandler(SessionInfoPacket packet, IRakConnection connection)
        {
            Logger.Information($"Validating client for world!");
            
            if (!Server.SessionCache.IsKey(packet.SessionKey))
            {
                await connection.CloseAsync();
                Logger.Warning($"{connection} attempted to connect with an invalid session key");
                return;
            }

            Server.SessionCache.RegisterKey(connection.EndPoint, packet.SessionKey);
            var session = Server.SessionCache.GetSession(connection.EndPoint);

            await using var ctx = new UchuContext();
            
            var character = await ctx.Characters.FindAsync(session.CharacterId);
            if (character == null)
            {
                Logger.Warning(
                    $"{connection} attempted to connect to world with an invalid character {session.CharacterId}"
                );

                connection.Send(new DisconnectNotifyPacket
                {
                    DisconnectId = DisconnectId.CharacterCorruption
                });

                return;
            }

            var zoneId = (ZoneId) character.LastZone;
            if (zoneId == ZoneId.VentureExplorerCinematic) zoneId = ZoneId.VentureExplorer;

            var worldServer = (WorldServer) Server;
            var zone = await worldServer.GetZoneAsync(zoneId);
            
            connection.Send(new WorldInfoPacket
            {
                ZoneId = zoneId,
                Checksum = zoneId.GetChecksum(),
                SpawnPosition = zone.ZoneInfo.SpawnPosition
            });
        }

        [PacketHandler]
        public async Task ClientLoadCompleteHandler(ClientLoadCompletePacket packet, IRakConnection connection)
        {
            var session = Server.SessionCache.GetSession(connection.EndPoint);

            await using var ctx = new UchuContext();
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

            Server.SessionCache.SetZone(connection.EndPoint, zoneId);

            // Zone should already be initialized at this point.
            var zone = await ((WorldServer) Server).GetZoneAsync(zoneId);

            // Send the character init XML data for this world to the client
            _sendCharacterXMLDataToClient(character, connection, session);

            var player = await Player.ConstructAsync(character, connection, zone);
            if (character.LandingByRocket)
            {
                character.LandingByRocket = false;
                await ctx.SaveChangesAsync();
            }

            player.Perspective.OnLoaded.AddListener(() =>
            {
                player.Message(new DoneLoadingObjectsMessage {Associate = player});
            });
            
            player.Message(new PlayerReadyMessage {Associate = player});

            var relations = ctx.Friends.Where(f =>
                f.FriendTwoId == character.CharacterId
            ).ToArray();

            foreach (var friend in relations.Where(f => !f.RequestHasBeenSent))
            {
                connection.Send(new NotifyFriendRequestPacket
                {
                    FriendName = (await ctx.Characters.SingleAsync(c => c.CharacterId == friend.FriendTwoId)).Name,
                    IsBestFriendRequest = friend.RequestingBestFriend
                });
            }
        }

        [PacketHandler]
        public async Task PlayerLoadedHandler(PlayerLoadedMessage message, Player player)
        {
            if (player != message.Player)
            {
                Logger.Error($"{player} sent invalid {nameof(PlayerLoadedMessage)} player id: {message.Player}");

                await player.Connection.CloseAsync();
                
                return;
            }

            player.Message(
                new RestoreToPostLoadStatsMessage {Associate = player}
            );
        }

        /// <summary>
        /// Sends the character initialization packet for a character to the current connection
        /// </summary>
        /// <param name="character">The character to generate the initialization data for</param>
        /// <param name="connection">The connection to send the initialization data to</param>
        /// <param name="session">The session cache for the connection</param>
        private async void _sendCharacterXMLDataToClient(Character character, IRakConnection connection, Session session)
        {
            // Get the XML data for this character for the initial character packet
            var xmlData = _generateCharacterXMLData(character);

            await using var ms = new MemoryStream();
            await using var writer = new StreamWriter(ms, Encoding.UTF8);
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

            connection.Send(new DetailedUserInfoPacket {Data = ldf});
        }

        /// <summary>
        /// Generates character initialization data in XML format for a character
        /// </summary>
        /// <remarks>
        /// Generally used by the client as the first character packet when entering a new world.
        /// The generated XML data is based on https://docs.google.com/document/d/1XDh_HcXMjSdaGeniG1dND5CA7jOFXIPA_fxCnjvjaO4/edit#
        /// </remarks>
        /// <param name="character">The character to generate the XML data for</param>
        /// <returns>XmlData conform with the LU Char Data XML Format</returns>
        private XmlData _generateCharacterXMLData(Character character)
        {
            var xmlData = new XmlData
            {
                Inventory = _inventoryNode(character),
                Character = _characterNode(character),
                Level = _levelNode(character),
                Missions = _missionsNode(character),
                Minifigure = _minifigureNode(character),
                Stats = _statsNode(character)
            };

            return xmlData;
        }

        /// <summary>
        /// Generates the inventory node based for a character, containing information about the items and bricks of a player
        /// </summary>
        /// <param name="character">The character to generate an inventory for</param>
        /// <returns>An inventory node for the character</returns>
        private static InventoryNode _inventoryNode(Character character)
        {
            return new InventoryNode
            {
                ItemContainers = new[]
                {
                    _itemContainerNode(character, InventoryType.Items),
                    _itemContainerNode(character, InventoryType.Bricks),
                    _itemContainerNode(character, InventoryType.Models),
                    _itemContainerNode(character, InventoryType.Behaviors)
                }
            };
        }

        /// <summary>
        /// Creates an item container node for a character, can be used in an inventory, for example
        /// </summary>
        /// <remarks>
        /// For inventory type <c>Models</c>, extra information is added
        /// </remarks>
        /// <param name="character">The character to create an item container for</param>
        /// <param name="type">The type of container to create, see remarks for extra info</param>
        /// <returns></returns>
        private static ItemContainerNode _itemContainerNode(Character character, InventoryType type)
        {
            return new ItemContainerNode
            {
                Type = (int)type,
                Items = character.Items.Where(i => i.InventoryType == (int)type).Select(i => new ItemNode
                {
                    Count = (int) i.Count,
                    Slot = i.Slot,
                    Lot = i.LOT,
                    ObjectId = i.InventoryItemId,
                    Equipped = i.IsEquipped ? 1 : 0,
                    Bound = i.IsBound ? 1 : 0,
                    
                    // Only provide extra information for models inventory
                    ExtraInfo = type == InventoryType.Models && i.ExtraInfo != null
                        ? new ExtraInfoNode
                        {
                            ModuleAssemblyInfo =
                                "0:" + LegoDataDictionary.FromString(i.ExtraInfo)["assemblyPartLOTs"]
                        }
                        : null
                }).ToArray()
            };
        }

        /// <summary>
        /// Creates a character node, containing billing info and subscription info
        /// </summary>
        /// <param name="character">The character to create a node from</param>
        /// <returns>The character node created from the character</returns>
        private static CharacterNode _characterNode(Character character)
        {
            return new CharacterNode
            {
                AccountId = character.User.UserId,
                Currency = character.Currency,
                FreeToPlay = character.FreeToPlay ? 1 : 0,
                UniverseScore = character.UniverseScore
            };
        }

        /// <summary>
        /// Creates the level node for a character, containing the level of the player
        /// </summary>
        /// <param name="character">The character to create the level node for</param>
        /// <returns>The level node for the character</returns>
        private static LevelNode _levelNode(Character character)
        {
            return new LevelNode
            {
                Level = character.Level
            };
        }

        /// <summary>
        /// Creates the missions node for a character, containing the completed and active missions separately
        /// </summary>
        /// <param name="character">The character to create a mission node for</param>
        /// <returns>The missions node for the character</returns>
        private static MissionsNode _missionsNode(Character character)
        {
            // Completed and active missions are stored in two separate lists
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
                        Progress = mission.Tasks.OrderBy(t => t.TaskId).Select(t =>
                            new MissionProgressNode {Value = t.Values.Count}
                        ).ToArray()
                    });
                }
            }

            return new MissionsNode
            {
                CompletedMissions = completed.ToArray(),
                CurrentMissions = missions.ToArray()
            };
        }

        /// <summary>
        /// Creates the minifigure node for a character, containing information about hair and mouth styles for example
        /// </summary>
        /// <param name="character"></param>
        /// <returns></returns>
        private static MinifigureNode _minifigureNode(Character character)
        {
            return new MinifigureNode
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
            };
        }

        /// <summary>
        /// Creates the statistics node for a character, containing information about the max / current health, armor and imagination
        /// </summary>
        /// <param name="character">The character to create the statistics node for</param>
        /// <returns>The statistics node for this character</returns>
        private static DestNode _statsNode(Character character)
        {
            return new DestNode
            {
                MaximumArmor = character.MaximumArmor,
                CurrentArmor = character.CurrentArmor,
                MaximumHealth = character.MaximumHealth,
                CurrentHealth = character.CurrentHealth,
                MaximumImagination = character.MaximumImagination,
                CurrentImagination = character.CurrentImagination
            };
        }
    }
}