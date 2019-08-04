using System.Collections.Generic;
using Uchu.Core;
using Uchu.World.Collections;
using Uchu.World.Parsers;

namespace Uchu.World
{
    public class Spawner : GameObject
    {
        public int SpawnTemplate { get; private set; }
        
        public uint SpawnNodeId { get; private set; }
        
        public LegoDataDictionary Settings { get; private set; }

        public readonly List<GameObject> ActiveSpawns = new List<GameObject>();
        
        public static Spawner Instantiate(LevelObject levelObject, Object parent)
        {
            if (!levelObject.Settings.TryGetValue("spawntemplate", out var spawnTemplate))
            {
                Logger.Error("Instantiating a spawner without a \"spawntemplete\" is now allowed.");
                return null;
            }

            var instance = GameObject.Instantiate<Spawner>(parent, position: levelObject.Position,
                rotation: levelObject.Rotation, objectId: Utils.GenerateObjectId(), lot: levelObject.LOT);

            levelObject.Settings.Remove("spawntemplate");
            instance.Settings = levelObject.Settings;

            instance.SpawnTemplate = (int) spawnTemplate;
            
            return instance;
        }
 
        public GameObject GetSpawnObject()
        {
            return GameObject.Instantiate(new LevelObject
            {
                LOT = SpawnTemplate,
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