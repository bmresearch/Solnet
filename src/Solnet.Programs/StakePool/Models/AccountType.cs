using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solnet.Programs.StakePool.Models
{
    /// <summary>
    /// Enum representing the account type managed by the program.
    /// </summary>
    public enum AccountType : byte
    {
        /// <summary>
        /// If the account has not been initialized, the enum will be 0.
        /// </summary>
        Uninitialized = 0,

        /// <summary>
        /// Stake pool.
        /// </summary>
        StakePool = 1,

        /// <summary>
        /// Validator stake list.
        /// </summary>
        ValidatorList = 2
    }
}
