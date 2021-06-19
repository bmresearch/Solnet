using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solnet.Rpc.Types
{
    /// <summary>
    /// Represents the filter account type.
    /// </summary>
    public enum AccountFilterType
    {
        /// <summary>
        /// Circulating accounts.
        /// </summary>
        Circulating,
        /// <summary>
        /// Non circulating accounts.
        /// </summary>
        NonCirculating
    }
}
