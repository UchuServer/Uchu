using Uchu.Core.Resources;
using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.AvantGardens
{
    /// <summary>
    /// Native implementation of scripts/02_client/map/ag/l_ag_plunger_target.lua
    /// </summary>
    [ScriptName("l_ag_plunger_target.lua")]
    public class TargetFilter : ObjectScript
    {
        /// <summary>
        /// Creates the object script.
        /// </summary>
        /// <param name="gameObject">Game object to control with the script.</param>
        public TargetFilter(GameObject gameObject) : base(gameObject)
        {
            var missionFilter = gameObject.AddComponent<MissionFilterComponent>();
            missionFilter.AddMissionIdToFilter(MissionId.SixShooter);
        }
    }
}