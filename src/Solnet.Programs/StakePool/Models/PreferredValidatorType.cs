using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solnet.Programs.StakePool.Models
{
    /// <summary>
    /// Represents the type of preferred validator in a stake pool.
    /// </summary>
    public enum PreferredValidatorType
    {
        /// <summary>
        /// Preferred validator for deposit operations.
        /// </summary>
        Deposit,

        /// <summary>
        /// Preferred validator for withdraw operations.
        /// </summary>
        Withdraw,
    }
}
