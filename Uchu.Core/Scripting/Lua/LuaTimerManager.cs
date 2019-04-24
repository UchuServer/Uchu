using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Timers;

namespace Uchu.Core.Scripting.Lua
{
    // GAMEOBJ:GetTimer()
    public class LuaTimerManager
    {
        private readonly Dictionary<LuaGameObject, Dictionary<string, Timer>> _timers;

        // onTimerDone(object, *) : void
        public event Action<LuaGameObject, dynamic> TimerDone;

        public LuaTimerManager()
        {
            _timers = new Dictionary<LuaGameObject, Dictionary<string, Timer>>();
        }

        // GAMEOBJ:GetTimer():AddTimerWithCancel(double, string, object) : void
        public void AddTimerWithCancel(double time, string name, LuaGameObject obj)
        {
            var timer = new Timer
            {
                AutoReset = false,
                Interval = time * 1000
            };

            timer.Elapsed += (sender, args) =>
            {
                dynamic res = new ExpandoObject();

                res.name = name;

                TimerDone?.Invoke(obj, res);

                _timers[obj].Remove(name);
            };

            if (!_timers.ContainsKey(obj))
                _timers[obj] = new Dictionary<string, Timer>();

            _timers[obj][name] = timer;

            timer.Start();
        }

        // GAMEOBJ:GetTimer():CancelAllTimers(object) : void
        public void CancelAllTimers(LuaGameObject obj)
        {
            var timers = _timers[obj];

            for (var i = 0; i < timers.Count; i++)
            {
                var key = timers.Keys.ElementAt(i);
                var timer = timers[key];

                timer.Stop();
                timer.Dispose();

                timers.Remove(key);
            }
        }
    }
}