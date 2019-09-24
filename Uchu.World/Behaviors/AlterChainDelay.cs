using System.Threading.Tasks;
using System.Timers;
using RakDotNet.IO;
using Uchu.Core;

namespace Uchu.World.Behaviors
{
    public class AlterChainDelay : Behavior
    {
        public override BehaviorTemplateId Id => BehaviorTemplateId.AlterChainDelay;

        public override async Task SerializeAsync(BitReader reader)
        {
            var time = await GetParameter(BehaviorId, "new_delay");

            // TODO: Fix

            /*
            var timer = new Timer
            {
                Interval = (time.Value ?? 0) * 1000,
                AutoReset = false
            };

            timer.Elapsed += Executioner.ActiveChainCallback;

            Executioner.ActiveChainTimer.Enabled = false;
            Executioner.ActiveChainTimer.Dispose();

            Executioner.ActiveChainTimer = timer;

            timer.Start();

            Logger.Debug($"Changed chain timer to {timer.Interval}");
            */
        }
    }
}