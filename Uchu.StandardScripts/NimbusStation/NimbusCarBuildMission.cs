using System.Collections.Generic;
using System.Linq;
using Uchu.Core;
using Uchu.Core.Resources;
using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.NimbusStation
{
    /// <summary>
    /// Native implementation of scripts/ai/ns/l_ns_car_modular_build.lua
    /// </summary>
    [ScriptName("l_ns_car_modular_build.lua")]
    public class NimbusCarBuildMission : ObjectScript
    {
        /// <summary>
        /// Creates the object script.
        /// </summary>
        /// <param name="gameObject">Game object to control with the script.</param>
        public NimbusCarBuildMission(GameObject gameObject) : base(gameObject)
        {
            Listen(Zone.OnPlayerLoad, player =>
            {
                if (!player.TryGetComponent<ModularBuilderComponent>(out var modularBuilderComponent)) return;
                if (!player.TryGetComponent<MissionInventoryComponent>(out var missionInventoryComponent)) return;

                // Listen for rockets being built.
                Listen(modularBuilderComponent.OnBuildFinished, async (build) =>
                {
                    Logger.Debug("OnBuildFinished: " + build.model);
                    if (build.model == Lot.ModularCar)
                    {
                        await missionInventoryComponent.ScriptAsync(939, 8044);
                        Logger.Debug("MissionFinished");
                    }
                });
            });
        }
    }
}