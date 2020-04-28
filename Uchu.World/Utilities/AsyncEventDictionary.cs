using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Uchu.Core;

namespace Uchu.World
{
    [LoggerIgnore]
    public abstract class AsyncEventDictionaryBase<TKey, TAction> where TAction : Delegate
    {
        protected readonly Dictionary<TKey, TAction[]> ActionDictionary = new Dictionary<TKey, TAction[]>();
        
        public void AddListener(TKey key, TAction action)
        {
            if (!ActionDictionary.ContainsKey(key))
                ActionDictionary[key] = new TAction[0];

            var actions = ActionDictionary[key];
            
            Array.Resize(ref actions, actions.Length + 1);

            ActionDictionary[key] = actions;
            
            ActionDictionary[key][^1] = action;
        }
        
        public void Clear()
        {
            foreach (var key in ActionDictionary.Keys.ToArray())
            {
                ActionDictionary[key] = new TAction[0];
            }
        }
    }
    
    [LoggerIgnore]
    public class AsyncEventDictionary<TKey> : AsyncEventDictionaryBase<TKey, Func<Task>>
    {
        public async Task InvokeAsync(TKey key)
        {
            if (!ActionDictionary.TryGetValue(key, out var actions)) return;
            
            foreach (var action in actions.ToArray())
            {
                await action.Invoke();
            }
        }
    }
    
    [LoggerIgnore]
    public class AsyncEventDictionary<TKey, T> : AsyncEventDictionaryBase<TKey, Func<T, Task>>
    {
        public async Task InvokeAsync(TKey key, T value)
        {
            if (!ActionDictionary.TryGetValue(key, out var actions)) return;
            
            foreach (var action in actions.ToArray())
            {
                await action.Invoke(value);
            }
        }
    }
    
    [LoggerIgnore]
    public class AsyncEventDictionary<TKey, T, T2> : AsyncEventDictionaryBase<TKey, Func<T, T2, Task>>
    {
        public async Task InvokeAsync(TKey key, T value, T2 value2)
        {
            if (!ActionDictionary.TryGetValue(key, out var actions)) return;
            
            foreach (var action in actions.ToArray())
            {
                await action.Invoke(value, value2);
            }
        }
    }
    
    [LoggerIgnore]
    public class AsyncEventDictionary<TKey, T, T2, T3> : AsyncEventDictionaryBase<TKey, Func<T, T2, T3, Task>>
    {
        public async Task InvokeAsync(TKey key, T value, T2 value2, T3 value3)
        {
            if (!ActionDictionary.TryGetValue(key, out var actions)) return;
            
            foreach (var action in actions.ToArray())
            {
                await action.Invoke(value, value2, value3);
            }
        }
    }
    
    [LoggerIgnore]
    public class AsyncEventDictionary<TKey, T, T2, T3, T4> : AsyncEventDictionaryBase<TKey, Func<T, T2, T3, T4, Task>>
    {
        public async Task InvokeAsync(TKey key, T value, T2 value2, T3 value3, T4 value4)
        {
            if (!ActionDictionary.TryGetValue(key, out var actions)) return;
            
            foreach (var action in actions.ToArray())
            {
                await action.Invoke(value, value2, value3, value4);
            }
        }
    }
}