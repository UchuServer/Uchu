using System.Linq;
using System.Threading.Tasks;

namespace Uchu.World.MissionSystem
{
    public class ObtainItemTask : MissionTaskBase
    {
        public override MissionTaskType Type => MissionTaskType.ObtainItem;

        public async Task Progress(int lot)
        {
            if (!Targets.Contains(lot)) return;

            await AddProgressAsync(lot);
        }
    }
}