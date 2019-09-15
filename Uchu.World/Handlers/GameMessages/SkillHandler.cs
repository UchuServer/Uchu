using System.Threading.Tasks;
using Uchu.Core;

namespace Uchu.World.Handlers.GameMessages
{
    public class SkillHandler : HandlerGroup
    {
        [PacketHandler]
        public async Task HandleSkillStart(StartSkillMessage message, Player player)
        {
            await player.GetComponent<SkillComponent>().StartUserSkillAsync(message);
        }

        [PacketHandler]
        public async Task HandleSyncSkill(SyncSkillMessage message, Player player)
        {
            await player.GetComponent<SkillComponent>().SyncUserSkillAsync(message);
        }
    }
}