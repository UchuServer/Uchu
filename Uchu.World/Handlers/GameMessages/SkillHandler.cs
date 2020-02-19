using System;
using System.Linq;
using System.Threading.Tasks;
using Uchu.Core;

namespace Uchu.World.Handlers.GameMessages
{
    public class SkillHandler : HandlerGroup
    {
        [PacketHandler]
        public void SkillStartHandler(StartSkillMessage message, Player player)
        {
            Task.Run(() =>
            {
                try
                {
                    return player.GetComponent<SkillComponent>().StartUserSkillAsync(message);
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                    
                    return Task.CompletedTask;
                }
            });
        }

        [PacketHandler]
        public void SyncSkillHandler(SyncSkillMessage message, Player player)
        {
            Task.Run(() =>
            {
                try
                {
                    return player.GetComponent<SkillComponent>().SyncUserSkillAsync(message);
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                    
                    return Task.CompletedTask;
                }
            });
        }

        [PacketHandler]
        public void SetConsumeableHandler(SetConsumableItemMessage message, Player player)
        {
            player.GetComponent<SkillComponent>().SelectedConsumeable = message.Lot;
        }

        [PacketHandler]
        public async Task ClientItemConsumedHandler(ClientItemConsumedMessage message, Player player)
        {
            await player.GetComponent<MissionInventoryComponent>().UseConsumableAsync(message.Item.Lot);
        }
        
        [PacketHandler]
        public void SelectSkillHandler(SelectSkillMessage message, Player player)
        {
            player.GetComponent<SkillComponent>().SelectedSkill = (uint) message.SkillId;
        }

        [PacketHandler]
        public async Task ServerProjectileImpactHandler(RequestServerProjectileImpactMessage message, Player player)
        {
            player.SendChatMessage($"Request [{message.Data.Length}]: {message.Projectile} -> {message.Target}");
            
            if (message.Projectile == 0) return;

            var projectile = player.Zone.Objects.OfType<Projectile>().FirstOrDefault(
                p => p.ClientObjectId == message.Projectile
            );

            if (projectile == default) return;
            
            await projectile.ImpactAsync(message.Data, message.Target);
        }
    }
}