using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.General
{
    //done with one script, comment out if needed
    [ScriptName("l_qb_spawner.lua")]
    //already implemented by checking if an object has the POI tag on every collision
    [ScriptName("l_poi_mission.lua")]
    public class UnusedScript : ObjectScript
    {
        /// <summary>
        /// Creates the object script.
        /// </summary>
        /// <param name="gameObject">Game object to control with the script.</param>
        public UnusedScript(GameObject gameObject) : base(gameObject)
        {
            //scripts that use this either do nothing or are implemented with other functions, and to prevent
            //the logger from showing those scripts for the "did not load" message, they use this blank
            //script
        }
    }
}