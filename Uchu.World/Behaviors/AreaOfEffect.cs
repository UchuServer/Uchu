using System.Threading.Tasks;

namespace Uchu.World.Behaviors
{
    public class AreaOfEffect : BehaviorBase
    {
        public override BehaviorTemplateId Id => BehaviorTemplateId.AreaOfEffect;
        
        public override Task BuildAsync()
        {
            return Task.CompletedTask;
        }
    }
}