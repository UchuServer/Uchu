using System;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.GnarledForest
{
    [ScriptName("l_gf_jail_walls.lua")]
    public class JailGate : ObjectScript
    {
        /// <summary>
        /// Creates the object script.
        /// </summary>
        /// <param name="gameObject">Game object to control with the script.</param>
        public JailGate(GameObject gameObject) : base(gameObject)
        {
            if (gameObject.TryGetComponent<QuickBuildComponent>(out var quickBuild) && gameObject.Settings.TryGetValue("Wall", out var wallObject) && wallObject is string wall)
            {
                SpawnerNetwork pirateSpawner = GetSpawnerByName($"Jail0{wall}");
                SpawnerNetwork captainSpawner = GetSpawnerByName($"JailCaptain0{wall}");
                Listen(quickBuild.OnStateChange, (state) =>
                {
                    if (state == RebuildState.Completed)
                    {
                        pirateSpawner.Deactivate();
                        captainSpawner.Deactivate();
                    }
                    else if (state == RebuildState.Resetting)
                    {
                        pirateSpawner.Reset();
                        pirateSpawner.Activate();
                        captainSpawner.Reset();
                        captainSpawner.Activate();
                    }
                });
            }
        }
    }
}