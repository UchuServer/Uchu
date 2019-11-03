using System.Threading.Tasks;
using Uchu.Core;

namespace Uchu.World.Handlers.GameMessages
{
    public class MissionHandler : HandlerGroup
    {
        [PacketHandler]
        public async Task RespondToMissionHandler(RespondToMissionMessage message, Player player)
        {
            await player.GetComponent<MissionInventoryComponent>().RespondToMissionAsync(
                message.MissionId,
                message.Receiver,
                message.RewardItem
            );
        }

        [PacketHandler]
        public async Task MissionDialogueOkHandler(MissionDialogueOkMessage message, Player player)
        {
            await player.GetComponent<MissionInventoryComponent>().MissionDialogueOk(message);
        }

        [PacketHandler]
        public void RequestLinkedMissionHandler(RequestLinkedMissionMessage message, Player player)
        {
            message.Associate.GetComponent<MissionGiverComponent>().OfferMission(player);
        }
        
        [PacketHandler]
        public async Task SetFlagHandler(SetFlagMessage message, Player player)
        {
            await player.GetComponent<MissionInventoryComponent>().UpdateObjectTaskAsync(MissionTaskType.Flag, message.FlagId);
            
            player.Message(new NotifyClientFlagChangeMessage
            {
                Flag = message.Flag,
                FlagId = message.FlagId
            });
        }
    }
}