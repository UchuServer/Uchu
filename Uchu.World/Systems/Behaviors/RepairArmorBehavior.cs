using System.Threading.Tasks;

namespace Uchu.World.Systems.Behaviors
{
    public class RepairArmorBehavior : BehaviorBase
    {
        public override BehaviorTemplateId Id => BehaviorTemplateId.RepairArmor;

        private int Armor { get; set; }
        
        public override void Build()
        {
            Armor = GetParameter<int>("armor");
        }

        public override void ExecuteStart(BehaviorExecutionParameters parameters)
        {
            if (parameters.BranchContext.Target.TryGetComponent<DestroyableComponent>(out var stats))
                stats.Armor = (uint) ((int) stats.Armor + Armor);
        }
    }
}