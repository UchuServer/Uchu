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
        private BehaviorBase Behavior1 { get; set; }
        private BehaviorBase Behavior2 { get; set; }
        private float Value1 { get; set; }

        public override async Task BuildAsync()
        {
            ChargeTime = await GetParameter<float>("charge_up");
            DistanceToTarget = await GetParameter<float>("distance_to_target");
            Behavior1 = await GetBehavior("behavior 1");
            Behavior2 = await GetBehavior("behavior 2");
            Value1 = await GetParameter<float>("value 1");
        }

        protected override void DeserializeStart(BitReader reader, SwitchMultipleBehaviorExecutionParameters parameters)
        {
            parameters.Value = reader.Read<float>();
            parameters.Behavior = parameters.Value <= Value1 ? Behavior1 : Behavior2;
            parameters.Parameters =
                parameters.Behavior.DeserializeStart(reader, parameters.Context, parameters.BranchContext);
        }

        protected override void ExecuteStart(SwitchMultipleBehaviorExecutionParameters parameters)
        {
            parameters.Behavior.ExecuteStart(parameters.Parameters);
        }
    }
}