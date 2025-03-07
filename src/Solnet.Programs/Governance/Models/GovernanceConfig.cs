using Solnet.Programs.Governance.Enums;
using Solnet.Programs.Utilities;
using System;

namespace Solnet.Programs.Governance.Models
{
    /// <summary>
    /// Governance config
    /// </summary>
    public class GovernanceConfig
    {
        /// <summary>
        /// The layout of the <see cref="GovernanceConfig"/> structure.
        /// </summary>
        public static class Layout
        {
            /// <summary>
            /// The length of the <see cref="GovernanceConfig"/> structure.
            /// </summary>
            public const int Length = 31;

            /// <summary>
            /// The offset at which the vote threshold percentage enum begins.
            /// </summary>
            public const int VoteThresholdPercentageOffset = 0;

            /// <summary>
            /// The offset at which the minimum community tokens to create proposal value begins.
            /// </summary>
            public const int MinCommunityTokensToCreateProposalOffset = 2;

            /// <summary>
            /// The offset at which the minimum instruction hold up time value begins.
            /// </summary>
            public const int MinInstructionHoldUpTimeOffset = 10;

            /// <summary>
            /// The offset at which the maximum voting time value begins.
            /// </summary>
            public const int MaxVotingTimeOffset = 14;

            /// <summary>
            /// The offset at which the vote weight source enum begins.
            /// </summary>
            public const int VoteWeightSourceOffset = 18;

            /// <summary>
            /// The offset at which the proposal cool off value begins.
            /// </summary>
            public const int ProposalCoolOffset = 19;

            /// <summary>
            /// The offset at which the minimum council tokens to create proposal value begins.
            /// </summary>
            public const int MinCouncilTokensToCreateProposalOffset = 23;
        }

        /// <summary>
        /// The type of the vote threshold percentage.
        /// </summary>
        public VoteThresholdPercentage VoteThresholdPercentageType;

        /// <summary>
        /// The vote threshold percentage.
        /// </summary>
        public byte VoteThresholdPercentage;

        /// <summary>
        /// The minimum amount of community tokens needed to create a proposal.
        /// </summary>
        public ulong MinCommunityTokensToCreateProposal;

        /// <summary>
        /// The minimum instruction hold up time.
        /// </summary>
        public uint MinInstructionHoldUpTime;

        /// <summary>
        /// The maximum voting time.
        /// </summary>
        public uint MaxVotingTime;

        /// <summary>
        /// The vote weight source.
        /// </summary>
        public VoteWeightSource VoteWeightSource;

        /// <summary>
        /// The proposal cool off time.
        /// </summary>
        public uint ProposalCoolOffTime;

        /// <summary>
        /// The minimum amount of council tokens needed to create a proposal.
        /// </summary>
        public ulong MinCouncilTokensToCreateProposal;

        /// <summary>
        /// Deserialize the data into the <see cref="GovernanceConfig"/> structure.
        /// </summary>
        /// <param name="data">The data to deserialize.</param>
        /// <returns>The <see cref="GovernanceConfig"/> structure.</returns>
        public static GovernanceConfig Deserialize(ReadOnlySpan<byte> data)
        {
            return new GovernanceConfig
            {
                VoteThresholdPercentageType = (VoteThresholdPercentage)Enum.Parse(typeof(VoteThresholdPercentage), data.GetU8(Layout.VoteThresholdPercentageOffset).ToString()),
                VoteThresholdPercentage = data.GetU8(Layout.VoteThresholdPercentageOffset + 1),
                MinCommunityTokensToCreateProposal = data.GetU64(Layout.MinCommunityTokensToCreateProposalOffset),
                MinInstructionHoldUpTime = data.GetU32(Layout.MinInstructionHoldUpTimeOffset),
                MaxVotingTime = data.GetU32(Layout.MaxVotingTimeOffset),
                VoteWeightSource = (VoteWeightSource)Enum.Parse(typeof(VoteWeightSource), data.GetU8(Layout.VoteWeightSourceOffset).ToString()),
                ProposalCoolOffTime = data.GetU32(Layout.ProposalCoolOffset),
                MinCouncilTokensToCreateProposal = data.GetU64(Layout.MinCouncilTokensToCreateProposalOffset),
            };
        }
    }
}
