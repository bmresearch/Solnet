using Solnet.Programs.Governance.Enums;
using Solnet.Programs.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            /// 
            /// </summary>
            public const int Length = 31;

            /// <summary>
            /// 
            /// </summary>
            public const int VoteThresholdPercentageOffset = 0;

            /// <summary>
            /// 
            /// </summary>
            public const int MinCommunityTokensToCreateProposalOffset = 2;

            /// <summary>
            /// 
            /// </summary>
            public const int MinInstructionHoldUpTimeOffset = 10;

            /// <summary>
            /// 
            /// </summary>
            public const int MaxVotingTimeOffset = 14;

            /// <summary>
            /// 
            /// </summary>
            public const int VoteWeightSourceOffset = 18;

            /// <summary>
            /// 
            /// </summary>
            public const int ProposalCoolOffset = 19;

            /// <summary>
            /// 
            /// </summary>
            public const int MinCouncilTokensToCreateProposalOffset = 23;
        }

        /// <summary>
        /// 
        /// </summary>
        public VoteThresholdPercentage VoteThresholdPercentageType;

        /// <summary>
        /// 
        /// </summary>
        public byte VoteThresholdPercentage;

        /// <summary>
        /// 
        /// </summary>
        public ulong MinCommunityTokensToCreateProposal;

        /// <summary>
        /// 
        /// </summary>
        public uint MinInstructionHoldUpTime;

        /// <summary>
        /// 
        /// </summary>
        public uint MaxVotingTime;

        /// <summary>
        /// 
        /// </summary>
        public VoteWeightSource VoteWeightSource;

        /// <summary>
        /// 
        /// </summary>
        public uint ProposalCoolOffTime;

        /// <summary>
        /// 
        /// </summary>
        public ulong MinCouncilTokensToCreateProposal;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
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
