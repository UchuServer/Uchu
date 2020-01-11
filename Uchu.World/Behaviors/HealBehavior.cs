using System.Threading.Tasks;

namespace Uchu.World.Behaviors
{
    public class HealBehavior : BehaviorBase
    {
        public override BehaviorTemplateId Id => BehaviorTemplateId.Heal;
        
        public int Health { get; set; }
        
        public override async Task BuildAsync()
        {
            Health = await GetParameter<int>("health");
        }

        public override Task ExecuteAsync(ExecutionContext context, ExecutionBranchContext branchContext)
        {
            if (!branchContext.Target.TryGetComponent<Stats>(out var stats)) return Task.CompletedTask;

            stats.Health = (uint) ((int) stats.Health + Health);
            
            return Task.CompletedTask;
        }
    }
}