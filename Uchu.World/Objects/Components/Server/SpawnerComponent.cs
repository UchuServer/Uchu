using System.Collections.Generic;
using InfectedRose.Lvl;
using Uchu.Core;

namespace Uchu.World
{
    [ServerComponent(Id = ComponentId.SpawnerComponent)]
    public class SpawnerComponent : Component
    {
        public List<GameObject> ActiveSpawns { get; }

        public LevelObjectTemplate LevelObject { get; set; }

        protected SpawnerComponent()
        {
            ActiveSpawns = new List<GameObject>();
            
            Listen(OnStart, () =>
            {
                GameObject.Layer = StandardLayer.Spawner;
            });
        }

        public Lot SpawnTemplate { get; set; }

        public uint SpawnNodeId { get; set; }

        public LegoDataDictionary Settings { get; set; }

        public GameObject GetSpawnObject()
        {
            return GameObject.Instantiate(new LevelObjectTemplate
            {
                Lot = SpawnTemplate,
                Position = Transform.Position,
                Rotation = Transform.Rotation,
                Scale = LevelObject.Scale,
                LegoInfo = Settings,
                ObjectId = ObjectId.NewObjectId(ObjectIdFlags.Spawned | ObjectIdFlags.Client)
            }, Zone, this);
        }

        public GameObject Spawn()
        {
            var obj = GetSpawnObject();

            Start(obj);

            ActiveSpawns.Add(obj);

            Listen(obj.OnDestroyed, () => { ActiveSpawns.Remove(obj); });

            return obj;
        }
    }
}