using System.Threading.Tasks;
using RakDotNet.IO;

namespace Uchu.World.Systems.Behaviors
{
    public class StunBehavior : BehaviorBase
    {
        public override BehaviorTemplateId Id => BehaviorTemplateId.Stun;
        private int StunCaster { get; set; }
        
        public override async Task BuildAsync()
        {
            StunCaster = await GetParameter<int>("stun_caster");
        }

        public override BehaviorExecutionParameters DeserializeStart(BitReader reader, ExecutionContext context,
            ExecutionBranchContext branchContext)
        {
            if (StunCaster != 1 && branchContext.Target != context.Associate)
                reader.ReadBit();
            return base.DeserializeStart(reader, context, branchContext);
        }

        public override BehaviorExecutionParameters SerializeStart(BitWriter writer, NpcExecutionContext context,
            ExecutionBranchContext branchContext)
        {
            if (StunCaster != 1)
                writer.WriteBit(false);
            return base.SerializeStart(writer, context, branchContext);
        }
    }
}