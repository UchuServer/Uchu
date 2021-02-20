using System.Linq;
using System.Threading.Tasks;

namespace Uchu.World.Systems.Missions
{
    public class QuickBuildTask : MissionTaskInstance
    {
        public QuickBuildTask(MissionInstance mission, int taskId, int missionTaskIndex) 
            : base(mission, taskId, missionTaskIndex)
        {
        }

        public override MissionTaskType Type => MissionTaskType.QuickBuild;

        public async Task ReportProgress(int lot, int activity)
        {
            if (!TryProgress(lot))
                TryProgress(activity);

            if (Completed)
                await CheckMissionCompletedAsync();
        }

        private bool TryProgress(int value)
        {
            if (!Targets.Contains(value))
                return false;
            
            AddProgress(value);

            return true;
        }
    }
}