using System.Threading.Tasks;

namespace Uchu.World.Systems.Behaviors
{
    public class AndBehavior : BehaviorBase
    {
        public override BehaviorTemplateId Id => BehaviorTemplateId.And;
        
        public BehaviorBase[] Behaviors { get; set; }
        
        public override async Task BuildAsync()
        {
            var actions = GetParameters();

            Behaviors = new BehaviorBase[actions.Length];

            for (var i = 0; i < actions.Length; i++)
            {
                Behaviors[i] = await GetBehavior($"behavior {i + 1}");
            }
        }

        public override async Task ExecuteAsync(ExecutionContext context, ExecutionBranchContext branchContext)
        {
            await base.ExecuteAsync(context, branchContext);
            
            foreach (var behavior in Behaviors)
            {
                await behavior.ExecuteAsync(context, branchContext);
            }
        }

        public override async Task CalculateAsync(NpcExecutionContext context, ExecutionBranchContext branchContext)
        {
            foreach (var behavior in Behaviors)
            {
                await behavior.CalculateAsync(context, branchContext);
            }
        }
    }
}