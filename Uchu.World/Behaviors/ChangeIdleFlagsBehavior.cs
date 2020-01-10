using System.Threading.Tasks;

namespace Uchu.World.Behaviors
{
    public class ChangeIdleFlagsBehavior : BehaviorBase
    {
        public override BehaviorTemplateId Id => BehaviorTemplateId.ChangeIdleFlags;
        
        public override Task BuildAsync()
        {
            return Task.CompletedTask;
        }
    }
}