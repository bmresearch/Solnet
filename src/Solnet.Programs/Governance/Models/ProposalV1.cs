using Solnet.Programs.Governance.Enums;
using Solnet.Programs.Utilities;
using System;

namespace Solnet.Programs.Governance.Models
{
    /// <summary>
    /// Governance Proposal v1 
    /// </summary>
    public class ProposalV1 : Proposal
    {
        /// <summary>
        /// Additional layout info for <see cref="ProposalV1"/>
        /// </summary>
        public static class AdditionalLayout
        {
            /// <summary>
            /// The offset at which the yes votes count value begins.
            /// </summary>
            public const int YesVotesCountOffset = 100;

            /// <summary>
            /// The offset at which the no votes count value begins.
            /// </summary>
            public const int NoVotesCountOffset = 108;

            /// <summary>
            /// The offset at which the instructions executed count value begins.
            /// </summary>
            public const int InstructionsExecutedCountOffset = 116;

            /// <summary>
            /// The offset at which the instructions count value begins.
            /// </summary>
            public const int InstructionsCountOffset = 118;

            /// <summary>
            /// The offset at which the instructions next index value begins.
            /// </summary>
            public const int InstructionsNextIndexOffset = 120;

            /// <summary>
            /// The offset at which the draft at timestamp value begins.
            /// </summary>
            public const int DraftAtOffset = 122;
        }

        /// <summary>
        /// The number of Yes votes
        /// </summary>
        public ulong YesVotesCount;

        /// <summary>
        /// The number of No votes
        /// </summary>
        public ulong NoVotesCount;

        /// <summary>
        /// The number of the instructions already executed
        /// </summary>
        public ushort InstructionsExecutedCount;

        /// <summary>
        /// The number of instructions included in the option
        /// </summary>
        public ushort InstructionsCount;

        /// <summary>
        /// The index of the the next instruction to be added
        /// </summary>
        public ushort InstructionsNextIndex;

        /// <summary>
        /// Deserialize the data into the <see cref="ProposalV1"/> structure.
        /// </summary>
        /// <param name="data">The data to deserialize.</param>
        /// <returns>The <see cref="ProposalV1"/>.</returns>
        public static ProposalV1 Deserialize(byte[] data)
        {
            ReadOnlySpan<byte> span = data.AsSpan();

            int offset = AdditionalLayout.DraftAtOffset + sizeof(ulong);
            bool signingOffAtTimestampExists = span.GetBool(offset);
            ulong signingOffAtTimestamp = 0;
            offset += sizeof(byte);
            if (signingOffAtTimestampExists)
            {
                signingOffAtTimestamp = span.GetU64(offset);
                offset += sizeof(ulong);
            }

            bool votingAtTimestampExists = span.GetBool(offset);
            ulong votingAtTimestamp = 0;
            offset += sizeof(byte);
            if (votingAtTimestampExists)
            {
                votingAtTimestamp = span.GetU64(offset);
                offset += sizeof(ulong);
            }

            bool votingAtSlotExists = span.GetBool(offset);
            ulong votingAtSlot = 0;
            offset += sizeof(byte);
            if (votingAtSlotExists)
            {
                votingAtSlot = span.GetU64(offset);
                offset += sizeof(ulong);
            }

            bool votingCompletedAtTimestampExists = span.GetBool(offset);
            ulong votingCompletedAtTimestamp = 0;
            offset += sizeof(byte);
            if (votingCompletedAtTimestampExists)
            {
                votingCompletedAtTimestamp = span.GetU64(offset);
                offset += sizeof(ulong);
            }

            bool executingAtTimestampExists = span.GetBool(offset);
            ulong executingAtTimestamp = 0;
            offset += sizeof(byte);
            if (executingAtTimestampExists)
            {
                executingAtTimestamp = span.GetU64(offset);
                offset += sizeof(ulong);
            }

            bool closedAtTimestampExists = span.GetBool(offset);
            ulong closedAtTimestamp = 0;
            offset += sizeof(byte);
            if (closedAtTimestampExists)
            {
                closedAtTimestamp = span.GetU64(offset);
                offset += sizeof(ulong);
            }

            InstructionExecutionFlags ixExecutionFlags = (InstructionExecutionFlags)Enum.Parse(typeof(InstructionExecutionFlags), span.GetU8(offset).ToString());
            offset += sizeof(byte);

            bool maxVoteWeightExists = span.GetBool(offset);
            ulong maxVoteWeight = 0;
            offset += sizeof(byte);
            if (maxVoteWeightExists)
            {
                maxVoteWeight = span.GetU64(offset);
                offset += sizeof(ulong);
            }

            bool voteThresholdPercentageTypeExists = span.GetBool(offset);
            VoteThresholdPercentage voteThresholdPercentageType = Enums.VoteThresholdPercentage.YesVote;
            byte voteThresholdPercentage = 0;
            offset += sizeof(byte);
            if (voteThresholdPercentageTypeExists)
            {
                voteThresholdPercentageType = (VoteThresholdPercentage)Enum.Parse(typeof(VoteThresholdPercentage), span.GetU8(offset).ToString());
                offset += sizeof(byte);
                voteThresholdPercentage = span.GetU8(offset);
                offset += sizeof(byte);
            }

            int nameLength = span.GetBorshString(offset, out string name);
            _ = span.GetBorshString(offset + nameLength, out string descriptionLink);

            return new ProposalV1
            {
                AccountType = (GovernanceAccountType)Enum.Parse(typeof(GovernanceAccountType), span.GetU8(Layout.AccountTypeOffset).ToString()),
                Governance = span.GetPubKey(ExtraLayout.GovernanceOffset),
                GoverningTokenMint = span.GetPubKey(ExtraLayout.GoverningTokenMintOffset),
                State = (ProposalState)Enum.Parse(typeof(ProposalState), span.GetU8(ExtraLayout.StateOffset).ToString()),
                TokenOwnerRecord = span.GetPubKey(ExtraLayout.TokenOwnerRecordOffset),
                SignatoriesCount = span.GetU8(ExtraLayout.SignatoriesOffset),
                SignatoriesSignedOffCount = span.GetU8(ExtraLayout.SignatoriesSignedOffOffset),
                YesVotesCount = span.GetU64(AdditionalLayout.YesVotesCountOffset),
                NoVotesCount = span.GetU64(AdditionalLayout.NoVotesCountOffset),
                InstructionsExecutedCount = span.GetU16(AdditionalLayout.InstructionsExecutedCountOffset),
                InstructionsCount = span.GetU16(AdditionalLayout.InstructionsCountOffset),
                InstructionsNextIndex = span.GetU16(AdditionalLayout.InstructionsNextIndexOffset),
                DraftAt = span.GetU64(AdditionalLayout.DraftAtOffset),
                SigningOffAt = signingOffAtTimestamp,
                VotingAt = votingAtTimestamp,
                VotingAtSlot = votingAtSlot,
                VotingCompletedAt = votingCompletedAtTimestamp,
                ExecutingAt = executingAtTimestamp,
                ClosedAt = closedAtTimestamp,
                InstructionExecutionFlags = ixExecutionFlags,
                MaxVoteWeight = maxVoteWeight,
                VoteThresholdPercentageType = voteThresholdPercentageType,
                VoteThresholdPercentage = voteThresholdPercentage,
                Name = name,
                DescriptionLink = descriptionLink
            };
        }
    }
}
