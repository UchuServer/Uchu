using System.Linq;
using System.Threading.Tasks;

namespace Uchu.World.Systems.Missions
{
    public class UseConsumableTask : MissionTaskInstance
    {
        public UseConsumableTask(MissionInstance mission, int taskId, int missionTaskIndex) 
            : base(mission, taskId, missionTaskIndex)
        {
        }
        
        public override MissionTaskType Type => MissionTaskType.UseConsumable;

        public override bool Completed => Progress.Contains(Target);

        public async Task ReportProgress(int lot)
        {
            if (Target != lot)
                return;
            
            AddProgress(lot);

            if (Completed)
                await CheckMissionCompletedAsync();
        }
    }
}