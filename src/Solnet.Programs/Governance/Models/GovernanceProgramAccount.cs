using Solnet.Programs.Governance.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solnet.Programs.Governance.Models
{
    /// <summary>
    /// An account owned by the <see cref="GovernanceProgram"/>.
    /// </summary>
    public abstract class GovernanceProgramAccount
    {
        /// <summary>
        /// The account type.
        /// </summary>
        public GovernanceAccountType AccountType;
    }
}
