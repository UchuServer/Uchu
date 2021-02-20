using System.Linq;
using System.Threading.Tasks;

namespace Uchu.World.Systems.Missions
{
    public class UseSkillTask : MissionTaskInstance
    {
        public UseSkillTask(MissionInstance mission, int taskId, int missionTaskIndex) 
            : base(mission, taskId, missionTaskIndex)
        {
        }

        public override MissionTaskType Type => MissionTaskType.UseSkill;

        public override bool Completed => Parameters.Any(t => Progress.Contains(t));

        public async Task ReportProgress(int skillId)
        {
            if (!Parameters.Contains(skillId))
                return;

            AddProgress(skillId);
            
            if (Completed)
                await CheckMissionCompletedAsync();
        }
    }
}