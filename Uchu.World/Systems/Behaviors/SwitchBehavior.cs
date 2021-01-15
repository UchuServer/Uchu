using System.Threading.Tasks;
using RakDotNet.IO;

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
        
        public override void Build()
        {
            Action = GetBehavior("action");
            ActionFalse = GetBehavior("action_false");
            ActionTrue = GetBehavior("action_true");
            Imagination = GetParameter<int>("imagination");
            IsEnemyFaction = (GetParameter("isEnemyFaction"))?.Value > 0;
        }

        protected override void DeserializeStart(BitReader reader, SwitchBehaviorExecutionParameters parameters)
        {
            parameters.State = true;
            if (Imagination > 0 || !IsEnemyFaction)
                parameters.State = reader.ReadBit();

            parameters.Parameters = parameters.State
                ? ActionTrue.DeserializeStart(reader, parameters.Context, parameters.BranchContext)
                : ActionFalse.DeserializeStart(reader, parameters.Context, parameters.BranchContext);
        }

        protected override void SerializeStart(BitWriter writer, SwitchBehaviorExecutionParameters parameters)
        {
            
            parameters.State = true;
            if (Imagination > 0 || !IsEnemyFaction)
            {
                parameters.State = parameters.BranchContext.Target != default && parameters.NpcContext.Alive;
                writer.WriteBit(parameters.State);
            }

            parameters.Parameters = parameters.State
                ? ActionTrue.SerializeStart(writer, parameters.NpcContext, parameters.BranchContext)
                : ActionFalse.SerializeStart(writer, parameters.NpcContext, parameters.BranchContext);
        }
        
        protected override void ExecuteStart(SwitchBehaviorExecutionParameters parameters)
        {
            if (parameters.State)
                ActionTrue.ExecuteStart(parameters.Parameters);
            else
                ActionFalse.ExecuteStart(parameters.Parameters);
        }
    }
}