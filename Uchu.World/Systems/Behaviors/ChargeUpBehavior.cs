using System.Threading.Tasks;
using Uchu.Core;

namespace Uchu.World.Systems.Behaviors
{
    public class ChargeUpBehaviorExecutionParameters : BehaviorExecutionParameters
    {
        public uint Handle { get; set; }
        public BehaviorExecutionParameters ActionExecutionParameters { get; set; }
    }
    public class ChargeUpBehavior : BehaviorBase<ChargeUpBehaviorExecutionParameters>
    {
        public override BehaviorTemplateId Id => BehaviorTemplateId.ChargeUp;

        private BehaviorBase Action { get; set; }

        private float MaxDuration { get; set; }

        public override async Task BuildAsync()
        {
            Action = await GetBehavior("action");
            MaxDuration = await GetParameter<float>("max_duration");
        }

        protected override void DeserializeStart(ChargeUpBehaviorExecutionParameters behaviorExecutionParameters)
        {
            behaviorExecutionParameters.Handle = behaviorExecutionParameters.Context.Reader.Read<uint>();
            behaviorExecutionParameters.ActionExecutionParameters = Action.DeserializeStart(
                behaviorExecutionParameters.Context, behaviorExecutionParameters.BranchContext);
            
            RegisterHandle(behaviorExecutionParameters.Handle, behaviorExecutionParameters);
        }

        protected override async Task ExecuteSync(ChargeUpBehaviorExecutionParameters behaviorExecutionParameters)
        {
            await Action.ExecuteStart(behaviorExecutionParameters.ActionExecutionParameters);
        }
    }
}