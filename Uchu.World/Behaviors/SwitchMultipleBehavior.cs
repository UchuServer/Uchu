using System.Threading.Tasks;

namespace Uchu.World.Behaviors
{
    public class SwitchMultipleBehavior : BehaviorBase
    {
        public override BehaviorTemplateId Id => BehaviorTemplateId.SwitchMultiple;
        
        public float ChargeTime { get; set; }
        
        public float DistanceToTarget { get; set; }
        
        public override async Task BuildAsync()
        {
            ChargeTime = await GetParameter<float>("charge_up");

            DistanceToTarget = await GetParameter<float>("distance_to_target");
        }

        public override Task ExecuteAsync(ExecutionContext context, ExecutionBranchContext branchContext)
        {
            var value = (int) context.Reader.Read<float>();
            
            // TODO
            
            return Task.CompletedTask;
        }
    }
}