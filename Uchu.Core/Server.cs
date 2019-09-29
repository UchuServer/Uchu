using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using RakDotNet;
using RakDotNet.IO;
using RakDotNet.TcpUdp;
using Uchu.Core.IO;

namespace Uchu.Core
{
    using HandlerMap = Dictionary<RemoteConnectionType, Dictionary<uint, Handler>>;

    using CommandHandleMap = Dictionary<char, Dictionary<string, CommandHandler>>;

    public class Server
    {
        private Task _runTask;

        protected readonly HandlerMap HandlerMap;

        protected readonly CommandHandleMap CommandHandleMap;

        public readonly IRakServer RakNetServer;

        public readonly ISessionCache SessionCache;

        public readonly IFileResources Resources;

        public readonly Configuration Config;

        public readonly int Port;

        public event Func<long, ushort, BitReader, IRakConnection, Task> GameMessageReceived;

        public event Action ServerStopped;

        private bool _running;

        public Server(ServerType type)
        {
            var serializer = new XmlSerializer(typeof(Configuration));
            var fn = File.Exists("config.xml") ? "config.xml" : "config.default.xml";

            if (File.Exists(fn))
            {
                using (var file = File.OpenRead(fn))
                {
                    Logger.Config = Config = (Configuration) serializer.Deserialize(file);
                }
            }
            else
            {
                Logger.Config = Config = new Configuration();

                var backup = File.CreateText("config.default.xml");

                serializer.Serialize(backup, Config);

                backup.Close();

                Logger.Warning("No config file found, creating default.");
            }

            if (!string.IsNullOrWhiteSpace(Config.ResourcesConfiguration?.GameResourceFolder))
            {
                Logger.Information($"Using Local Resources at `{Config.ResourcesConfiguration.GameResourceFolder}`");
                Resources = new LocalResources(Config);
            }


            
            Port = type switch
            {
                ServerType.Authentication => 21836,
                ServerType.Character => Config.Character.Port,
                ServerType.World => Config.World.Port,
                _ => throw new NotSupportedException($"{type} is not a supported ServerType")
            };

            RakNetServer = new TcpUdpServer(Port, "3.25 ND1");
            SessionCache = new RedisSessionCache();

            HandlerMap = new HandlerMap();
            CommandHandleMap = new CommandHandleMap();

            Logger.Information($"{type} Server created on port: {Port}");
        }

        public Task StartAsync()
        {
            RegisterAssembly(Assembly.GetExecutingAssembly());
            RegisterAssembly(Assembly.GetEntryAssembly());

            Logger.Information("Starting...");

            RakNetServer.MessageReceived += HandlePacketAsync;

            _running = true;

            Task.Run(async () =>
            {
                while (_running)
                {
                    var command = Console.ReadLine();

                    Console.WriteLine(await HandleCommandAsync(command, null, GameMasterLevel.Console));
                }
            });

            return _runTask = RakNetServer.RunAsync();
        }

        public Task StopAsync()
        {
            Logger.Log("Shutting down...");

            _running = false;

            ServerStopped?.Invoke();

            return RakNetServer.ShutdownAsync();
        }

        protected virtual void RegisterAssembly(Assembly assembly)
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

                        var remoteConnectionType = attr.RemoteConnectionType ?? packet.RemoteConnectionType;
                        var packetId = attr.PacketId ?? packet.PacketId;

                        if (!HandlerMap.ContainsKey(remoteConnectionType))
                            HandlerMap[remoteConnectionType] = new Dictionary<uint, Handler>();

                        var handlers = HandlerMap[remoteConnectionType];

                        Logger.Debug(!handlers.ContainsKey(packetId)
                            ? $"Registered handler for packet {packet}"
                            : $"Handler for packet {packet} overwritten"
                        );

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

        public async Task HandlePacketAsync(IPEndPoint endPoint, byte[] data, Reliability reliability)
        {
            var connection = RakNetServer.GetConnection(endPoint);

            using (var stream = new MemoryStream(data))
            using (var reader = new BitReader(stream))
            {
                var header = new PacketHeader();
                reader.Read(header);

                if (header.MessageId != MessageIdentifier.UserPacketEnum)
                    throw new ArgumentOutOfRangeException($"Packet is not {nameof(MessageIdentifier.UserPacketEnum)}");

                if (header.PacketId == 0x05)
                {
                    //
                    // Game Message
                    //

                    var objectId = reader.Read<long>();
                    var messageId = reader.Read<ushort>();

                    try
                    {
                        if (GameMessageReceived != null)
                            await GameMessageReceived(objectId, messageId, reader, connection);
                    }
                    catch (Exception e)
                    {
                        Logger.Error(e);
                    }

                    return;
                }

                //
                // Regular Packet
                //

                if (!HandlerMap.TryGetValue(header.RemoteConnectionType, out var temp) ||
                    !temp.TryGetValue(header.PacketId, out var handler))
                {
                    Logger.Warning($"No handler registered for Packet ({header.RemoteConnectionType}:0x{header.PacketId:x})!");

                    return;
                };

                Logger.Debug($"Received {handler.Packet.GetType().FullName}");

                reader.BaseStream.Position = 8;

                try
                {
                    reader.Read(handler.Packet);

                    await InvokeHandlerAsync(handler, connection);
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
        }

        public async Task<string> HandleCommandAsync(string command, object author, GameMasterLevel gameMasterLevel)
        {
            var prefix = command.First();

            if (!CommandHandleMap.TryGetValue(prefix, out var group)) return "Invalid prefix";

            command = command.Remove(0, 1);

            var arguments = command.Split(' ').ToList();

            if (!group.TryGetValue(arguments[0].ToLower(), out var handler))
            {
                var help = new StringBuilder();

                foreach (var handlerInfo in group.Values.Where(handlerInfo => gameMasterLevel >= handlerInfo.GameMasterLevel))
                {
                    if (author == null && !handlerInfo.ConsoleCommand) continue;

                    help.AppendLine($"{prefix}{handlerInfo.Signature}" +
                                    $"{(string.Concat(Enumerable.Repeat(" ", 20 - handlerInfo.Signature.Length)))}" +
                                    $"{handlerInfo.Help}");
                }

                return help.ToString();
            }

            if (gameMasterLevel < handler.GameMasterLevel) return "You don't have permission to run this command";

            arguments.RemoveAt(0);

            var paramLength = handler.Info.GetParameters().Length;

            object returnValue;

            switch (paramLength)
            {
                case 0:
                    returnValue = handler.Info.Invoke(handler.Group, new object[0]);
                    break;
                case 1:
                    returnValue = handler.Info.Invoke(handler.Group, new object[] {arguments.ToArray()});
                    break;
                default:
                    returnValue = handler.Info.Invoke(handler.Group, new[] {arguments.ToArray(), author});
                    break;
            }

            switch (returnValue)
            {
                case string s:
                    return s;
                case Task<string> s:
                    return await s;
                case Task t:
                    await t;
                    break;
            }

            return "";
        }

        private async Task InvokeHandlerAsync(Handler handler, IRakConnection endPoint)
        {
            var task = handler.Info.ReturnType == typeof(Task);

            Logger.Debug($"Invoking {handler.Group.GetType().Name}.{handler.Info.Name} for {handler.Packet}");

            var parameters = new object[] {handler.Packet, endPoint};

            var res = handler.Info.Invoke(handler.Group, parameters);

            if (task)
                await (Task)res;
        }
    }
}