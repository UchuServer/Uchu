using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using InfectedRose.Lvl;
using Uchu.Core;
using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.NinjagoMonastery
{
    [ZoneSpecific(2000)]
    public class SkeletonTowers : NativeScript
    {
        public override Task LoadAsync()
        {
            var towerSpawnerObjects = Zone.GameObjects.Where(gameObject =>
                gameObject.TryGetComponent<SpawnerComponent>(out var spawnerComponent)
                && spawnerComponent.SpawnTemplate == 15953);

            foreach (var towerSpawnerObject in towerSpawnerObjects)
            {
                SetUpLegs(towerSpawnerObject);

                Listen(towerSpawnerObject.GetComponent<SpawnerComponent>().OnRespawnTimeCompleted, p =>
                {
                    // Recreate leg spawners, they were destroyed when the tower was smashed
                    SetUpLegs(towerSpawnerObject);
                });
            }

            return Task.CompletedTask;
        }

        private void SetUpLegs(GameObject towerSpawnerObject)
        {
            var towerSpawnerComponent = towerSpawnerObject.GetComponent<SpawnerComponent>();
            var legSpawnerComponents = new List<SpawnerComponent>();

            var radius = (float) towerSpawnerComponent.Settings["hort_offset"];

            for (var i = 0; i < 3; i++)
            {
                // Calculate leg position
                var origin = towerSpawnerObject.Transform.Position;
                var towerRotation = towerSpawnerObject.Transform.EulerAngles.Y / 360 * Math.Tau;
                const double rotationOffset = -1f / 4 * Math.Tau;
                var relativePosition = new Vector3(
                    radius * (float) Math.Cos((float) i / 4 * Math.Tau - towerRotation + rotationOffset),
                    0,
                    radius * (float) Math.Sin((float) i / 4 * Math.Tau - towerRotation + rotationOffset));
                var position = origin + relativePosition;

                var legSpawnerObject = GameObject.Instantiate(
                    Zone,
                    "SkeletonTowerLegSpawner",
                    position,
                    Quaternion.Identity,
                    -1,
                    ObjectId.Standalone,
                    Lot.Spawner
                );

                // Set leg orientation
                legSpawnerObject.Transform.LookAt(towerSpawnerObject.Transform.Position);

                var legSpawnerComponent = legSpawnerObject.AddComponent<SpawnerComponent>();
                legSpawnerComponent.Settings = new LegoDataDictionary();
                legSpawnerComponent.SpawnTemplate = new Lot((int) towerSpawnerComponent.Settings["legLOT"]);
                legSpawnerComponent.LevelObject = new LevelObjectTemplate {Scale = 1};
                // this time is a wild guess
                legSpawnerComponent.RespawnTime = 50 * 1000;

                Start(legSpawnerObject);

                // Spawn leg
                legSpawnerComponent.Spawn();

                // Keep track of leg spawners
                legSpawnerComponents.Add(legSpawnerComponent);

                // Listen for legs being smashed
                Listen(legSpawnerComponent.OnRespawnInitiated, player =>
                {
                    // Return if any leg is alive
                    if (legSpawnerComponents.Any(spawnerComponent => spawnerComponent.ActiveSpawns.Count != 0))
                        return;

                    // Arriving here in the code means all legs are smashed;
                    // the tower should fall down and explode.

                    // Legs should not respawn legs until tower respawns.
                    // Destroy current leg spawners
                    foreach (var legSpawner in legSpawnerComponents.ToArray())
                    {
                        Destroy(legSpawner.GameObject);
                    }

                    // Get tower object
                    var tower = towerSpawnerComponent.ActiveSpawns.FirstOrDefault();
                    if (tower == default)
                        return;

                    // TODO: Move tower to the ground
                    // There is no MovingPlatform component, but the object type is MovingPlatforms.
                    // Not sure how to handle this.
                    // (https://lu.lcdruniverse.org/explorer/objects/15953)

                    // Damage tower, destroying it
                    Task.Run(() =>
                    {
                        tower.GetComponent<DestroyableComponent>().Damage(1, legSpawnerObject);
                    });

                    // Progress missions
                    if (player.TryGetComponent<MissionInventoryComponent>(out var missionInventoryComponent))
                    {
                        // https://lu.lcdruniverse.org/explorer/missions/1806
                        missionInventoryComponent.ScriptAsync(2567, 15953);
                        // https://lu.lcdruniverse.org/explorer/missions/2032
                        missionInventoryComponent.ScriptAsync(2851, 15953);
                    }
                });
            }
        }
    }
}
