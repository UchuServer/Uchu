using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using RakDotNet;
using RakDotNet.IO;
using Uchu.Core;
using Uchu.World.Client;

namespace Uchu.World
{
    using GameMessageHandlerMap = Dictionary<GameMessageId, Handler>;

    public class WorldServer : Server
    {
        private readonly GameMessageHandlerMap _gameMessageHandlerMap;

        private readonly ZoneId _zoneId;

        public List<Zone> Zones { get; }
        
        public ZoneParser ZoneParser { get; private set; }

        public uint MaxPlayerCount { get; }

        public uint ActiveUserCount
        {
            get
            {
                using var ctx = new UchuContext();
                
                var specification = ctx.Specifications.First(s => s.Id == Id);

                return specification.ActiveUserCount;
            }
            set
            {
                using var ctx = new UchuContext();
                
                var specification = ctx.Specifications.First(s => s.Id == Id);

                specification.ActiveUserCount = value;

                ctx.SaveChanges();
            }
        }

        public WorldServer(ServerSpecification specifications) : base(specifications.Id)
        {
            Logger.Information($"Created WorldServer on PID {Process.GetCurrentProcess().Id.ToString()}");
            
            Zones = new List<Zone>();

            _zoneId = specifications.ZoneId;

            MaxPlayerCount = specifications.MaxUserCount;

            _gameMessageHandlerMap = new GameMessageHandlerMap();
        }

        public override async Task ConfigureAsync(string configFile)
        {
            await base.ConfigureAsync(configFile);
            
            ZoneParser = new ZoneParser(Resources);
            
            GameMessageReceived += HandleGameMessageAsync;
            ServerStopped += () =>
            {
                foreach (var zone in Zones)
                {
                    Object.Destroy(zone);
                }
            };

            RakNetServer.ClientDisconnected += HandleDisconnect;

            var _ = Task.Run(async () =>
            {
                Logger.Information($"Loading zones...");

                await ZoneParser.LoadZoneDataAsync();

                Logger.Information($"Loading {ServerSpecification.ZoneId}");

                await LoadZone(ServerSpecification);
            });
            
            Logger.Information($"Setting up world server: {ServerSpecification.Id}");
        }

        private Task HandleDisconnect(IPEndPoint point, CloseReason reason)
        {
            Logger.Information($"{point} disconnected: {reason}");

            foreach (var player in Zones
                .Select(zone => zone.Players.FirstOrDefault(p => p.Connection.EndPoint.Equals(point)))
                .Where(player => !ReferenceEquals(player, default)))
            {
                Object.Destroy(player);

                break;
            }

            return Task.CompletedTask;
        }

        public async Task LoadZone(ServerSpecification zone)
        {
            if (ZoneParser.Zones == default) await ZoneParser.LoadZoneDataAsync();

            Logger.Information($"Starting {zone.ZoneId}");

            var info = ZoneParser.Zones?[zone.ZoneId];

            var zoneInstance = new Zone(info, this, zone.ZoneInstanceId, zone.ZoneCloneId);
            
            Zones.Add(zoneInstance);
            
            await zoneInstance.InitializeAsync();
        }

        public async Task<Zone> GetZoneAsync(ZoneId zoneId)
        {
            if (_zoneId == zoneId)
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

        protected override void RegisterAssembly(Assembly assembly)
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

            var player = Zones.Where(z => z.ZoneInfo.ZoneId == session.ZoneId).SelectMany(z => z.Players)
                .FirstOrDefault(p => p.Connection.Equals(connection));

            if (player == default)
            {
                Logger.Error($"{connection} is not logged in but sent a GameMessage.");
                return;
            }

            var associate = player.Zone.GameObjects.FirstOrDefault(o => o.ObjectId == objectId);

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