using System.Threading.Tasks;
using RakDotNet.IO;

namespace Uchu.World.Systems.Behaviors
{
    public class TargetCasterBehaviorExecutionParameters : BehaviorExecutionParameters
    {
        public BehaviorExecutionParameters Parameters { get; set; }

        public TargetCasterBehaviorExecutionParameters(ExecutionContext context, ExecutionBranchContext branchContext) 
            : base(context, branchContext)
        {
        }
    }
    public class TargetCasterBehavior : BehaviorBase<TargetCasterBehaviorExecutionParameters>
    {
        public override BehaviorTemplateId Id => BehaviorTemplateId.TargetCaster;

        private BehaviorBase Action { get; set; }
        
        public override void Build()
        {
            Action = GetBehavior("action");
        }

        protected override void DeserializeStart(BitReader reader, TargetCasterBehaviorExecutionParameters parameters)
        {
            parameters.Parameters = Action.DeserializeStart(reader, parameters.Context, parameters.BranchContext);
        }

        protected override void ExecuteStart(TargetCasterBehaviorExecutionParameters parameters)
        {
            Action.ExecuteStart(parameters.Parameters);
        }
    }
}