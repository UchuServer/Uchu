using System;
using System.Linq;
using Uchu.Core;

namespace Uchu.World
{
    [LoggerIgnore]
    public abstract class EventBase
    {
        public abstract void Clear();

        public abstract void RemoveListener(Delegate @delegate);
    }
    
    [LoggerIgnore]
    public abstract class EventBase<T> : EventBase where T : Delegate
    {
        protected T[] Actions = new T[0];

        public bool Any => Actions.Length != default;
        
        internal void AddListener(T action)
        {
            Array.Resize(ref Actions, Actions.Length + 1);

            Actions[^1] = action;
        }

        public override void RemoveListener(Delegate @delegate)
        {
            if (!(@delegate is T action) || !Actions.Contains(action)) return;
            
            var array = new T[Actions.Length - 1];

            var index = 0;
            
            foreach (var element in Actions.ToArray())
            {
                if (element == action) continue;

                array[index++] = element;
            }

            Actions = array;
        }

        public override void Clear()
        {
            Actions = new T[0];
        }
    }
    
    [LoggerIgnore]
    public class Event : EventBase<Action>
    {
        public void Invoke()
        {
            foreach (var action in Actions.ToArray())
            {
                action.Invoke();
            }
        }

        public void SafeInvoke()
        {
            foreach (var action in Actions.ToArray())
            {
                try
                {
                    action.Invoke();
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
        }
    }
    
    [LoggerIgnore]
    public class Event<T> : EventBase<Action<T>>
    {
        public void Invoke(T value)
        {
            foreach (var action in Actions.ToArray())
            {
                action.Invoke(value);
            }
        }
    }
    
    [LoggerIgnore]
    public class Event<T, T2> : EventBase<Action<T, T2>>
    {
        public void Invoke(T value, T2 value2)
        {
            foreach (var action in Actions.ToArray())
            {
                action.Invoke(value, value2);
            }
        }
    }
    
    [LoggerIgnore]
    public class Event<T, T2, T3> : EventBase<Action<T, T2, T3>>
    {
        public void Invoke(T value, T2 value2, T3 value3)
        {
            foreach (var action in Actions.ToArray())
            {
                action.Invoke(value, value2, value3);
            }
        }
    }
    
    [LoggerIgnore]
    public class Event<T, T2, T3, T4> : EventBase<Action<T, T2, T3, T4>>
    {
        public void Invoke(T value, T2 value2, T3 value3, T4 value4)
        {
            foreach (var action in Actions.ToArray())
            {
                action.Invoke(value, value2, value3, value4);
            }
        }
    }
}