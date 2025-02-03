using Solnet.Programs.Governance.Enums;
using Solnet.Wallet;

namespace Solnet.Programs.Governance.Models
{
    /// <summary>
    /// This structure contains properties that are common to <see cref="ProposalV1"/> and <see cref="ProposalV2"/>. 
    /// </summary>
    public abstract class Proposal : GovernanceProgramAccount
    {
        /// <summary>
        /// The layout of the <see cref="Proposal"/> structure.
        /// </summary>
        public static class ExtraLayout
        {
            /// <summary>
            /// The offset at which the governance public key begins.
            /// </summary>
            public const int GovernanceOffset = 1;

            /// <summary>
            /// The offset at which the governing token mint public key begins.
            /// </summary>
            public const int GoverningTokenMintOffset = 33;

            /// <summary>
            /// The offset at which the state enum begins.
            /// </summary>
            public const int StateOffset = 65;

            /// <summary>
            /// The offset at which the token owner record public key begins.
            /// </summary>
            public const int TokenOwnerRecordOffset = 66;

            /// <summary>
            /// The offset at which the signatories value begins.
            /// </summary>
            public const int SignatoriesOffset = 98;

            /// <summary>
            /// The offset at which the signatories signed off value begins.
            /// </summary>
            public const int SignatoriesSignedOffOffset = 99;
        }

        /// <summary>
        /// Governance account the Proposal belongs to
        /// </summary>
        public PublicKey Governance;

        /// <summary>
        /// Indicates which Governing Token is used to vote on the Proposal
        /// Whether the general Community token owners or the Council tokens owners vote on this Proposal
        /// </summary>
        public PublicKey GoverningTokenMint;

        /// <summary>
        /// Current proposal state
        /// </summary>
        public ProposalState State;

        /// <summary>
        /// The TokenOwnerRecord representing the user who created and owns this Proposal
        /// </summary>
        public PublicKey TokenOwnerRecord;

        /// <summary>
        /// The number of signatories assigned to the Proposal
        /// </summary>
        public byte SignatoriesCount;

        /// <summary>
        /// The number of signatories who already signed
        /// </summary>
        public byte SignatoriesSignedOffCount;

        /// <summary>
        /// When the Proposal was created and entered Draft state
        /// </summary>
        public ulong DraftAt;

        /// <summary>
        /// When Signatories started signing off the Proposal
        /// </summary>
        public ulong SigningOffAt;

        /// <summary>
        /// When the Proposal began voting as UnixTimestamp
        /// </summary>
        public ulong VotingAt;

        /// <summary>
        /// When the Proposal began voting as Slot
        /// Note: The slot is not currently used but the exact slot is going to be required to support snapshot based vote weights
        /// </summary>
        public ulong VotingAtSlot;

        /// <summary>
        /// When the Proposal ended voting and entered either Succeeded or Defeated
        /// </summary>
        public ulong VotingCompletedAt;

        /// <summary>
        /// When the Proposal entered Executing state
        /// </summary>
        public ulong ExecutingAt;

        /// <summary>
        /// When the Proposal entered final state Completed or Cancelled and was closed
        /// </summary>
        public ulong ClosedAt;

        /// <summary>
        /// Instruction execution flag for ordered and transactional instructions
        /// Note: This field is not used in the current version
        /// </summary>
        public InstructionExecutionFlags InstructionExecutionFlags;

        /// <summary>
        /// The max vote weight for the Governing Token mint at the time Proposal was decided
        /// It's used to show correct vote results for historical proposals in cases when the mint supply or max weight source changed
        /// after vote was completed.
        /// </summary>
        public ulong MaxVoteWeight;

        /// <summary>
        /// The vote threshold percentage at the time Proposal was decided
        /// It's used to show correct vote results for historical proposals in cases when the threshold
        /// was changed for governance config after vote was completed.
        /// </summary>
        public VoteThresholdPercentage VoteThresholdPercentageType;

        /// <summary>
        /// The vote threshold percentage at the time Proposal was decided
        /// It's used to show correct vote results for historical proposals in cases when the threshold
        /// was changed for governance config after vote was completed.
        /// </summary>
        public byte VoteThresholdPercentage;

        /// <summary>
        /// Proposal name
        /// </summary>
        public string Name;

        /// <summary>
        /// Link to proposal's description
        /// </summary>
        public string DescriptionLink;
    }
}
