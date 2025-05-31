using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solnet.Programs.StakePool.Models
{
    /// <summary>
    /// Storage list for all validator stake accounts in the pool.
    /// </summary>
    public class ValidatorList
    {
        /// <summary>
        /// Data outside of the validator list, separated out for cheaper deserializations.
        /// </summary>
        public ValidatorListHeader Header { get; set; }

        /// <summary>
        /// List of stake info for each validator in the pool.
        /// </summary>
        public List<ValidatorStakeInfo> Validators { get; set; }
    }
}
