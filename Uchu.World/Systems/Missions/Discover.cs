using System.Linq;
using System.Threading.Tasks;

namespace Uchu.World.Systems.Missions
{
    public class DiscoverTask : MissionTaskInstance
    {
        public DiscoverTask(MissionInstance mission, int taskId, int missionTaskIndex)
            : base(mission, taskId, missionTaskIndex)
        {
        }

        public override MissionTaskType Type => MissionTaskType.Discover;

        public async Task ReportProgress(GameObject gameObject)
        {
            // This is done from another component and just directly completed no need for this function
        }
    }
}