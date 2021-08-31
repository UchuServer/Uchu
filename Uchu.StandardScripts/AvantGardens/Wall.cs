using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Uchu.Physics;
using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.AvantGardens
{
    [ScriptName("l_ag_qb_wall.lua")]
    public class Wall : ObjectScript
    {
        /// <summary>
        /// Creates the object script.
        /// </summary>
        /// <param name="gameObject">Game object to control with the script.</param>
        public Wall(GameObject gameObject) : base(gameObject)
        {
            var quickBuildComponent = gameObject.GetComponent<QuickBuildComponent>();
            Listen(quickBuildComponent.OnStateChange, (state) => 
            {
                if (state != RebuildState.Completed) return;
                //this is done every time it is quickbuilt in the lua script, i don't know how much of a performance impact this makes
                //and there's only one spawner with this in AG
                GameObject[] spawners = Zone.GameObjects.Where(t => t.GetGroups().Contains("AG_WallSpawner_1")).ToArray();
                foreach (var spawnObject in spawners)
                {
                    if (spawnObject.TryGetComponent<SpawnerComponent>(out var spawnerComponent))
                    {
                        //this runs but doesnt appear to do anything, i'd put it up to needing the ai overhaul or me doing it wrong
                        spawnerComponent.Spawn();
                    }
                }
            });
        }
    }
}