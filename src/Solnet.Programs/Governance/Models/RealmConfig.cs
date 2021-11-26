using Solnet.Programs.Governance.Enums;
using Solnet.Wallet;

namespace Solnet.Programs.Governance.Models
{

    /// <summary>
    /// Realm Config defining Realm parameters.
    /// </summary>
    public class RealmConfig
    {
        /// <summary>
        /// 
        /// </summary>
        public static class Layout
        {
            /// <summary>
            /// 
            /// </summary>
            public const int Length = 55;

            /// <summary>
            /// 
            /// </summary>
            public const int UseCommunityVoterWeightAddinOffset = 0;

            /// <summary>
            /// 
            /// </summary>
            public const int MinCommunityTokensOffset = 8;

            /// <summary>
            /// 
            /// </summary>
            public const int CommunityMintMaxVoteOffset = 16;

            /// <summary>
            /// 
            /// </summary>
            public const int CouncilMintOffset = 23;
        }

        /// <summary>
        /// Indicates whether an external addin program should be used to provide voters weights for the community mint.
        /// </summary>
        public bool UseCommunityVoterWeightAddin;

        /// <summary>
        /// Min number of community tokens required to create a governance.
        /// </summary>
        public ulong MinCommunityTokensToCreateGovernance;

        /// <summary>
        /// The source used for community mint max vote weight source.
        /// </summary>
        public MintMaxVoteWeightSource CommunityMintMaxVoteWeightSource;

        /// <summary>
        /// Community mint max vote weight.
        /// </summary>
        public ulong CommunityMintMaxVoteWeight;

        /// <summary>
        /// Optional council mint.
        /// </summary>
        public PublicKey CouncilMint;
    }
}
