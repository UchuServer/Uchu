using System.Threading.Tasks;

namespace Uchu.World.Behaviors
{
    public class BlockBehavior : BehaviorBase
    {
        public override BehaviorTemplateId Id => BehaviorTemplateId.Block;
        
        public override Task BuildAsync()
        {
            return Task.CompletedTask;
        }
    }
}