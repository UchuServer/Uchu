using System.Threading.Tasks;

namespace Uchu.World.Systems.Missions
{
    public class ScriptTask : MissionTaskInstance
    {
        public ScriptTask(MissionInstance mission, int taskId, int missionTaskIndex) 
            : base(mission, taskId, missionTaskIndex)
        {
        }

        public ScriptTask(MissionInstance mission, MissionTaskInstance cachedInstance) : base(mission, cachedInstance)
        {
        }
        
        public override MissionTaskType Type => MissionTaskType.Script;

        public override bool Completed => CurrentProgress > 0;

        public async Task ReportProgress(int id)
        {
            AddProgress(id);
            
            if (Completed)
                await CheckMissionCompletedAsync();
        }
    }
}