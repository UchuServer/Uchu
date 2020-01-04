using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using InfectedRose.Lvl;
using Microsoft.EntityFrameworkCore;
using RakDotNet;
using Uchu.Core;
using Uchu.Core.Client;
using Uchu.World.Social;

namespace Uchu.World.Handlers
{
    public class WorldInitializationHandler : HandlerGroup
    {
        /// <summary>
        /// XML Serializer used for the character data init packet
        /// </summary>
        private XmlSerializer Serializer { get; } = new XmlSerializer(typeof(XmlData));

        /// <summary>
        /// Packet handler for a client request to join a world
        /// </summary>
        /// <remarks>
        /// Handles the request by checking if the provided character exists and then sets up a world info packet.
        /// If the request was invalid, a disconnect packet is sent.
        /// </remarks>
        /// <param name="packet">The request packet</param>
        /// <param name="connection">The connection with the client</param>
        [PacketHandler]
        public async Task ValidateClientHandler(SessionInfoPacket packet, IRakConnection connection)
        {
            Logger.Information($"{connection.EndPoint}'s validating client for world!");

            if (!Server.SessionCache.IsKey(packet.SessionKey))
            {
                await connection.CloseAsync();
                Logger.Warning($"{connection} attempted to connect with an invalid session key");
                return;
            }

            Server.SessionCache.RegisterKey(connection.EndPoint, packet.SessionKey);
            var session = Server.SessionCache.GetSession(connection.EndPoint);

            await using var ctx = new UchuContext();

            // Try to find the player, disconnect if the player is invalid
            var character = await ctx.Characters.FindAsync(session.CharacterId);
            if (character == null)
            {
                Logger.Warning(
                    $"{connection} attempted to connect to world with an invalid character {session.CharacterId}"
                );

                connection.Send(new DisconnectNotifyPacket
                {
                    DisconnectId = DisconnectId.CharacterNotFound
                });

                return;
            }

            // Initialize zone for player
            var zoneId = (ZoneId) character.LastZone;
            if (zoneId == ZoneId.VentureExplorerCinematic) zoneId = ZoneId.VentureExplorer;

            var worldServer = (WorldServer) Server;
            var zone = await worldServer.GetZoneAsync(zoneId);

            connection.Send(new WorldInfoPacket
            {
                ZoneId = zoneId,
                Checksum = zone.Checksum,
                SpawnPosition = zone.ZoneInfo.LuzFile.SpawnPoint
            });
        }

        /// <summary>
        /// Packet handler for when a client has finished loading by sending character info and world info to the client
        /// </summary>
        /// <remarks>
        /// This packet is sent when the client has finished loading the information packet.
        /// Sends the character init packet, constructs the player and sends friend requests that haven't been sent yet
        /// </remarks>
        /// <param name="packet">The request packet</param>
        /// <param name="connection">The client connection</param>
        [PacketHandler]
        public async Task ClientLoadCompleteHandler(ClientLoadCompletePacket packet, IRakConnection connection)
        {
            Logger.Information($"{connection.EndPoint}'s client load completed...");

            var session = Server.SessionCache.GetSession(connection.EndPoint);

            await using var ctx = new UchuContext();
            var character = await ctx.Characters
                .Include(c => c.Items)
                .Include(c => c.User)
                .Include(c => c.Missions)
                .ThenInclude(m => m.Tasks).ThenInclude(m => m.Values)
                .SingleAsync(c => c.CharacterId == session.CharacterId);

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
            await SendCharacterXmlDataToClient(character, connection, session);

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

        /// <summary>
        /// Packet handler for a PlayerLoadedMessage
        /// </summary>
        /// <remarks>
        /// This packet is sent when the DetailedPlayerInfoPacket has been parsed
        /// </remarks>
        /// <param name="message">The client message</param>
        /// <param name="player">The player of the client</param>
        [PacketHandler]
        public async Task PlayerLoadedHandler(PlayerLoadedMessage message, Player player)
        {
            Logger.Information($"{player} loaded...");

            if (player != message.Player)
            {
                Logger.Error($"{player} sent invalid {nameof(PlayerLoadedMessage)} player id: {message.Player}");

                await player.Connection.CloseAsync();

                return;
            }

            player.Message(
                new RestoreToPostLoadStatsMessage {Associate = player}
            );

            await player.OnWorldLoad.InvokeAsync();
        }

        /// <summary>
        /// Sends the character initialization packet for a character to the current connection
        /// </summary>
        /// <param name="character">The character to generate the initialization data for</param>
        /// <param name="connection">The connection to send the initialization data to</param>
        /// <param name="session">The session cache for the connection</param>
        private async Task SendCharacterXmlDataToClient(Character character, IRakConnection connection, Session session)
        {
            // Get the XML data for this character for the initial character packet
            var xmlData = GenerateCharacterXMLData(character);

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
        private XmlData GenerateCharacterXMLData(Character character)
        {
            var xmlData = new XmlData
            {
                Inventory = InventoryNode(character),
                Character = CharacterNode(character),
                Level = LevelNode(character),
                Flags = FlagNodes(character),
                Missions = MissionsNode(character),
                Minifigure = MinifigureNode(character),
                Stats = StatsNode(character)
            };

            return xmlData;
        }

        /// <summary>
        /// Generates the inventory node based for a character, containing information about the items and bricks of a player
        /// </summary>
        /// <param name="character">The character to generate an inventory for</param>
        /// <returns>An inventory node for the character</returns>
        private static InventoryNode InventoryNode(Character character)
        {
            return new InventoryNode
            {
                ItemContainers = new[]
                {
                    ItemContainerNode(character, InventoryType.Items),
                    ItemContainerNode(character, InventoryType.Bricks),
                    ItemContainerNode(character, InventoryType.Models),
                    ItemContainerNode(character, InventoryType.Behaviors)
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
        private static ItemContainerNode ItemContainerNode(Character character, InventoryType type)
        {
            return new ItemContainerNode
            {
                Type = (int) type,
                Items = character.Items.Where(i => i.InventoryType == (int) type).Select(i => new ItemNode
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
        private static CharacterNode CharacterNode(Character character)
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
        private static LevelNode LevelNode(Character character)
        {
            return new LevelNode
            {
                Level = character.Level
            };
        }

        /// <summary>
        /// Creates the flag nodes from a character, containing all the flags a player has obtained
        /// </summary>
        /// <param name="character">The character to create the flag nodes for</param>
        /// <returns>An array of flag nodes</returns>
        private static FlagNode[] FlagNodes(Character character)
        {
            var flags = new Dictionary<int, FlagNode>();
            using var cdContext = new CdClientContext();

            // Keep a list of all tasks ids that belong to flag tasks
            var flagTaskIds = cdContext.MissionTasksTable
                .Where(t => t.TaskType == (int) MissionTaskType.Flag)
                .Select(t => t.Uid);

            // Get all the mission task values that correspond to flag values
            var flagValues = character.Missions
                .SelectMany(m => m.Tasks
                    .Where(t => flagTaskIds.Contains(t.TaskId))
                    .SelectMany(t => t.ValueArray()));

            // The flags are stored as one long list of bits by separating them in unsigned longs
            foreach (var value in flagValues)
            {
                // Find the long this flag belongs to
                var index = (int) Math.Floor(value / 64);
                ulong shiftedValue = 1;
                shiftedValue <<= (int) value % 64;

                if (flags.TryAdd(index, new FlagNode()))
                {
                    flags[index].Flag = shiftedValue;
                    flags[index].FlagId = index;
                }
                else
                {
                    flags[index].Flag |= shiftedValue;
                }
            }

            return flags.Values.OrderBy(f => f.FlagId).ToArray();
        }

        /// <summary>
        /// Creates the missions node for a character, containing the completed and active missions separately
        /// </summary>
        /// <param name="character">The character to create a mission node for</param>
        /// <returns>The missions node for the character</returns>
        private static MissionsNode MissionsNode(Character character)
        {
            // Completed and active missions are stored in two separate lists
            var completed = new List<CompletedMissionNode>();
            var missions = new List<MissionNode>();

            // For all missions split them into active and completed missions
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
                        Progress = ProgressArrayForMission(mission),
                        Unknown = mission.MissionId == 1732 ? 751 : -1
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
        /// Gets all the progress nodes for a mission
        /// </summary>
        /// <remarks>
        /// If a mission task is a collectible, this will also add a separate XML element for each collectible
        /// </remarks>
        /// <param name="mission">The mission to create progress nodes for</param>
        /// <returns>All the progress nodes for a mission</returns>
        private static MissionProgressNode[] ProgressArrayForMission(Mission mission)
        {
            return mission.Tasks.OrderBy(task => task.TaskId).Select(task =>
                {
                    // Return the mission task progress as list as there might be more nodes for this task
                    var progressNodes = new List<MissionProgressNode>
                        {new MissionProgressNode {Value = task.Values.Count}};

                    using var cdClient = new CdClientContext();
                    var cdTask = cdClient.MissionTasksTable.First(t => t.Uid == task.TaskId);

                    // If the task type is collectible, also send all collectible ids
                    if (cdTask.TaskType != null && ((MissionTaskType) cdTask.TaskType) == MissionTaskType.Collect)
                    {
                        progressNodes.AddRange(task.ValueArray()
                            .Select(value => new MissionProgressNode {Value = value}));
                    }

                    return progressNodes;
                }
            ).SelectMany(pn => pn).ToArray();
        }

        /// <summary>
        /// Creates the minifigure node for a character, containing information about hair and mouth styles for example
        /// </summary>
        /// <param name="character"></param>
        /// <returns></returns>
        private static MinifigureNode MinifigureNode(Character character)
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
        private static DestNode StatsNode(Character character)
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