namespace Uchu.World.Systems.Behaviors
{
    /// <summary>
    /// The execution context of a behavior branch
    /// </summary>
    /// <remarks>
    /// Whenever a behavior starts up a new behavior, this is considered a branch. A branch may have different targets
    /// than it's parent node and therefore this class should always be newly created for each behavior branch and
    /// optionally copy parameters from its parent branch context.
    /// </remarks>
    public class ExecutionBranchContext
    {
        /// <summary>
        /// The target of this branch
        /// </summary>
        public GameObject Target { get; set; }

        /// <summary>
        /// Duration of this branch
        /// </summary>
        public int Duration { get; set; }

        public ExecutionBranchContext()
        {
        }
    }
}