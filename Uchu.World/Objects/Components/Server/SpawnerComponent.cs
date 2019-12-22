using System.Collections.Generic;
using InfectedRose.Lvl;

namespace Uchu.World
{
    [ServerComponent(Id = ComponentId.SpawnerComponent)]
    public class SpawnerComponent : Component
    {
        public List<GameObject> ActiveSpawns { get; }

        public LevelObjectTemplate LevelObject;

        protected SpawnerComponent()
        {
            ActiveSpawns = new List<GameObject>();
            
            OnStart.AddListener(() =>
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
                LegoInfo = Settings
            }, Zone, this);
        }

        public GameObject Spawn()
        {
            var obj = GetSpawnObject();

            Start(obj);

            ActiveSpawns.Add(obj);

            obj.OnDestroyed.AddListener(() => { ActiveSpawns.Remove(obj); });

            return obj;
        }
    }
}