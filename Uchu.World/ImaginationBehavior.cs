using System.Threading.Tasks;
using Uchu.World.Behaviors;

namespace Uchu.World
{
    public class ImaginationBehavior : BehaviorBase
    {
        public override BehaviorTemplateId Id => BehaviorTemplateId.Imagination;
        
        public override Task BuildAsync()
        {
            return Task.CompletedTask;
        }
    }
}