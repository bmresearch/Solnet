using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solnet.Programs.StakePool.Models
{
    /// <summary>
    /// Helper type to deserialize just the start of a ValidatorList.
    /// </summary>
    public class ValidatorListHeader
    {
        /// <summary>
        /// Account type, must be ValidatorList currently.
        /// </summary>
        public AccountType AccountType { get; set; }

        /// <summary>
        /// Maximum allowable number of validators.
        /// </summary>
        public uint MaxValidators { get; set; }

        /// <summary>
        /// Dynamically calculates the serialized length of the header.
        /// </summary>
        /// <returns>The length in bytes.</returns>
        public int GetSerializedLength()
        {
            // For example, assume:
            // AccountType: 1 byte
            // MaxValidators: 4 bytes
            return 1 + 4;
        }
    }
}
