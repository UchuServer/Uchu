using System.Threading.Tasks;
using RakDotNet.IO;

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

        public override BehaviorExecutionParameters DeserializeStart(BitReader reader, ExecutionContext context,
            ExecutionBranchContext branchContext)
        {
            if (branchContext.Target != context.Associate)
                reader.ReadBit();
            if (InterruptBlock == 0)
                reader.ReadBit();
            reader.ReadBit();
            
            return base.DeserializeStart(reader, context, branchContext);
        }

        public override BehaviorExecutionParameters SerializeStart(BitWriter writer, NpcExecutionContext context,
            ExecutionBranchContext branchContext)
        {
            if (branchContext.Target != context.Associate)
                writer.WriteBit(false);
            if (InterruptBlock == 0)
                writer.WriteBit(false);
            writer.WriteBit(false);

            return base.SerializeStart(writer, context, branchContext);
        }
    }
}