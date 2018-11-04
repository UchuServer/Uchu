using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using RakDotNet;

namespace Uchu.Core
{
    using HandlerMap = Dictionary<RemoteConnectionType, Dictionary<uint, List<Handler>>>;

    public class Server
    {
        private readonly RakNetServer _server;
        private readonly HandlerMap _handlerMap;

        public int Port { get; }
        public ICache Cache { get; }

        public Server(int port)
        {
            _server = new RakNetServer(port, "3.25 ND1");
            _handlerMap = new HandlerMap();
            Port = port;
            Cache = new RedisCache();

            _server.PacketReceived += _handlePacket;

            RegisterAssembly(Assembly.GetExecutingAssembly());
        }

        public void Start() => _server.Start();

        public void Stop() => _server.Stop();

        public void Send(IPacket packet, IPEndPoint endpoint)
        {
            var stream = new BitStream();

            stream.WriteSerializable(packet);

            _server.Send(stream, endpoint);

            Console.WriteLine($"Sent {packet}");
        }

        public void Send(byte[] data, IPEndPoint endpoint)
            => _server.Send(data, endpoint);

        public void Send(BitStream stream, IPEndPoint endpoint)
            => _server.Send(stream, endpoint);

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

                    if (!_handlerMap.ContainsKey(rct))
                        _handlerMap[rct] = new Dictionary<uint, List<Handler>>();

                    var handlers = _handlerMap[rct];

                    if (!handlers.ContainsKey(packetId))
                        handlers[packetId] = new List<Handler>();

                    Console.WriteLine($"Registered handler for packet {packet}");

                    handlers[packetId].Add(new Handler
                    {
                        Group = group,
                        Method = method,
                        Packet = packet
                    });
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

            if (!_handlerMap.TryGetValue(header.RemoteConnectionType, out var temp) ||
                !temp.TryGetValue(header.PacketId, out var handlers)) return;

            Console.WriteLine($"Received {handlers[0].Packet}");

            foreach (var handler in handlers)
            {
                try
                {
                    stream.ReadSerializable(handler.Packet);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    continue;
                }

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
}