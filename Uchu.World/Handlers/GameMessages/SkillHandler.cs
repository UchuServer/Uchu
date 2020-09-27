using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Uchu.Core;

namespace Uchu.World.Handlers.GameMessages
{
    public class SkillHandler : HandlerGroup
    {
        [PacketHandler]
        public async Task SkillStartHandler(StartSkillMessage message, Player player)
        {
            try
            {
                await player.GetComponent<SkillComponent>().StartUserSkillAsync(message);
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        [PacketHandler]
        public async Task SyncSkillHandler(SyncSkillMessage message, Player player)
        {
            try
            {
                await player.GetComponent<SkillComponent>().SyncUserSkillAsync(message);
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        [PacketHandler]
        public void SetConsumeableHandler(SetConsumableItemMessage message, Player player)
        {
            player.GetComponent<SkillComponent>().SelectedConsumeable = message.Lot;
        }

        [PacketHandler]
        public async Task ClientItemConsumedHandler(ClientItemConsumedMessage message, Player player)
        {
            await message.Item.ConsumeAsync();
        }
        
        [PacketHandler]
        public async Task UseNonEquipmentItemHandler(UseNonEquipmentItemMessage message, Player player)
        {
            await message.Item.UseNonEquipmentItem();
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