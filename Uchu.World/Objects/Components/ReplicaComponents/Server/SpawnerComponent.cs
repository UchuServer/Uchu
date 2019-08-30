using System.Collections.Generic;
using RakDotNet.IO;
using Uchu.Core;
using Uchu.World.Collections;
using Uchu.World.Parsers;

namespace Uchu.World
{
    public class SpawnerComponent : ReplicaComponent
    {
        public int SpawnTemplate { get; set; }
        
        public uint SpawnNodeId { get; set; }
        
        public LegoDataDictionary Settings { get; set; }

        public readonly List<GameObject> ActiveSpawns = new List<GameObject>();
        
        public override ReplicaComponentsId Id => ReplicaComponentsId.Spawner;
        
        public override void FromLevelObject(LevelObject levelObject)
        {
            
        }

        public override void Construct(BitWriter writer)
        {
        }

        public override void Serialize(BitWriter writer)
        {
        }
        
        public GameObject GetSpawnObject()
        {
            return GameObject.Instantiate(new LevelObject
            {
                Lot = SpawnTemplate,
                ObjectId = (ulong) Utils.GenerateObjectId(),
                Position = Transform.Position,
                Rotation = Transform.Rotation,
                Scale = 1,
                Settings = Settings
            }, Zone, this);
        }

        public GameObject Spawn()
        {
            var obj = GetSpawnObject();
            
            obj.Construct();

            ActiveSpawns.Add(obj);

            obj.OnDestroyed += () => { ActiveSpawns.Remove(obj); };
            
            return obj;
        }
    }
}