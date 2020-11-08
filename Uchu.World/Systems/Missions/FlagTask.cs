using System.Linq;
using System.Threading.Tasks;

namespace Uchu.World.Systems.Missions
{
    public class FlagTask : MissionTaskInstance
    {
        public override MissionTaskType Type => MissionTaskType.Flag;

        public override async Task<bool> IsCompleteAsync()
        {
            var values = await GetProgressAsync();

            return values >= TargetValue;
        }
        
        public async Task Progress(int id)
        {
            if (!Targets.Contains(id)) return;
            
            var progress = await GetProgressValuesAsync();
            
            if (progress.Contains(id)) return;

            await AddProgressAsync(id);
            
            if (await IsCompleteAsync())
                await CheckMissionCompleteAsync();
        }
    }
}