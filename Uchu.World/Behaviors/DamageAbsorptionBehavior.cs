using System.Threading.Tasks;

namespace Uchu.World.Behaviors
{
    public class DamageAbsorptionBehavior : BehaviorBase
    {
        public override BehaviorTemplateId Id => BehaviorTemplateId.DamageAbsorption;
        
        public override Task BuildAsync()
        {
            return Task.CompletedTask;
        }
    }
}