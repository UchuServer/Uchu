using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Reflection;
using System.Threading.Tasks;
using System.Timers;
using RakDotNet;
using Uchu.Core.Collections;
using Uchu.Core.Packets.Server.GameMessages;
using Uchu.Core.Scriptable;

namespace Uchu.Core
{
    public class World
    {
        /*
         * There is a problem of when the player crashes and the server does not pick up on it. The player is than
         * stuck in the world.
         */

        public static List<int> ComponentOrder = new List<int>
        {
            108, 61, 1, 3, 20, 30, 40, 7, 23, 26, 4, 19, 17, 5, 9, 60, 48, 25, 49, 16, 6, 39, 71, 75, 42, 2, 50, 107, 69
        };

        private readonly Dictionary<long, int> _loot;

        public readonly List<ReplicaPacket> Replicas;

        public readonly List<Player> Players;
        public readonly Server Server;

        public World(Server server)
        {
            Replicas = new List<ReplicaPacket>();
            _loot = new Dictionary<long, int>();
            Server = server;
            Players = new List<Player>();

            ReplicaManager = server.CreateReplicaManager();

            // Disconnected Players must be removed!
            Server.RakNetServer.Disconnection += RemovePlayer;
        }

        public Zone Zone { get; private set; }

        public ReplicaManager ReplicaManager { get; }

        public uint ZoneId { get; private set; }

        /// <summary>
        ///     Remove a player from this world.
        /// </summary>
        /// <param name="endPoint"></param>
        public void RemovePlayer(IPEndPoint endPoint)
        {
            Console.WriteLine($"{endPoint} disconnected from World!");
            var player = Players.First(p => p.EndPoint.Equals(endPoint));
            DestroyObject(player.CharacterId);
            ReplicaManager.RemoveConnection(endPoint);
            Players.Remove(player);
        }

        public async Task InitializeAsync(Zone zone)
        {
            ZoneId = zone.ZoneId;
            Zone = await Server.ZoneParser.ParseAsync(ZoneParser.Zones[(ushort) ZoneId]);

            foreach (var scene in zone.Scenes)
            {
                if (scene.Audio)
                    continue;

                foreach (var obj in scene.Objects)
                {
                    var pkt = await CreateObjectAsync(obj);

                    if (pkt != null)
                    {
                        SpawnObject(pkt);
                    }
                }
            }
            
            /*
             * GameScripts
             */

            var gameScripts = Assembly.GetEntryAssembly()
                .GetTypes().Where(t => t.BaseType == typeof(GameScript));
            foreach (var script in gameScripts)
            {
                var attribute = script.GetCustomAttribute<AutoAssignAttribute>();
                if (attribute == null) continue;
                
                Console.WriteLine($"");

                foreach (var replica in Replicas)
                {
                    if (attribute.Name != null && replica.Name != attribute.Name) continue;
                    if (attribute.LOT != 0 && replica.LOT != attribute.LOT) continue;
                    if (attribute.Component != null &&
                        replica.Components.All(c => c.GetType().FullName != attribute.Component.FullName)) continue;

                    var instance = (GameScript) Activator.CreateInstance(script, this, replica);
                    replica.GameScripts.Add(instance);
                }
            }

            foreach (var replica in Replicas)
            {
                foreach (var gameScript in replica.GameScripts)
                {
                    gameScript.Start();
                }
            }
        }

        public Player GetPlayer(long objectId) => Players.Find(p => p.CharacterId == objectId);

        public void SpawnPlayer(Character character, IPEndPoint endpoint)
        {
            ReplicaManager.AddConnection(endpoint);

            Players.Add(new Player(Server, endpoint));

            var replica = new ReplicaPacket
            {
                ObjectId = character.CharacterId,
                LOT = 1,
                Name = character.Name,
                Created = 0,
                Components = new IReplicaComponent[]
                {
                    new ControllablePhysicsComponent
                    {
                        HasPosition = true,
                        Position = Zone.SpawnPosition,
                        Rotation = Zone.SpawnRotation
                    },
                    new DestructibleComponent(),
                    new StatsComponent
                    {
                        HasStats = true,
                        CurrentArmor = (uint) character.CurrentArmor,
                        MaxArmor = (uint) character.MaximumArmor,
                        CurrentHealth = (uint) character.CurrentHealth,
                        MaxHealth = (uint) character.MaximumHealth,
                        CurrentImagination = (uint) character.CurrentImagination,
                        MaxImagination = (uint) character.MaximumImagination
                    },
                    new CharacterComponent
                    {
                        Level = (uint) character.Level,
                        Character = character
                    },
                    new InventoryComponent
                    {
                        Items = character.Items.Where(i => i.IsEquipped).ToArray()
                    },
                    new ScriptComponent(),
                    new SkillComponent(),
                    new RenderComponent(),
                    new Component107()
                }
            };

            SpawnObject(replica);

            // TODO: Fix
            Server.Send(new DisplayZoneSummaryMessage {IsZoneStart = true, Sender = 0}, endpoint);
        }

        public void RegisterLoot(long objectId, int lot)
        {
            _loot[objectId] = lot;
        }

        public int GetLootLOT(long objectId)
        {
            return _loot[objectId];
        }

        public void DestroyObject(long objectId)
        {
            var data = Replicas.Find(r => r.ObjectId == objectId);

            Replicas.Remove(data);

            ReplicaManager.SendDestruction(data);
        }

        public ReplicaPacket GetObject(long objectId)
        {
            return Replicas.Find(r => r.ObjectId == objectId);
        }

        public void UpdateObject(ReplicaPacket packet)
        {
            // _replicas[_replicas.FindIndex(r => r.ObjectId == packet.ObjectId)] = packet;

            ReplicaManager.SendSerialization(packet);
        }

        public void SpawnObject(ReplicaPacket packet)
        {
            Replicas.Add(packet);

            ReplicaManager.SendConstruction(packet);
        }

        public async Task<ReplicaPacket> CreateObjectAsync(LevelObject obj, long parentId = -1)
        {
            if (obj.Settings.TryGetValue("loadSrvrOnly", out var serverOnly) && (bool) serverOnly ||
                obj.Settings.TryGetValue("carver_only", out var carverOnly) && (bool) carverOnly ||
                obj.Settings.TryGetValue("renderDisabled", out var disabled) && (bool) disabled) // is this right?
                return null;

            var spawnLOT = obj.LOT;

            if (obj.Settings.TryGetValue("spawntemplate", out var spawnTemplate))
                obj.LOT = (int) spawnTemplate;

            var registryComponents = await Server.CDClient.GetComponentsAsync(obj.LOT);

            /*if (registryComponents.Select(c => c.ComponentType).Contains(78))
                Console.WriteLine($"Proximity LOT = {spawnLOT}");*/

            /*if (registryComponents.Select(c => c.ComponentType).Contains(69))
                return null;*/

            registryComponents = registryComponents.Where(c => ComponentOrder.Contains(c.ComponentType)).ToArray();

            Array.Sort(registryComponents,
                (a, b) => ComponentOrder.IndexOf(a.ComponentType).CompareTo(ComponentOrder.IndexOf(b.ComponentType)));

            var components = new List<IReplicaComponent>();

            foreach (var c in registryComponents)
            {
                var list = new List<IReplicaComponent>();

                switch (c.ComponentType)
                {
                    case 108:
                        list.Add(new PossesableComponent()); // TODO: set id
                        break;

                    case 61:
                        // ModuleAssembly
                        break;

                    case 1:
                        list.Add(new ControllablePhysicsComponent
                        {
                            HasPosition = true,
                            Position = obj.Position,
                            Rotation = obj.Rotation
                        });
                        break;

                    case 3:
                        list.Add(new SimplePhysicsComponent
                        {
                            Position = obj.Position,
                            Rotation = obj.Rotation
                        });
                        break;

                    case 20:
                        list.Add(new RigidBodyPhantomPhysicsComponent
                        {
                            Position = obj.Position,
                            Rotation = obj.Rotation
                        });
                        break;

                    case 30:
                        list.Add(new VehiclePhysicsComponent());
                        break;

                    case 40:
                        list.Add(new PhantomPhysicsComponent
                        {
                            Position = obj.Position,
                            Rotation = obj.Rotation
                        });
                        break;

                    case 7:
                        list.Add(new DestructibleComponent());

                        list.Add(new StatsComponent {HasStats = false});
                        break;

                    case 23:
                        list.Add(new StatsComponent {HasStats = false});

                        list.Add(new CollectibleComponent
                            {CollectibleId = (ushort) (int) obj.Settings["collectible_id"]});
                        break;

                    case 26:
                        list.Add(new PetComponent()); // TODO: set name and owner
                        break;

                    case 4:
                        list.Add(new CharacterComponent()); // TODO: set properties
                        break;

                    case 19:
                        // Shooting gallery
                        break;

                    case 17:
                        var items = await Server.CDClient.GetInventoryItemsAsync(c.ComponentId);

                        list.Add(new InventoryComponent // TODO: make this work
                        {
                            Items = items.Where(i => i.Equipped).Select(i => new InventoryItem
                            {
                                InventoryItemId = Utils.GenerateObjectId(),
                                Count = i.ItemCount,
                                LOT = i.ItemId,
                                Slot = -1,
                                InventoryType = -1
                            }).ToArray()
                        });
                        break;

                    case 5:
                        list.Add(new ScriptComponent());
                        break;

                    case 9:
                        list.Add(new SkillComponent());
                        break;

                    case 60:
                        list.Add(new BaseCombatAIComponent()); // TODO: set properties
                        break;

                    case 48:
                        components.Add(new StatsComponent {HasStats = false});

                        list.Add(new RebuildComponent
                        {
                            ActivatorPosition = (Vector3) obj.Settings["rebuild_activators"]
                        });
                        break;

                    case 25:
                        var pathName = obj.Settings.TryGetValue("attached_path", out var name) ? (string) name : "";
                        var pathStart = obj.Settings.TryGetValue("attached_path_start", out var start)
                            ? (uint) start
                            : 0;

                        var path = (MovingPlatformPath) Zone.Paths.FirstOrDefault(p =>
                            p is MovingPlatformPath && p.Name == pathName);

                        var nextWaypoint = pathStart + 1 > path.Waypoints.Length - 1 ? 0 : pathStart + 1;
                        var type = obj.Settings.TryGetValue("platformIsMover", out var isMover) && (bool) isMover
                            ? PlatformType.Mover
                            : obj.Settings.TryGetValue("platformIsSimpleMover", out var isSimpleMover) &&
                              (bool) isSimpleMover
                                ? PlatformType.SimpleMover
                                : PlatformType.None;

                        var waypoint = (MovingPlatformWaypoint) path.Waypoints[nextWaypoint];

                        list.Add(new MovingPlatformComponent
                        {
                            Path = path,
                            PathName = pathName,
                            PathStart = pathStart,
                            Type = type,
                            State = PlatformState.Idle,
                            CurrentWaypointIndex = pathStart,
                            NextWaypointIndex = nextWaypoint,
                            TargetPosition = waypoint.Position,
                            TargetRotation = waypoint.Rotation
                        });
                        break;

                    case 49:
                        list.Add(new SwitchComponent()); // TODO: set state
                        break;

                    case 16:
                        list.Add(new VendorComponent());
                        break;

                    case 6:
                        list.Add(new BouncerComponent()); // TODO: set pet required
                        break;

                    case 39:
                        list.Add(new ScriptedActivityComponent());
                        break;

                    case 71:
                        // Racing control
                        break;

                    case 75:
                        // LUP Exhibit
                        break;

                    case 42:
                        // Model
                        break;

                    case 2:
                        list.Add(new RenderComponent
                        {
                            Disabled = (bool?) disabled ?? false
                        });
                        break;

                    case 50:
                        // Minigame
                        break;

                    case 107:
                        list.Add(new Component107());
                        break;

                    /*case 69:
                        list.Add(new TriggerComponent()); // set id
                        break;*/
                }

                components.AddRange(list);
            }

            if (obj.Settings.TryGetValue("trigger_id", out var triggerId))
            {
                var str = (string) triggerId;
                var colonIndex = str.IndexOf(':');
                var v = str.Substring(colonIndex + 1);

                Console.WriteLine($"TriggerId = {v}");

                components.Add(new TriggerComponent
                {
                    TriggerId = int.Parse(v)
                });
            }

            var data = new ReplicaPacket
            {
                ObjectId = Utils.GenerateObjectId(),
                LOT = obj.LOT,
                Name = obj.Settings.TryGetValue("npcName", out var npcName) ? (string) npcName : "",
                Created = 0,
                Scale = obj.Scale,
                Components = components.ToArray(),
                SpawnerObjectId = spawnLOT == 176 ? (long) obj.ObjectId : -1,
                ParentObjectId = parentId,
                Settings = obj.Settings,
                Position = obj.Position,
                Rotation = obj.Rotation
            };

            if (obj.Settings.TryGetValue("spawnActivator", out var spawnActivator) && (bool) spawnActivator)
            {
                var spawner = await CreateObjectAsync(new LevelObject
                {
                    LOT = 6604,
                    Position = (Vector3) obj.Settings["rebuild_activators"],
                    Rotation = Vector4.Zero,
                    Scale = -1,
                    Settings = new LegoDataDictionary()
                }, data.ObjectId);

                SpawnObject(spawner);

                data.ChildObjectIds = new[] {spawner.ObjectId};
            }

            return data;
        }
    }
}