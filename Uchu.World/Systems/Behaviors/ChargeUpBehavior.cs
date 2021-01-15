using System.Threading.Tasks;
using RakDotNet.IO;
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

        public override void Build()
        {
            Action = GetBehavior("action");
            MaxDuration = GetParameter<float>("max_duration");
        }

        protected override void DeserializeStart(BitReader reader, ChargeUpBehaviorExecutionParameters parameters)
        {
            parameters.Handle = reader.Read<uint>();
            parameters.RegisterHandle<ChargeUpBehaviorExecutionParameters>(parameters.Handle, DeserializeSync, ExecuteSync);
        }

        protected override void DeserializeSync(BitReader reader, ChargeUpBehaviorExecutionParameters parameters)
        {
            parameters.ActionExecutionParameters = parameters.ActionExecutionParameters = Action.DeserializeStart(
                reader, parameters.Context, parameters.BranchContext);
        }

        protected override void ExecuteSync(ChargeUpBehaviorExecutionParameters parameters)
        {
            Action.ExecuteStart(parameters.ActionExecutionParameters);
        }
    }
}