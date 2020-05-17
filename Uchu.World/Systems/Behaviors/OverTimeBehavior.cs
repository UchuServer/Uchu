using System.Threading.Tasks;

namespace Uchu.World.Systems.Behaviors
{
    public class OverTimeBehavior : BehaviorBase
    {
        public override BehaviorTemplateId Id => BehaviorTemplateId.OverTime;
        
        public BehaviorBase Action { get; set; }
        
        public override async Task BuildAsync()
        {
            Action = await GetBehavior("action");
        }

        public override async Task ExecuteAsync(ExecutionContext context, ExecutionBranchContext branchContext)
        {
            await Action.ExecuteAsync(context, branchContext);
        }
    }
}