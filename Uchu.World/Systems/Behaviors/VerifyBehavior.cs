using System.Threading.Tasks;
using RakDotNet.IO;

namespace Uchu.World.Systems.Behaviors
{
    public class VerifyBehaviorExecutionParameters : BehaviorExecutionParameters
    {
        public BehaviorExecutionParameters Parameters { get; set; }

        public VerifyBehaviorExecutionParameters(ExecutionContext context, ExecutionBranchContext branchContext) 
            : base(context, branchContext)
        {
        }
    }
    
    public class VerifyBehavior : BehaviorBase<VerifyBehaviorExecutionParameters>
    {
        public override BehaviorTemplateId Id => BehaviorTemplateId.Verify;

        private BehaviorBase Action { get; set; }
        
        public override void Build()
        {
            Action = GetBehavior("action");
        }

        protected override void ExecuteStart(VerifyBehaviorExecutionParameters parameters)
        {
            Action.ExecuteStart(parameters.Parameters);
        }

        protected override void SerializeStart(BitWriter writer, VerifyBehaviorExecutionParameters parameters)
        {
            parameters.Parameters = Action.SerializeStart(writer, parameters.NpcContext, parameters.BranchContext);
        }
    }
}