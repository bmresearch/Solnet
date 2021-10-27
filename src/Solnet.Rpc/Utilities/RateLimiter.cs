using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Solnet.Rpc.Utilities
{
    /// <summary>
    /// A primitive blocking sliding time window rate limiter. Not thread-safe.
    /// </summary>
    public class RateLimiter : IRateLimiter
    {
        private int _hits;
        private int _duration_ms;
        private List<DateTime> _hit_list;

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
            _hit_list = new List<DateTime>();
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
            return DateTime.UtcNow >= resumeTime;
        }

        /// <summary>
        /// Pre-fire check - this will block if fire rates exceed defined limits until valid.
        /// </summary>
        public void Fire()
        {

            var checkTime = DateTime.UtcNow;
            var resumeTime = NextFireAllowed(checkTime);
            while (DateTime.UtcNow < resumeTime)
                Thread.Sleep(50);

            // record this trigger
            _hit_list.Add(DateTime.UtcNow);

        }

        /// <summary>
        /// When is a next fire allowed?
        /// </summary>
        /// <param name="checkTime"></param>
        /// <returns></returns>
        private DateTime NextFireAllowed(DateTime checkTime)
        {
            var cutoff = checkTime.AddMilliseconds(-_duration_ms);
            DateTime resumeTime = checkTime;
            
            // sliding window not set, allow everything through immediately
            if (_duration_ms == 0) return resumeTime;

            // check sliding time window
            var accumulated_hits = 0;
            var index = _hit_list.Count - 1;
            while (index >= 0)
            {
                if (_hit_list[index] >= cutoff)
                {
                    accumulated_hits++;
                    resumeTime = _hit_list[index].AddMilliseconds(_duration_ms);
                }
                else
                    break;
                index--;
            }

            // do we need a waste some time?
            if (accumulated_hits >= _hits)
                return resumeTime;
            else
                return checkTime;
        }

        /// <summary>
        /// Modify a rate limit
        /// </summary>
        /// <param name="seconds"></param>
        /// <returns></returns>
        public RateLimiter PerSeconds(int seconds)
        {
            return new RateLimiter(this._hits, seconds * 1000);
        }

        /// <summary>
        /// Create a new RateLimiter instance setting the number of hits.
        /// </summary>
        /// <param name="hits">Number of hits allowed per sliding time window.</param>
        /// <returns>An instance of the rate limiter with a sliding time window.</returns>
        public RateLimiter AllowHits(int hits)
        {
            return new RateLimiter(hits, this._duration_ms);
        }

    }

}
