using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml.Serialization;
using RakDotNet;
using RakDotNet.TcpUdp;
using Uchu.Core.IO;

namespace Uchu.Core
{
    using HandlerMap = Dictionary<RemoteConnectionType, Dictionary<uint, List<Handler>>>;
    using GameMessageHandlerMap = Dictionary<ushort, List<Handler>>;

    public class Server
    {
        public readonly IRakNetServer RakNetServer;

        private readonly HandlerMap _handlerMap;
        private readonly GameMessageHandlerMap _gameMessageHandlerMap;

        public int Port { get; }
        public ISessionCache SessionCache { get; }
        public IResources Resources { get; }
        public ZoneParser ZoneParser { get; }
        public CDClient CDClient { get; }
        public Dictionary<ZoneId, World> Worlds { get; }
        public ServerType ServerType { get; }
        public Configuration Config { get; }
        public InstanceManager InstanceManager { get; }

        public Server(ServerType type)
        {
            ServerType = type;

            var serializer = new XmlSerializer(typeof(Configuration));
            var fn = File.Exists("config.xml") ? "config.xml" : "config.default.xml";

            using (var file = File.OpenRead(fn))
            {
                Config = (Configuration)serializer.Deserialize(file);
            }

            Port =
                type == ServerType.Authentication ? 21836 :
                type == ServerType.Character ? Config.Character.Port :
                type == ServerType.World ? Config.World.Port :
                type == ServerType.Chat ? Config.Chat.Port :
                throw new NotSupportedException();

            RakNetServer = new TcpUdpServer(Port, "3.25 ND1");
            _handlerMap = new HandlerMap();
            _gameMessageHandlerMap = new GameMessageHandlerMap();
            SessionCache = new RedisSessionCache();
            Resources = new AssemblyResources("Uchu.Resources.dll");
            ZoneParser = new ZoneParser(Resources);
            CDClient = new CDClient();
            InstanceManager = new InstanceManager(this);

            Worlds = new Dictionary<ZoneId, World>(); // TODO: move everything to new impl

            RakNetServer.PacketReceived += _handlePacket;

            RegisterAssembly(Assembly.GetExecutingAssembly());
        }

        public void Start()
        {
            var active = true;

            RakNetServer.Start();

            while (active)
            {
                var input = Console.ReadLine();

                /*if (!input?.StartsWith('/') ?? true)
                {
                    continue;
                }

                input = input.Remove(0, 1);*/

                var split = input?.Split(' ');

                switch (split?[0].ToLower())
                {
                    /*case "players":
                        foreach (var connection in RakNetServer.Connections)
                        {
                            Console.WriteLine(connection);
                        }
                        break;*/
                    case "exit":
                    case "quit":
                    case "stop":
                        active = false;
                        break;

                    case "adduser":
                        var name = split[1];
                        var password = split[2];

                        using (var ctx = new UchuContext())
                        {
                            var hashed = BCrypt.Net.BCrypt.EnhancedHashPassword(password);

                            ctx.Users.Add(new User
                            {
                                Username = name,
                                Password = hashed,
                                CharacterIndex = 0
                            });

                            ctx.SaveChanges();
                        }
                        break;

                    case "help":
                        Console.WriteLine("help                         Display this message");
                        Console.WriteLine("adduser <name> <password>    Create a new user");
                        Console.WriteLine("stop                         Stop the server");
                        break;

                    default:
                        Console.WriteLine("Unknown command");
                        break;
                }
            }

            RakNetServer.Stop();
        }

        public void Stop()
        {
            Console.WriteLine("Stopping...");

            RakNetServer.Stop();
        }

        public ReplicaManager CreateReplicaManager()
            => new ReplicaManager(RakNetServer);

        public void DisconnectClient(IPEndPoint endpoint, DisconnectId id = DisconnectId.UnknownServerError)
            => Send(new DisconnectNotifyPacket {DisconnectId = id}, endpoint);

        public void Send(IPacket packet, IPEndPoint endpoint)
        {
            var stream = new BitStream();

            stream.WriteSerializable(packet);

            RakNetServer.Send(stream, endpoint);

            Console.WriteLine($"Sent {packet}");
        }

        public void Send(byte[] data, IPEndPoint endpoint)
            => RakNetServer.Send(data, endpoint);

        public void Send(BitStream stream, IPEndPoint endpoint)
            => RakNetServer.Send(stream, endpoint);

        public void RegisterAssembly(Assembly assembly)
        {
            var classes = assembly.GetTypes()
                .Where(t => t.IsClass && t.IsPublic && t.IsSubclassOf(typeof(HandlerGroupBase)));

            foreach (var clazz in classes)
            {
                var group = (HandlerGroupBase) Activator.CreateInstance(clazz);

                group.SetServer(this);

                var methods = clazz.GetMethods().Where(m => m.IsPublic && !m.IsStatic && !m.IsAbstract);

                foreach (var method in methods)
                {
                    var parameters = method.GetParameters().ToArray();
                    var attr = method.GetCustomAttribute<PacketHandlerAttribute>();

                    if (attr == null || parameters.Length == 0 ||
                        !typeof(IPacket).IsAssignableFrom(parameters[0].ParameterType))
                        continue;

                    var packet = (IPacket) Activator.CreateInstance(parameters[0].ParameterType);

                    var rct = attr.RemoteConnectionType ?? packet.RemoteConnectionType;
                    var packetId = attr.PacketId ?? packet.PacketId;

                    if (typeof(IGameMessage).IsAssignableFrom(parameters[0].ParameterType))
                    {
                        var msg = (IGameMessage) packet;

                        if (!_gameMessageHandlerMap.ContainsKey(msg.GameMessageId))
                            _gameMessageHandlerMap[msg.GameMessageId] = new List<Handler>();

                        _gameMessageHandlerMap[msg.GameMessageId].Add(new Handler
                        {
                            Group = group,
                            Method = method,
                            Packet = packet,
                            RunTask = attr.RunTask
                        });

                        Console.WriteLine($"Registered handler for game message {packet}");

                        continue;
                    }

                    if (!_handlerMap.ContainsKey(rct))
                        _handlerMap[rct] = new Dictionary<uint, List<Handler>>();

                    var handlers = _handlerMap[rct];

                    if (!handlers.ContainsKey(packetId))
                        handlers[packetId] = new List<Handler>();

                    handlers[packetId].Add(new Handler
                    {
                        Group = group,
                        Method = method,
                        Packet = packet,
                        RunTask = attr.RunTask
                    });

                    Console.WriteLine($"Registered handler for packet {packet}");
                }
            }
        }

        private void _handlePacket(IPEndPoint endpoint, byte[] data)
        {
            var stream = new BitStream(data);
            var header = new PacketHeader();

            stream.ReadSerializable(header);

            if (header.MessageId != MessageIdentifiers.UserPacketEnum)
                return;

            if (header.PacketId == 0x05)
            {
                var objId = stream.ReadLong();
                var msgId = stream.ReadUShort();

                if (!_gameMessageHandlerMap.TryGetValue(msgId, out var msgHandlers))
                {
                    Console.WriteLine($"Unhandled game message (0x{msgId:X})");

                    return;
                }

                Console.WriteLine($"Received {msgHandlers[0].Packet}");

                foreach (var handler in msgHandlers)
                {
                    stream.ReadPosition = 144;

                    ((IGameMessage) handler.Packet).ObjectId = objId;

                    try
                    {
                        stream.ReadSerializable(handler.Packet);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        continue;
                    }

                    _invokeHandler(handler, endpoint);
                }

                return;
            }

            if (!_handlerMap.TryGetValue(header.RemoteConnectionType, out var temp) ||
                !temp.TryGetValue(header.PacketId, out var handlers)) return;

            Console.WriteLine($"Received {handlers[0].Packet}");

            foreach (var handler in handlers)
            {
                stream.ReadPosition = 64;

                try
                {
                    stream.ReadSerializable(handler.Packet);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    continue;
                }

                _invokeHandler(handler, endpoint);
            }
        }

        private void _invokeHandler(Handler handler, IPEndPoint endpoint)
        {
            var task = handler.Method.ReturnType == typeof(Task);

            var parameters = new object[] {handler.Packet, endpoint};

            if (task)
            {
                Task.Run(async () =>
                {
                    try
                    {
                        await (Task) handler.Method.Invoke(handler.Group, parameters);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                });
            }
            else if (handler.RunTask)
            {
                Task.Run(() =>
                {
                    try
                    {
                        handler.Method.Invoke(handler.Group, parameters);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                });
            }
            else
            {
                try
                {
                    handler.Method.Invoke(handler.Group, parameters);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }
    }
}