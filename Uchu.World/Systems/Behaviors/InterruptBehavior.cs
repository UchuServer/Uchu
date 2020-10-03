using System.Threading.Tasks;

namespace Uchu.World.Systems.Behaviors
{
    public class InterruptBehavior : BehaviorBase
    {
        public override BehaviorTemplateId Id => BehaviorTemplateId.Interrupt;

        private int InterruptAttack { get; set; }
        private int InterruptBlock { get; set; }
        private int InterruptCharge { get; set; }
        private int InteruptAttack { get; set; }
        private int InteruptCharge { get; set; }
        
        public override async Task BuildAsync()
        {
            InterruptAttack = await GetParameter<int>("interrupt_attack");
            InterruptBlock = await GetParameter<int>("interrupt_block");
            InterruptCharge = await GetParameter<int>("interrupt_charge");
            InteruptAttack = await GetParameter<int>("interupt_attack");
            InteruptCharge = await GetParameter<int>("interupt_charge");
        }

        public override BehaviorExecutionParameters DeserializeStart(ExecutionContext context,
            ExecutionBranchContext branchContext)
        {
            if (branchContext.Target != context.Associate)
                context.Reader.ReadBit();
            if (InterruptBlock == 0)
                context.Reader.ReadBit();
            context.Reader.ReadBit();
            
            return base.DeserializeStart(context, branchContext);
        }

        public override BehaviorExecutionParameters SerializeStart(NpcExecutionContext context,
            ExecutionBranchContext branchContext)
        {
            if (branchContext.Target != context.Associate)
                context.Writer.WriteBit(false);

            if (InterruptBlock == 0)
                context.Writer.WriteBit(false);

            context.Writer.WriteBit(false);

            return base.SerializeStart(context, branchContext);
        }
    }
}