using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;

namespace AchtungDieKurve.Game.Core
{
    /// <summary>
    /// Central registry of game timers so that they can be paused and resumed
    /// together. Resume only restarts the timers that ran when paused —
    /// re-starting elapsed one-shot timers would fire them again.
    /// </summary>
    public class TimerPool
    {
        private static List<Timer> _pool;
        private static List<Timer> _pausedTimers = new List<Timer>();

        private static List<Timer> Pool
        {
            get { return _pool ?? (_pool = new List<Timer>()); }
        }

        public static Timer CreateTimer()
        {
            var timer = new Timer();
            Pool.Add(timer);
            return timer;
        }

        public static void Pause(object sender, EventArgs eventArgs)
        {
            _pausedTimers = Pool.Where(timer => timer.Enabled).ToList();
            foreach (var timer in _pausedTimers)
            {
                timer.Stop();
            }
        }

        public static void UnPause(object sender, EventArgs eventArgs)
        {
            foreach (var timer in _pausedTimers)
            {
                timer.Start();
            }
            _pausedTimers.Clear();
        }
    }
}
