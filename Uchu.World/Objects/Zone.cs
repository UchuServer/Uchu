using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Threading.Tasks;
using System.Timers;
using InfectedRose.Core;
using InfectedRose.Luz;
using InfectedRose.Lvl;
using InfectedRose.Utilities;
using RakDotNet;
using RakDotNet.IO;
using Sentry;
using Uchu.Api.Models;
using Uchu.Core;
using Uchu.Core.Client;
using Uchu.Core.Data;
using Uchu.Physics;
using Uchu.Python;
using Uchu.World.Client;
using Uchu.World.Objects.ReplicaManager;
using Uchu.World.Scripting;
using Uchu.World.Systems.AI;

namespace Uchu.World
{
    /// <summary>
    /// Represents a zone in a world server, for example Avant Gardens, Nimbus Station or Gnarled Forest
    /// </summary>
    /// <remarks>
    /// Runs a game loop that manages physics, AI and other scheduled events that might have been scheduled on received
    /// packets.
    /// </remarks>
    public class Zone : Object
    {
        private const int TicksPerSecondLimit = 20;
        private const float TimePerTickLimit = 1000f / TicksPerSecondLimit;
        
        public uint CloneId { get; }
        public ushort InstanceId { get; }
        public ZoneInfo ZoneInfo { get; }
        public WorldUchuServer Server { get; }
        public uint Checksum { get; private set; }
        public bool Loaded { get; private set; }
        public NavMeshManager NavMeshManager { get; private set; }
        private Timer GameLoop { get; set; }
        
        // Managed objects
        private ThreadedCollection<Object> ManagedObjects { get; }
        private ThreadedCollection<GameObject> SpawnedObjects { get; }
        public ThreadedCollection<SpawnerNetwork> SpawnerNetworks { get; }
        
        // Macro properties
        public Object[] Objects => this.ManagedObjects.GetArray((list) => list.ToArray());
        public GameObject[] GameObjects => this.ManagedObjects.GetArray((list) => list.OfType<GameObject>().ToArray());
        public Player[] Players => this.ManagedObjects.GetArray((list) => list.OfType<Player>().ToArray());
        public GameObject[] Spawned => SpawnedObjects.ToArray();
        public ZoneId ZoneId { get; private set; }
        public Vector3 SpawnPosition { get; private set; }
        public Quaternion SpawnRotation { get; private set; }
        public GameObject ZoneControlObject { get; private set; }
        
        // Runtime
        private long _passedTickTime;
        private bool _running;
        private int _ticks;
        private int _skippedTicks;
        private long _physicsTime;
        private long _objectUpdateTime;
        private long _scheduleUpdateTime;
        private long _timeSinceLastHeartBeat;
        private bool _loadObjectScriptsImmediately;
        private List<(GameObject, Type)> _queuedObjectScripts = new List<(GameObject, Type)>();
        
        public bool CalculatingTick { get; set; }
        public float DeltaTime { get; private set; }
        public ScriptManager ScriptManager { get; }
        public ManagedScriptEngine ManagedScriptEngine { get; }
        private ThreadedCollection<UpdatedObject> UpdatedObjects { get; }
        private List<ScheduledAction> NewScheduledActions { get; }
        private List<ScheduledAction> ScheduledActions { get; }
        
        // Physics
        public PhysicsSimulation Simulation { get; }
        public Event EarlyPhysics { get; }
        public Event LatePhysics { get; }
        
        // Events
        public Event<Player> OnPlayerLoad { get; }
        public Event<Player> OnPlayerLeave { get; }
        public Event<Object> OnObject { get; }
        public Event OnTick { get; }
        public Event<Player, string> OnChatMessage { get; }
        
        public Zone(ZoneInfo zoneInfo, WorldUchuServer server, ushort instanceId = default, uint cloneId = default)
        {
            Zone = this;
            ZoneInfo = zoneInfo;
            Server = server;
            InstanceId = instanceId;
            CloneId = cloneId;
            
            EarlyPhysics = new Event();
            LatePhysics = new Event();
            OnPlayerLoad = new Event<Player>();
            OnPlayerLeave = new Event<Player>();
            OnObject = new Event<Object>();
            OnTick = new Event();
            OnChatMessage = new Event<Player, string>();

            ScriptManager = new ScriptManager(this);
            ManagedScriptEngine = new ManagedScriptEngine();
            UpdatedObjects = new ThreadedCollection<UpdatedObject>();
            ScheduledActions = new List<ScheduledAction>();
            NewScheduledActions = new List<ScheduledAction>();
            ManagedObjects = new ThreadedCollection<Object>();
            SpawnedObjects = new ThreadedCollection<GameObject>();
            SpawnerNetworks = new ThreadedCollection<SpawnerNetwork>();
            Simulation = new PhysicsSimulation();

            Listen(OnDestroyed,() => { _running = false; });
        }

        #region Initializing

        /// <summary>
        /// Initializes the zone by loading level files, NPCs and enemies and setting up the game loop
        /// </summary>
        public async Task InitializeAsync()
        {
            Checksum = ZoneInfo.LuzFile.GenerateChecksum(ZoneInfo.LvlFiles);
            ZoneId = (ZoneId) ZoneInfo.LuzFile.WorldId;
            SpawnPosition = ZoneInfo.LuzFile.SpawnPoint;
            SpawnRotation = ZoneInfo.LuzFile.SpawnRotation;
            
            Logger.Information($"Checksum: 0x{Checksum:X}");
            Logger.Information($"Collecting objects for {ZoneId}");

            await ScriptManager.LoadDefaultScriptsAsync();
            await LoadObjects();
            this._loadObjectScriptsImmediately = true;
            await LoadScripts();
            SetupGameLoop();
            Loaded = true;
        }

        /// <summary>
        /// Sets up the object script for an object.
        /// </summary>
        /// <param name="gameObject">Game object to load.</param>
        public void LoadScriptForObject(GameObject gameObject)
        {
            // Get the script names to try to load.
            var scriptNames = new List<string>();
            var scriptComponent = gameObject.GetComponent<LuaScriptComponent>();

            if (gameObject.Settings.TryGetValue("custom_script_server", out var serverScriptOverride) &&
                (string) serverScriptOverride != "")
            {
                scriptNames.Add(((string) serverScriptOverride).ToLower());
            }

            if (!string.IsNullOrWhiteSpace(scriptComponent?.ScriptName) &&
                !scriptNames.Contains(scriptComponent.ScriptName.ToLower()))
            {
                scriptNames.Add(scriptComponent.ScriptName.ToLower());
            }

            // Log the script names.
            if (scriptNames.Count == 0) return;
            var scriptNamesOutput = scriptNames.Aggregate((i, j) => i + ", " + j);
            Logger.Debug($"Loading script for {gameObject} (LOT {gameObject.Lot}): {scriptNamesOutput}");

            // Attempt to load a script.
            var scriptLoaded = false;
            foreach (var scriptName in scriptNames)
            {
                if (scriptLoaded) break;
                foreach (var (objectScriptName, objectScriptType) in this.ScriptManager.ObjectScriptTypes)
                {
                    if (!scriptName.EndsWith(objectScriptName)) continue;
                    this.LoadObjectScript(gameObject, objectScriptType);
                    scriptLoaded = true;
                    break;
                }
            }

            // Output if no script was loaded.
            if (scriptLoaded || scriptNamesOutput == "") return;
            Logger.Debug($"No script found for {gameObject} (LOT {gameObject.Lot}): {scriptNamesOutput}");
        }
        
        /// <summary>
        /// Loads an object script. Either happens immediately or is queued
        /// for when all the objects load depending on if the zone has loaded.
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="objectScriptType"></param>
        public void LoadObjectScript(GameObject gameObject, Type objectScriptType)
        {
            if (this._loadObjectScriptsImmediately)
            {
                // Load the script.
                Logger.Information($"Running object script: {objectScriptType.Name}");
                Activator.CreateInstance(objectScriptType, gameObject);
            }
            else
            {
                // Queue the script to be loaded.
                this._queuedObjectScripts.Add((gameObject, objectScriptType));
            }
        }

        /// <summary>
        /// Uses the script manager of this zone to find and load all native and managed scripts
        /// </summary>
        private async Task LoadScripts()
        {
            // Load the script packs.
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
            
            // Load the object scripts.
            foreach (var (gameObject, objectScriptType) in this._queuedObjectScripts)
            {
                this.LoadObjectScript(gameObject, objectScriptType);
            }
        }
        
        /// <summary>
        /// Loads all objects, NPCs and enemies from the zone info and spawns them
        /// </summary>
        private async Task LoadObjects()
        {
            var objects = new List<LevelObjectTemplate>();
            foreach (var lvlFile in ZoneInfo.LvlFiles.Where(lvlFile => lvlFile.LevelObjects?.Templates != default))
            {
                objects.AddRange(lvlFile.LevelObjects.Templates);
            }

            Logger.Information($"Loading {objects.Count} objects for {ZoneId}");
            await LoadNavMeshes();
            
            ZoneInfo.LvlFiles = new List<LvlFile>();
            ZoneInfo.TerrainFile = default;
            
            GC.Collect();
            
            // Spawns all the NPCs in the area
            var zoneControlLot = ClientCache.Find<ZoneTable>(this.ZoneId.Id).ZoneControlTemplate ??= 2365;
            var zoneControlSettings = new LegoDataDictionary();
            foreach (var levelObject in objects)
            {
                if (levelObject.Lot == zoneControlLot)
                {
                    zoneControlSettings = levelObject.LegoInfo;
                    continue;
                }
                if (levelObject.LegoInfo.TryGetValue("trigger_id", out var trigger))
                {
                    Logger.Debug($"Trigger: {trigger}");
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

            var zoneObject = GameObject.Instantiate(this, lot: zoneControlLot, objectId: (ObjectId) 70368744177662);
            zoneObject.Settings = zoneControlSettings;
            zoneObject.InitializeComponents();
            Start(zoneObject);
            
            Objects.Append(zoneObject);
            ZoneControlObject = zoneObject;

            Logger.Information($"Loaded {GameObjects.Length}/{objects.Count} for {ZoneId}");
            LoadSpawnPaths();
            Logger.Information($"Loaded {Objects.Length} objects for {ZoneId}");
        }

        /// <summary>
        /// Generates the navmesh for this zone and initializes the pathfinding graph
        /// </summary>
        private async Task LoadNavMeshes()
        {
            NavMeshManager = new NavMeshManager(this, UchuServer.Config.GamePlay.PathFinding);
            if (NavMeshManager.Enabled)
            {
                Logger.Information("Generating navigation way points.");
                await NavMeshManager.GeneratePointsAsync();
                Logger.Information("Finished generating navigation way points.");
            }
        }

        /// <summary>
        /// Loads all the spawners in the zone and sets up all the spawned objects
        /// </summary>
        private void LoadSpawnPaths()
        {
            if (ZoneInfo.LuzFile.PathData == default)
                return;
            
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

            // Only spawns should get constructed on the client.
            spawner?.Spawn();
        }

        private void SpawnPath(LuzSpawnerPath spawnerPath)
        {
            var network = InstancingUtilities.SpawnerNetwork(spawnerPath, this);

            if (network.ActivateOnLoad)
            {
                network.Activate();
                network.SpawnAll();
            }
            this.SpawnerNetworks.Add(network);
        }

        #endregion

        #region Messages

        public void ExcludingMessage<T>(T message, Player excluded) where T : struct
        {
            foreach (var player in Players.Where(p => p != excluded)) player.Message(message);
        }
        
        public void BroadcastMessage<T>(T message) where T : struct
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
            return result != null;
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

                // Invoke the player left event if the object is an event.
                if (obj is Player player)
                {
                    OnPlayerLeave.Invoke(player);
                }

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
                if (gameObject != gameObject.Zone.ZoneControlObject && !recipient.Perspective.View(gameObject)) continue;
                if (!recipient.Perspective.Reveal(gameObject, out var id)) continue;
                if (id == 0) return;

                recipient.Connection.Send(new ConstructionPacket()
                {
                    Id = id,
                    GameObject = gameObject,
                });
            }
        }

        internal static void SendSerialization(GameObject gameObject, IEnumerable<Player> recipients)
        {
            foreach (var recipient in recipients)
            {
                if (gameObject != gameObject.Zone.ZoneControlObject && !recipient.Perspective.View(gameObject)) continue;
                if (!recipient.Perspective.TryGetNetworkId(gameObject, out var id)) continue;

                recipient.Connection.Send(new SerializePacket()
                {
                    Id = id,
                    GameObject = gameObject,
                });
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

                recipient.Connection.Send(new DestructionPacket()
                {
                    Id = id,
                });

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

        /// <summary>
        /// Creates a timer that reports ticks per second, time per tick and physics time
        /// </summary>
        /// <returns>The timer that was created</returns>
        private Timer SetupTickReporter()
        {
            var timer = new Timer
            {
                Interval = 1000,
                AutoReset = true
            };

            timer.Elapsed += (sender, args) =>
            {
                if (_ticks > 0)
                {
                    Logger.Debug($"TPS: {_ticks}/{TicksPerSecondLimit} " +
                                 $"TPT: {_passedTickTime / _ticks}ms [{_physicsTime / _ticks}|" +
                                 $"{_objectUpdateTime / _ticks}|{_scheduleUpdateTime / _ticks}]");
                }
                
                if (_skippedTicks >= TicksPerSecondLimit / 2)
                    Logger.Warning($"Can't keep up, skipped {_skippedTicks}/{TicksPerSecondLimit} ticks!");

                _passedTickTime = 0;
                _objectUpdateTime = 0;
                _physicsTime = 0;
                _scheduleUpdateTime = 0;
                _skippedTicks = 0;
                _ticks = 0;
            };

            timer.Start();
            return timer;
        }

        /// <summary>
        /// Executes one tick in the game loop, updating all entities subscribed to the <c>OnTick</c> event
        /// </summary>
        private async Task Tick()
        {
            if (Players.Length == 0)
                return;
            
            if (CalculatingTick)
            {
                _skippedTicks++;
                return;
            }

            CalculatingTick = true;
            
            var watch = new Stopwatch();
            watch.Start();

            await OnTick.InvokeAsync();
            _ticks++;

            var passedMs = watch.ElapsedMilliseconds;
            _passedTickTime += passedMs;

            DeltaTime = Math.Max(TimePerTickLimit, passedMs);

            CalculatingTick = false;
        }

        /// <summary>
        /// Sets up the game loop, initializing the scheduler, object updater and physics engine.
        /// </summary>
        /// <remarks>
        /// The game loop is started as long running task.
        /// </remarks>
        private void SetupGameLoop()
        {
            if (GameLoop != null)
                throw new ArgumentException("Can't setup game loop, game loop is already setup.");
            
            Listen(OnTick, ExecuteSchedule);
            Listen(OnTick, UpdateObjects);
            Listen(OnTick, PhysicsStep);
            Listen(OnTick, SendHeartbeat);

            _running = true;

            var tickReporter = SetupTickReporter();
            var gameLoop = new Timer
            {
                Interval = TimePerTickLimit,
                AutoReset = true
            };

            gameLoop.Elapsed += async (sender, args) =>
            {
                if (!_running)
                {
                    gameLoop.Enabled = false;
                    tickReporter.Enabled = false;
                    return;
                }
                
                await Tick();
            };
            
            gameLoop.Start();

            GameLoop = gameLoop;
        }

        /// <summary>
        /// Steps the physics by one step
        /// </summary>
        private async Task PhysicsStep()
        {
            var watch = new Stopwatch();
            watch.Start();

            try
            {
                await EarlyPhysics.InvokeAsync();
                Simulation.Step(DeltaTime);
                await LatePhysics.InvokeAsync();
            }
            catch (Exception e)
            {
                Logger.Error($"Physics error: {e}");
            }
            
            watch.Stop();
            _physicsTime += watch.ElapsedMilliseconds;
        }

        /// <summary>
        /// Updates all the objects in this zone by calling their update delegate
        /// </summary>
        /// <remarks>
        /// Optimizes for objects that are stuck or not in the player view filter
        /// </remarks>
        private void UpdateObjects()
        {
            var watch = new Stopwatch();
            watch.Start();

            var updatedObjects = UpdatedObjects.ToArray();
            var visibleObjects = updatedObjects.Select(o => o.Associate)
                .Intersect(Players.SelectMany(p => p.Perspective.LoadedObjects)).ToHashSet();
            var objectsToUpdate = updatedObjects
                .Where(o => visibleObjects.Contains(o.Associate));
            
            foreach (var updatedObject in objectsToUpdate)
            {
                // Ensure that the object ticks at its frequency
                if (updatedObject.Frequency > ++updatedObject.Ticks)
                    continue;

                updatedObject.Ticks = 0;
                try
                {
                    updatedObject.Delegate(DeltaTime);
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                    SentrySdk.CaptureException(e);
                }
            }
            
            watch.Stop();
            _objectUpdateTime += watch.ElapsedMilliseconds;
        }

        /// <summary>
        /// Sends a heart beat to the master server indicating that this server is still healthy
        /// </summary>
        private async Task SendHeartbeat()
        {
            _timeSinceLastHeartBeat += (long)DeltaTime;
            if (_timeSinceLastHeartBeat >= Server.HeartBeatInterval)
            {
                await Server.SendHeartBeat();
                _timeSinceLastHeartBeat = 0;
            }
        }
        
        /// <summary>
        /// Decrements the delay of all scheduled tasks and executes them if the timeout has been reached
        /// </summary>
        private void ExecuteSchedule()
        {
            var watch = new Stopwatch();
            watch.Start();
            
            // Before executing the schedule, check all incoming tasks and add them to the working queue
            lock (NewScheduledActions)
            {
                ScheduledActions.AddRange(NewScheduledActions);
                NewScheduledActions.Clear();
            }

            // Loop in reverse to allow removal of elements during the loop
            for (var i = ScheduledActions.Count - 1; i >= 0; i--)
            {
                var scheduledAction = ScheduledActions[i];
                
                scheduledAction.Delay -= DeltaTime;
                if (scheduledAction.Delay > 0)
                    continue;

                try
                {
                    scheduledAction.Delegate();
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                    SentrySdk.CaptureException(e);
                }
                finally
                {
                    ScheduledActions.RemoveAt(i);
                }
            }

            watch.Stop();
            _scheduleUpdateTime += watch.ElapsedMilliseconds;
        }

        /// <summary>
        /// Schedules an action to be executed in a certain amount of milliseconds
        /// </summary>
        /// <remarks>
        /// Warning: only schedule fine grained operations! Scheduled tasks should only directly calculate and run code,
        /// if any IO (for example database or networking) is needed make sure to run those parts of code inside a long
        /// running task using <c>Task.Factory.StartNew(() => {}, TaskCreationOptions.LongRunning);</c>.
        /// </remarks>
        /// <param name="delegate">The action to execute</param>
        /// <param name="delay">The milliseconds delay before execution</param>
        public void Schedule(Action @delegate, float delay)
        {
            lock (NewScheduledActions)
            {
                NewScheduledActions.Add(new ScheduledAction(@delegate, delay));
            }
        }

        /// <summary>
        /// Register an object as a managed object, calling its update function on a defined tick frequency
        /// </summary>
        /// <param name="associate">The game object to manage in the game loop</param>
        /// <param name="delegate">The function to execute on update</param>
        /// <param name="frequency">After how many ticks the action should be executed, 1 == every tick</param>
        public void Update(Object associate, Func<Task> @delegate, int frequency)
        {
            UpdatedObjects.Add(new UpdatedObject
            {
                Associate = associate,
                Delegate = _ => @delegate(),
                Frequency = frequency
            });
        }
        
        /// <summary>
        /// Register an object as a managed object, calling its update function on a defined tick frequency
        /// </summary>
        /// <param name="associate">The game object to manage in the game loop</param>
        /// <param name="delegate">The function to execute on update, the delta time is passed as argument to this function</param>
        /// <param name="frequency">After how many ticks the action should be executed, 1 == every tick</param>
        public void Update(Object associate, Func<float, Task> @delegate, int frequency)
        {
            UpdatedObjects.Add(new UpdatedObject
            {
                Associate = associate,
                Delegate = @delegate,
                Frequency = frequency
            });
        }
        
        /// <summary>
        /// An object managed by this zone. Whenever it's frequency is reached in the game loop, its update delegate is
        /// called.
        /// </summary>
        private class UpdatedObject
        {
            /// <summary>
            /// The game object to update
            /// </summary>
            public Object Associate { get; set; }
            
            /// <summary>
            /// The function to call on update, the delta time between ticks is passed to this function as argument
            /// </summary>
            public Func<float, Task> Delegate { get; set; }
            
            /// <summary>
            /// The frequency to execute on
            /// </summary>
            /// <remarks>
            /// 1 is every tick, 2 is every other tick, 10 is every 10 ticks, etc.
            /// </remarks>
            public int Frequency { get; set; }
            
            /// <summary>
            /// The amount of ticks that have passed since updating the object
            /// </summary>
            public int Ticks { get; set; }
        }
        
        /// <summary>
        /// A task that may be scheduled to execute in a certain amount of milliseconds
        /// </summary>
        private class ScheduledAction
        {
            public ScheduledAction(Action @delegate, float delay)
            {
                Delegate = @delegate;
                Delay = delay;
            }
            
            /// <summary>
            /// The action to execute if the task reaches its scheduled time
            /// </summary>
            public Action Delegate { get; }

            /// <summary>
            /// The amount of milliseconds to wait before executing this task
            /// </summary>
            public float Delay { get; set; }
        }

        #endregion
    }
}
