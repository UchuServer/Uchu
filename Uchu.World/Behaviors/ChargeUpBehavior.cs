using System.Threading.Tasks;

namespace Uchu.World.Behaviors
{
    public class ChargeUpBehavior : BehaviorBase
    {
        public override BehaviorTemplateId Id => BehaviorTemplateId.ChargeUp;
        
        public BehaviorBase Action { get; set; }
        
        public float MaxDuration { get; set; }
        
        public override async Task BuildAsync()
        {
            Action = await GetBehavior("action");

            MaxDuration = await GetParameter<float>("max_duration");
        }
    }
}