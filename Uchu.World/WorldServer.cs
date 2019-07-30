using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using RakDotNet;
using RakDotNet.IO;
using Uchu.Core;

namespace Uchu.World
{
    using GameMessageHandlerMap = Dictionary<ushort, List<Handler>>;
    
    public class WorldServer : Server
    {
        private readonly GameMessageHandlerMap _gameMessageHandlerMap;
        
        public readonly ReplicaManager ReplicaManager;
        
        public WorldServer(int port, string password = "3.25 ND1") : base(port, password)
        {
            ReplicaManager = new ReplicaManager(RakNetServer);
            
            _gameMessageHandlerMap = new GameMessageHandlerMap();

            OnGameMessage += HandleGameMessage;
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
                        var msg = (IGameMessage) packet;

                        if (!_gameMessageHandlerMap.ContainsKey(msg.GameMessageId))
                            _gameMessageHandlerMap[msg.GameMessageId] = new List<Handler>();

                        _gameMessageHandlerMap[msg.GameMessageId].Add(new Handler
                        {
                            Group = instance,
                            Info = method,
                            Packet = packet
                        });

                        Logger.Debug($"Registered handler for game message {packet}");

                        continue;
                    }

                    var remoteConnectionType = attr.RemoteConnectionType ?? packet.RemoteConnectionType;
                    var packetId = attr.PacketId ?? packet.PacketId;

                    if (!HandlerMap.ContainsKey(remoteConnectionType))
                        HandlerMap[remoteConnectionType] = new Dictionary<uint, List<Handler>>();

                    var handlers = HandlerMap[remoteConnectionType];

                    if (!handlers.ContainsKey(packetId))
                        handlers[packetId] = new List<Handler>();

                    handlers[packetId].Add(new Handler
                    {
                        Group = instance,
                        Info = method,
                        Packet = packet,
                        RunTask = attr.RunTask
                    });

                    Logger.Debug($"Registered handler for packet {packet}");
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
                    
            Logger.Debug($"Received {messageHandler[0].Packet.GetType().FullName}");

            foreach (var handler in messageHandler)
            {
                reader.BaseStream.Position = 18;

                ((IGameMessage) handler.Packet).ObjectId = objectId;

                try
                {
                    reader.Read(handler.Packet);
                    // TODO: Invoke handler
                    Logger.Debug($"Invoked handler for GameMessage {messageHandler[0].Packet.GetType().FullName}");
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