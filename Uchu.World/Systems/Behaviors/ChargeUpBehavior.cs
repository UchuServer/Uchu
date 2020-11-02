using System.Threading.Tasks;
using Uchu.Core;

namespace Uchu.World.Systems.Behaviors
{
    public class ChargeUpBehaviorExecutionParameters : BehaviorExecutionParameters
    {
        public uint Handle { get; set; }
        public BehaviorExecutionParameters ActionExecutionParameters { get; set; }

        public ChargeUpBehaviorExecutionParameters(ExecutionContext context, ExecutionBranchContext branchContext)
            : base(context, branchContext)
        {
        }
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

        protected override void DeserializeStart(ChargeUpBehaviorExecutionParameters parameters)
        {
            parameters.Handle = parameters.Context.Reader.Read<uint>();
            parameters.RegisterHandle<ChargeUpBehaviorExecutionParameters>(parameters.Handle, DeserializeSync, ExecuteSync);
        }

        protected override void DeserializeSync(ChargeUpBehaviorExecutionParameters parameters)
        {
            parameters.ActionExecutionParameters = parameters.ActionExecutionParameters = Action.DeserializeStart(
                parameters.Context, parameters.BranchContext);
        }

        protected override void ExecuteSync(ChargeUpBehaviorExecutionParameters parameters)
        {
            Action.ExecuteStart(parameters.ActionExecutionParameters);
        }
    }
}