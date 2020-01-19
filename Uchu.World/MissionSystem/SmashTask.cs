using System.Linq;
using System.Threading.Tasks;

namespace Uchu.World.MissionSystem
{
    public class SmashTask : MissionTaskBase
    {
        public override MissionTaskType Type => MissionTaskType.Smash;
        
        public async Task Progress(int lot)
        {
            if (!Targets.Contains(lot)) return;
            
            await AddProgressAsync(lot);

            if (await IsCompleteAsync())
                await CheckMissionCompleteAsync();
        }
    }
}