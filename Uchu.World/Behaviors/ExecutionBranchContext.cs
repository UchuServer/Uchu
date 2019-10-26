namespace Uchu.World.Behaviors
{
    public class ExecutionBranchContext
    {
        public readonly GameObject Target;

        public int Duration;

        public ExecutionBranchContext(GameObject target)
        {
            Target = target;
        }
    }
}