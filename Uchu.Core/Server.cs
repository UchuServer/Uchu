using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
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
        protected readonly HandlerMap HandlerMap;

        protected readonly CommandHandleMap CommandHandleMap;
        
        public readonly IRakServer RakNetServer;

        public readonly ISessionCache SessionCache;

        public readonly AssemblyResources Resources;
        
        public readonly int Port;

        protected event Action<long, ushort, BitReader, IRakConnection> OnGameMessage;

        protected event Action OnServerStopped;

        private bool _running;

        public Server(int port, string password = "3.25 ND1")
        {
            Port = port;

            RakNetServer = new TcpUdpServer(port, password);
            SessionCache = new RedisSessionCache();
            Resources = new AssemblyResources("Uchu.Resources.dll");
            
            HandlerMap = new HandlerMap();
            CommandHandleMap = new CommandHandleMap();
            
            Logger.Information($"Server created on port: {port}, password: {password}");
        }

        public Task Start()
        {
            RegisterAssembly(Assembly.GetExecutingAssembly());
            RegisterAssembly(Assembly.GetEntryAssembly());
            
            Logger.Information("Starting...");
            
            RakNetServer.MessageReceived += HandlePacket;

            _running = true;

            Task.Run(async () =>
            {
                while (_running)
                {
                    var command = Console.ReadLine();

                    Console.WriteLine(await HandleCommandAsync(command, null, GameMasterLevel.Console));
                }
            });

            return RakNetServer.RunAsync();
        }

        public Task Stop()
        {
            Logger.Log("Shutting down...");

            _running = false;

            OnServerStopped?.Invoke();

            return RakNetServer.ShutdownAsync();
        }

        protected virtual void RegisterAssembly(Assembly assembly)
        {
            var groups = assembly.GetTypes().Where(c => c.IsSubclassOf(typeof(HandlerGroup)));

            foreach (var group in groups)
            {
                var instance = (HandlerGroup) Activator.CreateInstance(group);
                instance.Server = this;
                
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
                            Packet = packet,
                            RunTask = attr.RunTask
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

        public async Task HandlePacket(IPEndPoint endPoint, byte[] data, Reliability reliability)
        {
            var connection = RakNetServer.GetConnection(endPoint);
            
            var stream = new MemoryStream(data);

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

                    OnGameMessage?.Invoke(objectId, messageId, reader, connection);

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
                    InvokeHandler(handler, connection);
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
        
        private static void InvokeHandler(Handler handler, IRakConnection endPoint)
        {
            var task = handler.Info.ReturnType == typeof(Task);
            
            Logger.Debug($"Invoking {handler.Group.GetType().Name}.{handler.Info.Name} for {handler.Packet}");

            var parameters = new object[] {handler.Packet, endPoint};

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
                        throw;
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
                        throw;
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
                    throw;
                }
            }
        }
    }
}