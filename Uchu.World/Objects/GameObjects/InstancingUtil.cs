using System;
using System.Linq;
using System.Numerics;
using InfectedRose.Luz;
using InfectedRose.Lvl;
using Uchu.Core;

namespace Uchu.World
{
    public static class InstancingUtil
    {
        private static readonly Random Random = new Random();
        
        public static GameObject Spawner(LevelObjectTemplate levelObject, Object parent)
        {
            if (!levelObject.LegoInfo.TryGetValue("spawntemplate", out var spawnTemplate))
            {
                Logger.Error("Instantiating a spawner without a \"spawntemplete\" is now allowed.");
                return null;
            }

            if (spawnTemplate is string s)
            {
                spawnTemplate = int.Parse(s);
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

            spawnerComponent.Settings = levelObject.LegoInfo;
            spawnerComponent.SpawnTemplate = new Lot((int) spawnTemplate);
            spawnerComponent.LevelObject = levelObject;

            levelObject.LegoInfo.Remove("spawntemplate");

            return instance;
        }

        public static GameObject Spawner(LuzSpawnerPath spawnerPath, Object parent)
        {
            if (spawnerPath.Waypoints.Length == default) return default;

            var wayPoint = (LuzSpawnerWaypoint) spawnerPath.Waypoints[default];

            var spawner = GameObject.Instantiate(
                parent,
                spawnerPath.PathName,
                wayPoint.Position,
                wayPoint.Rotation,
                -1,
                spawnerPath.SpawnerObjectId,
                Lot.Spawner
            );

            /* TODO : 
            
            var settings = new LegoDataDictionary();
            foreach (var config in wayPoint.Configs)
            {
                settings.Add(config.ConfigName, config.ConfigTypeAndValue);
            }

            spawner.Settings = settings;
            */

            spawner.Settings.Add("respawn", spawnerPath.RespawnTime);

            var spawnerComponent = spawner.AddComponent<SpawnerComponent>();
            
            spawnerComponent.Settings = spawner.Settings;
            spawnerComponent.SpawnTemplate = (int) spawnerPath.SpawnedLot;
            spawnerComponent.LevelObject = new LevelObjectTemplate
            {
                Scale = 1
            };

            return spawner;
        }

        public static GameObject Loot(Lot lot, Player owner, GameObject source, Vector3 spawn)
        {
            var drop = GameObject.Instantiate(
                owner.Zone,
                lot,
                spawn
            );

            drop.Layer = StandardLayer.Hidden;

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
            try
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
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }
    }
}