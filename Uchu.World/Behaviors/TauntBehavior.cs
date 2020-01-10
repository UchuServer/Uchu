using System.Threading.Tasks;

namespace Uchu.World.Behaviors
{
    public class TauntBehavior : BehaviorBase
    {
        public override BehaviorTemplateId Id => BehaviorTemplateId.Taunt;
        
        public override Task BuildAsync()
        {
            return Task.CompletedTask;
        }
    }
}