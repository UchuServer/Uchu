using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Threading.Tasks;
using System.Timers;
using InfectedRose.Luz;
using InfectedRose.Lvl;
using InfectedRose.Utilities;
using RakDotNet;
using RakDotNet.IO;
using Uchu.Core;
using Uchu.Physics;
using Uchu.Python;
using Uchu.World.Client;
using Uchu.World.Scripting;
using Uchu.World.Systems.AI;

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
        
        public new UchuServer UchuServer { get; }
        
        public uint Checksum { get; private set; }
        
        public bool Loaded { get; private set; }
        
        public NavMeshManager NavMeshManager { get; private set; }

        //
        // Managed objects
        //

        private List<Object> ManagedObjects { get; }
        
        private List<GameObject> SpawnedObjects { get; }

        //
        // Macro properties
        //

        public Object[] Objects => ManagedObjects.ToArray();

        public GameObject[] GameObjects => Objects.OfType<GameObject>().ToArray();
        
        public Player[] Players => Objects.OfType<Player>().ToArray();

        public GameObject[] Spawned => SpawnedObjects.ToArray();
        
        public ZoneId ZoneId { get; private set; }
        
        public Vector3 SpawnPosition { get; private set; }
        
        public Quaternion SpawnRotation { get; private set; }

        public GameObject ZoneControlObject { get; private set; }

        //
        // Runtime
        //
        
        private long _passedTickTime;
        private bool _running;
        private int _ticks;
        private long _physicsTime;
        
        public float DeltaTime { get; private set; }
        
        public ScriptManager ScriptManager { get; }
        
        public ManagedScriptEngine ManagedScriptEngine { get; }
        
        private List<UpdatedObject> UpdatedObjects { get; }
        
        //
        // Physics
        //
        
        public PhysicsSimulation Simulation { get; }
        
        public Event EarlyPhysics { get; }
        
        public Event LatePhysics { get; }
        
        //
        // Events
        //

        public Event<Player> OnPlayerLoad { get; } 

        public Event<Object> OnObject { get; }

        public Event OnTick { get; }
        
        public Event<Player, string> OnChatMessage { get; }
        
        public Zone(ZoneInfo zoneInfo, UchuServer uchuServer, ushort instanceId = default, uint cloneId = default)
        {
            Zone = this;
            ZoneInfo = zoneInfo;
            UchuServer = uchuServer;
            InstanceId = instanceId;
            CloneId = cloneId;
            
            EarlyPhysics = new Event();
            LatePhysics = new Event();
            OnPlayerLoad = new Event<Player>();
            OnObject = new Event<Object>();
            OnTick = new Event();
            OnChatMessage = new Event<Player, string>();

            ScriptManager = new ScriptManager(this);
            ManagedScriptEngine = new ManagedScriptEngine();
            UpdatedObjects = new List<UpdatedObject>();
            ManagedObjects = new List<Object>();
            SpawnedObjects = new List<GameObject>();
            Simulation = new PhysicsSimulation();

            Listen(OnDestroyed,() => { _running = false; });
            Listen(OnTick, PhysicsStep);
        }

        #region Initializing

        public async Task<Task> InitializeAsync()
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

            NavMeshManager = new NavMeshManager(this, UchuServer.Config.GamePlay.PathFinding);

            if (NavMeshManager.Enabled)
            {
                Logger.Information("Generating navigation way points.");

                await NavMeshManager.GeneratePointsAsync();
            
                Logger.Information("Finished generating navigation way points.");
            }

            ZoneId = (ZoneId) ZoneInfo.LuzFile.WorldId;

            SpawnPosition = ZoneInfo.LuzFile.SpawnPoint;
            
            SpawnRotation = ZoneInfo.LuzFile.SpawnRotation;

            ZoneInfo.LvlFiles = new List<LvlFile>();

            ZoneInfo.TerrainFile = default;

            GC.Collect();

            foreach (var levelObject in objects)
            {
                if (levelObject.LegoInfo.TryGetValue("trigger_id", out var trigger))
                {
                    Logger.Information($"Trigger: {trigger}");
                }
                
                Logger.Debug($"Loading {levelObject.Lot} [{levelObject.ObjectId}]...");
                
                try
                {
                    SpawnLevelObject(levelObject);
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }


            using var ctx = new Uchu.Core.Client.CdClientContext();

            int? ZoneControlLot = ctx.ZoneTableTable.FirstOrDefault(o => o.ZoneID == this.ZoneId.Id).ZoneControlTemplate;

            int Lot = ZoneControlLot ??= 2365;

            var ZoneObject = GameObject.Instantiate(this, lot: Lot, objectId: 70368744177662);

            Start(ZoneObject);

            Objects.Append(ZoneObject);

            ZoneControlObject = ZoneObject;

            Logger.Information($"Loaded {GameObjects.Length}/{objects.Count} for {ZoneId}");
            
            if (ZoneInfo.LuzFile.PathData != default)
            {
                foreach (var path in ZoneInfo.LuzFile.PathData.OfType<LuzSpawnerPath>())
                {
                    Logger.Information($"Loading {path.PathName}");
                    
                    try
                    {
                        SpawnPath(path);
                    }
                    catch (Exception e)
                    {
                        Logger.Error(e);
                    }
                }
            }
            
            Logger.Information($"Loaded {Objects.Length} objects for {ZoneId}");

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
            
            objects.Clear();

            return ExecuteUpdateAsync();
        }

        private void SpawnLevelObject(LevelObjectTemplate levelObject)
        {
            var obj = GameObject.Instantiate(levelObject, this);

            if (levelObject.LegoInfo.TryGetValue("loadSrvrOnly", out var serverOnly) && (bool) serverOnly ||
                levelObject.LegoInfo.TryGetValue("carver_only", out var carverOnly) && (bool) carverOnly ||
                levelObject.LegoInfo.TryGetValue("renderDisabled", out var disabled) && (bool) disabled)
            {
                obj.Layer = StandardLayer.Hidden;
            }
            
            if (!obj.TryGetComponent(out SpawnerComponent spawner))
            {
                obj.Layer = StandardLayer.Hidden;
            }

            Start(obj);
            
            //
            // Only spawns should get constructed on the client.
            //

            spawner?.SpawnCluster();
        }

        private void SpawnPath(LuzSpawnerPath spawnerPath)
        {
            if (spawnerPath.ActivateSpawnerNetworkOnLoad)
            {
                var obj = InstancingUtilities.Spawner(spawnerPath, this);

                if (obj == null) return;

                obj.Layer = StandardLayer.Hidden;

                var spawner = obj.GetComponent<SpawnerComponent>();

                spawner.SpawnsToMaintain = (int)spawnerPath.NumberToMaintain;

                spawner.SpawnLocations = spawnerPath.Waypoints.Select(w => new SpawnLocation
                {
                    Position = w.Position,
                    Rotation = Quaternion.Identity
                }).ToList();

                Start(obj);

                spawner.SpawnCluster();
            }
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

        public void BroadcastChatMessage(string message, Player author = null)
        {
            foreach (var player in Players) player.Message(new ChatMessagePacket
            {
                Message = $"{message}\0",
                Sender = author,
                IsMythran = author?.GameMasterLevel > 0,
                Channel = World.ChatChannel.Public
            });
        }

        #endregion

        #region Object Finder

        public GameObject GetGameObject(long objectId)
        {
            return objectId == -1 ? null : GameObjects.First(o => o.Id == objectId);
        }

        public bool TryGetGameObject(long objectId, out GameObject result)
        {
            result = GameObjects.FirstOrDefault(o => o.Id == objectId);
            return result != default;
        }

        public T GetGameObject<T>(long objectId) where T : GameObject
        {
            return GameObjects.OfType<T>().First(o => o.Id == objectId);
        }

        public bool TryGetGameObject<T>(long objectId, out T result) where T : GameObject
        {
            result = GameObjects.FirstOrDefault(o => o.Id == objectId) as T;
            return result != null;
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
            lock (ManagedObjects)
            {
                if (ManagedObjects.Contains(obj)) return;
            
                OnObject.Invoke(obj);

                ManagedObjects.Add(obj);

                if (!(obj is GameObject gameObject)) return;
                
                if ((gameObject.Id.Flags & ObjectIdFlags.Spawned) != 0)
                {
                    SpawnedObjects.Add(gameObject);
                }
            }
        }

        internal void UnregisterObject(Object obj)
        {
            lock (ManagedObjects)
            {
                if (!ManagedObjects.Contains(obj)) return;
                
                ManagedObjects.Remove(obj);

                if (obj is GameObject gameObject)
                {
                    if ((gameObject.Id.Flags & ObjectIdFlags.Spawned) != 0)
                    {
                        SpawnedObjects.Remove(gameObject);
                    }
                }

                var updated = UpdatedObjects.FirstOrDefault(u => u.Associate == obj);
                
                if (updated == default) return;

                UpdatedObjects.Remove(updated);
            }
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

                File.WriteAllBytes(Path.Combine(gameObject.UchuServer.MasterPath, path), content);
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

                File.WriteAllBytes(Path.Combine(gameObject.UchuServer.MasterPath, path), content);
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
                    Logger.Debug($"TPS: {_ticks}/{TicksPerSecondLimit} TPT: {_passedTickTime / _ticks} ms PHY: {_physicsTime / _ticks} ms");
                _passedTickTime = 0;
                _ticks = 0;
            };

            timer.Start();

            return Task.Run(async () =>
            {
                _running = true;
                var watch = new Stopwatch();
                
                while (_running)
                {
                    var players = Players;
                    if (players.Length == 0)
                    {
                        await Task.Delay(1000);
                        continue;
                    }

                    if (_ticks >= TicksPerSecondLimit)
                        continue;

                    watch.Restart();

                    // Update all tick tasks in parallel
                    var tickTasks = new List<Task>();
                    foreach (var updatedObject in UpdatedObjects.ToArray())
                    {
                        if (updatedObject.Stuck)
                            continue;
                        
                        if (updatedObject.Associate is GameObject gameObject)
                        {
                            if (players.All(p => !p.Perspective.LoadedObjects.Contains(gameObject)))
                                continue;
                        }

                        updatedObject.DeltaTime += DeltaTime;
                        if (updatedObject.Frequency != ++updatedObject.Ticks)
                            continue;
                        updatedObject.Ticks = 0;

                        tickTasks.Add(Task.Run(async () =>
                        {
                            var start = watch.ElapsedMilliseconds;
                            try
                            {
                                var tickTask = Task.Run(async () =>
                                {
                                    await updatedObject.Delegate(updatedObject.DeltaTime);
                                    updatedObject.Stuck = false;
                                });

                                var delay = Task.Delay(150);
                                await Task.WhenAny(tickTask, delay);
                                var elapsed = watch.ElapsedMilliseconds - start;

                                // Any task that takes more than 3 ticks to complete is considered stuck
                                if (delay.IsCompleted && !tickTask.IsCompleted)
                                {
                                    Logger.Warning($"{updatedObject.Associate} is now defined as stuck!");
                                    updatedObject.Stuck = true;
                                }
                                // Any task that took more than two ticks to execute but less than 3 is considered slow
                                else if (elapsed > 100)
                                {
                                    Logger.Warning($"Slow update: {updatedObject.Associate} in {elapsed}ms");
                                }
                            }
                            catch (Exception e)
                            {
                                Logger.Error(e);
                            }

                            updatedObject.DeltaTime = 0;
                        }));
                    }

                    await Task.WhenAll(tickTasks);
                    await OnTick.InvokeAsync();
                    
                    _ticks++;

                    var passedMs = watch.ElapsedMilliseconds;
                    _passedTickTime += passedMs;
                    
                    // Sync with the system tick clock
                    await Task.Delay((int) Math.Max(1000 / TicksPerSecondLimit - passedMs, 0));
                    
                    passedMs = watch.ElapsedMilliseconds;
                    DeltaTime = passedMs / 1000f;
                }
            });
        }

        private async Task PhysicsStep()
        {
            var watch = new Stopwatch();
            
            watch.Start();
            
            await EarlyPhysics.InvokeAsync();

            Simulation.Step(DeltaTime);
            
            await LatePhysics.InvokeAsync();
            
            watch.Stop();

            _physicsTime += watch.ElapsedMilliseconds;
        }

        public void Update(Object associate, Func<Task> @delegate, int frequency)
        {
            UpdatedObjects.Add(new UpdatedObject
            {
                Associate = associate,
                Delegate = _ => @delegate(),
                Frequency = frequency
            });
        }
        
        public void Update(Object associate, Func<float, Task> @delegate, int frequency)
        {
            UpdatedObjects.Add(new UpdatedObject
            {
                Associate = associate,
                Delegate = @delegate,
                Frequency = frequency
            });
        }
        
        private class UpdatedObject
        {
            public Object Associate { get; set; }
            
            public Func<float, Task> Delegate { get; set; }
            
            public int Frequency { get; set; }
            
            public int Ticks { get; set; }
            
            public float DeltaTime { get; set; }
            
            public bool Stuck { get; set; }
        }

        #endregion
    }
}