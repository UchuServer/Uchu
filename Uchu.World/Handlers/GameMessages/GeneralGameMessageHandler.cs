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
                
                if (message.MultiInteractType == default)
                {
                    player.GetComponent<QuestInventory>().MessageOfferMission(
                        (int) message.MultiInteractId,
                        message.TargetObject
                    );
                }
            }
            else
            {
                var questGiver = message.TargetObject.GetComponent<QuestGiverComponent>();
                if (!ReferenceEquals(questGiver, null)) await questGiver.OfferMissionAsync(player);
            }
        }

        [PacketHandler(RunTask = true)]
        public void RequestResurrectHandler(RequestResurrectMessage message, Player player)
        {
            player.GetComponent<DestructibleComponent>().Resurrect();
        }

        [PacketHandler(RunTask = true)]
        public void RequestSmashHandler(RequestSmashPlayer message, Player player)
        {
            player.GetComponent<DestructibleComponent>().Smash(player, player);
        }
    }
}