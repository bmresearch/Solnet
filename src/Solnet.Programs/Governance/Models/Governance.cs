using Solnet.Programs.Governance.Enums;
using Solnet.Programs.Utilities;
using Solnet.Wallet;
using System;

namespace Solnet.Programs.Governance.Models
{
    /// <summary>
    /// The governance account.
    /// </summary>
    public class GovernanceAccount : GovernanceProgramAccount
    {
        /// <summary>
        /// The layout of the <see cref="GovernanceAccount"/> structure.
        /// </summary>
        public static class ExtraLayout
        {
            /// <summary>
            /// The length of the <see cref="GovernanceAccount"/> structure.
            /// </summary>
            public const int Length = 101;

            /// <summary>
            /// The offset at which the realm public key begins.
            /// </summary>
            public const int RealmOffset = 1;

            /// <summary>
            /// The offset at which the governed account public key begins.
            /// </summary>
            public const int GovernedAccountOffset = 33;

            /// <summary>
            /// The offset at which the proposals count value begins.
            /// </summary>
            public const int ProposalsCountOffset = 65;

            /// <summary>
            /// The offset at which the <see cref="GovernanceConfig"/> structure begins.
            /// </summary>
            public const int ConfigOffset = 69;
        }

        /// <summary>
        /// The realm this governance belongs to.
        /// </summary>
        public PublicKey Realm;

        /// <summary>
        /// The governed account.
        /// </summary>
        public PublicKey GovernedAccount;

        /// <summary>
        /// The amount of proposals.
        /// </summary>
        public uint ProposalsCount;

        /// <summary>
        /// The governance config.
        /// </summary>
        public GovernanceConfig Config;

        /// <summary>
        /// Deserialize the data into the <see cref="GovernanceAccount"/> structure.
        /// </summary>
        /// <param name="data">The data to deserialize.</param>
        /// <returns>The <see cref="GovernanceAccount"/> structure.</returns>
        public static GovernanceAccount Deserialize(byte[] data)
        {
            ReadOnlySpan<byte> span = data.AsSpan();

            return new GovernanceAccount
            {
                AccountType = (GovernanceAccountType)Enum.Parse(typeof(GovernanceAccountType), span.GetU8(Layout.AccountTypeOffset).ToString()),
                Realm = span.GetPubKey(ExtraLayout.RealmOffset),
                GovernedAccount = span.GetPubKey(ExtraLayout.GovernedAccountOffset),
                ProposalsCount = span.GetU32(ExtraLayout.ProposalsCountOffset),
                Config = GovernanceConfig.Deserialize(span.GetSpan(ExtraLayout.ConfigOffset, GovernanceConfig.Layout.Length))
            };
        }
    }
}
