using System.Threading.Tasks;

namespace Uchu.World.Behaviors
{
    public class EmptyBehavior : BehaviorBase
    {
        public override BehaviorTemplateId Id => BehaviorTemplateId.Empty;

        public override Task BuildAsync()
        {
            return Task.CompletedTask;
        }
    }
}