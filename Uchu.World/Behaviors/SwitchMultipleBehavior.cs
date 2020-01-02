using System.Threading.Tasks;

namespace Uchu.World.Behaviors
{
    public class SwitchMultipleBehavior : BehaviorBase
    {
        public override BehaviorTemplateId Id => BehaviorTemplateId.SwitchMultiple;
        
        public float ChargeTime { get; set; }
        
        public float DistanceToTarget { get; set; }
        
        public BehaviorBase Behavior1 { get; set; }
        
        public BehaviorBase Behavior2 { get; set; }
        
        public override async Task BuildAsync()
        {
            ChargeTime = await GetParameter<float>("charge_up");

            DistanceToTarget = await GetParameter<float>("distance_to_target");

            Behavior1 = await GetBehavior("behavior_1");
            
            Behavior2 = await GetBehavior("behavior_2");
        }

        public override async Task ExecuteAsync(ExecutionContext context, ExecutionBranchContext branchContext)
        {
            await base.ExecuteAsync(context, branchContext);

            var value = (int) context.Reader.Read<float>();

            if (value > 1)
            {
                await Behavior1.ExecuteAsync(context, branchContext);
            }
            else
            {
                await Behavior2.ExecuteAsync(context, branchContext);
            }
        }
    }
}