using System.Threading.Tasks;

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
        
        public override async Task BuildAsync()
        {
            Action = await GetBehavior("action");
        }

        protected override void DeserializeStart(TargetCasterBehaviorExecutionParameters parameters)
        {
            parameters.Parameters = Action.DeserializeStart(parameters.Context, parameters.BranchContext);
        }

        protected override void ExecuteStart(TargetCasterBehaviorExecutionParameters parameters)
        {
            Action.ExecuteStart(parameters.Parameters);
        }
    }
}