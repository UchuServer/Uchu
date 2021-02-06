using System.Threading.Tasks;
using Uchu.Core;

namespace Uchu.World.Handlers.GameMessages
{
    public class MissionHandler : HandlerGroup
    {
        [PacketHandler]
        public async Task RespondToMissionHandler(RespondToMissionMessage message, Player player)
        {
            await player.OnRespondToMission.InvokeAsync(message.MissionId, message.Receiver, message.RewardItem);
        }

        [PacketHandler]
        public async Task MissionDialogueOkHandler(MissionDialogueOkMessage message, Player player)
        {
            await message.Associate.GetComponent<MissionGiverComponent>().OnMissionOk.InvokeAsync(
                (message.MissionId, message.IsComplete, message.MissionState, message.Responder)
            );
        }

        [PacketHandler]
        public void RequestLinkedMissionHandler(RequestLinkedMissionMessage message, Player player)
        {
            message.Associate.GetComponent<MissionGiverComponent>().HandleInteraction(player);
        }
        
        [PacketHandler]
        public async Task SetFlagHandler(SetFlagMessage message, Player player)
        {
            if (player.TryGetComponent<CharacterComponent>(out var character))
            {
                await character.SetFlagAsync(message.FlagId, message.Flag);
            }
        }
    }
}