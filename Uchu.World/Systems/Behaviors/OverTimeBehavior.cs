using System.Threading.Tasks;

namespace Uchu.World.Systems.Behaviors
{
    public class OverTimeBehaviorExecutionParameters : BehaviorExecutionParameters
    {
        public BehaviorExecutionParameters BehaviorExecutionParameters { get; set; }
    }
    public class OverTimeBehavior : BehaviorBase<OverTimeBehaviorExecutionParameters>
    {
        public override BehaviorTemplateId Id => BehaviorTemplateId.OverTime;

        private BehaviorBase Action { get; set; }
        
        public override async Task BuildAsync()
        {
            Action = await GetBehavior("action");
        }

        protected override void DeserializeStart(OverTimeBehaviorExecutionParameters parameters)
        {
            parameters.BehaviorExecutionParameters = Action.DeserializeStart(parameters.Context,
                parameters.BranchContext);
        }

        protected override async Task ExecuteStart(OverTimeBehaviorExecutionParameters parameters)
        {
            await Action.ExecuteStart(parameters.BehaviorExecutionParameters);
        }
    }
}