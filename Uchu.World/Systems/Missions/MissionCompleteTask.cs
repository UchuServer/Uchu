using System.Linq;
using System.Threading.Tasks;

namespace Uchu.World.Systems.Missions
{
    public class MissionCompleteTask : MissionTaskInstance
    {
        public MissionCompleteTask(MissionInstance mission, int taskId, int missionTaskIndex) 
            : base(mission, taskId, missionTaskIndex)
        {
        }

        public MissionCompleteTask(MissionInstance mission, MissionTaskInstance cachedInstance) : base(mission,
            cachedInstance)
        {
        }
        
        public override MissionTaskType Type => MissionTaskType.MissionComplete;

        public override bool Completed => Targets.All(target => Progress.Contains(target));

        public async Task ReportProgress(int id)
        {
            if (!Targets.Contains(id) || Progress.Contains(id))
                return;

            AddProgress(id);
            
            if (Completed)
                await CheckMissionCompletedAsync();
        }
    }
}