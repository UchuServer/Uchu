using System.Threading.Tasks;

namespace Uchu.World.Systems.Behaviors
{
    public class EndBehavior : BehaviorBase
    {
        public override BehaviorTemplateId Id => BehaviorTemplateId.End;

        private BehaviorBase StartAction { get; set; }

        private int UseTarget { get; set; }
        
        public override async Task BuildAsync()
        {
            StartAction = await GetBehavior("action");
            UseTarget = await GetParameter<int>("use_target");
        }
    }
}