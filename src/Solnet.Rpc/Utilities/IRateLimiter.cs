using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Solnet.Rpc.Utilities
{
    /// <summary>
    /// Provides rate limiting behaviour for RPC interactions.
    /// </summary>
    public interface IRateLimiter
    {

        /// <summary>
        /// Fire or block until we can fire.
        /// </summary>
        [Obsolete]
        void Fire();

        /// <summary>
        /// Fire or block until we can fire.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task WaitFireAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Would a fire method succeed?
        /// </summary>
        /// <returns></returns>
        bool CanFire();

    }
}
