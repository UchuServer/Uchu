using System;
using System.Numerics;
using Uchu.Core;
using Uchu.World.Parsers;

namespace Uchu.World
{
    public static class InstancingUtil
    {
        private static readonly Random Random = new Random(); 
        
        public static GameObject Spawner(LevelObject levelObject, Object parent)
        {
            if (!levelObject.Settings.TryGetValue("spawntemplate", out var spawnTemplate))
            {
                Logger.Error("Instantiating a spawner without a \"spawntemplete\" is now allowed.");
                return null;
            }

            var instance = GameObject.Instantiate(
                parent,
                position: levelObject.Position,
                rotation: levelObject.Rotation,
                scale: -1,
                objectId: (long) levelObject.ObjectId,
                lot: levelObject.Lot
            );

            var spawnerComponent = instance.AddComponent<SpawnerComponent>();

            spawnerComponent.Settings = levelObject.Settings;
            spawnerComponent.SpawnTemplate = new Lot((int) spawnTemplate);
            spawnerComponent.LevelObject = levelObject;

            levelObject.Settings.Remove("spawntemplate");

            return instance;
        }

        public static GameObject Loot(Lot lot, Player owner, GameObject source, Vector3 spawn)
        {
            var drop = GameObject.Instantiate(
                owner.Zone,
                lot,
                spawn
            );

            var finalPosition = new Vector3
            {
                X = spawn.X + ((float) Random.NextDouble() % 1f - 0.5f) * 20f,
                Y = spawn.Y,
                Z = spawn.Z + ((float) Random.NextDouble() % 1f - 0.5f) * 20f
            };

            owner.Message(new DropClientLootMessage
            {
                Associate = owner,
                Currency = 0,
                Lot = drop.Lot,
                Loot = drop,
                Owner = owner,
                Source = source,
                SpawnPosition = drop.Transform.Position + Vector3.UnitY,
                FinalPosition = finalPosition
            });
            
            return drop;
        }
        
        public static void Currency(int currency, Player owner, GameObject source, Vector3 spawn)
        {
            var finalPosition = new Vector3
            {
                X = spawn.X + ((float) Random.NextDouble() % 1f - 0.5f) * 20f,
                Y = spawn.Y,
                Z = spawn.Z + ((float) Random.NextDouble() % 1f - 0.5f) * 20f
            };

            owner.Message(new DropClientLootMessage
            {
                Associate = owner,
                Currency = currency,
                Owner = owner,
                Source = source,
                SpawnPosition = spawn + Vector3.UnitY,
                FinalPosition = finalPosition
            });

            owner.EntitledCurrency += currency;
        }
    }
}