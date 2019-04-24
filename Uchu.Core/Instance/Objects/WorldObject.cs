using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Threading.Tasks;
using Uchu.Core.Collections;
using Uchu.Core.IO;
using Uchu.Core.Scripting.Lua;

namespace Uchu.Core
{
    public class WorldObject : ISpawnableObject<IPhysics>
    {
        public static readonly int[] ComponentOrder =
        {
            108, 61, 1, 3, 20, 30, 40, 7, 23, 26, 4, 19, 17, 5, 9, 60, 48, 25, 49, 16, 6, 39, 71, 75, 42, 2, 50, 107, 69
        };

        private readonly WorldInstance _instance;
        private readonly LevelObject _lvlObject;
        private bool _spawned;
        private ReplicaPacket _replica;

        public int LOT { get; }
        public long ObjectId { get; }
        public IPhysics Physics { get; private set; }
        public LuaGameObject LuaObject { get; }

        public string Name { get; }
        public long SpawnerId { get; } = -1;
        public ISpawnable ParentObject { get; }
        public List<ISpawnable> ChildObjects { get; }
        public ComponentsRegistryRow[] Components { get; private set; }

        public dynamic LuaEnvironment { get; }
        public LuaObjectManager LuaObjectManager { get; }

        public WorldObject(WorldInstance instance, LevelObject lvlObject, ISpawnable parent = null)
        {
            _instance = instance;
            _lvlObject = lvlObject;
            _spawned = false;

            ObjectId = Utils.GenerateObjectId();
            Name = "";
            ParentObject = parent;
            ChildObjects = new List<ISpawnable>();
            Components = new ComponentsRegistryRow[0];

            if (lvlObject.LOT == 176)
            {
                LOT = (int) lvlObject.Settings["spawntemplate"];
                SpawnerId = (long) lvlObject.ObjectId;
            }
            else
            {
                LOT = lvlObject.LOT;
            }

            if (lvlObject.Settings.TryGetValue("spawnActivator", out var spawnActivator) && (bool) spawnActivator)
            {
                ChildObjects.Add(new WorldObject(instance, new LevelObject
                {
                    LOT = 6604,
                    Position = (Vector3) lvlObject.Settings["rebuild_activators"],
                    Rotation = Vector4.Zero,
                    Settings = new LegoDataDictionary()
                }, this));
            }

            LuaObject = new LuaGameObject(this);
            LuaObjectManager = new LuaObjectManager(instance);
            LuaEnvironment = _instance.Lua.CreateEnvironment();

            LuaEnvironment.GAMEOBJ = LuaObjectManager;
            LuaEnvironment.RESMGR = _instance.LuaResourceManager;

            if (lvlObject.Settings.TryGetValue("custom_script_server", out var script))
            {
                var fn = ((string) script).Replace('\\', '/').Replace("scripts/", "");

                var pth = Path.Combine(FileResources.AssemblyDirectory, "Scripts", fn);

                Console.WriteLine($"Loading Script: {pth}");

                LuaEnvironment.dofile(fn);
            }
        }

        public async Task SpawnAsync()
        {
            if (_spawned)
                throw new InvalidOperationException();

            var components = (await _instance.Server.CDClient.GetComponentsAsync(LOT))
                .Where(c => ComponentOrder.Contains(c.ComponentType)).ToArray();

            Array.Sort(components,
                (a, b) => Array.IndexOf(ComponentOrder, a.ComponentType)
                    .CompareTo(Array.IndexOf(ComponentOrder, b.ComponentType)));

            Components = components;

            var comps = new List<IReplicaComponent>();

            foreach (var component in components)
            {
                switch (component.ComponentType)
                {
                    case 108:
                        comps.Add(new PossessableComponent()); // TODO: set id
                        break;

                    case 61:
                        // ModuleAssemblyComponent
                        break;

                    case 1:
                        comps.Add(Physics = new ControllablePhysicsComponent
                        {
                            HasPosition = true,
                            Position = _lvlObject.Position,
                            Rotation = _lvlObject.Rotation
                        });
                        break;

                    case 3:
                        comps.Add(Physics = new SimplePhysicsComponent
                        {
                            HasPosition = true,
                            Position = _lvlObject.Position,
                            Rotation = _lvlObject.Rotation
                        });
                        break;

                    case 20:
                        comps.Add(Physics = new RigidBodyPhantomPhysicsComponent
                        {
                            HasPosition = true,
                            Position = _lvlObject.Position,
                            Rotation = _lvlObject.Rotation
                        });
                        break;

                    case 30:
                        comps.Add(new VehiclePhysicsComponent());
                        break;

                    case 40:
                        comps.Add(Physics = new PhantomPhysicsComponent
                        {
                            HasPosition = true,
                            Position = _lvlObject.Position,
                            Rotation = _lvlObject.Rotation
                        });
                        break;

                    case 7:
                        var stats = await _instance.Server.CDClient.GetDestructibleComponentAsync(component.ComponentId);

                        comps.AddRange(new IReplicaComponent[]
                        {
                            new DestructibleComponent(),
                            new StatsComponent
                            {
                                // TODO: implement damaging objects
                                HasStats = false, // = true,
                                Factions = stats.Factions,
                                Smashable = stats.IsSmashable,
                                CurrentImagination = (uint) stats.Imagination,
                                MaxImagination = stats.Imagination,
                                CurrentArmor = (uint) stats.Armor,
                                MaxArmor = stats.Armor,
                                CurrentHealth = (uint) stats.Health,
                                MaxHealth = stats.Health
                            }
                        });
                        break;

                    case 23:
                        comps.AddRange(new IReplicaComponent[]
                        {
                            new StatsComponent {HasStats = false},
                            new CollectibleComponent
                            {
                                CollectibleId = (ushort) (int) _lvlObject.Settings["collectible_id"]
                            }
                        });
                        break;

                    case 26:
                        comps.Add(new PetComponent()); // TODO: set name and owner
                        break;

                    case 4:
                        comps.Add(new CharacterComponent()); // TODO: set properties
                        break;

                    case 19:
                        // ShootingGalleryComponent
                        break;

                    case 17:
                        var items = await _instance.Server.CDClient.GetInventoryItemsAsync(component.ComponentId);

                        comps.Add(new InventoryComponent
                        {
                            Items = items.Where(i => i.Equipped).Select(i => new InventoryItem
                            {
                                InventoryItemId = Utils.GenerateObjectId(),
                                Count = i.ItemCount,
                                LOT = i.ItemId,
                                Slot = -1,
                                InventoryType = InventoryType.Invalid
                            }).ToArray()
                        });
                        break;

                    case 5:
                        var script = await _instance.Server.CDClient.GetScriptComponentAsync(component.ComponentId);

                        if (!string.IsNullOrEmpty(script.ServerScript) && !script.ServerScript.Contains("__removed"))
                        {
                            var pth = Path.Combine(FileResources.AssemblyDirectory, "Scripts", script.ServerScript);

                            Console.WriteLine($"Loading Script: {pth}");

                            LuaEnvironment.dofile(pth);

                            if (LuaEnvironment.onTimerDone != null)
                                _instance.LuaObjectManager.TimerManager.TimerDone += (self, msg) =>
                                {
                                    try
                                    {
                                        LuaEnvironment.onTimerDone(self, msg);
                                    }
                                    catch (Exception e)
                                    {
                                        Console.WriteLine(e);
                                    }
                                };
                        }

                        if (LuaEnvironment.onStartup != null)
                            LuaEnvironment.onStartup(LuaObject, new ExpandoObject());

                        comps.Add(new ScriptComponent());
                        break;

                    case 9:
                        comps.Add(new SkillComponent());
                        break;

                    case 60:
                        comps.Add(new BaseCombatAIComponent()); // TODO: set properties
                        break;

                    case 48:
                        comps.AddRange(new IReplicaComponent[]
                        {
                            new StatsComponent {HasStats = false},
                            new RebuildComponent
                            {
                                ActivatorPosition = (Vector3) _lvlObject.Settings["rebuild_activators"]
                            }
                        });
                        break;

                    case 25:
                        var pathName = _lvlObject.Settings.TryGetValue("attached_path", out var name)
                            ? (string) name
                            : "";
                        var pathStart = _lvlObject.Settings.TryGetValue("attached_path_start", out var start)
                            ? (uint) start
                            : 0;

                        var path = (MovingPlatformPath) _instance.Zone.Paths.FirstOrDefault(p =>
                            p is MovingPlatformPath && p.Name == pathName);

                        var nextWaypoint = pathStart + 1 >  path.Waypoints.Length - 1  ? 0 : pathStart + 1;
                        var type =
                            _lvlObject.Settings.TryGetValue("platformIsMover", out var isMover) && (bool) isMover
                                ?
                                PlatformType.Mover
                                : _lvlObject.Settings.TryGetValue("platformIsSimpleMover", out var isSimpleMover) &&
                                  (bool) isSimpleMover
                                    ? PlatformType.SimpleMover
                                    : PlatformType.None;

                        var waypoint = (MovingPlatformWaypoint) path.Waypoints[nextWaypoint];

                        comps.Add(new MovingPlatformComponent
                        {
                            Path = path,
                            PathName = pathName,
                            PathStart = pathStart,
                            Type = type,
                            State = PlatformState.Move,
                            CurrentWaypointIndex = pathStart,
                            NextWaypointIndex = nextWaypoint,
                            TargetPosition = waypoint.Position,
                            TargetRotation = waypoint.Rotation
                        });
                        break;

                    case 49:
                        comps.Add(new SwitchComponent()); // TODO: set state
                        break;

                    case 16:
                        comps.Add(new VendorComponent());
                        break;

                    case 6:
                        comps.Add(new BouncerComponent()); // TODO: set pet required
                        break;

                    case 39:
                        comps.Add(new ScriptedActivityComponent());
                        break;

                    case 71:
                        // RacingControlComponent
                        break;

                    case 75:
                        // LUPExhibitComponent
                        break;

                    case 42:
                        // ModelComponent?
                        break;

                    case 2:
                        comps.Add(new RenderComponent
                        {
                            Disabled = _lvlObject.Settings.TryGetValue("renderDisabled", out var renderDisabled) &&
                                       (bool) renderDisabled
                        });
                        break;

                    case 50:
                        // Minigame
                        break;

                    case 107:
                        comps.Add(new Component107());
                        break;
                }
            }

            _replica = new ReplicaPacket
            {
                ObjectId = ObjectId,
                LOT = LOT,
                Name = Name,
                Scale = _lvlObject.Scale,
                SpawnerObjectId = SpawnerId,
                ParentObjectId = ParentObject?.ObjectId ?? -1,
                ChildObjectIds = ChildObjects.Select(child => child.ObjectId).ToArray()
            };

            if (_lvlObject.Settings.TryGetValue("trigger_id", out var triggerId))
            {
                var str = (string) triggerId;
                var colonIndex = str.IndexOf(':');
                var v = str.Substring(colonIndex + 1);

                comps.Add(new TriggerComponent {TriggerId = int.Parse(v)});

                _replica.HasTriggerId = true;
            }

            foreach (var obj in ChildObjects) await obj.SpawnAsync();

            _replica.Components = comps.ToArray();

            _instance.ReplicaManager.SendConstruction(_replica);
        }

        public void Despawn(IPEndPoint[] endpoints = null)
        {
            if (!_spawned)
                throw new InvalidOperationException();

            _instance.ReplicaManager.SendDestruction(_replica, endpoints);
        }

        public void Update(IEnumerable<IReplicaComponent> components, IPEndPoint[] endpoints = null)
        {
            if (!_spawned)
                throw new InvalidOperationException();

            var updates = new Dictionary<int, IReplicaComponent>();

            foreach (var comp in components)
            {
                var type =
                    comp is PossessableComponent ? 108 :
                    comp is ControllablePhysicsComponent ? 61 :
                    comp is SimplePhysicsComponent ? 1 :
                    comp is RigidBodyPhantomPhysicsComponent ? 20 :
                    comp is VehiclePhysicsComponent ? 30 :
                    comp is PhantomPhysicsComponent ? 40 :
                    comp is DestructibleComponent ? 7 :
                    comp is CollectibleComponent ? 23 :
                    comp is PetComponent ? 26 :
                    comp is CharacterComponent ? 4 :
                    comp is InventoryComponent ? 17 :
                    comp is ScriptComponent ? 5 :
                    comp is SkillComponent ? 9 :
                    comp is BaseCombatAIComponent ? 60 :
                    comp is RebuildComponent ? 48 :
                    comp is MovingPlatformComponent ? 25 :
                    comp is SwitchComponent ? 49 :
                    comp is VendorComponent ? 16 :
                    comp is BouncerComponent ? 6 :
                    comp is ScriptedActivityComponent ? 39 :
                    comp is RenderComponent ? 2 :
                    comp is Component107 ? 107 :
                    comp is TriggerComponent ? 69 :
                    throw new NotSupportedException($"Invalid component: {comp}");

                updates[type] = comp;
            }

            var comps = new List<IReplicaComponent>();

            foreach (var comp in Components)
            {
                if (updates.ContainsKey(comp.ComponentType))
                {
                    if (comp.ComponentType == 7)
                    {
                        comps.AddRange(new[]
                        {
                            updates[comp.ComponentType],
                            new StatsComponent()
                        });
                    }
                    else if (comp.ComponentType == 23 || comp.ComponentType == 48)
                    {
                        comps.AddRange(new[]
                        {
                            new StatsComponent(),
                            updates[comp.ComponentType]
                        });
                    }
                    else
                    {
                        var update = updates[comp.ComponentType];

                        if (update is IPhysics physics)
                            Physics = physics;

                        comps.Add(update);
                    }
                }
                else
                {
                    switch (comp.ComponentType)
                    {
                        case 108:
                            comps.Add(new PossessableComponent());
                            break;

                        case 61:
                            // ModuleAssemblyComponent
                            break;

                        case 1:
                            comps.Add(Physics = new ControllablePhysicsComponent());
                            break;

                        case 3:
                            comps.Add(Physics = new SimplePhysicsComponent());
                            break;

                        case 20:
                            comps.Add(Physics = new RigidBodyPhantomPhysicsComponent());
                            break;

                        case 30:
                            comps.Add(new VehiclePhysicsComponent());
                            break;

                        case 40:
                            comps.Add(Physics = new PhantomPhysicsComponent());
                            break;

                        case 7:
                            comps.AddRange(new IReplicaComponent[]
                            {
                                new DestructibleComponent(),
                                new StatsComponent()
                            });
                            break;

                        case 23:
                            comps.AddRange(new IReplicaComponent[]
                            {
                                new StatsComponent(),
                                new CollectibleComponent()
                            });
                            break;

                        case 26:
                            comps.Add(new PetComponent());
                            break;

                        case 4:
                            comps.Add(new CharacterComponent());
                            break;

                        case 19:
                            // ShootingGalleryComponent
                            break;

                        case 17:
                            comps.Add(new InventoryComponent());
                            break;

                        case 5:
                            comps.Add(new ScriptComponent());
                            break;

                        case 9:
                            comps.Add(new SkillComponent());
                            break;

                        case 60:
                            comps.Add(new BaseCombatAIComponent());
                            break;

                        case 48:
                            comps.AddRange(new IReplicaComponent[]
                            {
                                new StatsComponent(),
                                new RebuildComponent()
                            });
                            break;

                        case 25:
                            comps.Add(new MovingPlatformComponent());
                            break;

                        case 49:
                            comps.Add(new SwitchComponent());
                            break;

                        case 16:
                            comps.Add(new VendorComponent());
                            break;

                        case 6:
                            comps.Add(new BouncerComponent());
                            break;

                        case 39:
                            comps.Add(new ScriptedActivityComponent());
                            break;

                        case 71:
                            // RacingControlComponent
                            break;

                        case 75:
                            // LUPExhibitComponent
                            break;

                        case 42:
                            // ModelComponent?
                            break;

                        case 2:
                            comps.Add(new RenderComponent());
                            break;

                        case 50:
                            // MinigameComponent
                            break;

                        case 107:
                            comps.Add(new Component107());
                            break;

                        case 69:
                            comps.Add(new TriggerComponent());
                            break;
                    }
                }
            }

            _replica.Components = comps.ToArray();

            _instance.ReplicaManager.SendSerialization(_replica, endpoints);
        }

        public void Update(IReplicaComponent component, IPEndPoint[] endpoints = null)
            => Update(new[] {component}, endpoints);

        public void UpdatePosition(Vector3 position)
        {
            Physics.Position = position;

            Update(Physics);
        }

        public void UpdatePosition(IPhysics physics)
        {
            Physics = physics;

            Update(physics);
        }

        public void Die(PlayerObject killer, KillType type = KillType.Violent)
        {
            var msg = new DieMessage
            {
                ObjectId = ObjectId,
                KillerObjectId = killer.ObjectId,
                LootOwnerId = killer.ObjectId
            };

            /*foreach (var (k, _) in _instance.Players)
            {
                if (killer.Endpoint.Equals(k))
                    continue;

                _instance.Server.Send(msg, k);
            }*/
        }

        public async Task DropLootAsync(PlayerObject owner)
        {
            var rand = new Random();
            var drops = await _instance.Server.CDClient.GetDropsForObjectAsync(LOT);
            var currencyDrops = await _instance.Server.CDClient.GetCurrencyForObjectAsync(LOT);

            Array.Sort(currencyDrops, (a, b) => a.MinimumNPCLevel.CompareTo(b.MinimumNPCLevel));
            Array.Reverse(currencyDrops);

            foreach (var drop in currencyDrops)
            {
                if (drop.MinimumNPCLevel > owner.Character.Level)
                    continue;

                var count = rand.Next(drop.MinimumValue, drop.MaximumValue);

                DropCurrency(count, owner);
                break;
            }

            foreach (var drop in drops)
            {
                var count = rand.Next(drop.MinDrops, drop.MaxDrops);
                var items = await _instance.Server.CDClient.GetItemDropsAsync(drop.LootTableIndex);

                if (items.Length == 0)
                    continue;

                for (var i = 0; i < count; i++)
                {
                    if (rand.NextDouble() <= drop.Percent)
                    {
                        var item = items[rand.Next(0, items.Length)];

                        DropLoot(item.ItemId, owner);
                    }
                }
            }
        }

        public void DropLoot(int lot, PlayerObject owner)
        {
            var rand = new Random();
            var lootId = Utils.GenerateObjectId();

            var spawnPos = Physics.Position;
            var finalPos = Physics.Position;

            spawnPos.Y++;

            finalPos.X += ((float) rand.NextDouble() % 1 - 0.5f) * 20;
            finalPos.Z += ((float) rand.NextDouble() % 1 - 0.5f) * 20;

            _instance.Server.Send(new DropClientLootMessage
            {
                ObjectId = owner.ObjectId,
                UsePosition = true,
                FinalPosition = finalPos,
                Currency = 0,
                ItemLOT = lot,
                LootObjectId = lootId,
                OwnerObjectId = owner.ObjectId,
                SourceObjectId = ObjectId,
                SpawnPosition = spawnPos
            }, owner.Endpoint);
        }

        public void DropCurrency(int amount, PlayerObject owner)
        {
            var rand = new Random();

            var spawnPos = Physics.Position;
            var finalPos = Physics.Position;

            spawnPos.Y++;

            finalPos.X += ((float) rand.NextDouble() % 1 - 0.5f) * 20;
            finalPos.Z += ((float) rand.NextDouble() % 1 - 0.5f) * 20;

            _instance.Server.Send(new DropClientLootMessage
            {
                ObjectId = owner.ObjectId,
                UsePosition = true,
                FinalPosition = finalPos,
                Currency = amount,
                ItemLOT = -1,
                LootObjectId = 0,
                OwnerObjectId = owner.ObjectId,
                SourceObjectId = ObjectId,
                SpawnPosition = spawnPos
            }, owner.Endpoint);
        }
    }
}