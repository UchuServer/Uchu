using System.Linq;
using System.Threading.Tasks;

namespace Uchu.World.MissionSystem
{
    public class MissionCompleteTask : MissionTaskBase
    {
        public override MissionTaskType Type => MissionTaskType.MissionComplete;

        public override async Task<bool> IsCompleteAsync()
        {
            var values = await GetProgressValuesAsync();

            return Targets.All(target => values.Contains(target));
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