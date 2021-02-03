using System.Linq;
using System.Threading.Tasks;

namespace Uchu.World.Systems.Missions
{
    public class SmashTask : MissionTaskInstance
    {
        public SmashTask(MissionInstance mission, int taskId, int missionTaskIndex) 
            : base(mission, taskId, missionTaskIndex)
        {
        }
        
        public override MissionTaskType Type => MissionTaskType.Smash;
        
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