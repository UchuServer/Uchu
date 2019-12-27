using System.Threading.Tasks;

namespace Uchu.World.Behaviors
{
    public class ApplyBuffBehavior : BehaviorBase
    {
        public override BehaviorTemplateId Id => BehaviorTemplateId.ApplyBuff;
        
        public override Task BuildAsync()
        {
            return Task.CompletedTask;
        }
    }
}