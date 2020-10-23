using System.Threading.Tasks;

namespace Uchu.World.Systems.Behaviors
{
    public class SwitchBehaviorExecutionParameters : BehaviorExecutionParameters
    {
        public bool State { get; set; }
        public BehaviorExecutionParameters Parameters { get; set; }

        public SwitchBehaviorExecutionParameters(ExecutionContext context, ExecutionBranchContext branchContext) 
            : base(context, branchContext)
        {
        }
    }
    public class SwitchBehavior : BehaviorBase<SwitchBehaviorExecutionParameters>
    {
        public override BehaviorTemplateId Id => BehaviorTemplateId.Switch;

        private BehaviorBase Action { get; set; }
        private int Imagination { get; set; }
        private bool IsEnemyFaction { get; set; }
        private BehaviorBase ActionFalse { get; set; }
        private BehaviorBase ActionTrue { get; set; }
        
        public override async Task BuildAsync()
        {
            Action = await GetBehavior("action");
            ActionFalse = await GetBehavior("action_false");
            ActionTrue = await GetBehavior("action_true");
            Imagination = await GetParameter<int>("imagination");
            IsEnemyFaction = (await GetParameter("isEnemyFaction"))?.Value > 0;
        }

        protected override void DeserializeStart(SwitchBehaviorExecutionParameters parameters)
        {
            parameters.State = true;
            if (Imagination > 0 || !IsEnemyFaction)
                parameters.State = parameters.Context.Reader.ReadBit();

            parameters.Parameters = parameters.State
                ? ActionTrue.DeserializeStart(parameters.Context, parameters.BranchContext)
                : ActionFalse.DeserializeStart(parameters.Context, parameters.BranchContext);
        }

        protected override void SerializeStart(SwitchBehaviorExecutionParameters parameters)
        {
            
            parameters.State = true;
            if (Imagination > 0 || !IsEnemyFaction)
            {
                parameters.State = parameters.BranchContext.Target != default && parameters.NpcContext.Alive;
                parameters.NpcContext.Writer.WriteBit(parameters.State);
            }

            parameters.Parameters = parameters.State
                ? ActionTrue.SerializeStart(parameters.NpcContext, parameters.BranchContext)
                : ActionFalse.SerializeStart(parameters.NpcContext, parameters.BranchContext);
        }
        
        protected override async Task ExecuteStart(SwitchBehaviorExecutionParameters parameters)
        {
            if (parameters.State)
                await ActionTrue.ExecuteStart(parameters.Parameters);
            else
                await ActionFalse.ExecuteStart(parameters.Parameters);
        }
    }
}