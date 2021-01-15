using System.Threading.Tasks;
using RakDotNet.IO;

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
        
        public override void Build()
        {
            Action = GetBehavior("action");
            UseTarget = GetParameter<int>("use_target");
        }

        protected override void DeserializeStart(BitReader reader, StartBehaviorExecutionParameters parameters)
        {
            parameters.Parameters = Action.DeserializeStart(reader, parameters.Context, parameters.BranchContext);
        }

        protected override void ExecuteStart(StartBehaviorExecutionParameters parameters)
        {
            Action.ExecuteStart(parameters.Parameters);
        }
    }
}