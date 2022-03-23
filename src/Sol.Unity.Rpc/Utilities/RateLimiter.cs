using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sol.Unity.Rpc.Utilities
{
    /// <summary>
    /// A primitive blocking sliding time window rate limiter. Not thread-safe.
    /// </summary>
    public class RateLimiter : IRateLimiter
    {
        private int _hits;
        private int _duration_ms;
        private Queue<DateTime> _hit_list;

        /// <summary>
        /// Create a simple rate-limit tracking instance.
        /// This allows a number of hits within a window of duration_ms. 
        /// </summary>
        /// <param name="hits">Number of allowed hits</param>
        /// <param name="duration_ms">Duration of timespan in miliseconds</param>
        public RateLimiter(int hits, int duration_ms)
        {
            _hits = hits;
            _duration_ms = duration_ms;
            _hit_list = new Queue<DateTime>();
        }

        /// <summary>
        /// Create a simple rate-limit tracking instance.
        /// Configure using `PerSeconds` and `AllowHits` 
        /// </summary>
        public static RateLimiter Create()
        {
            return new RateLimiter(1, 0);
        }

        /// <summary>
        /// Would a fire request be allowed without delay?
        /// </summary>
        /// <returns></returns>
        public bool CanFire()
        {
            var checkTime = DateTime.UtcNow;
            var resumeTime = NextFireAllowed(checkTime);
            return checkTime >= resumeTime;
        }

        /// <summary>
        /// Pre-fire check - this will block if fire rates exceed defined limits until valid.
        /// </summary>
        public void Fire()
        {

            var checkTime = DateTime.UtcNow;
            var resumeTime = NextFireAllowed(checkTime);
            var snoozeMs = resumeTime.Subtract(checkTime).TotalMilliseconds;
            while (DateTime.UtcNow <= resumeTime)
                Thread.Sleep(50);

            // record this trigger
            if (_duration_ms > 0)
                _hit_list.Enqueue(DateTime.UtcNow);

        }

        /// <summary>
        /// When is a next fire allowed?
        /// </summary>
        /// <param name="checkTime"></param>
        /// <returns></returns>
        private DateTime NextFireAllowed(DateTime checkTime)
        {
            DateTime resumeTime = checkTime;

            // sliding window not set, allow everything through immediately
            if (_duration_ms == 0) return resumeTime;

            // empty queue
            if (_hit_list.Count == 0) return resumeTime;

            // drop any hits before the time window
            var cutOff = checkTime.AddMilliseconds(-_duration_ms);
            while (_hit_list.Count > 0 && _hit_list.Peek().Subtract(cutOff).TotalMilliseconds < 0)
                _hit_list.Dequeue();

            // are we left with more than we are allowed?
            // do we need to waste some time?
            if (_hit_list.Count >= _hits)
            {
                var oldestHit = _hit_list.Peek();
                return oldestHit.AddMilliseconds(_duration_ms);
            }
            else
                return checkTime;
        }

        /// <summary>
        /// Modify a rate limit
        /// </summary>
        /// <param name="seconds">Number of seconds</param>
        /// <returns></returns>
        public RateLimiter PerSeconds(int seconds)
        {
            this._duration_ms = seconds * 1000;
            return this;
        }

        /// <summary>
        /// Modify a rate limit
        /// </summary>
        /// <param name="ms">Number of milliseconds</param>
        /// <returns></returns>
        public RateLimiter PerMs(int ms)
        {
            this._duration_ms = ms;
            return this;
        }

        /// <summary>
        /// Create a new RateLimiter instance setting the number of hits.
        /// </summary>
        /// <param name="hits">Number of hits allowed per sliding time window.</param>
        /// <returns>An instance of the rate limiter with a sliding time window.</returns>
        public RateLimiter AllowHits(int hits)
        {
            this._hits = hits;
            return this;
        }

        /// <summary>
        /// Show info about this rate limiter
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (_hit_list.Count>0)
                return $"{_hit_list.Count}-{_hit_list.Peek().ToString("HH:mm:ss.fff")}";
            else
                return $"(empty)";
        }

    }

}
