using System.Threading.Tasks;
using Uchu.Core;

namespace Uchu.World.Handlers.GameMessages
{
    public class GeneralGameMessageHandler : HandlerGroup
    {
        [PacketHandler]
        public async Task RequestUseHandler(RequestUseMessage message, Player player)
        {
            await player.GetComponent<QuestInventory>().UpdateObjectTask(MissionTaskType.Interact, message.TargetObject);
            
            if (message.IsMultiInteract)
            {
                //
                // Multi-interact is mission
                //
                
                if (message.MultiInteractType == 0x0)
                {
                    player.GetComponent<QuestInventory>().MessageOfferMission(
                        (int) message.MultiInteractId,
                        message.TargetObject
                    );
                }
            }
            else
            {
                await message.TargetObject.GetComponent<QuestGiverComponent>().OfferMissionAsync(player);
            }
        }
    }
}