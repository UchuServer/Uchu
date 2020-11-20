using System.Threading.Tasks;
using RakDotNet.IO;

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

        protected override void DeserializeStart(BitReader reader, DurationBehaviorExecutionParameters parameters)
        {
            parameters.ActionExecutionParameters = Action.DeserializeStart(reader, parameters.Context, 
                new ExecutionBranchContext()
                {
                    Target = parameters.BranchContext.Target,
                    Duration = ActionDuration * 1000
                });
        }

        protected override void ExecuteStart(DurationBehaviorExecutionParameters behaviorExecutionParameters)
        {
            Action.ExecuteStart(behaviorExecutionParameters.ActionExecutionParameters);
        }

        protected override void SerializeStart(BitWriter writer, DurationBehaviorExecutionParameters parameters)
        {
            parameters.ActionExecutionParameters = Action.SerializeStart(writer, parameters.NpcContext, 
                new ExecutionBranchContext()
                {
                    Target = parameters.BranchContext.Target,
                    Duration = ActionDuration * 1000
                });
        }
    }
}