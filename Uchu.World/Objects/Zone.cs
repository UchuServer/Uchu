using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using System.Timers;
using RakDotNet;
using RakDotNet.IO;
using Uchu.Core;
using Uchu.World.Parsers;

namespace Uchu.World
{
    public class Zone : Object
    {
        private readonly List<Object> _objects = new List<Object>();

        private readonly List<GameObject> _gameObjects = new List<GameObject>();

        private readonly List<Player> _players = new List<Player>();

        public readonly ZoneInfo ZoneInfo;

        public readonly ushort InstanceId;

        public readonly uint CloneId;

        public new readonly Server Server;
        
        private int _ticks;

        private long _passedTickTime;

        /// <summary>
        /// Fractions of a second passed during the last tick. Should be used to fit actions with the TPS
        /// </summary>
        public float DeltaTime { get; private set; }
        
        /// <summary>
        /// This should be set to something the server can sustain.
        /// </summary>
        public const int TicksPerSecondLimit = 20;

        public ZoneId ZoneId => (ZoneId) ZoneInfo.ZoneId;

        public Object[] Objects => _objects.ToArray();

        public GameObject[] GameObjects => _gameObjects.ToArray();

        public Player[] Players => _players.ToArray();

        public Zone(ZoneInfo zoneInfo, Server server, ushort instanceId = default, uint cloneId = default)
        {
            Zone = this;
            ZoneInfo = zoneInfo;
            Server = server;
            InstanceId = instanceId;
            CloneId = cloneId;
        }

        public void Initialize()
        {
            var objects = ZoneInfo.ScenesInfo.SelectMany(s => s.Objects).ToArray();
            
            Logger.Information($"Loading {objects.Length} objects for {ZoneId}");
            
            foreach (var levelObject in objects)
            {
                var obj = GameObject.Instantiate(levelObject, this);

                if (levelObject.Settings.TryGetValue("loadSrvrOnly", out var serverOnly) && (bool) serverOnly ||
                    levelObject.Settings.TryGetValue("carver_only", out var carverOnly) && (bool) carverOnly ||
                    levelObject.Settings.TryGetValue("renderDisabled", out var disabled) && (bool) disabled)
                {
                    obj.Layer = Layer.Hidden;
                }

                Start(obj);
                
                if (obj.GetType().GetCustomAttribute<UnconstructedAttribute>() == null) GameObject.Construct(obj);

                if (obj.TryGetComponent<SpawnerComponent>(out var spawner))
                {
                    spawner.Spawn();
                }
            }
            
            Logger.Information($"Loaded {objects.Length} objects for {ZoneId}");

            Task.Run(async () => { await ExecuteUpdate(); });
        }

        #region Object Mangement

        public void RequestConstruction(Player player)
        {
            _players.Add(player);

            foreach (var gameObject in GameObjects)
            {
                if (gameObject.GetType().GetCustomAttribute<UnconstructedAttribute>() != null) continue;
                
                SendConstruction(gameObject, new [] {player});
            }
        }

        public void RegisterObject(Object obj)
        {
            _objects.Add(obj);
        }
        
        public void UnRegisterObject(Object obj)
        {
            _objects.Remove(obj);
        }
        
        /// <summary>
        /// Add a GameObject to the managed GameObjects of this zone.
        /// </summary>
        /// <param name="gameObject">Unmanaged GameObject</param>
        public void RegisterGameObject(GameObject gameObject)
        {
            RegisterObject(gameObject);
            
            _gameObjects.Add(gameObject);
        }

        /// <summary>
        /// Remove a GameObject from the managed GameObjects of this zone.
        /// </summary>
        /// <param name="gameObject">Managed GameObject</param>
        public void UnRegisterGameObject(GameObject gameObject)
        {
            UnRegisterObject(gameObject);
            
            _gameObjects.Remove(gameObject);
            
            if (gameObject is Player player) _players.Remove(player);
        }

        public void SendConstruction(GameObject gameObject)
        {
            SendConstruction(gameObject, _players);
        }

        public void SendConstruction(GameObject gameObject, Player recipient)
        {
            SendConstruction(gameObject, new[] {recipient});
        }
        
        public void SendConstruction(GameObject gameObject, IEnumerable<Player> recipients)
        {
            foreach (var recipient in recipients)
            {
                recipient.Perspective.Reveal(gameObject, id =>
                {
                    var stream = new MemoryStream();
            
                    using (var writer = new BitWriter(stream))
                    {
                        writer.Write((byte) MessageIdentifiers.ReplicaManagerConstruction);
                    
                        writer.WriteBit(true);
                        writer.Write(id);

                        gameObject.WriteConstruct(writer);
                    }

                    Server.Send(stream, recipient.EndPoint);
                });
            }
        }

        public void SendSerialization(GameObject gameObject)
        {
            SendSerialization(gameObject, _players);
        }
        
        public void SendSerialization(GameObject gameObject, Player player)
        {
            SendSerialization(gameObject, new[] {player});
        }
        
        public void SendSerialization(GameObject gameObject, IEnumerable<Player> recipients)
        {
            foreach (var recipient in recipients)
            {
                if (!recipient.Perspective.TryGetNetworkId(gameObject, out var id)) continue;
                
                var stream = new MemoryStream();
                
                using (var writer = new BitWriter(stream))
                {
                    writer.Write((byte) MessageIdentifiers.ReplicaManagerSerialize);
                    
                    writer.Write(id);

                    gameObject.WriteSerialize(writer);
                }

                Server.Send(stream, recipient.EndPoint);
            }
        }

        public void SendDestruction(GameObject gameObject)
        {
            SendDestruction(gameObject, _players);
        }
        
        public void SendDestruction(GameObject gameObject, Player player)
        {
            SendDestruction(gameObject, new[] {player});
        }
        
        public void SendDestruction(GameObject gameObject, IEnumerable<Player> recipients)
        {
            foreach (var recipient in recipients)
            {
                if (!recipient.Perspective.TryGetNetworkId(gameObject, out var id)) continue;
                
                var stream = new MemoryStream();
            
                using (var writer = new BitWriter(stream))
                {
                    writer.Write((byte) MessageIdentifiers.ReplicaManagerDestruction);
                    
                    writer.Write(id);
                }

                Server.Send(stream, recipient.EndPoint);

                recipient.Perspective.Drop(gameObject);
            }
        }

        #endregion

        public void SelectiveMessage(IGameMessage message, IEnumerable<Player> players)
        {
            foreach (var player in players)
            {
                player.Message(message);
            }
        }

        public void ExcludingMessage(IGameMessage message, Player excluded)
        {
            foreach (var player in _players.Where(p => p != excluded))
            {
                player.Message(message);
            }
        }
        
        public void BroadcastMessage(IGameMessage message)
        {
            foreach (var player in _players)
            {
                player.Message(message);
            }
        }
        
        public GameObject GetGameObject(long objectId)
        {
            return objectId == -1 ? null : _gameObjects.First(o => o.ObjectId == objectId);
        }
        
        public bool TryGetGameObject(long objectId, out GameObject result)
        {
            result = _gameObjects.FirstOrDefault(o => o.ObjectId == objectId);
            return result != default;
        }
        
        public T GetGameObject<T>(long objectId) where T : GameObject
        {
            return _gameObjects.OfType<T>().First(o => o.ObjectId == objectId);
        }
        
        public bool TryGetGameObject<T>(long objectId, out T result) where T : GameObject
        {
            result = _gameObjects.FirstOrDefault(o => o.ObjectId == objectId) as T;
            return result != default;
        }
        
        public static ZoneChecksum GetChecksum(ZoneId id)
        {
            var name = Enum.GetName(typeof(ZoneId), id);

            var names = Enum.GetNames(typeof(ZoneChecksum));
            var values = Enum.GetValues(typeof(ZoneChecksum));

            return (ZoneChecksum) values.GetValue(names.ToList().IndexOf(name));
        }

        private async Task ExecuteUpdate()
        {
            var timer = new Timer(1000);

            timer.Elapsed += (sender, args) =>
            {
                Logger.Debug($"TPS: {_ticks}/{TicksPerSecondLimit} TPT: {_passedTickTime / _ticks} ms");
                _passedTickTime = default;
                _ticks = default;
            };
            
            timer.Start();

            var stopWatch = new Stopwatch();

            stopWatch.Start();
            
            while (true)
            {
                if (_ticks >= TicksPerSecondLimit) continue;
                
                await Task.Delay(1000 / TicksPerSecondLimit);

                Task.WaitAll(_objects.Select(o => Task.Run(() =>
                {
                    try
                    {
                        Update(o);
                    }
                    catch (Exception e)
                    {
                        Logger.Error(e);
                    }
                })).ToArray());
                
                _ticks++;

                var passedMs = stopWatch.ElapsedMilliseconds;
                
                DeltaTime = passedMs / 1000f;
                
                _passedTickTime += passedMs;

                stopWatch.Restart();
            }
        }
    }
}