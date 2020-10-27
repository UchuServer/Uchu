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
        
        public override async Task BuildAsync()
        {
            Life = await GetParameter<int>("life");
            Armor = await GetParameter<int>("armor");
            Imagination = await GetParameter<int>("imag");
            RunSpeed = await GetParameter<int>("run_speed");
            AttackSpeed = await GetParameter<int>("attack_speed");
            Brain = await GetParameter<int>("brain");
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