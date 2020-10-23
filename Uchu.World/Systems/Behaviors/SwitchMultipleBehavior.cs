using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Uchu.World.Systems.Behaviors
{
    public class SwitchMultipleBehaviorExecutionParameters : BehaviorExecutionParameters
    {
        public float Value { get; set; }
        public BehaviorExecutionParameters Parameters { get; set; }
        public BehaviorBase Behavior { get; set; }

        public SwitchMultipleBehaviorExecutionParameters(ExecutionContext context, ExecutionBranchContext branchContext) 
            : base(context, branchContext)
        {
        }
    }
    public class SwitchMultipleBehavior : BehaviorBase<SwitchMultipleBehaviorExecutionParameters>
    {
        public override BehaviorTemplateId Id => BehaviorTemplateId.SwitchMultiple;

        private float ChargeTime { get; set; }
        private float DistanceToTarget { get; set; }
        private Dictionary<BehaviorBase, float> Behaviors { get; set; }

        public override async Task BuildAsync()
        {
            ChargeTime = await GetParameter<float>("charge_up");
            DistanceToTarget = await GetParameter<float>("distance_to_target");
            Behaviors = new Dictionary<BehaviorBase, float>();

            var parameters = GetParameters();
            
            for (var index = 0; index < parameters.Length; index++)
            {
                var behavior = await GetBehavior($"behavior {index + 1}");
                if (behavior == default || behavior.Id == BehaviorTemplateId.Empty)
                    continue;
                var value = await GetParameter<float>($"value {index + 1}");
                Behaviors[behavior] = value;
            }
        }

        protected override void DeserializeStart(SwitchMultipleBehaviorExecutionParameters parameters)
        {
            parameters.Value = parameters.Context.Reader.Read<float>();

            var defaultValue = Behaviors.ToArray()[0].Value;
            if (parameters.Value <= defaultValue)
                parameters.Value = defaultValue;
            
            foreach (var (behavior, mark) in Behaviors.ToArray().Reverse())
            {
                if (parameters.Value < mark)
                    continue;
                parameters.Behavior = behavior;
                parameters.Parameters = parameters.Behavior.DeserializeStart(parameters.Context,
                    parameters.BranchContext);
                break;
            }
        }

        protected override async Task ExecuteStart(SwitchMultipleBehaviorExecutionParameters parameters)
        {
            await parameters.Behavior.ExecuteStart(parameters.Parameters);
        }
    }
}