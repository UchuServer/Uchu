using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Timers;
using InfectedRose.Luz;
using InfectedRose.Lvl;
using InfectedRose.Utilities;
using RakDotNet;
using RakDotNet.IO;
using Uchu.Core;
using Uchu.Python;
using Uchu.World.Client;
using Uchu.World.Scripting;

namespace Uchu.World
{
    public class Zone : Object
    {
        //
        // Consts
        //

        private const int TicksPerSecondLimit = 20;

        //
        // Zone info
        //

        public uint CloneId { get; }
        
        public ushort InstanceId { get; }
        
        public ZoneInfo ZoneInfo { get; }
        
        public new Server Server { get; }
        
        public uint Checksum { get; private set; }
        
        public bool Loaded { get; private set; }

        //
        // Managed objects
        //

        private readonly List<Object> _managedObjects = new List<Object>();

        //
        // Macro properties
        //

        public Object[] Objects => _managedObjects.ToArray();
        public GameObject[] GameObjects => Objects.OfType<GameObject>().ToArray();
        public Player[] Players => Objects.OfType<Player>().ToArray();
        public ZoneId ZoneId => (ZoneId) ZoneInfo.LuzFile.WorldId;

        //
        // Runtime
        //

        public float DeltaTime { get; private set; }
        private long _passedTickTime;
        private bool _running;
        private int _ticks;
        public ScriptManager ScriptManager { get; }
        public ManagedScriptEngine ManagedScriptEngine { get; }

        //
        // Events
        //

        public AsyncEvent<Player> OnPlayerLoad { get; } = new AsyncEvent<Player>();
        
        public AsyncEvent<Player, string> OnChatMessage { get; } = new AsyncEvent<Player, string>();

        public Zone(ZoneInfo zoneInfo, Server server, ushort instanceId = default, uint cloneId = default)
        {
            Zone = this;
            ZoneInfo = zoneInfo;
            Server = server;
            InstanceId = instanceId;
            CloneId = cloneId;
            
            ScriptManager = new ScriptManager(this);
            ManagedScriptEngine = new ManagedScriptEngine();

            Listen(OnStart,async () => await InitializeAsync());

            Listen(OnDestroyed,() => { _running = false; });
        }

        #region Initializing

        public async Task InitializeAsync()
        {
            Checksum = ZoneInfo.LuzFile.GenerateChecksum(ZoneInfo.LvlFiles);
            
            Logger.Information($"Checksum: 0x{Checksum:X}");
            
            Logger.Information($"Collecting objects for {ZoneId}");

            var objects = new List<LevelObjectTemplate>();

            foreach (var lvlFile in ZoneInfo.LvlFiles.Where(lvlFile => lvlFile.LevelObjects?.Templates != default))
            {
                objects.AddRange(lvlFile.LevelObjects.Templates);
            }

            Logger.Information($"Loading {objects.Count} objects for {ZoneId}");

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

            foreach (var path in ZoneInfo.LuzFile.PathData.OfType<LuzSpawnerPath>())
            {
                try
                {
                    SpawnPath(path);
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }

            Logger.Information($"Loaded {objects.Count} objects for {ZoneId}");

            //
            // Load zone scripts
            //

            await ScriptManager.LoadDefaultScriptsAsync();
            
            foreach (var scriptPack in ScriptManager.ScriptPacks)
            {
                try
                {
                    Logger.Information($"Running: {scriptPack.Name}");
                
                    await scriptPack.LoadAsync();
                }
                catch (Exception e)
                {
                    Logger.Information(e);
                }
            }
            
            Loaded = true;

            var _ = ExecuteUpdateAsync();
        }

        private void SpawnLevelObject(LevelObjectTemplate levelObject)
        {
            var obj = GameObject.Instantiate(levelObject, this);

            SpawnerComponent spawner = default;

            if (levelObject.LegoInfo.TryGetValue("loadSrvrOnly", out var serverOnly) && (bool) serverOnly ||
                levelObject.LegoInfo.TryGetValue("carver_only", out var carverOnly) && (bool) carverOnly ||
                levelObject.LegoInfo.TryGetValue("renderDisabled", out var disabled) && (bool) disabled)
            {
                obj.Layer = StandardLayer.Hidden;
            }
            else if (!obj.TryGetComponent(out spawner))
            {
                obj.Layer = StandardLayer.Hidden;
            }

            Start(obj);
            
            //
            // Only spawns should get constructed on the client.
            //
            
            if (spawner == default)
            {
                return;
            }

            GameObject.Construct(spawner.Spawn());
        }

        private void SpawnPath(LuzSpawnerPath spawnerPath)
        {
            var obj = InstancingUtil.Spawner(spawnerPath, this);

            if (obj == null) return;

            obj.Layer = StandardLayer.Hidden;
            
            Start(obj);
            
            var spawn = obj.GetComponent<SpawnerComponent>().Spawn();

            GameObject.Construct(spawn);
        }

        #endregion

        #region Messages

        public void SelectiveMessage(IGameMessage message, IEnumerable<Player> players)
        {
            foreach (var player in players) player.Message(message);
        }

        public void ExcludingMessage(IGameMessage message, Player excluded)
        {
            foreach (var player in Players.Where(p => p != excluded)) player.Message(message);
        }

        public void BroadcastMessage(IGameMessage message)
        {
            foreach (var player in Players) player.Message(message);
        }

        #endregion

        #region Object Finder

        public GameObject GetGameObject(long objectId)
        {
            return objectId == -1 ? null : GameObjects.First(o => o.ObjectId == objectId);
        }

        public bool TryGetGameObject(long objectId, out GameObject result)
        {
            result = GameObjects.FirstOrDefault(o => o.ObjectId == objectId);
            return result != default;
        }

        public T GetGameObject<T>(long objectId) where T : GameObject
        {
            return GameObjects.OfType<T>().First(o => o.ObjectId == objectId);
        }

        public bool TryGetGameObject<T>(long objectId, out T result) where T : GameObject
        {
            result = GameObjects.OfType<T>().FirstOrDefault(o => o.ObjectId == objectId);
            return result != default;
        }

        #endregion

        #region Object Mangement

        #region Register

        internal async Task RegisterPlayer(Player player)
        {
            await OnPlayerLoad.InvokeAsync(player);

            foreach (var gameObject in GameObjects)
            {
                if (gameObject.GetType().GetCustomAttribute<UnconstructedAttribute>() != null) continue;

                SendConstruction(gameObject, new[] {player});
            }
        }

        internal void RegisterObject(Object obj)
        {
            if (!_managedObjects.Contains(obj)) _managedObjects.Add(obj);
        }

        internal void UnregisterObject(Object obj)
        {
            if (_managedObjects.Contains(obj)) _managedObjects.Remove(obj);
        }

        #endregion

        #region Networking

        internal static void SendConstruction(GameObject gameObject, Player recipient)
        {
            SendConstruction(gameObject, new[] {recipient});
        }

        internal static void SendConstruction(GameObject gameObject, IEnumerable<Player> recipients)
        {
            foreach (var recipient in recipients)
            {
                if (!recipient.Perspective.View(gameObject)) continue;

                if (!recipient.Perspective.Reveal(gameObject, out var id)) continue;

                if (id == 0) return;

                using var stream = new MemoryStream();
                using var writer = new BitWriter(stream);

                writer.Write((byte) MessageIdentifier.ReplicaManagerConstruction);

                writer.WriteBit(true);
                writer.Write(id);

                gameObject.WriteConstruct(writer);

                recipient.Connection.Send(stream);
            }
        }

        internal static void SendSerialization(GameObject gameObject, IEnumerable<Player> recipients)
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

        internal static void SendDestruction(GameObject gameObject, Player player)
        {
            SendDestruction(gameObject, new[] {player});
        }

        internal static void SendDestruction(GameObject gameObject, IEnumerable<Player> recipients)
        {
            foreach (var recipient in recipients)
            {
                if (recipient.Perspective.View(gameObject)) continue;

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

        #endregion

        #region Debug
        
        internal static void SaveCreation(GameObject gameObject, IEnumerable<Player> recipients, string path)
        {
            foreach (var recipient in recipients)
            {
                if (!recipient.Perspective.TryGetNetworkId(gameObject, out var id)) continue;

                using var stream = new MemoryStream();
                using var writer = new BitWriter(stream);

                writer.Write((byte) MessageIdentifier.ReplicaManagerConstruction);

                writer.WriteBit(true);
                writer.Write(id);

                gameObject.WriteConstruct(writer);

                recipient.Connection.Send(stream);

                var content = stream.ToArray();

                File.WriteAllBytes(Path.Combine(gameObject.Server.MasterPath, path), content);
            }
        }
        
        internal static void SaveSerialization(GameObject gameObject, IEnumerable<Player> recipients, string path)
        {
            foreach (var recipient in recipients)
            {
                if (!recipient.Perspective.TryGetNetworkId(gameObject, out var id)) continue;

                using var stream = new MemoryStream();
                using var writer = new BitWriter(stream);

                writer.Write((byte) MessageIdentifier.ReplicaManagerSerialize);

                writer.Write(id);

                gameObject.WriteSerialize(writer);

                var content = stream.ToArray();

                File.WriteAllBytes(Path.Combine(gameObject.Server.MasterPath, path), content);
            }
        }

        #endregion
        
        #region Runtime

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

                    var players = Players;

                    foreach (var obj in Objects)
                    {
                        try
                        {
                            Update(obj);

                            if (!(obj is GameObject gameObject)) continue;
                            
                            if (obj is Item) continue;

                            foreach (var player in players)
                            {
                                var spawned = player.Perspective.LoadedObjects.ToArray().Contains(gameObject);

                                var view = player.Perspective.View(gameObject);
                                
                                if (spawned && !view)
                                {
                                    SendDestruction(gameObject, player);

                                    continue;
                                }

                                if (!spawned && view)
                                {
                                    SendConstruction(gameObject, player);
                                }
                            }
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

        #endregion
    }
}