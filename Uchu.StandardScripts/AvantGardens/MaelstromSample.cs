using Uchu.World;
using Uchu.World.Scripting.Native;
using Uchu.Core.Resources;

namespace Uchu.StandardScripts.AvantGardens
{
    /// <summary>
    /// Native implementation of scripts/02_client/map/ag/l_ag_maelstrom_sample.lua
    /// </summary>
    [ScriptName("l_ag_maelstrom_sample.lua")]
    public class MaelstromSample : ObjectScript
    {
        /// <summary>
        /// Creates the object script.
        /// </summary>
        /// <param name="gameObject">Game object to control with the script.</param>
        public MaelstromSample(GameObject gameObject) : base(gameObject)
        {
            var missionFilter = gameObject.AddComponent<MissionFilterComponent>();
            missionFilter.AddMissionIdToFilter(MissionId.FollowingtheTrail);
            missionFilter.AddMissionIdToFilter(MissionId.SampleforScience);
        }
    }
}