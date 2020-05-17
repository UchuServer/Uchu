using System.Threading.Tasks;

namespace Uchu.World.Systems.Behaviors
{
    public class ImmunityBehavior : BehaviorBase
    {
        public override BehaviorTemplateId Id => BehaviorTemplateId.Immunity;
        
        public override Task BuildAsync()
        {
            // TODO
            
            return Task.CompletedTask;
        }
    }
}