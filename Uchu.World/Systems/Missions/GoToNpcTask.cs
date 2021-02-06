using System.Linq;
using System.Threading.Tasks;

namespace Uchu.World.Systems.Missions
{
    public class GoToNpcTask : MissionTaskInstance
    {
        public GoToNpcTask(MissionInstance mission, int taskId, int missionTaskIndex) 
            : base(mission, taskId, missionTaskIndex)
        {
        }

        public override MissionTaskType Type => MissionTaskType.GoToNpc;

        public override bool Completed => Progress.Contains(Target);

        public async Task ReportProgress(Lot lot)
        {
            if (Target != lot)
                return;

            AddProgress(lot);
            
            if (Completed)
                await CheckMissionCompletedAsync();
        }
    }
}