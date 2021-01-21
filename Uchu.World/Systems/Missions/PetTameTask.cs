using System.Linq;
using System.Threading.Tasks;

namespace Uchu.World.Systems.Missions
{
    public class PetTameTask: MissionTaskInstance
    {
        public PetTameTask(MissionInstance mission, int taskId, int missionTaskIndex) 
            : base(mission, taskId, missionTaskIndex)
        {
        }

        public PetTameTask(MissionInstance mission, MissionTaskInstance cachedInstance) : base(mission,
            cachedInstance)
        {
        }
        
        public override MissionTaskType Type => MissionTaskType.TamePet;

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