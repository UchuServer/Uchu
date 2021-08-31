using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.General
{
    [ScriptName("l_qb_spawner.lua")]
    public class QBSpawner : ObjectScript
    {
        /// <summary>
        /// Creates the object script.
        /// </summary>
        /// <param name="gameObject">Game object to control with the script.</param>
        public QBSpawner(GameObject gameObject) : base(gameObject)
        {
            //going off only AG, the effect of this can be done with only one script (Wall.cs), but i don't know if this script is used in other worlds
            //if needed, comment out the script name so that it can be checked
        }
    }
}