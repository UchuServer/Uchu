using System.Collections.Generic;

namespace Uchu.World.Behaviors
{
    public class ExecutionBranchContext
    {
        public readonly List<GameObject> Targets = new List<GameObject>();

        public int Duration;

        public void AddTarget(GameObject target)
        {
            if (target == null)
            {
                return;
            }
            
            if (Targets.Contains(target)) return;
            
            Targets.Add(target);
        }
    }
}