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
                GameObject[] pirateSpawners = default;
                GameObject[] captainSpawners = default;
                Listen(quickBuild.OnStateChange, (state) =>
                {
                    if (pirateSpawners == default || captainSpawners == default)
                    {
                        //prevent zone freeze when player loads in an area where this script is present
                        pirateSpawners = Zone.GameObjects.Where(i => i.Name == $"Jail0{wall}" && i is SpawnerNetwork).ToArray();
                        captainSpawners = Zone.GameObjects.Where(i => i.Name == $"JailCaptain0{wall}" && i is SpawnerNetwork).ToArray();
                    }
                    if (state == RebuildState.Completed)
                    {
                        Console.WriteLine("rebuilt");
                        foreach (var pirate in pirateSpawners)
                        {
                            if (pirate is SpawnerNetwork network) network.Deactivate();
                        }
                        foreach (var captain in captainSpawners)
                        {
                            if (captain is SpawnerNetwork network) network.Deactivate();
                        }
                    }
                    else if (state == RebuildState.Resetting)
                    {
                        foreach (var pirate in pirateSpawners)
                        {
                            if (pirate is SpawnerNetwork network){
                                network.Reset();
                                network.Activate();
                            }
                        }
                        foreach (var captain in captainSpawners)
                        {
                            if (captain is SpawnerNetwork network){
                                network.Reset();
                                network.Activate();
                            }
                        }
                    }
                });
            }
        }
    }
}