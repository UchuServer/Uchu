namespace Uchu.World.Systems.Behaviors
{
    public class BehaviorExecutionParameters
    {
        public ExecutionContext Context { get; set; }
        public ExecutionBranchContext BranchContext { get; set; }
        public BehaviorExecutionParameters(ExecutionContext context, ExecutionBranchContext branchContext)
        {
            Context = context;
            BranchContext = new ExecutionBranchContext()
            {
                Duration = branchContext.Duration,
                Target = branchContext.Target
            };
        }
        protected BehaviorExecutionParameters()
        {
        }
    }
}