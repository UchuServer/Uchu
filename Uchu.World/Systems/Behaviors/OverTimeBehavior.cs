using System.Threading.Tasks;
using RakDotNet.IO;

namespace Uchu.World.Systems.Behaviors
{
    public class OverTimeBehaviorExecutionParameters : BehaviorExecutionParameters
    {
        public BehaviorExecutionParameters Parameters { get; set; }

        public OverTimeBehaviorExecutionParameters(ExecutionContext context, ExecutionBranchContext branchContext) 
            : base(context, branchContext)
        {
        }
    }
    public class OverTimeBehavior : BehaviorBase<OverTimeBehaviorExecutionParameters>
    {
        public override BehaviorTemplateId Id => BehaviorTemplateId.OverTime;

        private BehaviorBase Action { get; set; }
        
        public override async Task BuildAsync()
        {
            Action = await GetBehavior("action");
        }

        protected override void DeserializeStart(BitReader reader, OverTimeBehaviorExecutionParameters parameters)
        {
            parameters.Parameters = Action.DeserializeStart(reader, parameters.Context,
                parameters.BranchContext);
        }

        protected override void ExecuteStart(OverTimeBehaviorExecutionParameters parameters)
        {
            Action.ExecuteStart(parameters.Parameters);
        }
    }
}