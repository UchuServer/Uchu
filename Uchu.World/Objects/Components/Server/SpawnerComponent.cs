using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InfectedRose.Lvl;
using Uchu.Core;

namespace Uchu.World
{
    [ServerComponent(Id = ComponentId.SpawnerComponent)]
    public class SpawnerComponent : Component
    {
        private readonly Random _random;
        public List<GameObject> ActiveSpawns { get; }

        public LevelObjectTemplate LevelObject { get; set; }
        
        public List<SpawnLocation> SpawnLocations { get; set; }

        public int SpawnsToMaintain { get; set; } = 1;

        public int RespawnTime { get; set; } = 10000;

        public Lot SpawnTemplate { get; set; }

        public uint SpawnNodeId { get; set; }
        
        /// <summary>
        /// Event that's called after a game object is smashed and the spawner is waiting to respawn the game object,
        /// the argument is the respawn time until the spawner will respawn the GO 
        /// </summary>
        public Event<int> OnRespawnInitiated { get; }

        public LegoDataDictionary Settings { get; set; }

        protected SpawnerComponent()
        {
            _random = new Random();
            SpawnLocations = new List<SpawnLocation>();
            ActiveSpawns = new List<GameObject>();
            OnRespawnInitiated = new Event<int>();
            
            Listen(OnStart, () =>
            {
                if (Settings != default)
                {
                    if (Settings.TryGetValue("number_to_maintain", out var value))
                    {
                        SpawnsToMaintain = (int) value;
                    }
                }

                if (SpawnLocations.Count == 0)
                {
                    SpawnLocations.Add(new SpawnLocation
                    {
                        Position = Transform.Position,
                        Rotation = Transform.Rotation
                    });
                }

                GameObject.Layer = StandardLayer.Spawner;
            });
        }

        private GameObject GenerateSpawnObject()
        {
            var location = FindLocation();

            location.InUse = true;

            var o = new LevelObjectTemplate
            {
                Lot = SpawnTemplate,
                Position = location.Position,
                Rotation = location.Rotation,
                Scale = LevelObject.Scale,
                LegoInfo = Settings,
                ObjectId = ObjectId.FromFlags(ObjectIdFlags.Spawned | ObjectIdFlags.Client)
            };
            
            var obj = GameObject.Instantiate(o, Zone, this);

            if (obj.TryGetComponent<DestructibleComponent>(out var destructibleComponent))
            {
                Listen(destructibleComponent.OnSmashed, (smasher, lootOwner) =>
                {
                    location.InUse = false;
                });
            }

            return obj;
        }

        private SpawnLocation FindLocation()
        {
            var locations = SpawnLocations.Where(s => !s.InUse).ToArray();

            if (locations.Length == 0)
            {
                return new SpawnLocation
                {
                    Position = Transform.Position,
                    Rotation = Transform.Rotation
                };
            }
            
            var location = locations[_random.Next(locations.Length)];

            return location;
        }

        public GameObject Spawn()
        {
            var obj = GenerateSpawnObject();
            Start(obj);
            GameObject.Construct(obj);
            ActiveSpawns.Add(obj);

            Listen(obj.OnDestroyed, () =>
            {
                ActiveSpawns.Remove(obj);
            });

            if (obj.TryGetComponent<DestructibleComponent>(out var destructibleComponent))
            {
                Listen(destructibleComponent.OnSmashed, async (smasher, lootOwner) =>
                {
                    Destroy(obj);
                    await Task.WhenAll(OnRespawnInitiated.InvokeAsync(RespawnTime), Task.Delay(RespawnTime));
                    Spawn();
                });
            }

            return obj;
        }

        public void SpawnCluster()
        {
            for (var i = 0; i < SpawnsToMaintain; i++)
            {
                Spawn();
            }
        }
    }
}