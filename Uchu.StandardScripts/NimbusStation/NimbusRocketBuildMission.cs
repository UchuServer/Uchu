using System.Collections.Generic;
using System.Linq;
using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.NimbusStation
{
    /// <summary>
    /// Native implementation of scripts/ai/ns/l_ns_modular_build.lua
    /// </summary>
    [ScriptName("l_ns_modular_build.lua")]
    public class NimbusRocketBuildMission : ObjectScript
    {
        /// <summary>
        /// Parts of the Nimbus Rocket to complete the mission.
        /// </summary>
        public static readonly List<Lot> NimbusRocketParts = new List<Lot>()
        {
            9516, // Nimbus Rocket Nose Cone
            9517, // Nimbus Rocket Cockpit
            9518, // Nimbus Rocket Engine
        };
        
        /// <summary>
        /// Creates the object script.
        /// </summary>
        /// <param name="gameObject">Game object to control with the script.</param>
        public NimbusRocketBuildMission(GameObject gameObject) : base(gameObject)
        {
            Listen(Zone.OnPlayerLoad, player =>
            {
                if (!player.TryGetComponent<ModularBuilderComponent>(out var modularBuilderComponent)) return;
                if (!player.TryGetComponent<MissionInventoryComponent>(out var missionInventoryComponent)) return;
                
                // Listen for rockets being built.
                Listen(modularBuilderComponent.OnBuildFinished, async (models) =>
                {
                    // Complete the task if one of the models is a Nimbus Rocket part.
                    var isNimbusRocket = models.Any(model => NimbusRocketParts.Contains(model));
                    if (!isNimbusRocket) return;
                    await missionInventoryComponent.ScriptAsync(1178, 9980);
                });
            });
        }
    }
}