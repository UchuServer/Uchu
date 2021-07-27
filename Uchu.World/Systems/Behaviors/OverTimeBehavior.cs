using System;
using System.Threading.Tasks;
using RakDotNet.IO;

namespace Uchu.World.Systems.Behaviors
{
    public class OverTimeBehaviorExecutionParameters : BehaviorExecutionParameters
    {
        public BehaviorExecutionParameters Parameters { get; set; }

        public OverTimeBehaviorExecutionParameters(ExecutionContext context, ExecutionBranchContext branchContext) 
            : base(context, branchContext)
        {
        }
    }
    public class OverTimeBehavior : BehaviorBase<OverTimeBehaviorExecutionParameters>
    {
        public override BehaviorTemplateId Id => BehaviorTemplateId.OverTime;

        private BehaviorBase Action { get; set; }

        private int Delay { get; set; }

        private int Intervals { get; set; }
        
        public override async Task BuildAsync()
        {
            Action = await GetBehavior("action");
            Delay = await GetParameter<int>("delay");
            Intervals = await GetParameter<int>("num_intervals");
        }

        protected override void DeserializeStart(BitReader reader, OverTimeBehaviorExecutionParameters parameters)
        {
            parameters.Parameters = Action.DeserializeStart(reader, parameters.Context,
                parameters.BranchContext);
        }

        protected override void ExecuteStart(OverTimeBehaviorExecutionParameters parameters)
        {
            Action Execute = (() => 
            {
                //Console.WriteLine("did one tick for over time");
                Action.ExecuteStart(parameters.Parameters);
            });
            for (int i = 1; i <= Intervals; i++){
                parameters.Schedule(Execute, i * 1000 * Delay);
            }
        }
    }
}