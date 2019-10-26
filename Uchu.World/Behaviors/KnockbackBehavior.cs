using System.Threading.Tasks;

namespace Uchu.World.Behaviors
{
    public class KnockbackBehavior : BehaviorBase
    {
        public override BehaviorTemplateId Id => BehaviorTemplateId.Knockback;
        
        public override Task BuildAsync()
        {
            return Task.CompletedTask;
        }

        public override async Task ExecuteAsync(ExecutionContext context, ExecutionBranchContext branchContext)
        {
            await base.ExecuteAsync(context, branchContext);
        }
    }
}