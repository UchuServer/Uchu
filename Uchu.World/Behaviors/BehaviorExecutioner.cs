using System;
using System.Collections.Generic;
using System.Threading;
using Uchu.Core;

namespace Uchu.World.Behaviors
{
    public class BehaviorExecutioner
    {
        public readonly List<GameObject> Targets = new List<GameObject>();
        public Player Executioner;

        public CancellationTokenSource ChainCancellationToken;

        public Action ChainAction;

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