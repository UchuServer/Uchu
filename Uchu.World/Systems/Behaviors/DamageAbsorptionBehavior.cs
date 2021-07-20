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
            //this returns with only parameter and BranchContext in console
            /*
            if (parameters == null) return;
            Console.WriteLine("parameter");
            if (parameters.BranchContext == null) return;
            Console.WriteLine("BranchContext");
            if (parameters.BranchContext.Target == null) return;
            Console.WriteLine("Target");
            if (parameters.BranchContext.Target.GetComponent<DestroyableComponent>() == null) return;
            Console.WriteLine("Component");
            */
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
                //Console.WriteLine("Absorption wore off...");
                //using this behavior by itself works okay, but it does not track absorption damage taken. this is fine for using the ability once,
                //but if you were to use the ability multiple times, you'd lose a lot of absorption points at the very end of the first skill, 
                //as it reduces it by the total amount given as well, regardless of the amount of absorption damage taken.
                //it may not be worth trying to do this complex behavior, as (from my gameplay) it is rare to get more than one source of absorption,
                //and the effect only comes in at the very end, where it likely wouldn't matter.
            });
            //most of these comments should be removed before being pushed to dev
        }
    }
}