using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Timers;
using RakDotNet;
using RakDotNet.IO;
using Uchu.Core;
using Uchu.World.Parsers;
using Uchu.World.Scripting;

namespace Uchu.World
{
    public partial class Zone : Object
    {
        /// <summary>
        ///     This should be set to something the server can sustain.
        /// </summary>
        public const int TicksPerSecondLimit = 20;

        private readonly List<GameObject> _gameObjects = new List<GameObject>();
        private readonly List<Object> _objects = new List<Object>();
        private readonly List<Player> _players = new List<Player>();

        public readonly uint CloneId;

        public readonly ushort InstanceId;

        public new readonly Server Server;

        public readonly ZoneInfo ZoneInfo;

        private long _passedTickTime;

        private bool _running;

        private int _ticks;

        public Zone(ZoneInfo zoneInfo, Server server, ushort instanceId = default, uint cloneId = default)
        {
            Zone = this;
            ZoneInfo = zoneInfo;
            Server = server;
            InstanceId = instanceId;
            CloneId = cloneId;

            OnDestroyed.AddListener(() => { _running = false; });
        }

        /// <summary>
        ///     Fractions of a second passed during the last tick. Should be used to fit actions with the TPS
        /// </summary>
        public float DeltaTime { get; private set; }

        public ZoneId ZoneId => (ZoneId) ZoneInfo.ZoneId;

        public Object[] Objects => _objects.ToArray();

        public GameObject[] GameObjects => _gameObjects.ToArray();

        public Player[] Players => _players.ToArray();
        
        public bool Loaded { get; private set; }

        public async Task InitializeAsync()
        {
            var objects = ZoneInfo.ScenesInfo.SelectMany(s => s.Objects).ToArray();

            Logger.Information($"Loading {objects.Length} objects for {ZoneId}");

            foreach (var levelObject in objects)
            {
                try
                {
                    SpawnLevelObject(levelObject);
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }

            Logger.Information($"Loaded {objects.Length} objects for {ZoneId}");

            await LoadScripts();
            
            Loaded = true;

            var _ = ExecuteUpdateAsync();
        }

        private void SpawnLevelObject(LevelObject levelObject)
        {
            var obj = GameObject.Instantiate(levelObject, this);

            SpawnerComponent spawner = default;
                
            if (levelObject.Settings.TryGetValue("loadSrvrOnly", out var serverOnly) && (bool) serverOnly ||
                levelObject.Settings.TryGetValue("carver_only", out var carverOnly) && (bool) carverOnly ||
                levelObject.Settings.TryGetValue("renderDisabled", out var disabled) && (bool) disabled)
            {
                obj.Layer = Layer.Hidden;
            }
            
            //
            // Only spawns should get constructed on the client.
            //
            
            else if (!obj.TryGetComponent(out spawner))
            {
                obj.Layer = Layer.Hidden;
            }

            Start(obj);
            
            if (obj.Layer == Layer.Hidden) return;

            if (spawner == default)
            {
                Logger.Warning($"{obj} is not a spawner but got qualified to spawn");
                
                return;
            }
            
            GameObject.Construct(spawner.Spawn());
        }

        public void SelectiveMessage(IGameMessage message, IEnumerable<Player> players)
        {
            foreach (var player in players) player.Message(message);
        }

        public void ExcludingMessage(IGameMessage message, Player excluded)
        {
            foreach (var player in _players.Where(p => p != excluded)) player.Message(message);
        }

        public void BroadcastMessage(IGameMessage message)
        {
            foreach (var player in _players) player.Message(message);
        }

        public GameObject GetGameObject(long objectId)
        {
            return objectId == -1 ? null : _gameObjects.FirstOrDefault(o => o.ObjectId == objectId);
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
            result = _gameObjects.OfType<T>().FirstOrDefault(o => o.ObjectId == objectId);
            return result != default;
        }

        public static ZoneChecksum GetChecksum(ZoneId id)
        {
            var name = Enum.GetName(typeof(ZoneId), id);

            var names = Enum.GetNames(typeof(ZoneChecksum));
            var values = Enum.GetValues(typeof(ZoneChecksum));

            return (ZoneChecksum) values.GetValue(names.ToList().IndexOf(name));
        }

        private Task ExecuteUpdateAsync()
        {
            var timer = new Timer
            {
                Interval = 1000,
                AutoReset = true
            };

            timer.Elapsed += (sender, args) =>
            {
                if (_ticks > 0)
                    Logger.Debug($"TPS: {_ticks}/{TicksPerSecondLimit} TPT: {_passedTickTime / _ticks} ms");
                _passedTickTime = 0;
                _ticks = 0;
                
                //
                // Set player count
                //

                var worldServer = (WorldServer) Server;

                worldServer.ActiveUserCount = (uint) worldServer.Zones.Sum(z => z.Players.Length);
            };

            timer.Start();

            return Task.Run(async () =>
            {
                var stopWatch = new Stopwatch();

                stopWatch.Start();

                _running = true;

                while (_running)
                {
                    if (_ticks >= TicksPerSecondLimit) continue;

                    await Task.Delay(1000 / TicksPerSecondLimit);

                    foreach (var obj in _objects)
                    {
                        try
                        {
                            Update(obj);
                        }
                        catch (Exception e)
                        {
                            Logger.Error(e);
                        }
                    }

                    _ticks++;

                    var passedMs = stopWatch.ElapsedMilliseconds;

                    DeltaTime = passedMs / 1000f;

                    _passedTickTime += passedMs;

                    stopWatch.Restart();
                }
            });
        }

        #region Object Mangement

        public void RequestConstruction(Player player)
        {
            _players.Add(player);

            foreach (var gameObject in GameObjects)
            {
                if (gameObject.GetType().GetCustomAttribute<UnconstructedAttribute>() != null) continue;

                SendConstruction(gameObject, new[] {player});
            }
        }

        public void RegisterObject(Object obj)
        {
            _objects.Add(obj);
        }

        public void UnregisterObject(Object obj)
        {
            if (_objects.Contains(obj)) _objects.Remove(obj);
        }

        /// <summary>
        ///     Add a GameObject to the managed GameObjects of this zone.
        /// </summary>
        /// <param name="gameObject">Unmanaged GameObject</param>
        public void RegisterGameObject(GameObject gameObject)
        {
            _gameObjects.Add(gameObject);
        }

        /// <summary>
        ///     Remove a GameObject from the managed GameObjects of this zone.
        /// </summary>
        /// <param name="gameObject">Managed GameObject</param>
        public void UnregisterGameObject(GameObject gameObject)
        {
            UnregisterObject(gameObject);

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
                    using var stream = new MemoryStream();
                    using var writer = new BitWriter(stream);
                    
                    writer.Write((byte) MessageIdentifier.ReplicaManagerConstruction);

                    writer.WriteBit(true);
                    writer.Write(id);

                    gameObject.WriteConstruct(writer);

                    recipient.Connection.Send(stream);
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

                using var stream = new MemoryStream();
                using var writer = new BitWriter(stream);
                
                writer.Write((byte) MessageIdentifier.ReplicaManagerSerialize);

                writer.Write(id);

                gameObject.WriteSerialize(writer);

                recipient.Connection.Send(stream);
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

                using (var stream = new MemoryStream())
                {
                    using var writer = new BitWriter(stream);
                    
                    writer.Write((byte) MessageIdentifier.ReplicaManagerDestruction);

                    writer.Write(id);

                    recipient.Connection.Send(stream);
                }

                recipient.Perspective.Drop(gameObject);
            }
        }

        #endregion
    }
}