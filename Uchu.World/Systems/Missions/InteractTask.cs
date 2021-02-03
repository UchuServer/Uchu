using System.Linq;
using System.Threading.Tasks;

namespace Uchu.World.Systems.Missions
{
    public class InteractTask : MissionTaskInstance
    {
        public InteractTask(MissionInstance mission, int taskId, int missionTaskIndex) 
            : base(mission, taskId, missionTaskIndex)
        {
        }

        public override MissionTaskType Type => MissionTaskType.Interact;

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