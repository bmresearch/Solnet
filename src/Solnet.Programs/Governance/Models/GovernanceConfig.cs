using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solnet.Programs.Governance.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class GovernanceConfig
    {

        /// <summary>
        /// 
        /// </summary>
        public VoteThresholdPercentage VoteThresholdPercentage;

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
    }
}
