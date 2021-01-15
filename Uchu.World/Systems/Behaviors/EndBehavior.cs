using System.Threading.Tasks;

namespace Uchu.World.Systems.Behaviors
{
    public class EndBehavior : BehaviorBase
    {
        public override BehaviorTemplateId Id => BehaviorTemplateId.End;

        private BehaviorBase StartAction { get; set; }

        private int UseTarget { get; set; }
        
        public override void Build()
        {
            StartAction = GetBehavior("action");
            UseTarget = GetParameter<int>("use_target");
        }
    }
}