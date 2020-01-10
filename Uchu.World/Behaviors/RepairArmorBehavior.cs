using System.Threading.Tasks;

namespace Uchu.World.Behaviors
{
    public class RepairArmorBehavior : BehaviorBase
    {
        public override BehaviorTemplateId Id => BehaviorTemplateId.RepairArmor;
        
        public int Armor { get; set; }
        
        public override async Task BuildAsync()
        {
            Armor = await GetParameter<int>("armor");
        }

        public override Task ExecuteAsync(ExecutionContext context, ExecutionBranchContext branchContext)
        {
            if (!branchContext.Target.TryGetComponent<Stats>(out var stats)) return Task.CompletedTask;

            stats.Armor = (uint) ((int) stats.Armor + Armor);

            return Task.CompletedTask;
        }
    }
}