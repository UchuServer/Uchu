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

        public override async Task ExecuteAsync(ExecutionContext context, ExecutionBranchContext branchContext)
        {
            await base.ExecuteAsync(context, branchContext);

            if (!branchContext.Target.TryGetComponent<Stats>(out var stats)) return;

            stats.Health = (uint) ((int) stats.Health + Health);
        }
    }
}