using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solnet.Programs.StakePool.Models
{
    /// <summary>
    /// Represents the status of a stake account in the stake pool.
    /// </summary>
    public enum StakeStatus : byte
    {
        /// <summary>
        /// Stake account is active, there may be a transient stake as well.
        /// </summary>
        Active = 0,

        /// <summary>
        /// Only transient stake account exists, when a transient stake is deactivating during validator removal.
        /// </summary>
        DeactivatingTransient = 1,

        /// <summary>
        /// No more validator stake accounts exist, entry ready for removal during UpdateStakePoolBalance.
        /// </summary>
        ReadyForRemoval = 2,

        /// <summary>
        /// Only the validator stake account is deactivating, no transient stake account exists.
        /// </summary>
        DeactivatingValidator = 3,

        /// <summary>
        /// Both the transient and validator stake account are deactivating, when a validator is removed with a transient stake active.
        /// </summary>
        DeactivatingAll = 4
    }
}
