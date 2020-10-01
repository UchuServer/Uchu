using System.Threading.Tasks;

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

        public override BehaviorExecutionParameters DeserializeStart(ExecutionContext context,
            ExecutionBranchContext branchContext)
        {
            if (StunCaster != 1 && branchContext.Target != context.Associate)
                context.Reader.ReadBit();
            return base.DeserializeStart(context, branchContext);
        }

        public override Task SerializeStart(NpcExecutionContext context, ExecutionBranchContext branchContext)
        {
            if (StunCaster != 1)
                context.Writer.WriteBit(false);
            return Task.CompletedTask;
        }
    }
}