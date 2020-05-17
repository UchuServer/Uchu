using System.Threading.Tasks;

namespace Uchu.World.Systems.Behaviors
{
    public class InterruptBehavior : BehaviorBase
    {
        public override BehaviorTemplateId Id => BehaviorTemplateId.Interrupt;
        
        public int InterruptAttack { get; set; }
        public int InterruptBlock { get; set; }
        public int InterruptCharge { get; set; }
        public int InteruptAttack { get; set; }
        public int InteruptCharge { get; set; }
        
        public override async Task BuildAsync()
        {
            InterruptAttack = await GetParameter<int>("interrupt_attack");
            InterruptBlock = await GetParameter<int>("interrupt_block");
            InterruptCharge = await GetParameter<int>("interrupt_charge");
            InteruptAttack = await GetParameter<int>("interupt_attack");
            InteruptCharge = await GetParameter<int>("interupt_charge");
        }

        public override async Task ExecuteAsync(ExecutionContext context, ExecutionBranchContext branchContext)
        {
            await base.ExecuteAsync(context, branchContext);

            if (branchContext.Target != context.Associate)
            {
                context.Reader.ReadBit();
            }

            if (InterruptBlock == 0)
            {
                context.Reader.ReadBit();
            }

            context.Reader.ReadBit();
        }

        public override async Task CalculateAsync(NpcExecutionContext context, ExecutionBranchContext branchContext)
        {
            if (branchContext.Target != context.Associate)
            {
                context.Writer.WriteBit(false);
            }

            if (InterruptBlock == 0)
            {
                context.Writer.WriteBit(false);
            }
            
            context.Writer.WriteBit(false);
        }
    }
}