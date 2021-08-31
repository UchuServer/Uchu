using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.General
{
    [ScriptName("l_poi_mission.lua")]
    public class POIMission : ObjectScript
    {
        /// <summary>
        /// Creates the object script.
        /// </summary>
        /// <param name="gameObject">Game object to control with the script.</param>
        public POIMission(GameObject gameObject) : base(gameObject)
        {
        }
    }
}