using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Reflection;
using System.Threading.Tasks;
using RakDotNet;
using RakDotNet.IO;
using Uchu.Core;
using Uchu.World.Collections;
using Uchu.World.Parsers;

namespace Uchu.World
{
    using GameMessageHandlerMap = Dictionary<ushort, Handler>;
    
    public class WorldServer : Server
    {
        private readonly GameMessageHandlerMap _gameMessageHandlerMap;
        
        private readonly ZoneParser _parser;

        public readonly List<Zone> Zones = new List<Zone>();
        
        public WorldServer(int port, string password = "3.25 ND1") : base(port, password)
        {
            _gameMessageHandlerMap = new GameMessageHandlerMap();

            _parser = new ZoneParser(Resources);

            OnGameMessage += HandleGameMessage;

            RakNetServer.Disconnection += point =>
            {
                Logger.Information($"{point} disconnected");
                foreach (var player in Zones
                    .Select(zone => zone.Players.FirstOrDefault(p => p.EndPoint.Equals(point)))
                    .Where(player => !ReferenceEquals(player, default)))
                {
                    Object.Destroy(player);

                    break;
                }
            };
        }

        public async Task<Zone> GetZone(ZoneId zoneId)
        {
            if (Zones.Any(z => z.ZoneInfo.ZoneId == (uint) zoneId))
                return Zones.First(z => z.ZoneInfo.ZoneId == (uint) zoneId);
            
            var info = await _parser.ParseAsync(ZoneParser.Zones[zoneId]);

            // Create new Zone
            var zone = new Zone(info, this);
            Zones.Add(zone);
            zone.Initialize();

            return Zones.First(z => z.ZoneInfo.ZoneId == (uint) zoneId);
        }

        public async Task<string> AdminCommand(string command, Player player)
        {
            var arguments = command?.Split(' ');

            uint count;
            int lot;
            switch (arguments?[0].ToLower())
            {
                case "give":
                    if (arguments.Length < 2 || arguments.Length > 3)
                    {
                        return "give <lot> <count(optional)>";
                    }

                    if (!int.TryParse(arguments[1], out lot))
                    {
                        return "Invalid <lot>";
                    }

                    count = 1;
                    if (arguments.Length == 3)
                    {
                        if (!uint.TryParse(arguments[2], out count))
                        {
                            return "Invalid <count(optional)>";
                        }
                    }

                    await player.GetComponent<InventoryManager>().AddItemAsync(lot, count);

                    return $"Successfully added {lot} x {count} to your inventory";
                case "remove":
                    if (arguments.Length < 2 || arguments.Length > 3)
                    {
                        return "remove <lot> <count(optional)>";
                    }

                    if (!int.TryParse(arguments[1], out lot))
                    {
                        return "Invalid <lot>";
                    }

                    count = 1;
                    if (arguments.Length == 3)
                    {
                        if (!uint.TryParse(arguments[2], out count))
                        {
                            return "Invalid <count(optional)>";
                        }
                    }

                    await player.GetComponent<InventoryManager>().RemoveItemAsync(lot, count);

                    return $"Successfully removed {lot} x {count} to your inventory";
                case "coin":
                    if (arguments.Length != 2)
                    {
                        return "coin <delta>";
                    }

                    if (!int.TryParse(arguments[1], out var delta) || delta == default)
                    {
                        return "Invalid <delta>";
                    }

                    player.Currency += delta;

                    return $"Successfully {(delta > 0 ? "added" : "removed")} coins";
                case "spawn":
                    if (arguments.Length != 2 || arguments.Length > 8)
                    {
                        return "spawn <lot> <x(optional)> <y(optional)> <z(optional)>";
                    }

                    arguments = arguments.Select(a => a.Replace('.', ',')).ToArray();

                    if (!int.TryParse(arguments[1], out lot))
                    {
                        return "Invalid <lot>";
                    }
                    
                    var position = player.Transform.Position;
                    if (arguments.Length >= 5)
                    {
                        try
                        {
                            position = new Vector3
                            {
                                X = float.Parse(arguments[2]),
                                Y = float.Parse(arguments[3]),
                                Z = float.Parse(arguments[4])
                            };
                        }
                        catch
                        {
                            return "Invalid <x(optional)> or <y(optional)> or <z(optional)>";
                        }
                    }

                    var rotation = player.Transform.Rotation;

                    var obj = GameObject.Instantiate(new LevelObject
                    {
                        Lot = lot,
                        Position = position,
                        Rotation = rotation,
                        Scale = 1,
                        Settings = new LegoDataDictionary()
                    }, player.Zone);
                    
                    obj.Construct();

                    return $"Successfully spawned {lot} at\npos: {position}\nrot: {rotation}";
                case "position":
                    return $"{player.Transform.Position}";
                case "rotation":
                    return $"{player.Transform.Rotation}";
                case "die":
                    player.GetComponent<DestructibleComponent>().Smash(player, player);
                    return "You smashed yourself.";
                default:
                    return AdminCommand(command, false);
            }
        }
        
        protected override void RegisterAssembly(Assembly assembly)
        {
            var groups = assembly.GetTypes().Where(c => c.IsSubclassOf(typeof(HandlerGroup)));

            foreach (var group in groups)
            {
                var instance = (HandlerGroup) Activator.CreateInstance(group);
                instance.Server = this;
                
                foreach (var method in group.GetMethods().Where(m => !m.IsStatic && !m.IsAbstract))
                {
                    var attr = method.GetCustomAttribute<PacketHandlerAttribute>();
                    if (attr == null) continue;

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
                            Packet = packet,
                            RunTask = attr.RunTask
                        });
                        
                        continue;
                    }

                    var remoteConnectionType = attr.RemoteConnectionType ?? packet.RemoteConnectionType;
                    var packetId = attr.PacketId ?? packet.PacketId;

                    if (!HandlerMap.ContainsKey(remoteConnectionType))
                        HandlerMap[remoteConnectionType] = new Dictionary<uint, Handler>();

                    var handlers = HandlerMap[remoteConnectionType];
                    
                    Logger.Debug(!handlers.ContainsKey(packetId) ? $"Registered handler for packet {packet}" : $"Handler for packet {packet} overwritten");
                    
                    handlers[packetId] = new Handler
                    {
                        Group = instance,
                        Info = method,
                        Packet = packet,
                        RunTask = attr.RunTask
                    };
                }
            }
        }

        private void HandleGameMessage(long objectId, ushort messageId, BitReader reader, IPEndPoint endPoint)
        {
            if (!_gameMessageHandlerMap.TryGetValue(messageId, out var messageHandler))
            {
                Logger.Warning($"No handler registered for GameMessage (0x{messageId:x})!");
                
                return;
            }

            var session = SessionCache.GetSession(endPoint);
            
            Logger.Debug($"Received {messageHandler.Packet.GetType().FullName}");

            var player = Zones.Where(z => z.ZoneInfo.ZoneId == session.ZoneId).SelectMany(z => z.Players)
                .FirstOrDefault(p => p.EndPoint.Equals(endPoint));

            if (ReferenceEquals(player, null))
            {
                Logger.Error($"{endPoint} is not logged in but sent a GameMessage.");
                return;
            }
            
            var associate = player.Zone.GameObjects.FirstOrDefault(o => o.ObjectId == objectId);
            
            if (ReferenceEquals(associate, null))
            {
                Logger.Error($"{objectId} is not a valid object in {endPoint}'s zone.");
                return;
            }

            var gameMessage = (IGameMessage) messageHandler.Packet;

            gameMessage.Associate = associate;

            reader.BaseStream.Position = 18;
            
            reader.Read(gameMessage);

            InvokeHandler(messageHandler, player);
        }
        
        private static void InvokeHandler(Handler handler, Player player)
        {
            var task = handler.Info.ReturnType == typeof(Task);

            var parameters = new object[] {handler.Packet, player};

            if (task)
            {
                Task.Run(async () =>
                {
                    try
                    {
                        await (Task) handler.Info.Invoke(handler.Group, parameters);
                    }
                    catch (Exception e)
                    {
                        Logger.Error(e);
                    }
                });
            }
            else if (handler.RunTask)
            {
                Task.Run(() =>
                {
                    try
                    {
                        handler.Info.Invoke(handler.Group, parameters);
                    }
                    catch (Exception e)
                    {
                        Logger.Error(e);
                    }
                });
            }
            else
            {
                try
                {
                    handler.Info.Invoke(handler.Group, parameters);
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
        }
    }
}