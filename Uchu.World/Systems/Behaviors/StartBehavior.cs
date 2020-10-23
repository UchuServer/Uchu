using System.Threading.Tasks;

namespace Uchu.World.Systems.Behaviors
{
    public class StartBehaviorExecutionParameters : BehaviorExecutionParameters
    {
        public BehaviorExecutionParameters Parameters { get; set; }

        public StartBehaviorExecutionParameters(ExecutionContext context, ExecutionBranchContext branchContext) 
            : base(context, branchContext)
        {
        }
    }
    public class StartBehavior : BehaviorBase<StartBehaviorExecutionParameters>
    {
        public override BehaviorTemplateId Id => BehaviorTemplateId.Start;
        private BehaviorBase Action { get; set; }
        private int UseTarget { get; set; }
        
        public override async Task BuildAsync()
        {
            Action = await GetBehavior("action");
            UseTarget = await GetParameter<int>("use_target");
        }

        protected override void DeserializeStart(StartBehaviorExecutionParameters parameters)
        {
            parameters.Parameters = Action.DeserializeStart(parameters.Context, parameters.BranchContext);
        }

        protected override async Task ExecuteStart(StartBehaviorExecutionParameters parameters)
        {
            await Action.ExecuteStart(parameters.Parameters);
        }
    }
}