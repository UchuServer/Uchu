using System.Threading.Tasks;

namespace Uchu.World.Systems.Behaviors
{
    public class ModularBuildBehavior : BehaviorBase
    {
        public override BehaviorTemplateId Id => BehaviorTemplateId.ModularBuild;
        
        public override Task BuildAsync()
        {
            return Task.CompletedTask;
        }
    }
}