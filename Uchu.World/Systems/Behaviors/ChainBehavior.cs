using System.Collections.Generic;
using System.Threading.Tasks;

namespace Uchu.World.Systems.Behaviors
{
    public class ChainBehaviorExecutionParameters : BehaviorExecutionParameters
    {
        public uint ChainIndex { get; set; }
        public BehaviorExecutionParameters ChainIndexExecutionParameters { get; set; }
    }
    public class ChainBehavior : BehaviorBase<ChainBehaviorExecutionParameters>
    {
        public override BehaviorTemplateId Id => BehaviorTemplateId.Chain;

        private int Delay { get; set; }

        private BehaviorBase[] Behaviors { get; set; }

        public override async Task BuildAsync()
        {
            var actions = GetParameters();
            var behaviors = new List<BehaviorBase>();

            for (var i = 0; i < actions.Length; i++)
            {
                var behavior = await GetBehavior($"behavior {i + 1}");
                if (behavior == default) continue;
                behaviors.Add(behavior);
            }

            Behaviors = behaviors.ToArray();
            
            var delay = await GetParameter("chain_delay");
            if (delay.Value == null) return;

            Delay = (int) delay.Value;
        }

        protected override void DeserializeStart(ChainBehaviorExecutionParameters behaviorExecutionParameters)
        {
            behaviorExecutionParameters.ChainIndex = behaviorExecutionParameters.Context.Reader.Read<uint>();
            behaviorExecutionParameters.ChainIndexExecutionParameters = Behaviors[behaviorExecutionParameters.ChainIndex - 1]
                .DeserializeStart(behaviorExecutionParameters.Context, behaviorExecutionParameters.BranchContext);
        }

        protected override async Task ExecuteStart(ChainBehaviorExecutionParameters behaviorExecutionParameters)
        {
            await Behaviors[behaviorExecutionParameters.ChainIndex - 1]
                .ExecuteStart(behaviorExecutionParameters.ChainIndexExecutionParameters);
        }

        public override async Task SerializeStart(NpcExecutionContext context, ExecutionBranchContext branchContext)
        {
            // TODO
            context.Writer.Write(1);
            await Behaviors[1 - 1].SerializeStart(context, branchContext);
        }
    }
}