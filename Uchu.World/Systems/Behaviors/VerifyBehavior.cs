using System.Threading.Tasks;

namespace Uchu.World.Systems.Behaviors
{
    public class VerifyBehaviorExecutionParameters : BehaviorExecutionParameters
    {
        public BehaviorExecutionParameters Parameters { get; set; }        
    }
    
    public class VerifyBehavior : BehaviorBase<VerifyBehaviorExecutionParameters>
    {
        public override BehaviorTemplateId Id => BehaviorTemplateId.Verify;

        private BehaviorBase Action { get; set; }
        
        public override async Task BuildAsync()
        {
            Action = await GetBehavior("action");
        }

        protected override Task ExecuteStart(VerifyBehaviorExecutionParameters parameters)
        {
            return Action.ExecuteStart(parameters.Parameters);
        }

        protected override void SerializeStart(VerifyBehaviorExecutionParameters parameters)
        {
            parameters.Parameters = Action.SerializeStart(parameters.NpcContext, parameters.BranchContext);
        }
    }
}