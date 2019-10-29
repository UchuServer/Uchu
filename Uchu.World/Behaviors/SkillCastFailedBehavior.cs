using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Uchu.World.Behaviors
{
    public class SkillCastFailedBehavior : BehaviorBase
    {
        public override BehaviorTemplateId Id => BehaviorTemplateId.SkillCastFailed;
        
        public int EffectId { get; set; }
        
        public override async Task BuildAsync()
        {
            EffectId = await GetParameter<int>("effect_id");
        }

        public override Task ExecuteAsync(ExecutionContext context, ExecutionBranchContext branchContext)
        {
            /*
            if (EffectId == default) return Task.CompletedTask;
            
            context.Associate.Zone.BroadcastMessage(new PlayFXEffectMessage
            {
                Associate = context.Associate,
                EffectId = EffectId
            });
            */
            
            return Task.CompletedTask;
        }
    }
}