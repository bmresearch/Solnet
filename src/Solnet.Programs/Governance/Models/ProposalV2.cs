using Solnet.Programs.Governance.Enums;
using Solnet.Programs.Utilities;
using System;
using System.Collections.Generic;

namespace Solnet.Programs.Governance.Models
{
    /// <summary>
    /// Governance Proposal v2
    /// </summary>
    public class ProposalV2 : Proposal
    {
        /// <summary>
        /// Additional layout info for <see cref="ProposalV2"/>.
        /// </summary>
        public static class AdditionalLayout
        {
            /// <summary>
            /// The offset at which the vote type enum begins.
            /// </summary>
            public const int VoteTypeOffset = 100;
        }

        /// <summary>
        /// Vote type
        /// </summary>
        public VoteType VoteType;

        /// <summary>
        /// The number of max options in case <c>VoteType</c> is <see cref="VoteType.MultiChoice"/>.
        /// </summary>
        public ushort MultiChoiceMaxOptions;

        /// <summary>
        /// Proposal options
        /// </summary>
        public List<ProposalOption> Options;

        /// <summary>
        /// The weight of the Proposal rejection votes
        /// If the proposal has no deny option then the weight is None
        /// Only proposals with the deny option can have executable instructions attached to them
        /// Without the deny option a proposal is only non executable survey
        /// </summary>
        public ulong DenyVoteWeight;

        /// <summary>
        /// Deserialize the data into the <see cref="Proposal"/> structure.
        /// </summary>
        /// <param name="data">The data to deserialize.</param>
        /// <returns>The <see cref="Proposal"/>.</returns>
        public static ProposalV2 Deserialize(byte[] data)
        {
            ReadOnlySpan<byte> span = data.AsSpan();

            int offset = AdditionalLayout.VoteTypeOffset;
            VoteType voteType = (VoteType)Enum.Parse(typeof(VoteType), span.GetU8(offset).ToString());
            ushort multiChoiceMaxOpts = 0;

            if(voteType == VoteType.MultiChoice)
            {
                multiChoiceMaxOpts = span.GetU16(offset + 1);
            }

            // adjust offset, increase by 3 in case vote type is multi choice and 1 in case it is single choice
            offset += voteType == VoteType.MultiChoice ? 3 : 1;
            List<ProposalOption> proposalOptions = new();
            int numProposalOptions = (int)span.GetU32(offset);
            offset += sizeof(uint);

            for(int i = 0; i<numProposalOptions; i++)
            {
                ProposalOption proposalOption = ProposalOption.Deserialize(span.Slice(offset));
                proposalOptions.Add(proposalOption);
                // adjust offset by taking into account the proposal option's label length and the remainder of the structure
                offset += proposalOption.LabelLength + ProposalOption.Layout.LengthWithoutLabel;
            }
            
            // the following data is predominantly optional so we'll have to check if it exists and adjust offsets accordingly
            bool denyVoteWeightExists = span.GetBool(offset);
            ulong denyVoteWeight = 0;
            offset += sizeof(byte);
            if (denyVoteWeightExists)
            {
                denyVoteWeight = span.GetU64(offset);
                offset += sizeof(ulong);
            }

            ulong draftAtTimestamp = span.GetU64(offset);
            offset += sizeof(ulong);

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

            return new ProposalV2
            {
                AccountType = (GovernanceAccountType)Enum.Parse(typeof(GovernanceAccountType), span.GetU8(Layout.AccountTypeOffset).ToString()),
                Governance = span.GetPubKey(ExtraLayout.GovernanceOffset),
                GoverningTokenMint = span.GetPubKey(ExtraLayout.GoverningTokenMintOffset),
                State = (ProposalState)Enum.Parse(typeof(ProposalState), span.GetU8(ExtraLayout.StateOffset).ToString()),
                TokenOwnerRecord = span.GetPubKey(ExtraLayout.TokenOwnerRecordOffset),
                SignatoriesCount = span.GetU8(ExtraLayout.SignatoriesOffset),
                SignatoriesSignedOffCount = span.GetU8(ExtraLayout.SignatoriesSignedOffOffset),
                VoteType = voteType,
                MultiChoiceMaxOptions = multiChoiceMaxOpts,
                Options = proposalOptions,
                DenyVoteWeight = denyVoteWeight,
                DraftAt = draftAtTimestamp,
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
