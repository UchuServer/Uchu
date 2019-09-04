using System.Threading.Tasks;
using Uchu.Core;

namespace Uchu.World.Handlers.GameMessages
{
    public class MissionHandler : HandlerGroup
    {
        [PacketHandler]
        public async Task RespondToMissionHandler(RespondToMissionMessage message, Player player)
        {
            await player.GetComponent<QuestInventory>().RespondToMissionAsync(message.MissionId, message.Receiver);
        }
    }
}