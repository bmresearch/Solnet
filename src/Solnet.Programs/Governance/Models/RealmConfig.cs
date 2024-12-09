using Solnet.Programs.Governance.Enums;
using Solnet.Programs.Utilities;
using Solnet.Wallet;
using System;

namespace Solnet.Programs.Governance.Models
{

    /// <summary>
    /// Realm Config defining Realm parameters.
    /// </summary>
    public class RealmConfig
    {
        /// <summary>
        /// The layout of the <see cref="RealmConfig"/> structure.
        /// </summary>
        public static class Layout
        {
            /// <summary>
            /// The length of the <see cref="RealmConfig"/> structure.
            /// </summary>
            public const int Length = 58;

            /// <summary>
            /// The offset at which the use community voter weight addin value begins.
            /// </summary>
            public const int UseCommunityVoterWeightAddinOffset = 0;

            /// <summary>
            /// The offset at which the min community tokens to create governance value begins.
            /// </summary>
            public const int MinCommunityTokensToCreateGovernanceOffset = 8;

            /// <summary>
            /// The offset at which the community mint max vote weight source enum begins.
            /// </summary>
            public const int CommunityMintMaxVoteWeightSourceOffset = 16;

            /// <summary>
            /// The offset at which the community mint max vote weight value begins.
            /// </summary>
            public const int CommunityMintMaxVoteWeightOffset = 17;

            /// <summary>
            /// The offset at which the council mint public key begins.
            /// </summary>
            public const int CouncilMintOffset = 25;
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

        /// <summary>
        /// Deserialize the data into the <see cref="RealmConfig"/> structure.
        /// </summary>
        /// <param name="data">The data to deserialize.</param>
        /// <returns>The <see cref="RealmConfig"/> structure.</returns>
        public static RealmConfig Deserialize(ReadOnlySpan<byte> data)
        {
            if (data.Length != Layout.Length)
                throw new Exception("data length is invalid");

            bool councilMintExists = data.GetBool(Layout.CouncilMintOffset);

            return new RealmConfig
            {
                UseCommunityVoterWeightAddin = data.GetBool(Layout.UseCommunityVoterWeightAddinOffset),
                MinCommunityTokensToCreateGovernance = data.GetU64(Layout.MinCommunityTokensToCreateGovernanceOffset),
                CommunityMintMaxVoteWeightSource = (MintMaxVoteWeightSource)Enum.Parse(typeof(MintMaxVoteWeightSource), data.GetU8(Layout.CommunityMintMaxVoteWeightSourceOffset).ToString()),
                CommunityMintMaxVoteWeight = data.GetU64(Layout.CommunityMintMaxVoteWeightOffset),
                CouncilMint = councilMintExists ? data.GetPubKey(Layout.CouncilMintOffset + 1) : null,
            };
        }
    }
}
