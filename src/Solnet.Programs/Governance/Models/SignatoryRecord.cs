using Solnet.Programs.Governance.Enums;
using Solnet.Programs.Utilities;
using Solnet.Wallet;
using System;

namespace Solnet.Programs.Governance.Models
{
    /// <summary>
    /// Account PDA seeds: ['governance', proposal, signatory]
    /// </summary>
    public class SignatoryRecord : GovernanceProgramAccount
    {
        /// <summary>
        /// The layout of the <see cref="SignatoryRecord"/> structure.
        /// </summary>
        public static class ExtraLayout
        {
            /// <summary>
            /// The length of the <see cref="SignatoryRecord"/> structure.
            /// </summary>
            public const int Length = 66;

            /// <summary>
            /// The offset at which the proposal public key begins.
            /// </summary>
            public const int ProposalOffset = 1;

            /// <summary>
            /// The offset at which the signatory public key begins.
            /// </summary>
            public const int SignatoryOffset = 33;

            /// <summary>
            /// The offset at which the signed off value begins.
            /// </summary>
            public const int SignedOffOffset = 65;
        }

        /// <summary>
        /// Proposal the signatory is assigned for
        /// </summary>
        public PublicKey Proposal;

        /// <summary>
        /// The account of the signatory who can sign off the proposal
        /// </summary>
        public PublicKey Signatory;

        /// <summary>
        /// Indicates whether the signatory signed off the proposal
        /// </summary>
        public bool SignedOff;

        /// <summary>
        /// Deserialize the data into the <see cref="RealmConfig"/> structure.
        /// </summary>
        /// <param name="data">The data to deserialize.</param>
        /// <returns>The <see cref="RealmConfig"/> structure.</returns>
        public static SignatoryRecord Deserialize(ReadOnlySpan<byte> data)
        {
            if (data.Length != ExtraLayout.Length)
                throw new Exception("data length is invalid");

            return new SignatoryRecord
            {
                AccountType = (GovernanceAccountType)Enum.Parse(typeof(GovernanceAccountType), data.GetU8(Layout.AccountTypeOffset).ToString()),
                Proposal = data.GetPubKey(ExtraLayout.ProposalOffset),
                Signatory = data.GetPubKey(ExtraLayout.SignatoryOffset),
                SignedOff = data.GetBool(ExtraLayout.SignedOffOffset)
            };
        }
    }
}
