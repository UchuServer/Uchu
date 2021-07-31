using System;
using System.Threading.Tasks;

namespace Uchu.World.Systems.Behaviors
{
    public class DamageAbsorptionBehavior : BehaviorBase
    {
        public override BehaviorTemplateId Id => BehaviorTemplateId.DamageAbsorption;

        private uint AbsorbAmount { get; set; }

        public override async Task BuildAsync()
        {
            AbsorbAmount = await GetParameter<uint>("absorb_amount");
        }
        public override void ExecuteStart(BehaviorExecutionParameters parameters)
        {
            DestroyableComponent absorb;
            if (parameters.BranchContext.Target != null && parameters.BranchContext.Target.TryGetComponent<DestroyableComponent>(out absorb)) {} 
            else if (!(parameters.Context.Associate.TryGetComponent<DestroyableComponent>(out absorb))) return;
            absorb.DamageAbsorptionPoints += AbsorbAmount;
            var _ = Task.Run(async () =>
            {
                await Task.Delay(parameters.BranchContext.Duration);
                absorb.DamageAbsorptionPoints = Math.Max(0, absorb.DamageAbsorptionPoints - AbsorbAmount);
            });
        }
    }
}