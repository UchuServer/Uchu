using System.Threading.Tasks;

namespace Uchu.World.Systems.Behaviors
{
    public class HealBehavior : BehaviorBase
    {
        public override BehaviorTemplateId Id => BehaviorTemplateId.Heal;

        private int Health { get; set; }
        
        public override void Build()
        {
            Health = GetParameter<int>("health");
        }

        public override void ExecuteStart(BehaviorExecutionParameters parameters)
        {
            if (parameters.BranchContext.Target.TryGetComponent<DestroyableComponent>(out var stats))
                stats.Health = (uint) ((int) stats.Health + Health);
        }
    }
}