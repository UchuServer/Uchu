using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using InfectedRose.Luz;
using InfectedRose.Lvl;
using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.NinjagoMonastery
{
    [ZoneSpecific(2000)]
    public class SkeletonBossWeapons : NativeScript
    {
        public override Task LoadAsync()
        {
            // Add death listener to initially spawned enemies
            var chopov = Zone.GameObjects.First(obj => obj.Lot == 16047);
            Listen(chopov.GetComponent<DestroyableComponent>().OnDeath,
                () => { SpawnQuickbuild("EarthShrine_ERail"); }
            );

            // var frakjaw = Zone.GameObjects.First(obj => obj.Lot == 16048);
            // Listen(frakjaw.GetComponent<DestroyableComponent>().OnDeath,
            //     () => { SpawnQuickbuild("..."); }
            // );

            var krazi = Zone.GameObjects.First(obj => obj.Lot == 16049);
            Listen(krazi.GetComponent<DestroyableComponent>().OnDeath,
                () => { SpawnQuickbuild("LightningShrine_LRail"); }
            );

            var bonezai = Zone.GameObjects.First(obj => obj.Lot == 16050);
            Listen(bonezai.GetComponent<DestroyableComponent>().OnDeath,
                () => { SpawnQuickbuild("IceShrine_QBBouncer"); }
            );

            // Add listener to enemies spawned in the future
            Listen(Zone.OnObject, newObject =>
            {
                if (!(newObject is GameObject newObj))
                    return;

                switch (newObj.Lot)
                {
                    case 16047: // Chopov (Earth)
                        Listen(newObj.GetComponent<DestroyableComponent>().OnDeath,
                            () => { SpawnQuickbuild("EarthShrine_ERail"); }
                        );
                        break;

                    // case 16048: // Frakjaw (Fire)
                    //     Listen(newObj.GetComponent<DestroyableComponent>().OnDeath,
                    //         () => { SpawnQuickbuild("..."); }
                    //     );
                    //     break;

                    case 16049: // Krazi (Lightning)
                        Listen(newObj.GetComponent<DestroyableComponent>().OnDeath,
                            () => { SpawnQuickbuild("LightningShrine_LRail"); }
                        );
                        break;

                    case 16050: // Bonezai (Ice)
                        Listen(newObj.GetComponent<DestroyableComponent>().OnDeath,
                            () => { SpawnQuickbuild("IceShrine_QBBouncer"); }
                        );
                        break;
                }
            });
            return Task.CompletedTask;
        }

        private void SpawnQuickbuild(string pathName)
        {
            // Find spawner path in LUZ
            var path = Zone.ZoneInfo.LuzFile.PathData.OfType<LuzSpawnerPath>()
                .First(p => p.PathName == pathName);

            // Create spawner, this keeps the quickbuild available until the enemy respawns (there's a
            // DestroySpawnerNetworkObjects command triggered when that happens)
            var network = InstancingUtilities.SpawnerNetwork(path, Zone);

            Start(network);

            network?.TrySpawn();
        }
    }
}
