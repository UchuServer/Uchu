using System.Linq;
using System.Threading.Tasks;

namespace Uchu.World.Systems.Missions
{
    public class ObtainItemTask : MissionTaskInstance
    {
        public override MissionTaskType Type => MissionTaskType.ObtainItem;

        public async Task Progress(int lot)
        {
            if (!Targets.Contains(lot)) return;

            await AddProgressAsync(lot);
            
            if (await IsCompleteAsync())
                await CheckMissionCompleteAsync();
        }
    }
}