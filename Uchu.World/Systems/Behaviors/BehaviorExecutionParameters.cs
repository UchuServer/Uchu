using System.Threading;

namespace Uchu.World.Systems.Behaviors
{
    public class BehaviorExecutionParameters
    {
        public Mutex Lock { get; set; } = new Mutex();
        public ExecutionContext Context { get; set; }
        public ExecutionBranchContext BranchContext { get; set; }

        /// <summary>
        /// Returns the context as a NpcContext, can cause a NullReference if the context can't be casted.
        /// </summary>
        public NpcExecutionContext NpcContext
        {
            get => (NpcExecutionContext) Context;
            set => Context = value;
        }

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