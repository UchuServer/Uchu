using System.Collections.Generic;
using System.Threading.Tasks;
using RakDotNet.IO;

namespace Uchu.World.Systems.Behaviors
{
    public class ChainBehaviorExecutionParameters : BehaviorExecutionParameters
    {
        public uint ChainIndex { get; set; } = 1;
        public BehaviorExecutionParameters ChainIndexExecutionParameters { get; set; }

        public ChainBehaviorExecutionParameters(ExecutionContext context, ExecutionBranchContext branchContext) 
            : base(context, branchContext)
        {
        }
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

        protected override void DeserializeStart(BitReader reader, ChainBehaviorExecutionParameters parameters)
        {
            parameters.ChainIndex = reader.Read<uint>();
            parameters.ChainIndexExecutionParameters = Behaviors[parameters.ChainIndex - 1]
                .DeserializeStart(reader, parameters.Context, parameters.BranchContext);
        }

        protected override void ExecuteStart(ChainBehaviorExecutionParameters parameters)
        {
            Behaviors[parameters.ChainIndex - 1]
                .ExecuteStart(parameters.ChainIndexExecutionParameters);
        }

        protected override void SerializeStart(BitWriter writer, ChainBehaviorExecutionParameters parameters)
        {
            writer.Write(parameters.ChainIndex);
            parameters.ChainIndexExecutionParameters = Behaviors[0].SerializeStart(writer, parameters.NpcContext,
                parameters.BranchContext);
        }
    }
}