using System;
using System.Threading;

namespace PointBlank
{
    public class TimerState
    {
        public Timer Timer = null;
        public DateTime EndDate = new DateTime();
        private object sync = new object();
        public void StartJob(int period, TimerCallback callback)
        {
            lock (sync)
            {
                Timer = new Timer(callback, this, period, -1);
                EndDate = DateTime.Now.AddMilliseconds(period);
            }
        }
        public void StartTimer(TimeSpan period, TimerCallback callback)
        {
            lock (sync)
            {
                Timer = new Timer(callback, this, period, TimeSpan.Zero);
            }
        }

        /// <summary>
        /// Retorna a quantia de segundos restantes para executar o trabalho.
        /// </summary>
        /// <returns></returns>
        public int GetTimeLeft()
        {
            if (Timer == null)
            {
                return 0;
            }
            TimeSpan span = EndDate - DateTime.Now;
            int seconds = (int)span.TotalSeconds;
            return seconds < 0 ? 0 : seconds;
        }
    }
}