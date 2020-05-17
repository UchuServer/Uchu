using System.Linq;
using System.Threading.Tasks;

namespace Uchu.World.Systems.Missions
{
    public class InteractTask : MissionTaskBase
    {
        public override MissionTaskType Type => MissionTaskType.Interact;

        public override async Task<bool> IsCompleteAsync()
        {
            var values = await GetProgressValuesAsync();

            return values.Contains(Target);
        }

        public async Task Progress(Lot lot)
        {
            if (Target != lot) return;

            await AddProgressAsync(lot);
            
            if (await IsCompleteAsync())
                await CheckMissionCompleteAsync();
        }
    }
}