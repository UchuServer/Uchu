using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.General
{
    //done with one script, comment out if needed
    [ScriptName("l_qb_spawner.lua")]
    //already implemented by checking if an object has the POI tag on every collision
    [ScriptName("l_poi_mission.lua")]
    //function done by LaunchpadEvent.cs
    [ScriptName("l_ag_zone_player.lua")]
    public class UnusedScript : ObjectScript
    {
        /// <summary>
        /// Creates the object script.
        /// </summary>
        /// <param name="gameObject">Game object to control with the script.</param>
        public UnusedScript(GameObject gameObject) : base(gameObject)
        {
            //TODO: replace this with actual script ignore list
        }
    }
}