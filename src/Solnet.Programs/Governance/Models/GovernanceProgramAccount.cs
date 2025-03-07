using Solnet.Programs.Governance.Enums;

namespace Solnet.Programs.Governance.Models
{
    /// <summary>
    /// An account owned by the <see cref="GovernanceProgram"/>.
    /// </summary>
    public abstract class GovernanceProgramAccount
    {
        /// <summary>
        /// The layout of the <see cref="GovernanceProgramAccount"/> structure.
        /// </summary>
        public static class Layout
        {
            /// <summary>
            /// The offset at which the account type byte begins.
            /// </summary>
            public const int AccountTypeOffset = 0;
        }
        /// <summary>
        /// The account type.
        /// </summary>
        public GovernanceAccountType AccountType;
    }
}
