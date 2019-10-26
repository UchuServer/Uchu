using System;
using System.Threading;
using System.Threading.Tasks;

namespace Uchu.World
{
    public struct Delay
    {
        private readonly Func<Task> _callback;

        private readonly int _delay;

        private readonly CancellationTokenSource _cancellation;

        public Delay(Func<Task> callback, int delay)
        {
            _cancellation = new CancellationTokenSource();
            _callback = callback;
            _delay = delay;

            var cancellation = _cancellation;
            
            Task.Run(async () =>
            {
                await Task.Delay(delay);
                
                cancellation.Token.ThrowIfCancellationRequested();
                
                await callback.Invoke();
            }, cancellation.Token);
        }

        public Delay ChangeDelay(int delay)
        {
            _cancellation.Cancel();

            return new Delay(_callback, delay);
        }
    }
}