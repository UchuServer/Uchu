using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Uchu.World.Behaviors
{
    public class SwitchMultipleBehavior : BehaviorBase
    {
        public override BehaviorTemplateId Id => BehaviorTemplateId.SwitchMultiple;
        
        public float ChargeTime { get; set; }
        
        public float DistanceToTarget { get; set; }
        
        public Dictionary<BehaviorBase, float> Behaviors { get; set; }

        public override async Task BuildAsync()
        {
            ChargeTime = await GetParameter<float>("charge_up");

            DistanceToTarget = await GetParameter<float>("distance_to_target");

            Behaviors = new Dictionary<BehaviorBase, float>();

            var parameters = GetParameters();
            
            for (var index = 0; index < parameters.Length; index++)
            {
                var behavior = await GetBehavior($"behavior {index + 1}");
                
                if (behavior == default || behavior.Id == BehaviorTemplateId.Empty) continue;

                var value = await GetParameter<float>($"value {index + 1}");

                Behaviors[behavior] = value;
            }
        }

        public override async Task ExecuteAsync(ExecutionContext context, ExecutionBranchContext branchContext)
        {
            await base.ExecuteAsync(context, branchContext);

            var value = context.Reader.Read<float>();
            
            ((Player) context.Associate)?.SendChatMessage($"Switch Multiple: {value} -> {Behaviors.Count}");

            var defaultValue = Behaviors.ToArray()[0].Value;

            if (value <= defaultValue) value = defaultValue;
            
            foreach (var (behavior, mark) in Behaviors.ToArray().Reverse())
            {
                ((Player) context.Associate)?.SendChatMessage($"SWITCH: {behavior.Id} [{mark}]");
                
                if (value < mark) continue;

                await behavior.ExecuteAsync(context, branchContext);
                
                break;
            }
        }
    }
}