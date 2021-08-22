using System.Linq;
using System.Threading.Tasks;

namespace Uchu.World.Systems.Missions
{
    public class CollectPowerupTask : MissionTaskInstance
    {
        public CollectPowerupTask(MissionInstance mission, int taskId, int missionTaskIndex) 
            : base(mission, taskId, missionTaskIndex)
        {
        }

        public override MissionTaskType Type => MissionTaskType.CollectPowerup;

        public async Task ReportProgress(int lot)
        {
            if (!Targets.Contains(lot))
                return;
            
            AddProgress(lot);

            if (Completed)
                await CheckMissionCompletedAsync();
        }
    }
}