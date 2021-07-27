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
            //i have no idea if there is a case of a behavior casting absorption on something else, but the branch context target creates a null exception
            DestroyableComponent absorb;
            if (parameters.BranchContext.Target != null && parameters.BranchContext.Target.TryGetComponent<DestroyableComponent>(out absorb)) {} 
            else if (!(parameters.Context.Associate.TryGetComponent<DestroyableComponent>(out absorb))) return;
            absorb.DamageAbsorptionPoints += AbsorbAmount;
            //Console.WriteLine("Given " + AbsorbAmount + " absorption points to " + absorb.GameObject.Name);
            var _ = Task.Run(async () =>
            {
                await Task.Delay(parameters.BranchContext.Duration);
                absorb.DamageAbsorptionPoints = Math.Max(0, absorb.DamageAbsorptionPoints - AbsorbAmount);
            });
        }
    }
}