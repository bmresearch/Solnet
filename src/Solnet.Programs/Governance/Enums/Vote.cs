using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solnet.Programs.Governance.Enums
{
    /// <summary>
    /// User's vote
    /// </summary>
    public enum Vote : byte
    {
        /// <summary>
        /// Vote approving choices
        /// </summary>
        Approve = 0,

        /// <summary>
        /// Vote rejecting proposal
        /// </summary>
        Deny = 1
    }
}
