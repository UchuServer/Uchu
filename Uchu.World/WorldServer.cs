using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using RakDotNet;
using RakDotNet.IO;
using Uchu.Api.Models;
using Uchu.Core;
using Uchu.Python;
using Uchu.World.Api;
using Uchu.World.Client;
using Uchu.World.Social;

namespace Uchu.World
{
    using GameMessageHandlerMap = Dictionary<GameMessageId, Handler>;

    public class WorldServer : Server
    {
        private readonly GameMessageHandlerMap _gameMessageHandlerMap;

        private ZoneId ZoneId { get; set; }

        public List<Zone> Zones { get; }

        public uint MaxPlayerCount { get; }
        
        public ZoneParser ZoneParser { get; private set; }
        
        public Whitelist Whitelist { get; private set; }

        public WorldServer(Guid id) : base(id)
        {
            Zones = new List<Zone>();

            MaxPlayerCount = 20; // TODO: Set
            
            _gameMessageHandlerMap = new GameMessageHandlerMap();
        }

        public override async Task ConfigureAsync(string configFile)
        {
            Logger.Information($"Created WorldServer on PID {Process.GetCurrentProcess().Id.ToString()}");

            await base.ConfigureAsync(configFile);
            
            ZoneParser = new ZoneParser(Resources);
            
            Whitelist = new Whitelist(Resources);
            
            await Whitelist.LoadDefaultWhitelist();
            
            GameMessageReceived += HandleGameMessageAsync;
            ServerStopped += () =>
            {
                foreach (var zone in Zones)
                {
                    Object.Destroy(zone);
                }
            };

            RakNetServer.ClientDisconnected += HandleDisconnect;
            
            var instance = await Api.RunCommandAsync<InstanceInfoResponse>(
                Config.ApiConfig.Port, $"instance/target?i={Id}"
            ).ConfigureAwait(false);

            ZoneId = (ZoneId) instance.Info.Zones.First();
            
            var info = await Api.RunCommandAsync<InstanceInfoResponse>(MasterApi, $"instance/target?i={Id}");

            Api.RegisterCommandCollection<WorldCommands>(this);
            
            var _ = Task.Run(async () =>
            {
                foreach (var zone in info.Info.Zones)
                {
                    await ZoneParser.LoadZoneDataAsync(zone);

                    await LoadZone(zone);
                }
            });

            ManagedScriptEngine.AdditionalPaths = Config.ManagedScriptSources.Paths.ToArray();
            
            Logger.Information($"Setting up world server: {Id}");
        }

        private Task HandleDisconnect(IPEndPoint point, CloseReason reason)
        {
            Logger.Information($"{point} disconnected: {reason}");

            var players = Zones.Select(zone =>
                zone.Players.FirstOrDefault(p => p.Connection.EndPoint.Equals(point))
            ).Where(player => player != default);
            
            foreach (var player in players)
            {
                Object.Destroy(player);
            }

            return Task.CompletedTask;
        }

        private async Task LoadZone(int zone)
        {
            if (ZoneParser.Zones == default) await ZoneParser.LoadZoneDataAsync(zone);

            Logger.Information($"Starting {zone}");

            if (ZoneParser?.Zones == default || !ZoneParser.Zones.TryGetValue(zone, out var info))
            {
                throw new Exception($"Failed to find info for {(ZoneId) zone}");
            }

            var zoneInstance = new Zone(info, this, 0, 0); // TODO Instance/Clone
            
            Zones.Add(zoneInstance);
            
            await zoneInstance.InitializeAsync();
        }

        public async Task<Zone> GetZoneAsync(ZoneId zoneId)
        {
            if (ZoneId == zoneId)
            {
                //
                // Wait for zone to load
                //
                
                while (Running)
                {
                    var zone = Zones.FirstOrDefault(z => z.ZoneId == zoneId);

                    if (zone?.Loaded ?? false)
                    {
                        return zone;
                    }
                    
                    await Task.Delay(1000);
                }
            }
            
            Logger.Error($"{zoneId} is not in the Zone Table for this server.");
            return default;
        }

        public override void RegisterAssembly(Assembly assembly)
        {
            var groups = assembly.GetTypes().Where(c => c.IsSubclassOf(typeof(HandlerGroup)));

            foreach (var group in groups)
            {
                var instance = (HandlerGroup) Activator.CreateInstance(group);

                instance.SetServer(this);

                foreach (var method in group.GetMethods().Where(m => !m.IsStatic && !m.IsAbstract))
                {
                    var attr = method.GetCustomAttribute<PacketHandlerAttribute>();
                    if (attr != null)
                    {
                        var parameters = method.GetParameters();
                        if (parameters.Length == 0 ||
                            !typeof(IPacket).IsAssignableFrom(parameters[0].ParameterType)) continue;
                        var packet = (IPacket) Activator.CreateInstance(parameters[0].ParameterType);

                        if (typeof(IGameMessage).IsAssignableFrom(parameters[0].ParameterType))
                        {
                            var gameMessage = (IGameMessage) packet;

                            _gameMessageHandlerMap.Add(gameMessage.GameMessageId, new Handler
                            {
                                Group = instance,
                                Info = method,
                                Packet = packet
                            });

                            continue;
                        }

                        var remoteConnectionType = attr.RemoteConnectionType ?? packet.RemoteConnectionType;
                        var packetId = attr.PacketId ?? packet.PacketId;

                        if (!HandlerMap.ContainsKey(remoteConnectionType))
                            HandlerMap[remoteConnectionType] = new Dictionary<uint, Handler>();

                        var handlers = HandlerMap[remoteConnectionType];

                        Logger.Debug(!handlers.ContainsKey(packetId)
                            ? $"Registered handler for packet {packet}"
                            : $"Handler for packet {packet} overwritten");

                        handlers[packetId] = new Handler
                        {
                            Group = instance,
                            Info = method,
                            Packet = packet
                        };
                    }
                    else
                    {
                        var cmdAttr = method.GetCustomAttribute<CommandHandlerAttribute>();
                        if (cmdAttr == null) continue;

                        if (!CommandHandleMap.ContainsKey(cmdAttr.Prefix))
                            CommandHandleMap[cmdAttr.Prefix] = new Dictionary<string, CommandHandler>();

                        CommandHandleMap[cmdAttr.Prefix][cmdAttr.Signature] = new CommandHandler
                        {
                            Group = instance,
                            Info = method,
                            GameMasterLevel = cmdAttr.GameMasterLevel,
                            Help = cmdAttr.Help,
                            Signature = cmdAttr.Signature,
                            ConsoleCommand = method.GetParameters().Length != 2
                        };
                    }
                }
            }
        }

        private async Task HandleGameMessageAsync(long objectId, ushort messageId, BitReader reader, IRakConnection connection)
        {
            if (!_gameMessageHandlerMap.TryGetValue((GameMessageId) messageId, out var messageHandler))
            {
                Logger.Warning(Enum.IsDefined(typeof(GameMessageId), messageId)
                    ? $"No handler registered for GameMessage: {(GameMessageId) messageId}!"
                    : $"Undocumented GameMessage: 0x{messageId:x8}!"
                );

                return;
            }

            var session = SessionCache.GetSession(connection.EndPoint);

            Logger.Debug($"Received {((IGameMessage) messageHandler.Packet).GameMessageId}");

            var player = Zones.Where(z => z.ZoneId == session.ZoneId).SelectMany(z => z.Players)
                .FirstOrDefault(p => p.Connection.Equals(connection));

            if (player?.Zone == default)
            {
                Logger.Error($"{connection} is not logged in but sent a GameMessage.");
                return;
            }

            var associate = player.Zone.GameObjects.FirstOrDefault(o => o.Id == objectId);

            if (associate == default)
            {
                Logger.Error($"{objectId} is not a valid object in {connection}'s zone.");
                return;
            }

            var gameMessage = (IGameMessage) messageHandler.Packet;

            gameMessage.Associate = associate;

            reader.BaseStream.Position = 18;

            reader.Read(gameMessage);

            await InvokeHandlerAsync(messageHandler, player);
        }

        private static async Task InvokeHandlerAsync(Handler handler, Player player)
        {
            var task = handler.Info.ReturnType == typeof(Task);

            var parameters = new object[] {handler.Packet, player};

            var res = handler.Info.Invoke(handler.Group, parameters);

            if (task) await (Task) res;
        }
    }
}