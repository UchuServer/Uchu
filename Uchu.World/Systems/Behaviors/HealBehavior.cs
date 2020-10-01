using System.Threading.Tasks;

namespace Uchu.World.Systems.Behaviors
{
    public class HealBehavior : BehaviorBase
    {
        public override BehaviorTemplateId Id => BehaviorTemplateId.Heal;

        private int Health { get; set; }
        
        public override async Task BuildAsync()
        {
            Health = await GetParameter<int>("health");
        }

        public override Task ExecuteStart(BehaviorExecutionParameters parameters)
        {
            if (parameters.BranchContext.Target.TryGetComponent<Stats>(out var stats))
                stats.Health = (uint) ((int) stats.Health + Health);
            return Task.CompletedTask;
        }
    }
}