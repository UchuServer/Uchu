using System.Linq;
using System.Threading.Tasks;

namespace Uchu.World.Systems.Missions
{
    public class UseEmoteTask : MissionTaskBase
    {
        public override MissionTaskType Type => MissionTaskType.UseEmote;

        public override async Task<bool> IsCompleteAsync()
        {
            var values = await GetProgressValuesAsync();

            return values.Contains(Parameters.FirstOrDefault());
        }

        public async Task Progress(GameObject gameObject, int emote)
        {
            if (gameObject.Lot != Target) return;
            
            if (Parameters.FirstOrDefault() != emote) return;

            await AddProgressAsync(emote);

            if (await IsCompleteAsync())
                await CheckMissionCompleteAsync();
        }
    }
}