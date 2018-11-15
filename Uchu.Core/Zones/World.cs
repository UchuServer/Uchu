using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Threading.Tasks;
using RakDotNet;

namespace Uchu.Core
{
    public class World
    {
        public static List<int> ComponentOrder = new List<int>
        {
            108, 61, 1, 3, 20, 30, 40, 7, 23, 26, 4, 19, 17, 5, 9, 60, 48, 25, 49, 16, 6, 39, 71, 75, 42, 2, 50, 107, 69
        };

        private readonly List<ReplicaPacket> _replicas;
        private readonly Dictionary<long, int> _loot;
        private readonly Server _server;

        public ReplicaManager ReplicaManager { get; }
        public uint ZoneId { get; private set; }

        public World(Server server)
        {
            _replicas = new List<ReplicaPacket>();
            _loot = new Dictionary<long, int>();
            _server = server;

            ReplicaManager = server.CreateReplicaManager();
        }

        public async Task InitializeAsync(Zone zone)
        {
            ZoneId = zone.ZoneId;
            /*foreach (var file in Directory.GetFiles("Logs/"))
            {
                if (file.EndsWith(".txt"))
                    File.Delete(file);
            }*/

            foreach (var scene in zone.Scenes)
            {
                if (scene.Audio)
                    continue;

                foreach (var obj in scene.Objects)
                {
                    var pkt = await CreateObjectAsync(obj);

                    if (pkt != null)
                        SpawnObject(pkt);
                }
            }
        }

        public void RegisterLoot(long objectId, int lot)
        {
            _loot[objectId] = lot;
        }

        public int GetLootLOT(long objectId)
            => _loot[objectId];

        public void DestroyObject(long objectId)
        {
            var data = _replicas.Find(r => r.ObjectId == objectId);

            _replicas.Remove(data);

            ReplicaManager.SendDestruction(data);
        }

        public ReplicaPacket GetObject(long objectId)
            => _replicas.Find(r => r.ObjectId == objectId);

        public void UpdateObject(ReplicaPacket packet)
        {
            // _replicas[_replicas.FindIndex(r => r.ObjectId == packet.ObjectId)] = packet;

            ReplicaManager.SendSerialization(packet);
        }

        public void SpawnObject(ReplicaPacket packet)
        {
            _replicas.Add(packet);

            ReplicaManager.SendConstruction(packet);
        }

        public async Task<ReplicaPacket> CreateObjectAsync(LevelObject obj, long parentId = -1)
        {
            if (obj.Settings.TryGetValue("loadOnClientOnly", out var clientOnly) && (bool) clientOnly ||
                obj.Settings.TryGetValue("renderDisabled", out var disabled) && (bool) disabled) // is this right?
                return null;

            var spawnLOT = obj.LOT;

            if (obj.Settings.TryGetValue("spawntemplate", out var spawnTemplate))
                obj.LOT = (int) spawnTemplate;

            var registryComponents = await _server.CDClient.GetComponentsAsync(obj.LOT);

            if (registryComponents.Select(c => c.ComponentType).Contains(69))
                return null;

            registryComponents = registryComponents.Where(c => ComponentOrder.Contains(c.ComponentType)).ToArray();

            Array.Sort(registryComponents,
                (a, b) => ComponentOrder.IndexOf(a.ComponentType).CompareTo(ComponentOrder.IndexOf(b.ComponentType)));

            var components = new List<IReplicaComponent>();

            /*if (obj.LOT == 6316)
            {
                foreach (var (k, v) in obj.Settings)
                {
                    Console.WriteLine($"{k}: {v}");
                }

                Console.WriteLine();
            }*/

            /*using (var file = File.AppendText($"Logs/{obj.LOT}.txt"))
            {
                file.WriteLine($"{obj.LOT} ({spawnLOT}):");
                file.WriteLine("    Settings:");
                foreach (var (k, v) in obj.Settings) file.WriteLine($"        {k}: {v}");
                file.WriteLine("    Components:");*/

            foreach (var c in registryComponents)
            {
                IReplicaComponent component = null;

                switch (c.ComponentType)
                {
                    case 108:
                        component = new PossesableComponent(); // TODO: set id
                        break;

                    case 61:
                        // ModuleAssembly
                        break;

                    case 1:
                        component = new ControllablePhysicsComponent
                        {
                            HasPosition = true,
                            Position = obj.Position,
                            Rotation = obj.Rotation
                        };
                        break;

                    case 3:
                        component = new SimplePhysicsComponent
                        {
                            Position = obj.Position,
                            Rotation = obj.Rotation
                        };
                        break;

                    case 20:
                        component = new RigidBodyPhantomPhysicsComponent
                        {
                            Position = obj.Position,
                            Rotation = obj.Rotation
                        };
                        break;

                    case 30:
                        component = new VehiclePhysicsComponent();
                        break;

                    case 40:
                        component = new PhantomPhysicsComponent
                        {
                            Position = obj.Position,
                            Rotation = obj.Rotation
                        };
                        break;

                    case 7:
                        component = new DestructibleComponent();

                        components.Add(new StatsComponent {HasStats = false});
                        break;

                    case 23:
                        components.Add(new StatsComponent {HasStats = false});

                        component = new CollectibleComponent {CollectibleId = (ushort) (int) obj.Settings["collectible_id"]};
                        break;

                    case 26:
                        component = new PetComponent(); // TODO: set name and owner
                        break;

                    case 4:
                        component = new CharacterComponent(); // TODO: set properties
                        break;

                    case 19:
                        // Shooting gallery
                        break;

                    case 17:
                        var items = await _server.CDClient.GetInventoryItemsAsync(c.ComponentId);

                        component = new InventoryComponent // TODO: make this work
                        {
                            Items = items.Select(i => new InventoryItem
                            {
                                InventoryItemId = Utils.GenerateObjectId(),
                                Count = i.ItemCount,
                                LOT = i.ItemId,
                                Slot = -1
                            }).ToArray()
                        };
                        break;

                    case 5:
                        component = new ScriptComponent();
                        break;

                    case 9:
                        component = new SkillComponent();
                        break;

                    case 60:
                        component = new BaseCombatAIComponent(); // TODO: set properties
                        break;

                    case 48:
                        components.Add(new StatsComponent {HasStats = false});

                        component = new RebuildComponent
                        {
                            ActivatorPosition = (Vector3) obj.Settings["rebuild_activators"],
                            // following is temp
                            State = RebuildState.Completed,
                            Success = true
                        };
                        break;

                    case 25:
                        component = new MovingPlatformComponent
                        {
                            PathName = obj.Settings.TryGetValue("attached_path", out var path) ? (string) path : ""
                        };
                        break;

                    case 49:
                        component = new SwitchComponent(); // TODO: set state
                        break;

                    case 16:
                        component = new VendorComponent();
                        break;

                    case 6:
                        component = new BouncerComponent(); // TODO: set pet required
                        break;

                    case 39:
                        component = new ScriptedActivityComponent();
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
                        component = new RenderComponent
                        {
                            Disabled = (bool?) disabled ?? false
                        };
                        break;

                    case 50:
                        // Minigame
                        break;

                    case 107:
                        component = new Component107();
                        break;

                    case 69:
                        component = new TriggerComponent(); // set id
                        break;
                }

                if (component != null)
                {
                        //file.WriteLine($"        {component.GetType().Name} ({c.ComponentType}): {c.ComponentId}");
                    components.Add(component);
                }
            }

                //file.WriteLine();
            // }

            var data = new ReplicaPacket
            {
                ObjectId = Utils.GenerateObjectId(),
                LOT = obj.LOT,
                Name = obj.Settings.TryGetValue("npcName", out var npcName) ? (string) npcName : "",
                Created = 0,
                Scale = obj.Scale,
                Components = components.ToArray(),
                SpawnerObjectId = spawnLOT == 176 ? (long) obj.ObjectId : -1,
                ParentObjectId = parentId
            };

            if (obj.Settings.TryGetValue("spawnActivator", out var spawnActivator) && (bool) spawnActivator)
            {
                var spawner = await CreateObjectAsync(new LevelObject
                {
                    LOT = 6604,
                    Position = (Vector3) obj.Settings["rebuild_activators"],
                    Rotation = Vector4.Zero,
                    Scale = -1,
                    Settings = new Dictionary<string, object>()
                }, data.ObjectId);

                SpawnObject(spawner);

                data.ChildObjectIds = new[] {spawner.ObjectId};
            }

            return data;
        }
    }
}