using Uchu.Core;
using Uchu.World.Parsers;

namespace Uchu.World
{
    public static class InstancingUtil
    {
        public static GameObject Spawner(LevelObject levelObject, Object parent)
        {
            if (!levelObject.Settings.TryGetValue("spawntemplate", out var spawnTemplate))
            {
                Logger.Error("Instantiating a spawner without a \"spawntemplete\" is now allowed.");
                return null;
            }

            var instance = GameObject.Instantiate<GameObject>(parent, position: levelObject.Position,
                rotation: levelObject.Rotation, objectId: Utils.GenerateObjectId(), lot: levelObject.Lot);

            var spawnerComponent = instance.AddComponent<SpawnerComponent>();
            
            spawnerComponent.Settings = levelObject.Settings;
            spawnerComponent.SpawnTemplate = (int) spawnTemplate;
            
            levelObject.Settings.Remove("spawntemplate");
            
            return instance;
        }
    }
}