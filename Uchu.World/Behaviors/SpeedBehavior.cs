using System.Threading.Tasks;

namespace Uchu.World.Behaviors
{
    public class SpeedBehavior : BehaviorBase
    {
        public override BehaviorTemplateId Id => BehaviorTemplateId.Speed;
        
        public override Task BuildAsync()
        {
            // TODO
            
            return Task.CompletedTask;
        }
    }
}