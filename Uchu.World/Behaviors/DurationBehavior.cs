using System.Threading.Tasks;
using RakDotNet.IO;
using Uchu.Core.CdClient;

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

            ActionDuration = (int) (duration.Value * 1000); // Get in ms
        }

        public override async Task ExecuteAsync(ExecutionContext context, ExecutionBranchContext branchContext)
        {
            await base.ExecuteAsync(context, branchContext);
            
            await Action.ExecuteAsync(context, new ExecutionBranchContext(branchContext.Target)
            {
                Duration = ActionDuration
            });
        }
    }
}