using System;
using System.Linq;
using System.Threading.Tasks;
using Uchu.Core;

namespace Uchu.World
{
    [LoggerIgnore]
    public class AsyncEvent : EventBase<Func<Task>>
    {
        public void Invoke()
        {
            Task.Run(InvokeAsync);
        }
        
        public async Task InvokeAsync()
        {
            foreach (var action in Actions.ToArray())
            {
                await action.Invoke();
            }
        }
    }
    
    [LoggerIgnore]
    public class AsyncEvent<T> : EventBase<Func<T, Task>>
    {
        public void Invoke(T value)
        {
            Task.Run(async () => { await InvokeAsync(value); });
        }
        
        public async Task InvokeAsync(T value)
        {
            foreach (var action in Actions.ToArray())
            {
                await action.Invoke(value);
            }
        }
    }
    
    [LoggerIgnore]
    public class AsyncEvent<T, T2> : EventBase<Func<T, T2, Task>>
    {
        public void Invoke(T value, T2 value2)
        {
            Task.Run(async () => { await InvokeAsync(value, value2); });
        }
        
        public async Task InvokeAsync(T value, T2 value2)
        {
            foreach (var action in Actions.ToArray())
            {
                await action.Invoke(value, value2);
            }
        }
    }
    
    [LoggerIgnore]
    public class AsyncEvent<T, T2, T3> : EventBase<Func<T, T2, T3, Task>>
    {
        public void Invoke(T value, T2 value2, T3 value3)
        {
            Task.Run(async () => { await InvokeAsync(value, value2, value3); });
        }
        
        public async Task InvokeAsync(T value, T2 value2, T3 value3)
        {
            foreach (var action in Actions.ToArray())
            {
                await action.Invoke(value, value2, value3);
            }
        }
    }
    
    [LoggerIgnore]
    public class AsyncEvent<T, T2, T3, T4> : EventBase<Func<T, T2, T3, T4, Task>>
    {
        public void Invoke(T value, T2 value2, T3 value3, T4 value4)
        {
            Task.Run(async () => { await InvokeAsync(value, value2, value3, value4); });
        }
        
        public async Task InvokeAsync(T value, T2 value2, T3 value3, T4 value4)
        {
            foreach (var action in Actions.ToArray())
            {
                await action.Invoke(value, value2, value3, value4);
            }
        }
    }
}