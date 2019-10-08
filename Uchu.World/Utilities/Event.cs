using System;

namespace Uchu.World
{
    public abstract class EventBase<T> where T : Delegate
    {
        protected T[] Actions = new T[0];

        public void AddListener(T action)
        {
            Array.Resize(ref Actions, Actions.Length + 1);

            Actions[^1] = action;
        }

        public void Clear()
        {
            Actions = new T[0];
        }
    }
    
    public class Event : EventBase<Action>
    {
        public void Invoke()
        {
            foreach (var action in Actions)
            {
                action.Invoke();
            }
        }
    }
    
    public class Event<T> : EventBase<Action<T>>
    {
        public void Invoke(T value)
        {
            foreach (var action in Actions)
            {
                action.Invoke(value);
            }
        }
    }
    
    public class Event<T, T2> : EventBase<Action<T, T2>>
    {
        public void Invoke(T value, T2 value2)
        {
            foreach (var action in Actions)
            {
                action.Invoke(value, value2);
            }
        }
    }
    
    public class Event<T, T2, T3> : EventBase<Action<T, T2, T3>>
    {
        public void Invoke(T value, T2 value2, T3 value3)
        {
            foreach (var action in Actions)
            {
                action.Invoke(value, value2, value3);
            }
        }
    }
    
    public class Event<T, T2, T3, T4> : EventBase<Action<T, T2, T3, T4>>
    {
        public void Invoke(T value, T2 value2, T3 value3, T4 value4)
        {
            foreach (var action in Actions)
            {
                action.Invoke(value, value2, value3, value4);
            }
        }
    }
}