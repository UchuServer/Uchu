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
using Uchu.World.Systems.Missions;

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

            var verified = await UchuServer.ValidateUserAsync(connection, packet.Username, packet.SessionKey);
            
            if (!verified) return;
            
            var session = UchuServer.SessionCache.GetSession(connection.EndPoint);

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
            if (zoneId == 0) zoneId = 1000;

            var worldServer = (WorldUchuServer) UchuServer;
            var zone = await worldServer.GetZoneAsync(zoneId);

            UchuServer.SessionCache.SetZone(connection.EndPoint, zoneId);

            connection.Send(new WorldInfoPacket
            {
                ZoneId = zoneId,
                Checksum = zone.Checksum,
                SpawnPosition = zone.SpawnPosition
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
            
            var session = UchuServer.SessionCache.GetSession(connection.EndPoint);

            // Zone should already be initialized at this point.
            Logger.Information("[55%] Getting zone from worldserver.");
            var zone = await ((WorldUchuServer) UchuServer).GetZoneAsync((ZoneId)session.ZoneId);
            
            Logger.Information("[55%] Constructing player.");
            var player = await Player.Instantiate(connection, zone, (int)session.CharacterId);

            Logger.Information("[55%] Setting session zone.");
            UchuServer.SessionCache.SetZone(connection.EndPoint, player.LastZone);

            // Send the character init XML data for this world to the client
            Logger.Information("[55%] Sending XML client info.");
            var character = player.GetComponent<CharacterComponent>();
            await SendCharacterXmlDataToClient(player, connection, session);

            Logger.Information("[55%] Checking rocket landing conditions.");
            if (character.LandingByRocket)
            {
                Logger.Information("[55%] Player landed by rocket, saving changes.");
                character.LandingByRocket = false;
            }

            Logger.Information("[55%] Player is ready to join world.");
            player.Message(new PlayerReadyMessage {Associate = player});
            player.Message(new PlayerReadyMessage { Associate = player.Zone.ZoneControlObject });

            Logger.Information("[55%] Server is done loading object.");
            player.Message(new DoneLoadingObjectsMessage {Associate = player});
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
            
            player.OnWorldLoad.Clear();
        }

        /// <summary>
        /// Sends the character initialization packet for a character to the current connection
        /// </summary>
        /// <param name="player">The character to generate the initialization data for</param>
        /// <param name="connection">The connection to send the initialization data to</param>
        /// <param name="session">The session cache for the connection</param>
        private async Task SendCharacterXmlDataToClient(GameObject player, IRakConnection connection, Session session)
        {
            await using var ctx = new UchuContext();
            
            // Get the XML data for this character for the initial character packet
            var xmlData = GenerateCharacterXmlData(player);

            await using var ms = new MemoryStream();
            await using var writer = new StreamWriter(ms, Encoding.UTF8);
            Serializer.Serialize(writer, xmlData);

            var bytes = ms.ToArray();
            var xml = new byte[bytes.Length - 3];

            Buffer.BlockCopy(bytes, 3, xml, 0, bytes.Length - 3);

            var character = player.GetComponent<CharacterComponent>();
            var ldf = new LegoDataDictionary
            {
                ["gmlevel", 1] = character.GameMasterLevel != 1 ? character.GameMasterLevel : 0,
                ["name"] = character.Name,
                ["objid", 9] = character.Id,
                ["template", 1] = 1,
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
        /// <param name="gmLevel">The GM level of the user</param>
        /// <returns>XmlData conform with the LU Char Data XML Format</returns>
        private static XmlData GenerateCharacterXmlData(GameObject player)
        {
            var xmlData = new XmlData
            {
                Inventory = InventoryNode(player),
                Character = CharacterNode(player),
                Level = LevelNode(player),
                Flags = FlagNodes(player),
                Missions = MissionsNode(player),
                Minifigure = MinifigureNode(player),
                Stats = StatsNode(player)
            };

            return xmlData;
        }

        /// <summary>
        /// Generates the inventory node based for a character, containing information about the items and bricks of a player
        /// </summary>
        /// <param name="player">The character to generate an inventory for</param>
        /// <returns>An inventory node for the character</returns>
        private static InventoryNode InventoryNode(GameObject player)
        {
            return new InventoryNode
            {
                ItemContainers = new[]
                {
                    ItemContainerNode(player, InventoryType.Items),
                    ItemContainerNode(player, InventoryType.Bricks),
                    ItemContainerNode(player, InventoryType.Models),
                    ItemContainerNode(player, InventoryType.Behaviors)
                }
            };
        }

        /// <summary>
        /// Creates an item container node for a character, can be used in an inventory, for example
        /// </summary>
        /// <remarks>
        /// For inventory type <c>Models</c>, extra information is added
        /// </remarks>
        /// <param name="player">The character to create an item container for</param>
        /// <param name="type">The type of container to create, see remarks for extra info</param>
        /// <returns></returns>
        private static ItemContainerNode ItemContainerNode(GameObject player, InventoryType type)
        {
            var inventory = player.GetComponent<InventoryManagerComponent>()[type];
            
            return new ItemContainerNode
            {
                Type = (int) type,
                Items = inventory.Items.Select(item =>
                {
                    var node = new ItemNode
                    {
                        Count = (int) item.Count,
                        Slot = (int) item.Slot,
                        Lot = item.Lot,
                        ObjectId = item.Id,
                        Equipped = item.IsEquipped ? 1 : 0,
                        Bound = item.IsBound ? 1 : 0
                    };

                    if (item.Settings == default)
                        return node;

                    node.ExtraInfo = type == InventoryType.Models ? new ExtraInfoNode {
                            ModuleAssemblyInfo = "0:" + item.Settings
                    } : null;
                    
                    return node;
                }).ToArray()
            };
        }

        /// <summary>
        /// Creates a character node, containing billing info and subscription info
        /// </summary>
        /// <param name="player">The character to create a node from</param>
        /// <returns>The character node created from the character</returns>
        private static CharacterNode CharacterNode(GameObject player)
        {
            var character = player.GetComponent<CharacterComponent>();
            
            return new CharacterNode
            {
                AccountId = character.CharacterId,
                Currency = character.Currency,
                FreeToPlay = character.FreeToPlay ? 1 : 0,
                UniverseScore = character.UniverseScore,
                GmLevel = character.GameMasterLevel
            };
        }

        /// <summary>
        /// Creates the level node for a character, containing the level of the player
        /// </summary>
        /// <param name="player">The character to create the level node for</param>
        /// <returns>The level node for the character</returns>
        private static LevelNode LevelNode(GameObject player)
        {
            var character = player.GetComponent<CharacterComponent>();
            
            return new LevelNode
            {
                Level = character.Level
            };
        }

        /// <summary>
        /// Creates the flag nodes from a character, containing all the flags a player has obtained
        /// </summary>
        /// <param name="player">The character to create the flag nodes for</param>
        /// <returns>An array of flag nodes</returns>
        private static FlagNode[] FlagNodes(GameObject player)
        {
            var character = player.GetComponent<CharacterComponent>();
            var flags = new Dictionary<int, FlagNode>();

            // The flags are stored as one long list of bits by separating them in unsigned longs
            foreach (var value in character.FlagsList)
            {
                // Find the long this flag belongs to
                var index = (int) Math.Floor(value / 64.0);
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
        /// <param name="player">The character to create a mission node for</param>
        /// <returns>The missions node for the character</returns>
        private static MissionsNode MissionsNode(GameObject player)
        {
            // Completed and active missions are stored in two separate lists
            var missionsInventory = player.GetComponent<MissionInventoryComponent>();
            var completed = new List<CompletedMissionNode>();
            var missions = new List<MissionNode>();

            // For all missions split them into active and completed missions
            foreach (var mission in missionsInventory.AllMissions)
            {
                if (mission.State == MissionState.Completed)
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
        private static MissionProgressNode[] ProgressArrayForMission(MissionInstance mission)
        {
            return mission.Tasks.OrderBy(task => task.TaskId).Select(task =>
                {
                    // Return the mission task progress as list as there might be more nodes for this task
                    var progressNodes = new List<MissionProgressNode>
                    {
                        new MissionProgressNode
                        {
                            Value = task.CurrentProgress
                        }
                    };

                    // If the task type is collectible, also send all collectible ids
                    if (task.Type == MissionTaskType.Collect)
                    {
                        progressNodes.AddRange(task.Progress
                            .Select(value => new MissionProgressNode
                            {
                                Value = value
                            }));
                    }

                    return progressNodes;
                }
            ).SelectMany(pn => pn).ToArray();
        }

        /// <summary>
        /// Creates the minifigure node for a character, containing information about hair and mouth styles for example
        /// </summary>
        /// <param name="player">The player to create the minifigure node for</param>
        /// <returns>The mini figure node created from the player</returns>
        private static MinifigureNode MinifigureNode(GameObject player)
        {
            var character = player.GetComponent<CharacterComponent>();
            
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
        /// <param name="player">The character to create the statistics node for</param>
        /// <returns>The statistics node for this character</returns>
        private static DestNode StatsNode(GameObject player)
        {
            var character = player.GetComponent<DestroyableComponent>();
            
            return new DestNode
            {
                MaximumArmor = (int) character.MaxArmor,
                CurrentArmor = (int) character.Armor,
                MaximumHealth = (int) character.MaxHealth,
                CurrentHealth = (int) character.Health,
                MaximumImagination = (int) character.MaxImagination,
                CurrentImagination = (int) character.Imagination
            };
        }
    }
}
