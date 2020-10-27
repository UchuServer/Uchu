using System.Threading.Tasks;

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
        
        public override async Task BuildAsync()
        {
            Action = await GetBehavior("action");
        }

        protected override void ExecuteStart(VerifyBehaviorExecutionParameters parameters)
        {
            Action.ExecuteStart(parameters.Parameters);
        }

        protected override void SerializeStart(VerifyBehaviorExecutionParameters parameters)
        {
            parameters.Parameters = Action.SerializeStart(parameters.NpcContext, parameters.BranchContext);
        }
    }
}