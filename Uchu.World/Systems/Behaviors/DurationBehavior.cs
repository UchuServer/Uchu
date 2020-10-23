using System.Threading.Tasks;

namespace Uchu.World.Systems.Behaviors
{
    public class DurationBehaviorExecutionParameters : BehaviorExecutionParameters
    {
        public BehaviorExecutionParameters ActionExecutionParameters { get; set; }

        public DurationBehaviorExecutionParameters(ExecutionContext context, ExecutionBranchContext branchContext) 
            : base(context, branchContext)
        {
        }
    }
    public class DurationBehavior : BehaviorBase<DurationBehaviorExecutionParameters>
    {
        public override BehaviorTemplateId Id => BehaviorTemplateId.Duration;

        private BehaviorBase Action { get; set; }

        private int ActionDuration { get; set; }
        
        public override async Task BuildAsync()
        {
            Action = await GetBehavior("action");

            var duration = await GetParameter("duration");
            if (duration.Value == null) return;

            ActionDuration = (int) duration.Value;
        }

        protected override void DeserializeStart(DurationBehaviorExecutionParameters parameters)
        {
            parameters.ActionExecutionParameters = Action.DeserializeStart(parameters.Context, 
                new ExecutionBranchContext()
                {
                    Target = parameters.BranchContext.Target,
                    Duration = ActionDuration * 1000
                });
        }

        protected override async Task ExecuteStart(DurationBehaviorExecutionParameters behaviorExecutionParameters)
        {
            await Action.ExecuteStart(behaviorExecutionParameters.ActionExecutionParameters);
        }

        protected override void SerializeStart(DurationBehaviorExecutionParameters parameters)
        {
            parameters.ActionExecutionParameters = Action.SerializeStart(parameters.NpcContext, 
                new ExecutionBranchContext()
                {
                    Target = parameters.BranchContext.Target,
                    Duration = ActionDuration * 1000
                });
        }
    }
}