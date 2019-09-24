using System.Threading.Tasks;
using System.Timers;
using RakDotNet.IO;
using Uchu.Core;

namespace Uchu.World.Behaviors
{
    public class Chain : Behavior
    {
        public override BehaviorTemplateId Id => BehaviorTemplateId.Chain;

        public override async Task SerializeAsync(BitReader reader)
        {
            var index = reader.Read<uint>();

            var behaviorParam = await GetParameter(BehaviorId, $"behavior {index}");

            var delay = await GetParameter(BehaviorId, "chain_delay");

            Logger.Debug($"Chain: {delay.Value}s delay");

            var timer = new Timer
            {
                Interval = (delay.Value ?? 0) * 1000,
                AutoReset = false
            };

            Executioner.ActiveChainCallback = async (sender, args) =>
            {
                if (behaviorParam.Value != null) await StartBranch((int) behaviorParam.Value, reader);
            };

            timer.Elapsed += Executioner.ActiveChainCallback;

            Executioner.ActiveChainTimer = timer;

            timer.Start();
        }
    }
}