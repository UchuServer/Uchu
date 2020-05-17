using System.Threading.Tasks;

namespace Uchu.World.Systems.Behaviors
{
    public class AlterChainDelayBehavior : BehaviorBase
    {
        public override BehaviorTemplateId Id => BehaviorTemplateId.AlterChainDelay;

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