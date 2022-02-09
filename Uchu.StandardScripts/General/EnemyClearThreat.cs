using Uchu.World;
using Uchu.World.Scripting.Native;
using System.Linq;

namespace Uchu.StandardScripts.General
{
    [ScriptName("l_enemy_clear_threat.lua")]
    public class EnemyClearThreat : ObjectScript
    {
        /// <summary>
        /// Creates the object script.
        /// </summary>
        /// <param name="gameObject">Game object to control with the script.</param>
        public EnemyClearThreat(GameObject gameObject) : base(gameObject)
        {
            //TODO: When enemy AI works, turn enemy around, display cross sign above head
        }
    }
}