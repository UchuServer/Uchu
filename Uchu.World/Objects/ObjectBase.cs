using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Uchu.World
{
    public abstract class ObjectBase
    {
        private (EventBase, Delegate)[] _listening = new (EventBase, Delegate)[0];

        protected Delegate Listen<T, T2>(EventBase<T, T2> @event, T @delegate) where T : Delegate where T2 : Delegate
        {
            @event.AddListener(@delegate);

            Array.Resize(ref _listening, _listening.Length + 1);

            _listening[^1] = (@event, @delegate);

            return @delegate;
        }

        protected Delegate Listen<T, T2>(EventBase<T, T2> @event, T2 @delegate) where T : Delegate where T2 : Delegate
        {
            @event.AddListener(@delegate);

            Array.Resize(ref _listening, _listening.Length + 1);

            _listening[^1] = (@event, @delegate);

            return @delegate;
        }
        
        protected void ClearListeners()
        {
            foreach (var (@event, listener) in _listening)
            {
                @event.RemoveListener(listener);
            }
        }

        protected void ReleaseListener(Delegate listener)
        {
            var list = new List<(EventBase, Delegate)>();

            foreach (var (@event, @delegate) in _listening)
            {
                if (@delegate == listener)
                {
                    @event.RemoveListener(@delegate);
                    
                    continue;
                }

                list.Add((@event, @delegate));
            }

            _listening = list.ToArray();
        }
    }
}