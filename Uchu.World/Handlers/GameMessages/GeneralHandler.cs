using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Uchu.Core;
using Uchu.Core.Client;

namespace Uchu.World.Handlers.GameMessages
{
    public class GeneralHandler : HandlerGroup
    {
        [PacketHandler]
        public async Task RequestUseHandler(RequestUseMessage message, Player player)
        {
            player.SendChatMessage($"Interacted with {message.TargetObject}");
            
            player.GetComponent<MissionInventoryComponent>().UpdateObjectTask(
                MissionTaskType.Interact,
                message.TargetObject.Lot,
                message.TargetObject
            );

            if (message.IsMultiInteract)
            {
                //
                // Multi-interact is mission
                //

                if (message.MultiInteractType == default)
                {
                    player.GetComponent<MissionInventoryComponent>().MessageOfferMission(
                        (int) message.MultiInteractId,
                        message.TargetObject
                    );
                }
            }
            else
            {
                message.TargetObject?.OnInteract.Invoke(player);
            }
        }

        [PacketHandler]
        public void RequestResurrectHandler(RequestResurrectMessage message, Player player)
        {
            player.GetComponent<DestructibleComponent>().Resurrect();
        }

        [PacketHandler]
        public void RequestSmashHandler(RequestSmashPlayerMessage message, Player player)
        {
            player.GetComponent<DestructibleComponent>().Smash(player, player);
        }

        [PacketHandler]
        public void RebuildCancelHandler(RebuildCancelMessage message, Player player)
        {
            if (message.Associate.TryGetComponent<RebuildComponent>(out var rebuild))
            {
                rebuild.StopRebuild(player, RebuildFailReason.Canceled);
            }
        }

        [PacketHandler]
        public void ReadyForUpdatesHandler(ReadyForUpdateMessage message, Player player)
        {
            Logger.Debug($"Loaded: {message.GameObject}");
            
            player.Perspective.ClientLoadedObjectCount++;
        }

        [PacketHandler]
        public async Task PlayEmoteHandler(PlayEmoteMessage message, Player player)
        {
            await using var ctx = new CdClientContext();

            var animation = await ctx.EmotesTable.FirstOrDefaultAsync(e => e.Id == message.EmoteId);
            
            player.Zone.BroadcastMessage(new PlayAnimationMessage
            {
                Associate = player,
                AnimationsId = animation.AnimationName
            });
            
            player.Zone.ExcludingMessage(new EmotePlayedMessage
            {
                Associate = player,
                EmoteId = message.EmoteId,
                Target = message.Target
            }, player);
            
            if (message.Target != default)
            {
                await message.Target.OnEmoteReceived.InvokeAsync(message.EmoteId, player);
            }
        }
    }
}