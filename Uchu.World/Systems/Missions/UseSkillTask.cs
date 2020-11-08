using System.Linq;
using System.Threading.Tasks;

namespace Uchu.World.Systems.Missions
{
    public class UseSkillTask : MissionTaskInstance
    {
        public override MissionTaskType Type => MissionTaskType.UseSkill;

        public override async Task<bool> IsCompleteAsync()
        {
            var values = await GetProgressValuesAsync();

            return Parameters.Any(t => values.Contains(t));
        }

        public async Task Progress(int skillId)
        {
            if (!Parameters.Contains(skillId)) return;

            await AddProgressAsync(skillId);
            
            if (await IsCompleteAsync())
                await CheckMissionCompleteAsync();
        }
    }
}