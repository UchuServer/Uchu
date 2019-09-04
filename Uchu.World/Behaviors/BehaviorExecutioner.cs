using System.Collections.Generic;
using Uchu.Core;

namespace Uchu.World.Behaviors
{
    public class BehaviorExecutioner
    {
        public Player Executioner;
        
        public readonly List<GameObject> Targets = new List<GameObject>();

        public void Execute()
        {
            Logger.Information($"Executing {Targets.Count} targets: {string.Join(", ", Targets)}");
            foreach (var target in Targets)
            {
                Logger.Debug($"Executing {target}");
                target.GetComponent<DestructibleComponent>()?.Smash(Executioner, Executioner);
            }
        }
    }
}