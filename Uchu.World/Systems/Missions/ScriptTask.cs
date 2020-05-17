using System.Threading.Tasks;

namespace Uchu.World.Systems.Missions
{
    public class ScriptTask : MissionTaskBase
    {
        public override MissionTaskType Type => MissionTaskType.Script;

        public override async Task<bool> IsCompleteAsync()
        {
            var length = await GetProgressAsync();

            return length > 0;
        }

        public async Task Progress(int id)
        {
            await AddProgressAsync(id);
            
            if (await IsCompleteAsync())
                await CheckMissionCompleteAsync();
        }
    }
}