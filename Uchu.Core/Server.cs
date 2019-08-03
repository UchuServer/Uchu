using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RakDotNet;
using RakDotNet.IO;
using RakDotNet.TcpUdp;
using Uchu.Core.IO;

namespace Uchu.Core
{
    using HandlerMap = Dictionary<RemoteConnectionType, Dictionary<uint, Handler>>;

    public class Server
    {
        protected readonly HandlerMap HandlerMap;
        
        public readonly IRakNetServer RakNetServer;

        public readonly ISessionCache SessionCache;

        public readonly AssemblyResources Resources;
        
        public readonly int Port;

        protected event Action<long, ushort, BitReader, IPEndPoint> OnGameMessage;

        private bool _running;

        public Server(int port, string password = "3.25 ND1")
        {
            Port = port;

            RakNetServer = new TcpUdpServer(port, password);
            SessionCache = new RedisSessionCache();
            Resources = new AssemblyResources("Uchu.Resources.dll");
            
            HandlerMap = new HandlerMap();
            
            Logger.Information($"Server created on port: {port}, password: {password}, protocol: {RakNetServer.Protocol}");
        }

        public void Start()
        {
            RegisterAssembly(Assembly.GetExecutingAssembly());
            RegisterAssembly(Assembly.GetEntryAssembly());
            
            Logger.Information("Starting...");
            
            RakNetServer.Start();
            RakNetServer.PacketReceived += HandlePacket;

            _running = true;

            while (_running)
            {
                var command = Console.ReadLine();

                Console.WriteLine(AdminCommand(command, true));
            }
        }

        public void Stop()
        {
            Logger.Log("Shutting down...");

            RakNetServer.Stop();

            _running = false;
        }

        #region Admin

        public string AdminCommand(string command, bool fromConsole)
        {
            if (!fromConsole) Logger.Information($"Admin Command: \"{command}\" is executing in chat mode");
            
            var arguments = command?.Split(' ');

            string name;
            switch (arguments?[0].ToLower())
            {
                case "exit":
                case "quit":
                case "stop":
                    Stop();
                    return "Stopped the server.";
                case "adduser":
                    if (!fromConsole) return "This command is not supported in chat mode.";
                    
                    if (arguments.Length != 2)
                    {
                        return "adduser <name>";
                    }

                    name = arguments[1];
                    
                    if (name.Length > 33)
                    {
                        return "Usernames with more than 33 characters is not supported";
                    }

                    using (var ctx = new UchuContext())
                    {
                        if (ctx.Users.Any(u => u.Username == name))
                        {
                            return "A user with that username already exists";
                        }
                        
                        Console.Write("Password: ");
                        var password = GetPassword();

                        if (password.Length > 42)
                        {
                            return "Passwords with more than 42 characters is not supported";
                        }

                        ctx.Users.Add(new User
                        {
                            Username = name,
                            Password = BCrypt.Net.BCrypt.EnhancedHashPassword(password),
                            CharacterIndex = 0
                        });

                        ctx.SaveChanges();

                        return $"\nSuccessfully added user: {name}!";
                    }
                    
                case "deleteuser":
                    if (!fromConsole) return "This command is not supported in chat mode.";
                    
                    if (arguments.Length != 2)
                    {
                        return "removeuser <name>";
                    }

                    name = arguments[1];

                    using (var ctx = new UchuContext())
                    {
                        var user = ctx.Users.FirstOrDefault(u => u.Username == name);

                        if (user == null)
                        {
                            return $"No user with the username of: {name}";
                        }

                        Console.Write("Write the username again to confirm deletion: ");
                        if (Console.ReadLine() != name) return "Deletion aborted";
                        
                        ctx.Remove(user);
                        ctx.SaveChanges();
                            
                        return $"Successfully deleted user: {name}";

                    }
                
                case "ban":
                case "suspend":
                    if (arguments.Length != 3)
                    {
                        return $"{arguments[0]} <name> <reason>";
                    }

                    name = arguments[1];
                    var reason = arguments[2];
                    
                    using (var ctx = new UchuContext())
                    {
                        var user = ctx.Users.FirstOrDefault(u => u.Username == name);
                        
                        if (user == null)
                        {
                            return $"No user with the username of: {name}";
                        }

                        user.Banned = true;
                        user.BannedReason = reason;

                        ctx.SaveChanges();

                        return $"Successfully banned {name}!";
                    }
                
                case "unban":
                case "pardon":
                    if (arguments.Length != 2)
                    {
                        return $"{arguments[0]} <name>";
                    }
                    
                    name = arguments[1];

                    using (var ctx = new UchuContext())
                    {
                        var user = ctx.Users.FirstOrDefault(u => u.Username == name);
                        
                        if (user == null)
                        {
                            return $"No user with the username of: {name}";
                        }

                        user.Banned = false;
                        user.BannedReason = null;

                        ctx.SaveChanges();
                        
                        return $"Successfully pardoned {name}!";
                    }
                    
                case "users":
                    using (var ctx = new UchuContext())
                    {
                        var users = ctx.Users;
                        return !users.Any()
                            ? "No registered users"
                            : string.Join("\n", users.Select(s => s.Username));
                    }
                    
                case "approve":
                    using (var ctx = new UchuContext())
                    {
                        if (arguments.Length == 1 || arguments[1].ToLower() == "all")
                        {
                            var unApproved = ctx.Characters.Where(c => !c.NameRejected && c.Name != c.CustomName && !string.IsNullOrEmpty(c.CustomName));

                            if (arguments.Length == 2 && arguments[1] == "all")
                            {
                                foreach (var character in unApproved)
                                {
                                    character.Name = character.CustomName;
                                    character.CustomName = "";
                                }

                                ctx.SaveChanges();

                                return "Successfully approved all names!";
                            }

                            return string.Join("\n", unApproved.Select(s => s.CustomName)) + "\napprove <name> / all";
                        }

                        var selectedCharacter = ctx.Characters.FirstOrDefault(c => c.CustomName == arguments[1] && !c.NameRejected);

                        if (selectedCharacter == null)
                        {
                            return $"No unapproved character with name: \"{arguments[1]}\"";
                        }
                        
                        selectedCharacter.Name = selectedCharacter.CustomName;
                        selectedCharacter.CustomName = "";

                        ctx.SaveChanges();

                        return $"Successfully approved \"{selectedCharacter.Name}\"!";
                    }
                    
                case "reject":
                    using (var ctx = new UchuContext())
                    {
                        if (arguments.Length == 1 || arguments[1].ToLower() == "all")
                        {
                            var unApproved = ctx.Characters.Where(c => !c.NameRejected && c.Name != c.CustomName && !string.IsNullOrEmpty(c.CustomName));

                            if (arguments.Length == 2 && arguments[1] == "all")
                            {
                                foreach (var character in unApproved)
                                {
                                    character.NameRejected = true;
                                }

                                ctx.SaveChanges();

                                return "Successfully rejected all names!";
                            }

                            return string.Join("\n", unApproved.Select(s => s.CustomName)) + "\nreject <name> / all";
                        }

                        var selectedCharacter = ctx.Characters.FirstOrDefault(c => c.CustomName == arguments[1] && !c.NameRejected);

                        if (selectedCharacter == null)
                        {
                            return $"No unapproved character with name: \"{arguments[1]}\"";
                        }

                        selectedCharacter.NameRejected = true;
                        
                        ctx.SaveChanges();

                        return $"Successfully rejected \"{selectedCharacter.CustomName}\"!";
                    }
                    
                
                case "clear":
                    if (!fromConsole) return "This command is not supported in chat mode.";
                    Console.Clear();
                    return "";
                
                case "help":
                    const string help = "stop                        Stop the server\n" +
                                        "users                       Displays all users\n" + 
                                        "adduser <name>              Add a new user\n" +
                                        "deleteuser <name>           Delete user\n" +
                                        "suspend <name> <reason>     Suspend a user\n" +
                                        "pardon <name>               Pardon a user\n" +
                                        "approve <name> / all        Approve names\n" +
                                        "reject <name> / all         Reject names\n" +
                                        "help                        Displays this message";
                    return help;
                default:
                    return "Unknown command, type \"help\" for a list of commands.";
            }
        }

        public static string GetPassword()
        {
            var pwd = new StringBuilder();
            while (true)
            {
                var i = Console.ReadKey(true);
                if (i.Key == ConsoleKey.Enter)
                {
                    break;
                }

                if (i.Key == ConsoleKey.Backspace)
                {
                    if (pwd.Length <= 0) continue;
                    pwd.Length--;
                    Console.Write("\b \b");
                }
                else if (i.KeyChar != '\u0000')
                {
                    pwd.Append(i.KeyChar);
                    Console.Write("*");
                }
            }

            return pwd.ToString();
        }

        #endregion

        #region Management

        public void DisconnectClient(IPEndPoint endPoint, DisconnectId disconnectId = DisconnectId.Kick) =>
            Send(new DisconnectNotifyPacket {DisconnectId = disconnectId}, endPoint);

        #endregion

        #region Send

        public void Send(byte[] data, IPEndPoint endPoint) => RakNetServer.Send(data, endPoint);

        public void Send(Stream stream, IPEndPoint endPoint) => RakNetServer.Send(stream, endPoint);

        public void Send(ISerializable serializable, IPEndPoint endPoint)
        {
            var stream = new MemoryStream();
            using (var writer = new BitWriter(stream))
            {
                writer.Write(serializable);
            }
            
            RakNetServer.Send(stream, endPoint);
            
            Logger.Debug($"Sent {serializable} to {endPoint}");
        }
        
        #endregion

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
                    if (attr == null) continue;

                    var parameters = method.GetParameters();
                    if (parameters.Length == 0 ||
                        !typeof(IPacket).IsAssignableFrom(parameters[0].ParameterType)) continue;
                    var packet = (IPacket) Activator.CreateInstance(parameters[0].ParameterType);

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

        public void HandlePacket(IPEndPoint endPoint, byte[] data)
        {
            Logger.Debug($"Received packet from {endPoint}");
            var stream = new MemoryStream(data);

            using (var reader = new BitReader(stream))
            {
                var header = new PacketHeader();
                reader.Read(header);

                if (header.MessageId != MessageIdentifiers.UserPacketEnum)
                    throw new ArgumentOutOfRangeException($"Packet is not {nameof(MessageIdentifiers.UserPacketEnum)}");

                if (header.PacketId == 0x05)
                {
                    //
                    // Game Message
                    //
                    
                    var objectId = reader.Read<long>();
                    var messageId = reader.Read<ushort>();

                    OnGameMessage?.Invoke(objectId, messageId, reader, endPoint);

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
                    InvokeHandler(handler, endPoint);
                    Logger.Debug($"Invoked handler for packet {handler.Packet.GetType().FullName}");
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
        }

        private static void InvokeHandler(Handler handler, IPEndPoint endPoint)
        {
            var task = handler.Info.ReturnType == typeof(Task);

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