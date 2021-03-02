using System;
using System.Linq;
using System.Threading.Tasks;
using Sentry;
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
    public abstract class EventBase<T, T2> : EventBase where T : Delegate where T2 : Delegate
    {
        protected Delegate[] Actions = new Delegate[0];

        internal void AddListener(T action)
        {
            Array.Resize(ref Actions, Actions.Length + 1);

            Actions[^1] = action;
        }

        internal void AddListener(T2 action)
        {
            Array.Resize(ref Actions, Actions.Length + 1);

            Actions[^1] = action;
        }

        public override void RemoveListener(Delegate @delegate)
        {
            if (!Actions.Contains(@delegate)) return;

            var array = new Delegate[Actions.Length - 1];

            var index = 0;

            foreach (var element in Actions.ToArray())
            {
                if (element == @delegate) continue;

                array[index++] = element;
            }

            Actions = array;
        }

        public override void Clear()
        {
            Actions = new Delegate[0];
        }
    }

    [LoggerIgnore]
    public class Event : EventBase<Action, Func<Task>>
    {
        public void Invoke()
        {
            var actions = Actions.ToArray();

            foreach (var action in actions.OfType<Action>())
            {
                try
                {
                    action.Invoke();
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                    SentrySdk.CaptureException(e);
                }
            }

            foreach (var func in actions.OfType<Func<Task>>())
            {
                try
                {
                    func.Invoke().Wait();
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                    SentrySdk.CaptureException(e);
                }
            }
        }

        public async Task InvokeAsync()
        {
            var actions = Actions.ToArray();

            foreach (var action in actions.OfType<Action>())
            {
                try
                {
                    action.Invoke();
                }
                catch (Exception e)
                {
                    SentrySdk.CaptureException(e);
                    Logger.Error(e);
                }
            }

            foreach (var func in actions.OfType<Func<Task>>())
            {
                try
                {
                    await func.Invoke();
                }
                catch (Exception e)
                {
                    SentrySdk.CaptureException(e);
                    Logger.Error(e);
                }
            }
        }
    }

    [LoggerIgnore]
    public class Event<T> : EventBase<Action<T>, Func<T, Task>>
    {
        public void Invoke(T value)
        {
            var actions = Actions.ToArray();

            foreach (var action in actions.OfType<Action<T>>())
            {
                try
                {
                    action.Invoke(value);
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                    SentrySdk.CaptureException(e);
                }
            }

            foreach (var func in actions.OfType<Func<T, Task>>())
            {
                try
                {
                    func.Invoke(value).Wait();
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                    SentrySdk.CaptureException(e);
                }
            }
        }

        public async Task InvokeAsync(T value)
        {
            var actions = Actions.ToArray();

            foreach (var action in actions.OfType<Action<T>>())
            {
                try
                {
                    action.Invoke(value);
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                    SentrySdk.CaptureException(e);
                }
            }

            foreach (var func in actions.OfType<Func<T, Task>>())
            {
                try
                {
                    await func.Invoke(value);
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                    SentrySdk.CaptureException(e);
                }
            }
        }
    }

    [LoggerIgnore]
    public class Event<T, T2> : EventBase<Action<T, T2>, Func<T, T2, Task>>
    {
        public void Invoke(T value, T2 value2)
        {
            var actions = Actions.ToArray();

            foreach (var action in actions.OfType<Action<T, T2>>())
            {
                try
                {
                    action.Invoke(value, value2);
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                    SentrySdk.CaptureException(e);
                }
            }

            foreach (var func in actions.OfType<Func<T, T2, Task>>())
            {
                try
                {
                    func.Invoke(value, value2).Wait();
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                    SentrySdk.CaptureException(e);
                }
            }
        }

        public async Task InvokeAsync(T value, T2 value2)
        {
            var actions = Actions.ToArray();

            foreach (var action in actions.OfType<Action<T, T2>>())
            {
                try
                {
                    action.Invoke(value, value2);
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                    SentrySdk.CaptureException(e);
                }
            }

            foreach (var func in actions.OfType<Func<T, T2, Task>>())
            {
                try
                {
                    await func.Invoke(value, value2);
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                    SentrySdk.CaptureException(e);
                }
            }
        }
    }

    [LoggerIgnore]
    public class Event<T, T2, T3> : EventBase<Action<T, T2, T3>, Func<T, T2, T3, Task>>
    {
        public void Invoke(T value, T2 value2, T3 value3)
        {
            var actions = Actions.ToArray();

            foreach (var action in actions.OfType<Action<T, T2, T3>>())
            {
                try
                {
                    action.Invoke(value, value2, value3);
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                    SentrySdk.CaptureException(e);
                }
            }

            foreach (var func in actions.OfType<Func<T, T2, T3, Task>>())
            {
                try
                {
                    func.Invoke(value, value2, value3).Wait();
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                    SentrySdk.CaptureException(e);
                }
            }
        }

        public async Task InvokeAsync(T value, T2 value2, T3 value3)
        {
            var actions = Actions.ToArray();

            foreach (var action in actions.OfType<Action<T, T2, T3>>())
            {
                try
                {
                    action.Invoke(value, value2, value3);
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                    SentrySdk.CaptureException(e);
                }
            }

            foreach (var func in actions.OfType<Func<T, T2, T3, Task>>())
            {
                try
                {
                    await func.Invoke(value, value2, value3);
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                    SentrySdk.CaptureException(e);
                }
            }
        }
    }
}