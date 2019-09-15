using System.Collections.Generic;
using System.Timers;
using Uchu.Core;

namespace Uchu.World.Behaviors
{
    public class BehaviorExecutioner
    {
        public readonly List<GameObject> Targets = new List<GameObject>();
        public Player Executioner;

        public Timer ActiveChainTimer { get; set; }

        public ElapsedEventHandler ActiveChainCallback { get; set; }

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