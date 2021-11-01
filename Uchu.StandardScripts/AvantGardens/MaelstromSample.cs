using Uchu.World;
using Uchu.World.Scripting.Native;
using Uchu.Core.Resources;

namespace Uchu.StandardScripts.AvantGardens
{
    /// <summary>
    /// Script to show/hide maelstrom samples based on whether the player has the relevant mission
    /// </summary>
    [LotSpecific(14718)]
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
