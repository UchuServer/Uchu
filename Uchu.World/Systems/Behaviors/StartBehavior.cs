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

        public Event End { get; set; }
        
        public override async Task BuildAsync()
        {
            Action = await GetBehavior("action");
            UseTarget = await GetParameter<int>("use_target");
            End = new Event();
        }

        protected override void DeserializeStart(BitReader reader, StartBehaviorExecutionParameters parameters)
        {
            parameters.Parameters = Action.DeserializeStart(reader, parameters.Context, new ExecutionBranchContext{
                Target = parameters.BranchContext.Target,
                Duration = parameters.BranchContext.Duration,
                StartNode = this,
            });
        }
        protected override void SerializeStart(BitWriter writer, StartBehaviorExecutionParameters parameters)
        {
            parameters.Parameters = Action.SerializeStart(writer, parameters.NpcContext, new ExecutionBranchContext{
                Target = parameters.BranchContext.Target,
                Duration = parameters.BranchContext.Duration,
                StartNode = this,
            });
        }

        protected override void ExecuteStart(StartBehaviorExecutionParameters parameters)
        {
            parameters.Parameters.BranchContext.StartNode = this;
            //System.Console.WriteLine(parameters.Parameters.BranchContext.StartNode.BehaviorId);
            //this seems to give the ID just fine, but when BlockBehavior tries to access the StartNode, it is always null. Why?
            Action.ExecuteStart(parameters.Parameters);
        }
    }
}