using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sol.Unity.Rpc.Utilities
{
    /// <summary>
    /// Provides rate limiting behaviour for RPC interactions.
    /// </summary>
    public interface IRateLimiter
    {

        /// <summary>
        /// Fire or block until we can fire.
        /// </summary>
        void Fire();

        /// <summary>
        /// Would a fire method succeed?
        /// </summary>
        /// <returns></returns>
        bool CanFire();

     }
}
