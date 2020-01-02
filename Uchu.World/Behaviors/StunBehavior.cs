using System.Linq;
using System.Threading.Tasks;

namespace Uchu.World.Behaviors
{
    public class StunBehavior : BehaviorBase
    {
        public override BehaviorTemplateId Id => BehaviorTemplateId.Stun;
        
        public override Task BuildAsync()
        {
            return Task.CompletedTask;
        }

        public override async Task ExecuteAsync(ExecutionContext context, ExecutionBranchContext branchContext)
        {
            await base.ExecuteAsync(context, branchContext);
            
            if (branchContext.Target == context.Associate) return;

            context.Reader.ReadBit();
        }
    }
}