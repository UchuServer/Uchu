using System.Linq;
using System.Threading.Tasks;

namespace Uchu.World.Systems.Missions
{
    public class UseConsumableTask : MissionTaskInstance
    {
        public override MissionTaskType Type => MissionTaskType.UseConsumable;

        public override async Task<bool> IsCompleteAsync()
        {
            var values = await GetProgressValuesAsync();

            return values.Contains(Target);
        }

        public async Task Progress(int lot)
        {
            if (Target != lot) return;
            
            await AddProgressAsync(lot);

            if (await IsCompleteAsync())
                await CheckMissionCompleteAsync();
        }
    }
}