using System.Threading.Tasks;

namespace Uchu.World.Behaviors
{
    public class PlayEffectBehavior : BehaviorBase
    {
        public override BehaviorTemplateId Id => BehaviorTemplateId.PlayEffect;
        
        public int EffectId { get; set; }
        
        public override async Task BuildAsync()
        {
            var effectId = await GetParameter("effectID");
            
            if (effectId?.Value == null) return;

            EffectId = (int) effectId.Value;
        }

        public override async Task ExecuteAsync(ExecutionContext context, ExecutionBranchContext branchContext)
        {
            await base.ExecuteAsync(context, branchContext);
        }
    }
}