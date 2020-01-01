using System.Threading.Tasks;

namespace Uchu.World.Behaviors
{
    public class DurationBehavior : BehaviorBase
    {
        public override BehaviorTemplateId Id => BehaviorTemplateId.Duration;
        
        public BehaviorBase Action { get; set; }
        
        public int ActionDuration { get; set; }
        
        public override async Task BuildAsync()
        {
            Action = await GetBehavior("action");

            var duration = await GetParameter("duration");
            
            if (duration.Value == null) return;

            ActionDuration = (int) duration.Value;
        }

        public override async Task ExecuteAsync(ExecutionContext context, ExecutionBranchContext branchContext)
        {
            await base.ExecuteAsync(context, branchContext);
            
            await Action.ExecuteAsync(context, branchContext);
        }
    }
}