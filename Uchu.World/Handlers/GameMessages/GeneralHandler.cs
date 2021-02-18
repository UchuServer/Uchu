using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Uchu.Core;
using Uchu.Core.Client;
using Uchu.World.Client;

namespace Uchu.World.Handlers.GameMessages
{
    public class GeneralHandler : HandlerGroup
    {
        [PacketHandler]
        public async Task RequestUseHandler(RequestUseMessage message, Player player)
        {
            Logger.Information($"{player} interacted with {message.TargetObject}");

            var inventory = player.GetComponent<MissionInventoryComponent>();
            
            await inventory.GoToNpcAsync(
                message.TargetObject.Lot
            );

            if (message.IsMultiInteract)
            {
                //
                // Multi-interact is mission
                //

                if (message.MultiInteractType == 0) // Mission Component
                {
                    player.GetComponent<MissionInventoryComponent>().MessageOfferMission(
                        (int) message.MultiInteractId,
                        message.TargetObject
                    );
                } 
                else if (message.MultiInteractType == 1) // Any other case
                {
                    await message.TargetObject.OnInteract.InvokeAsync(player);

                    await inventory.InteractAsync(message.TargetObject.Lot);
                }
            }
            else if (message.TargetObject != default)
            {
                await message.TargetObject.OnInteract.InvokeAsync(player);
                
                await inventory.InteractAsync(message.TargetObject.Lot);
            }
        }

        [PacketHandler]
        public async Task RequestResurrectHandler(RequestResurrectMessage message, Player player)
        {
            await player.GetComponent<DestructibleComponent>().ResurrectAsync();
        }

        [PacketHandler]
        public async Task RequestSmashHandler(RequestSmashPlayerMessage message, Player player)
        {
            await player.GetComponent<DestructibleComponent>().SmashAsync(player, player);
        }

        [PacketHandler]
        public void RebuildCancelHandler(RebuildCancelMessage message, Player player)
        {
            if (message.Associate.TryGetComponent<QuickBuildComponent>(out var rebuild))
            {
                rebuild.StopRebuild(player, RebuildFailReason.Canceled);
            }
        }

        [PacketHandler]
        public async Task ReadyForUpdatesHandler(ReadyForUpdateMessage message, Player player)
        {
            Logger.Debug($"Loaded: {message.GameObject}");
            await player.OnReadyForUpdatesEvent.InvokeAsync(message);
            Zone.SendSerialization(message.GameObject,new []{ player });
        }

        [PacketHandler]
        public async Task PlayEmoteHandler(PlayEmoteMessage message, Player player)
        {
            var animation = (await ClientCache.GetTableAsync<Emotes>()).FirstOrDefault(e => e.Id == message.EmoteId);
            
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

            if (player.TryGetComponent<MissionInventoryComponent>(out var missionInventoryComponent))
            {
                await missionInventoryComponent.UseEmoteAsync(message.Target, message.EmoteId);
            }
            
            if (message.Target?.OnEmoteReceived != default)
            {
                await message.Target.OnEmoteReceived.InvokeAsync(message.EmoteId, player);
            }
        }

        [PacketHandler]
        public async Task NotifyServerLevelProcessingCompleteHandler(NotifyServerLevelProcessingCompleteMessage message, Player player)
        {
            var character = player.GetComponent<CharacterComponent>();
            if (character.UniverseScore > character.RequiredUniverseScore)
            {
                await character.LevelUpAsync();
                GameObject.Serialize(player);
                
                // Show the levelup animation
                player.Zone.BroadcastMessage(new PlayFXEffectMessage
                {
                    Associate = player,
                    EffectId = 7074,
                    EffectType = "create",
                    Name = "levelup_body_glow"
                });

                player.Zone.BroadcastChatMessage($"{character.Name} has reached Level {character.Level}!");
            }
        }
    }
}