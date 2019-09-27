using System.Threading;
using System.Threading.Tasks;
using RakDotNet.IO;
using Uchu.Core;

namespace Uchu.World.Behaviors
{
    public class Chain : Behavior
    {
        public override BehaviorTemplateId Id => BehaviorTemplateId.Chain;
        
        public float Delay { get; private set; }

        public override async Task SerializeAsync(BitReader reader)
        {
            var index = reader.Read<uint>();

            var behaviorParam = await GetParameter(BehaviorId, $"behavior {index}");

            Delay = (await GetParameter(BehaviorId, "chain_delay")).Value ?? 0;

            Logger.Debug($"Chain: {Delay}s delay");

            var source = new CancellationTokenSource();
            var token = source.Token;

            Executioner.ChainAction = async () =>
            {
                if (behaviorParam.Value != null) await StartBranch((int) behaviorParam.Value, reader);
            };
            
            Task.Run(async () =>
            {
                await Task.Delay((int) (Delay * 1000), token);
                
                if (token.IsCancellationRequested) return;

                Executioner.ChainAction();

                Executioner.ChainAction = null;
            }, token);

            Executioner.ChainCancellationToken = source;
        }
    }
}