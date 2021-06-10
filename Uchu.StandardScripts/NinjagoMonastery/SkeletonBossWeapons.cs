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
        // This is very likely not the correct way to handle this.
        // There are some settings that, when set, cause the object to not appear client-side.
        // It'd probably be better to remove those with a deny list instead of using this allow list,
        // and this list should probably be used in more places than just this script.
        private readonly string[] _allowedSettings =
        {
            "quickbuild_cinematic",
            "quickbuild_cinematic_lead_in",
            "quickbuild_single_build",
            "quickbuild_single_build_player_flag",
            "quickbuild_single_build_shared",
            "quickbuild_skip_slam",
            "rebuild_activators",
            "rebuild_reset_time",
            "radius",
            "bouncer_destination",
            "bouncer_speed",
            "bouncer_uses_high_arc",
            "stickLanding",
            "lock_controls",
            "cancelBehaviorMovement",
            "rail_activator_active",
            "rail_activator_damage_immune",
            "rail_activator_start_on_activate",
            "rail_loop_sound",
            "rail_no_aggro",
            "rail_notify_activator_arrived",
            "rail_path",
            "rail_path_direction",
            "rail_path_start",
            "rail_show_name_billboard",
            "rail_start_sound",
            "rail_stop_sound",
            "rail_use_db",
            "lock_rail_camera",
            "interaction_distance",
            "enable_rail_collision",
            "RailGroup",
            "CamRlFaceTgt",
            "attached_cinematic_path",
            "setsRailCam",
            "bounding_radius_override",
            "plrToCam",
            "camGradSnap",
            "camPrefersToFadeObject",
            "rlLeadOut",
            "ignoreCameraCollision",
            "compTime",
            "tmeSmsh",
            "vlntDth",
            "respawn",
        };

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
            // TODO: fix camera path for spawned rails, currently it follows the player (looks weird for Chopov rail)
            // TODO: Bonezai bouncer does not work

            // Find spawner path in LUZ
            var path = Zone.ZoneInfo.LuzFile.PathData.OfType<LuzSpawnerPath>()
                .First(p => p.PathName == pathName);

            // Create spawner, this keeps the quickbuild available until the enemy respawns (there's a
            // DestroySpawnerNetworkObjects command triggered when that happens)
            var obj = InstancingUtilities.Spawner(path, Zone);
            if (obj == null)
                return;

            obj.Layer = StandardLayer.Hidden;

            var spawner = obj.GetComponent<SpawnerComponent>();
            spawner.SpawnsToMaintain = 1;
            spawner.SpawnLocations = path.Waypoints.Select(w => new SpawnLocation
            {
                Position = w.Position,
                Rotation = Quaternion.Identity,
                // TODO: Change Configs from LuzPathConfig[] to LegoDataDictionary in InfectedRose to avoid this ugly conversion
                Config = LegoDataDictionary.FromString(
                    string.Join(',', ((LuzSpawnerWaypoint) w).Configs
                        .Where(c => _allowedSettings.Contains(c.ConfigName))
                        .Select(c => c.ConfigName + "=" + c.ConfigTypeAndValue)), ','),
            }).ToList();

            Start(obj);

            spawner.Spawn();
        }
    }
}
