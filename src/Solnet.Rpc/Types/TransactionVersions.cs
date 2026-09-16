using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solnet.Rpc.Types
{
    /// <summary>
    /// Represents the different versions of transactions in Solana.
    /// </summary>
    public enum TransactionVersion
    {
        /// <summary>
        /// Represents the legacy transaction version.
        /// </summary>
        Legacy = -1,
        /// <summary>
        /// Represents the first version of transactions.
        /// </summary>
        V0 = 0,
        /// <summary>
        /// Represents the second version of transactions.
        /// </summary>
        V1 = 1
    }
}
