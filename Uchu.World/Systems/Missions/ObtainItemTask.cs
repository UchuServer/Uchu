using System.Linq;
using System.Threading.Tasks;

namespace Uchu.World.Systems.Missions
{
    public class ObtainItemTask : MissionTaskInstance
    {
        public ObtainItemTask(MissionInstance mission, int taskId, int missionTaskIndex) 
            : base(mission, taskId, missionTaskIndex)
        {
        }
        
        public override MissionTaskType Type => MissionTaskType.ObtainItem;

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