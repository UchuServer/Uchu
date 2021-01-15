using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RakDotNet.IO;

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

        public override void Build()
        {
            ChargeTime = GetParameter<float>("charge_up");
            DistanceToTarget = GetParameter<float>("distance_to_target");
            Behaviors = new Dictionary<BehaviorBase, float>();

            var parameters = GetParameters();
            
            for (var index = 0; index < parameters.Length; index++)
            {
                var behavior = GetBehavior($"behavior {index + 1}");
                if (behavior == default || behavior.Id == BehaviorTemplateId.Empty)
                    continue;
                var value = GetParameter<float>($"value {index + 1}");
                Behaviors[behavior] = value;
            }
        }

        protected override void DeserializeStart(BitReader reader, SwitchMultipleBehaviorExecutionParameters parameters)
        {
            parameters.Value = reader.Read<float>();

            var defaultValue = Behaviors.ToArray()[0].Value;
            if (parameters.Value <= defaultValue)
                parameters.Value = defaultValue;
            
            foreach (var (behavior, mark) in Behaviors.ToArray().Reverse())
            {
                if (parameters.Value < mark)
                    continue;
                parameters.Behavior = behavior;
                parameters.Parameters = parameters.Behavior.DeserializeStart(reader, parameters.Context,
                    parameters.BranchContext);
                break;
            }
        }

        protected override void ExecuteStart(SwitchMultipleBehaviorExecutionParameters parameters)
        {
            parameters.Behavior.ExecuteStart(parameters.Parameters);
        }
    }
}