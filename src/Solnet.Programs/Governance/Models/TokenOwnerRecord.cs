using Solnet.Programs.Governance.Enums;
using Solnet.Programs.Utilities;
using Solnet.Wallet;
using System;

namespace Solnet.Programs.Governance.Models
{
    /// <summary>
    /// Governance Token Owner Record
    /// Account PDA seeds: ['governance', realm, token_mint, token_owner ]
    /// </summary>
    public class TokenOwnerRecord : GovernanceProgramAccount
    {
        /// <summary>
        /// The layout of the <see cref="TokenOwnerRecord"/> structure.
        /// </summary>
        public static class ExtraLayout
        {
            /// <summary>
            /// The offset at which the realm public key begins.
            /// </summary>
            public const int RealmOffset = 1;

            /// <summary>
            /// The offset at which the governing token mint public key begins.
            /// </summary>
            public const int GoverningTokenMintOffset = 33;

            /// <summary>
            /// The offset at which the governing token owner public key begins.
            /// </summary>
            public const int GoverningTokenOwnerOffset = 65;

            /// <summary>
            ///  The offset at which the governing token deposit amount begins.
            /// </summary>
            public const int GoverningTokenDepositAmountOffset = 97;

            /// <summary>
            /// The offset at which the unrelinquished votes count value begins.
            /// </summary>
            public const int UnrelinquishedVotesCountOffset = 105;

            /// <summary>
            /// The offset at which the total votes count value begins.
            /// </summary>
            public const int TotalVotesCountOffset = 109;

            /// <summary>
            /// The offset at which the outstanding proposal count value begins.
            /// </summary>
            public const int OutstandingProposalCountOffset = 113;

            /// <summary>
            /// The offset at which the governance delegate public key begins.
            /// </summary>
            public const int GovernanceDelegateOffset = 121;
        }

        /// <summary>
        /// The Realm the TokenOwnerRecord belongs to
        /// </summary>
        public PublicKey Realm;

        /// <summary>
        /// Governing Token Mint the TokenOwnerRecord holds deposit for
        /// </summary>
        public PublicKey GoverningTokenMint;

        /// <summary>
        /// The owner (either single or multisig) of the deposited governing SPL Tokens
        /// This is who can authorize a withdrawal of the tokens
        /// </summary>
        public PublicKey GoverningTokenOwner;

        /// <summary>
        /// The amount of governing tokens deposited into the Realm
        /// This amount is the voter weight used when voting on proposals
        /// </summary>
        public ulong GoverningTokenDepositAmount;

        /// <summary>
        /// The number of votes cast by TokenOwner but not relinquished yet
        /// Every time a vote is cast this number is increased and it's always decreased when relinquishing a vote regardless of the vote state
        /// </summary>
        public uint UnrelinquishedVotesCount;

        /// <summary>
        /// The total number of votes cast by the TokenOwner
        /// If TokenOwner withdraws vote while voting is still in progress total_votes_count is decreased  and the vote doesn't count towards the total
        /// </summary>
        public uint TotalVotesCount;

        /// <summary>
        /// The number of outstanding proposals the TokenOwner currently owns
        /// The count is increased when TokenOwner creates a proposal
        /// and decreased  once it's either voted on (Succeeded or Defeated) or Cancelled
        /// By default it's restricted to 1 outstanding Proposal per token owner
        /// </summary>
        public byte OutstandingProposalCount;

        /// <summary>
        /// A single account that is allowed to operate governance with the deposited governing tokens
        /// It can be delegated to by the governing_token_owner or current governance_delegate
        /// </summary>
        public PublicKey GovernanceDelegate;

        /// <summary>
        /// Deserialize the data into the <see cref="TokenOwnerRecord"/> structure.
        /// </summary>
        /// <param name="data">The data to deserialize.</param>
        /// <returns>The <see cref="TokenOwnerRecord"/> structure.</returns>
        public static TokenOwnerRecord Deserialize(byte[] data)
        {
            ReadOnlySpan<byte> span = data.AsSpan();

            bool governanceDelegateExists = span.GetBool(ExtraLayout.GovernanceDelegateOffset);

            return new TokenOwnerRecord
            {
                AccountType = (GovernanceAccountType)Enum.Parse(typeof(GovernanceAccountType), span.GetU8(Layout.AccountTypeOffset).ToString()),
                Realm = span.GetPubKey(ExtraLayout.RealmOffset),
                GoverningTokenMint = span.GetPubKey(ExtraLayout.GoverningTokenMintOffset),
                GoverningTokenOwner = span.GetPubKey(ExtraLayout.GoverningTokenOwnerOffset),
                GoverningTokenDepositAmount = span.GetU64(ExtraLayout.GoverningTokenDepositAmountOffset),
                UnrelinquishedVotesCount = span.GetU32(ExtraLayout.UnrelinquishedVotesCountOffset),
                TotalVotesCount = span.GetU32(ExtraLayout.TotalVotesCountOffset),
                OutstandingProposalCount = span.GetU8(ExtraLayout.OutstandingProposalCountOffset),
                GovernanceDelegate = governanceDelegateExists ? span.GetPubKey(ExtraLayout.GovernanceDelegateOffset + 1) : null
            };
        }
    }
}
