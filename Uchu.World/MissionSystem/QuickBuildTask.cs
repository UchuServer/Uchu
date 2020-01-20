using System.Linq;
using System.Threading.Tasks;

namespace Uchu.World.MissionSystem
{
    public class QuickBuildTask : MissionTaskBase
    {
        public override MissionTaskType Type => MissionTaskType.QuickBuild;

        public async Task Progress(int lot)
        {
            if (!Targets.Contains(lot)) return;
            
            await AddProgressAsync(lot);

            if (await IsCompleteAsync())
                await CheckMissionCompleteAsync();
        }
    }
}