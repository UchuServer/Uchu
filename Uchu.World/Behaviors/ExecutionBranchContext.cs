namespace Uchu.World.Behaviors
{
    public class ExecutionBranchContext
    {
        public GameObject Target { get; set; }

        public int Duration { get; set; }

        public ExecutionBranchContext(GameObject target)
        {
            Target = target;
        }
    }
}