using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Microsoft.EntityFrameworkCore;
using RakDotNet;
using RakDotNet.IO;
using RakDotNet.TcpUdp;
using StackExchange.Redis;
using Uchu.Api;
using Uchu.Api.Models;
using Uchu.Core.Api;
using Uchu.Core.IO;
using Uchu.Core.Providers;
using Uchu.Sso;

namespace Uchu.Core
{
    using HandlerMap = Dictionary<RemoteConnectionType, Dictionary<uint, Handler>>;

    using CommandHandleMap = Dictionary<char, Dictionary<string, CommandHandler>>;

    public class Server
    {
        protected HandlerMap HandlerMap { get; }

        protected CommandHandleMap CommandHandleMap { get; }

        public IRakServer RakNetServer { get; private set; }

        public ISessionCache SessionCache { get; private set; }

        public IFileResources Resources { get; private set; }

        public Configuration Config { get; private set; }
        
        public SsoService SsoService { get; private set; }
        
        public Guid Id { get; }
        
        public int ApiPort { get; private set; }
        
        public int Port { get; private set; }
        
        public int MasterApi { get; private set; }

        public string MasterPath { get; private set; }
        
        public ApiManager Api { get; private set; }

        public event Func<long, ushort, BitReader, IRakConnection, Task> GameMessageReceived;

        public event Action ServerStopped;

        protected bool Running { get; private set; }

        protected X509Certificate Certificate { get; set; }

        public Server(Guid id)
        {
            Id = id;
            
            HandlerMap = new HandlerMap();
            
            CommandHandleMap = new CommandHandleMap();
        }

        public virtual async Task ConfigureAsync(string configFile)
        {
            MasterPath = Path.GetDirectoryName(configFile);
            
            var serializer = new XmlSerializer(typeof(Configuration));

            if (!File.Exists(configFile))
            {
                throw new ArgumentException($"{configFile} config file does not exist.");
            }

            await using (var fs = File.OpenRead(configFile))
            {
                Logger.Config = Config = (Configuration) serializer.Deserialize(fs);

                UchuContextBase.Config = Config;
            }

            await SetupApiAsync().ConfigureAwait(false);

            if (!string.IsNullOrWhiteSpace(Config.ResourcesConfiguration?.GameResourceFolder))
            {
                Resources = new LocalResources(Config);
            }
            
            var certificateFilePath = Path.Combine(MasterPath, Config.Networking.Certificate);
            
            if (Config.Networking?.Certificate != default && File.Exists(certificateFilePath))
            {
                var cert = new X509Certificate2(certificateFilePath);

                Logger.Information($"PRIVATE KEY: {cert.HasPrivateKey} {cert.PrivateKey}");

                Certificate = cert;
                
                RakNetServer = new TcpUdpServer(Port, "3.25 ND1", Certificate, 150);
            }
            else
            {
                RakNetServer = new TcpUdpServer(Port, "3.25 ND1");
            }

            SsoService = new SsoService(Config.SsoConfig?.Domain ?? "");

            try
            {
                SessionCache = new RedisSessionCache();
            }
            catch (RedisConnectionException)
            {
                Logger.Error("Failed to establish Redis connection, falling back to database.");

                SessionCache = new DatabaseCache();
            }

            Logger.Information($"Server {Id} configured on port: {Port}");
        }

        public async Task SetupApiAsync()
        {
            Api = new ApiManager(Config.ApiConfig.Protocol, Config.ApiConfig.Domain);

            var instance = await Api.RunCommandAsync<InstanceInfoResponse>(
                Config.ApiConfig.Port, $"instance/target?i={Id}"
            ).ConfigureAwait(false);

            if (!instance.Success)
            {
                Logger.Error(instance.FailedReason);

                throw new Exception(instance.FailedReason);
            }

            Port = instance.Info.Port;
            ApiPort = instance.Info.ApiPort;
            MasterApi = instance.Info.MasterApi;

            Api.RegisterCommandCollection<InstanceCommands>(this);
        }

        public async Task StartAsync(Assembly assembly, bool acceptConsoleCommands = false)
        {
            RegisterDefaultAssemblies(assembly);

            if (acceptConsoleCommands)
            {
                StartConsole();
            }
            
            Running = true;

            var tasks = new[]
            {
                RakNetServer.RunAsync(),
                Api.StartAsync(ApiPort)
            };

            await Task.WhenAny(tasks).ConfigureAwait(false);

            await StopAsync().ConfigureAwait(false);
        }

        private void StartConsole()
        {
            Task.Run(async () =>
            {
                Logger.Information("Ready to accept console command...");

                while (Running)
                {
                    var command = Console.ReadLine();

                    if (string.IsNullOrWhiteSpace(command)) continue;

                    Console.WriteLine(await HandleCommandAsync(command, null, GameMasterLevel.Console)
                        .ConfigureAwait(false));
                }
            });
        }

        private void RegisterDefaultAssemblies(Assembly assembly)
        {
            Logger.Information("Registering assemblies...");

            RegisterAssembly(typeof(Server).Assembly);
            RegisterAssembly(assembly);

            RakNetServer.MessageReceived += HandlePacketAsync;
        }

        public Task StopAsync()
        {
            Logger.Log("Shutting down...");

            Running = false;

            ServerStopped?.Invoke();

            Certificate.Dispose();
            
            return RakNetServer.ShutdownAsync();
        }

        public string GetHost()
        {
            return !string.IsNullOrWhiteSpace(Config.Networking.Hostname) ? Config.Networking.Hostname : "localhost";
        }

        public virtual void RegisterAssembly(Assembly assembly)
        {
            if (assembly == default)
            {
                throw new ArgumentNullException(nameof(assembly));
            }
            
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

            await using var stream = new MemoryStream(data);
            using var reader = new BitReader(stream);
            
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
                    {
                        await GameMessageReceived(objectId, messageId, reader, connection).ConfigureAwait(false);
                    }
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

                await InvokeHandlerAsync(handler, connection).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        public async Task<string> HandleCommandAsync(string command, object author, GameMasterLevel gameMasterLevel)
        {
            if (command == default) return default;
            
            var prefix = command.FirstOrDefault();

            if (!CommandHandleMap.TryGetValue(prefix, out var group)) return default;

            command = command.Remove(0, 1);

            Logger.Information($"EXEC: {command}");
            
            var arguments = command.Split(' ').ToList();

            if (!group.TryGetValue(arguments[0].ToLower(CultureInfo.CurrentCulture), out var handler))
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

            var returnValue = paramLength switch
            {
                0 => handler.Info.Invoke(handler.Group, Array.Empty<object>()),
                1 => handler.Info.Invoke(handler.Group, new object[] {arguments.ToArray()}),
                _ => handler.Info.Invoke(handler.Group, new[] {arguments.ToArray(), author})
            };

            switch (returnValue)
            {
                case string s:
                    return s;
                case Task<string> s:
                    return await s.ConfigureAwait(false);
                case Task t:
                    await t.ConfigureAwait(false);
                    break;
            }

            return "";
        }

        public async Task<bool> ValidateUserAsync(IRakConnection connection, string username, string key)
        {
            if (connection == null)
                throw new ArgumentNullException(nameof(connection));
            
            var sso = await SsoService.VerifyAsync(username, key).ConfigureAwait(false);
            var local = SessionCache.IsKey(key);
            
            if (!sso && !local)
            {
                Logger.Warning($"{connection} attempted to connect with an invalid session key");
                
                await connection.CloseAsync().ConfigureAwait(false);
                
                return false;
            }

            if (sso)
            {
                //
                // Register Sso users
                //

                await using (var ctx = new UchuContext())
                {
                    var user = await ctx.Users.Where(u => u.Sso).FirstOrDefaultAsync(
                        u => u.Username == username
                    ).ConfigureAwait(false);

                    if (user == default)
                    {
                        user = new User
                        {
                            Username = username,
                            Sso = true
                        };

                        await ctx.Users.AddAsync(user).ConfigureAwait(false);
                    }

                    await ctx.SaveChangesAsync().ConfigureAwait(false);
                }

                await using (var ctx = new UchuContext())
                {
                    var user = await ctx.Users.Where(u => u.Sso).FirstOrDefaultAsync(
                        u => u.Username == username
                    ).ConfigureAwait(false);
                    
                    if (user == default) return false;
                    
                    if (user.Banned || !string.IsNullOrWhiteSpace(user.CustomLockout))
                    {
                        await connection.CloseAsync().ConfigureAwait(false);

                        return false;
                    }
                    
                    if (!SessionCache.IsKey(key))
                    {
                        SessionCache.CreateSession(user.Id, key);
                    }
                }
            }

            SessionCache.RegisterKey(connection.EndPoint, key);

            return true;
        }

        private static async Task InvokeHandlerAsync(Handler handler, IRakConnection endPoint)
        {
            var task = handler.Info.ReturnType == typeof(Task);
            
            var parameters = new object[] {handler.Packet, endPoint};

            var res = handler.Info.Invoke(handler.Group, parameters);

            if (task)
                await ((Task) res).ConfigureAwait(false);
        }
    }
}