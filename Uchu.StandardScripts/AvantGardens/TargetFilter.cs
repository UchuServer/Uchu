using Uchu.Core.Resources;
using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.AvantGardens
{
    /// <summary>
    /// Script to show/hide targets based on whether the player has the relevant mission
    /// </summary>
    [LotSpecific(14380)]
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
