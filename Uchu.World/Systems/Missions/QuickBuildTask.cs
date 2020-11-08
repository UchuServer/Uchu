using System.Linq;
using System.Threading.Tasks;

namespace Uchu.World.Systems.Missions
{
    public class QuickBuildTask : MissionTaskInstance
    {
        public override MissionTaskType Type => MissionTaskType.QuickBuild;

        public async Task Progress(int lot, int activity)
        {
            if (!await TryProgress(lot))
                await TryProgress(activity);

            if (await IsCompleteAsync())
                await CheckMissionCompleteAsync();
        }

        public async Task<bool> TryProgress(int value)
        {
            if (!Targets.Contains(value)) return false;
            
            await AddProgressAsync(value);

            return true;
        }
    }
}