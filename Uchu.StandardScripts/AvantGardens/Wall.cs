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
                    var obj = GameObject.Instantiate(new LevelObjectTemplate
                    {
                        Lot = 4712, //stromling
                        Position = spawnObject.Transform.Position - new Vector3(15, 0 , 0), //manual offset because it spawns directly in the wall and causes weird behavior
                        //it appears to be dead but still attacking, i've seen this bug elsewhere and i don't know how to fix it
                        Rotation = spawnObject.Transform.Rotation,
                        Scale = 1,
                        LegoInfo = new LegoDataDictionary()
                    }, spawnObject.Zone);

                    Object.Start(obj);
                    GameObject.Construct(obj);
                }
            });
        }
    }
}