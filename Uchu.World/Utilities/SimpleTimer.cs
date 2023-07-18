using System.Timers;

namespace Uchu.World
{
    public class SimpleTimer
    {
        private double _time;

        private Timer _timer;

        public Event OnElapsed { get; }

        /// <summary>
        /// A timer that counts down
        /// </summary>
        /// <param name="time">The time in ms that it should take to count down.</param>
        public SimpleTimer(double time)
        {
            _time = time;

            _timer = new Timer(time);
            _timer.Stop();
            _timer.AutoReset = false;
            _timer.Elapsed += Elapsed;

            OnElapsed = new Event();
        }

        /// <summary>
        /// Starts the timer with the predefined time
        /// </summary>
        public void Start()
        {
            _timer.Interval = _time;
            _timer.Start();
        }

        /// <summary>
        /// Stops the timer
        /// </summary>   
        public void Stop()
        {
            _timer.Stop();
        }

        public bool IsRunning => _timer.Enabled;

        private void Elapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            Stop();
            OnElapsed.Invoke();
        }
    }
}