using System.Linq;
using System.Threading.Tasks;

namespace Uchu.World.Systems.Missions
{
    public class StatPickupsTask : MissionTaskInstance
    {
        public StatPickupsTask(MissionInstance mission, int taskId, int missionTaskIndex) 
            : base(mission, taskId, missionTaskIndex)
        {
        }

        public override MissionTaskType Type => MissionTaskType.StatPickups;

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