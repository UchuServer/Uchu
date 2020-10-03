using System.Threading.Tasks;

namespace Uchu.World.Systems.Behaviors
{
    public class DurationBehaviorExecutionParameters : BehaviorExecutionParameters
    {
        public BehaviorExecutionParameters ActionExecutionParameters { get; set; }
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
                parameters.BranchContext);
        }

        protected override async Task ExecuteStart(DurationBehaviorExecutionParameters behaviorExecutionParameters)
        {
            behaviorExecutionParameters.ActionExecutionParameters.BranchContext.Duration = ActionDuration * 1000;
            await Action.ExecuteStart(behaviorExecutionParameters.ActionExecutionParameters);
        }

        protected override void SerializeStart(DurationBehaviorExecutionParameters parameters)
        {
            parameters.BranchContext.Duration = ActionDuration * 1000;
            parameters.ActionExecutionParameters = Action.SerializeStart(parameters.NpcContext, 
                parameters.BranchContext);
        }
    }
}