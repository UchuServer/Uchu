using System.Linq;
using System.Threading.Tasks;

namespace Uchu.World.Systems.Missions
{
    public class FlagTask : MissionTaskInstance
    {
        public FlagTask(MissionInstance mission, int taskId, int missionTaskIndex) 
            : base(mission, taskId, missionTaskIndex)
        {
        }

        public FlagTask(MissionInstance mission, MissionTaskInstance cachedInstance) : base(mission, cachedInstance)
        {
        }
        
        public override MissionTaskType Type => MissionTaskType.Flag;
        public override bool Completed => CurrentProgress >= RequiredProgress;
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