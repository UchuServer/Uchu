using System;
using System.Threading.Tasks;
using Uchu.Core;

namespace Uchu.World.Handlers.GameMessages
{
    public class SkillHandler : HandlerGroup
    {
        [PacketHandler]
        public void HandleSkillStart(StartSkillMessage message, Player player)
        {
            try
            {
                Task.Run(() => player.GetComponent<SkillComponent>().StartUserSkillAsync(message));
            }
            catch (Exception e)
            {
                Logger.Error($"Skill Execution failed: {e.Message}\n{e.StackTrace}");
            }
        }

        [PacketHandler]
        public void HandleSyncSkill(SyncSkillMessage message, Player player)
        {
            try
            {
                Task.Run(() => player.GetComponent<SkillComponent>().SyncUserSkillAsync(message));
            }
            catch (Exception e)
            {
                Logger.Error($"Skill Syncing failed: {e.Message}\n{e.StackTrace}");
            }
        }

        [PacketHandler]
        public void HandleSetConsumeable(SetConsumableItemMessage message, Player player)
        {
            player.GetComponent<SkillComponent>().SelectedConsumeable = message.Lot;
        }
        
        [PacketHandler]
        public void HandleSelectSkill(SelectSkillMessage message, Player player)
        {
            player.GetComponent<SkillComponent>().SelectedSkill = (uint) message.SkillId;
        }
    }
}