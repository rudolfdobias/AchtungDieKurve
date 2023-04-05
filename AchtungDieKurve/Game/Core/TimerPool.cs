using System;
using System.Collections.Generic;
using System.Timers;

namespace AchtungDieKurve.Game.Core
{
    public class TimerPool
    {
        private static List<Timer> _pool;

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
            foreach (var timer in Pool)
            {
                timer.Stop();
            }
        }

        public static void UnPause(object sender, EventArgs eventArgs)
        {
            foreach (var timer in Pool)
            {
                timer.Start();
            }
        }
    }
}
