using System.Threading.Tasks;

namespace Uchu.World.Systems.Missions
{
    public class ScriptTask : MissionTaskInstance
    {
        public ScriptTask(MissionInstance mission, int taskId, int missionTaskIndex) 
            : base(mission, taskId, missionTaskIndex)
        {
        }

        public override MissionTaskType Type => MissionTaskType.Script;

        public async Task ReportProgress(int id, Lot target)
        {
            if (TaskId != id) return;

            AddProgress(target);
            
            if (Completed)
                await CheckMissionCompletedAsync();
        }
    }
}