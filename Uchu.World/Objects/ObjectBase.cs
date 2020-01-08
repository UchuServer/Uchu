using System;

namespace Uchu.World
{
    public abstract class ObjectBase
    {
        private (EventBase, Delegate)[] _listening = new (EventBase, Delegate)[0];

        protected void Listen<T>(EventBase<T> @event, T @delegate) where T : Delegate
        {
            @event.AddListener(@delegate);

            Array.Resize(ref _listening, _listening.Length + 1);

            _listening[^1] = (@event, @delegate);
        }

        protected void ClearListeners()
        {
            foreach (var (@event, listener) in _listening)
            {
                @event.RemoveListener(listener);
            }
        }
    }
}