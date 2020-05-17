using System.Linq;
using System.Threading.Tasks;

namespace Uchu.World.Systems.Missions
{
    public class CollectTask : MissionTaskBase
    {
        public override MissionTaskType Type => MissionTaskType.Collect;
        
        public async Task Progress(GameObject gameObject)
        {
            var component = gameObject.GetComponent<CollectibleComponent>();

            if (!Targets.Contains((int) gameObject.Lot)) return;

            var shiftedId = component.CollectibleId + ((int) gameObject.Zone.ZoneId << 8);

            await AddProgressAsync(shiftedId);

            if (await IsCompleteAsync())
                await CheckMissionCompleteAsync();
        }
    }
}