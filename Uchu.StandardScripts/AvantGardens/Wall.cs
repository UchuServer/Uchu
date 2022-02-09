using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using InfectedRose.Core;
using InfectedRose.Lvl;
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
                GameObject[] spawners = Zone.GameObjects.Where(t => t.GetGroups().Contains("AG_WallSpawner_1")).ToArray();
                foreach (var spawnObject in spawners)
                {
                    //TODO: add functional enemy spawn code
                }
            });
        }
    }
}