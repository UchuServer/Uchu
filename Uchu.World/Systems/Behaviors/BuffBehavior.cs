using System.Threading.Tasks;

namespace Uchu.World.Systems.Behaviors
{
    public class BuffBehavior : BehaviorBase
    {
        public override BehaviorTemplateId Id => BehaviorTemplateId.Buff;

        private int Life { get; set; }

        private int Armor { get; set; }

        private int Imagination { get; set; }

        private float RunSpeed { get; set; }

        private float AttackSpeed { get; set; }

        private float Brain { get; set; }
        
        public override void Build()
        {
            Life = GetParameter<int>("life");
            Armor = GetParameter<int>("armor");
            Imagination = GetParameter<int>("imag");
            RunSpeed = GetParameter<int>("run_speed");
            AttackSpeed = GetParameter<int>("attack_speed");
            Brain = GetParameter<int>("brain");
        }

        public override void ExecuteStart(BehaviorExecutionParameters behaviorExecutionParameters)
        {
            if (!behaviorExecutionParameters.Context.Associate.TryGetComponent<DestroyableComponent>(
                out var stats))
                return;

            stats.MaxHealth += (uint) Life;
            stats.MaxArmor += (uint) Armor;
            stats.MaxImagination += (uint) Imagination;
        }

        public override void Dismantle(BehaviorExecutionParameters behaviorExecutionParameters)
        {
            if (!behaviorExecutionParameters.Context.Associate.TryGetComponent<DestroyableComponent>(
                out var stats))
                return;

            stats.MaxHealth -= (uint) Life;
            stats.MaxArmor -= (uint) Armor;
            stats.MaxImagination -= (uint) Imagination;
        }
    }
}