using System.Threading;
using System.Threading.Tasks;
using RakDotNet.IO;

namespace Uchu.World.Behaviors
{
    public class AlterChainDelay : Behavior
    {
        public override BehaviorTemplateId Id => BehaviorTemplateId.AlterChainDelay;

        public float Delay { get; private set; }

        public override async Task SerializeAsync(BitReader reader)
        {
            Delay = (await GetParameter(BehaviorId, "new_delay")).Value ?? 0;
            
            var source = new CancellationTokenSource();
            var token = source.Token;
            
            Executioner.ChainCancellationToken.Cancel();

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