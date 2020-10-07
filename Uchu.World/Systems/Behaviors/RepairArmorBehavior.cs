using System.Threading.Tasks;

namespace Uchu.World.Systems.Behaviors
{
    public class RepairArmorBehavior : BehaviorBase
    {
        public override BehaviorTemplateId Id => BehaviorTemplateId.RepairArmor;

        private int Armor { get; set; }
        
        public override async Task BuildAsync()
        {
            Armor = await GetParameter<int>("armor");
        }

        public override Task ExecuteStart(BehaviorExecutionParameters parameters)
        {
            if (parameters.BranchContext.Target.TryGetComponent<DestroyableComponent>(out var stats))
                stats.Armor = (uint) ((int) stats.Armor + Armor);
            return Task.CompletedTask;
        }
    }
}