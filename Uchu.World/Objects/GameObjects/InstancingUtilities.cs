using System;
using System.Numerics;
using InfectedRose.Luz;
using InfectedRose.Lvl;
using Uchu.Core;

namespace Uchu.World
{
    public static class InstancingUtilities
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
                objectId: (ObjectId) levelObject.ObjectId,
                lot: levelObject.Lot
            );

            if (levelObject.LegoInfo.TryGetValue("trigger_id", out var trigger))
            {
                Logger.Debug($"SPAWN TRIGGER: {trigger}");
            }

            var spawnerComponent = instance.AddComponent<SpawnerComponent>();

            spawnerComponent.Settings = levelObject.LegoInfo;
            spawnerComponent.SpawnTemplate = new Lot((int) spawnTemplate);
            spawnerComponent.LevelObject = levelObject;

            if (levelObject.LegoInfo.TryGetValue("respawn", out var respawnTime))
                spawnerComponent.RespawnTime = Convert.ToInt32((float) respawnTime * 1000);

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
                (ObjectId) spawnerPath.SpawnerObjectId,
                Lot.Spawner
            );

            spawner.Settings.Add("respawn", spawnerPath.RespawnTime);

            var spawnerComponent = spawner.AddComponent<SpawnerComponent>();

            //spawnerComponent.SpawnsToMaintain = (int) spawnerPath.NumberToMaintain;
            spawnerComponent.RespawnTime = (int) spawnerPath.RespawnTime * 1000;
            spawnerComponent.Settings = spawner.Settings;
            spawnerComponent.SpawnTemplate = (int) spawnerPath.SpawnedLot;
            spawnerComponent.LevelObject = new LevelObjectTemplate
            {
                Scale = 1
            };

            return spawner;
        }

        public static GameObject InstantiateLoot(Lot lot, Player owner, GameObject source, Vector3 spawn)
        {
            if (owner is null) return default;

            try
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
            catch (Exception e)
            {
                Logger.Error(e);

                return default;
            }
        }
        
        public static void InstantiateCurrency(int currency, Player owner, GameObject source, Vector3 spawn)
        {
            if (owner is null) return;
            if (currency <= 0) return;
            
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

                var character = owner.GetComponent<CharacterComponent>();
                character.EntitledCurrency += currency;
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }
    }
}
