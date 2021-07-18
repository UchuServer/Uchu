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
        
        private List<BehaviorBase> Behaviors = new List<BehaviorBase>();

        private List<float> Values = new List<float>();

        public override async Task BuildAsync()
        {
            //what do these do?
            ChargeTime = await GetParameter<float>("charge_up");
            DistanceToTarget = await GetParameter<float>("distance_to_target");

            //dynamically load all possible behaviors the switch can have
            var valid = true;
            var count = 1;
            while (valid){
                var Behavior = await GetBehavior("behavior " + count);
                var Value = await GetParameter<float>("value " + count);
                if (Behavior == null || Behavior == default || Behavior.BehaviorId == 0){
                    valid = false;
                } else {
                    count++;
                    Behaviors.Add(Behavior);
                    Values.Add(Value);
                }
            }
        }

        protected override void DeserializeStart(BitReader reader, SwitchMultipleBehaviorExecutionParameters parameters)
        {
            parameters.Value = reader.Read<float>();
            var currentBehavior = Behaviors[0];
            //check each behavior's threshold to see if we go past it, when we find one that's too much we break out of the loop
            for (int i = 1; i < Values.Count(); i++){
                if (parameters.Value > Values[i-1]){
                    currentBehavior = Behaviors[i];
                } else {
                    break;
                }
            }
            parameters.Behavior = currentBehavior;
            parameters.Parameters =
                parameters.Behavior.DeserializeStart(reader, parameters.Context, parameters.BranchContext);
        }

        protected override void ExecuteStart(SwitchMultipleBehaviorExecutionParameters parameters)
        {
            parameters.Behavior.ExecuteStart(parameters.Parameters);
        }
    }
}