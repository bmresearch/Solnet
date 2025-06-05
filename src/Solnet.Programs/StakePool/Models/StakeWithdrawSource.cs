using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solnet.Programs.StakePool.Models
{
    /// <summary>
    /// Withdrawal type, figured out during process_withdraw_stake.
    /// </summary>
    internal enum StakeWithdrawSource : byte
    {
        /// <summary>
        /// Some of an active stake account, but not all.
        /// </summary>
        Active = 0,

        /// <summary>
        /// Some of a transient stake account.
        /// </summary>
        Transient = 1,

        /// <summary>
        /// Take a whole validator stake account.
        /// </summary>
        ValidatorRemoval = 2
    }
}
