using System.Collections.Generic;
using System.Threading.Tasks;

namespace Uchu.World.Systems.Behaviors
{
    public class AndBehaviorExecutionParameters : BehaviorExecutionParameters
    {
        public List<BehaviorExecutionParameters> BehaviorExecutionParameters { get; set; } 
            = new List<BehaviorExecutionParameters>();
    }
    public class AndBehavior : BehaviorBase<AndBehaviorExecutionParameters>
    {
        public override BehaviorTemplateId Id => BehaviorTemplateId.And;
        
        private BehaviorBase[] Behaviors { get; set; }
        
        public override async Task BuildAsync()
        {
            var actions = GetParameters();

            Behaviors = new BehaviorBase[actions.Length];

            for (var i = 0; i < actions.Length; i++)
            {
                Behaviors[i] = await GetBehavior($"behavior {i + 1}");
            }
        }

        protected override void DeserializeStart(AndBehaviorExecutionParameters executionParameters)
        {
            foreach (var behaviorBase in Behaviors)
            {
                executionParameters.BehaviorExecutionParameters.Add(
                    behaviorBase.DeserializeStart(executionParameters.Context, executionParameters.BranchContext));
            }
        }

        protected override async Task ExecuteStart(AndBehaviorExecutionParameters executionParameters)
        {
            for (var i = 0; i < Behaviors.Length; i++)
            {
                await Behaviors[i].ExecuteStart(executionParameters.BehaviorExecutionParameters[i]);
            }
        }

        public override async Task SerializeStart(NpcExecutionContext context, ExecutionBranchContext branchContext)
        {
            foreach (var behavior in Behaviors)
            {
                await behavior.SerializeStart(context, branchContext);
            }
        }
    }
}