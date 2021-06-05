using System.Text.Json.Serialization;

namespace Solnet.Rpc.Models
{
    /// <summary>
    /// Represents the account info and associated stake for all the voting accounts in the current bank.
    /// </summary>
    public class VoteAccount
    {
        /// <summary>
        /// The root slot for this vote account.
        /// </summary>
        public ulong RootSlot { get; set; }

        /// <summary>
        /// The vote account address, as a base-58 encoded string.
        /// </summary>
        [JsonPropertyName("votePubkey")]
        public string VotePublicKey { get; set; }

        /// <summary>
        /// The validator identity, as a base-58 encoded string.
        /// </summary>
        [JsonPropertyName("nodePubkey")]
        public string NodePublicKey { get; set; }

        /// <summary>
        /// The stake, in lamports, delegated to this vote account and active in this epoch.
        /// </summary>
        public ulong ActivatedStake { get; set; }

        /// <summary>
        /// Whether the vote account is staked for this epoch.
        /// </summary>
        public bool EpochVoteAccount { get; set; }

        /// <summary>
        /// Percentage of rewards payout owed to the vote account.
        /// </summary>
        public decimal Commission { get; set; }

        /// <summary>
        /// Most recent slot voted on by this vote account.
        /// </summary>
        public ulong LastVote { get; set; }

        /// <summary>
        /// History of how many credits earned by the end of the each epoch.
        /// <remarks>
        /// Each array contains [epoch, credits, previousCredits];
        /// </remarks>
        /// </summary>
        public ulong[][] EpochCredits { get; set; }
    }

    /// <summary>
    /// Represents the vote accounts.
    /// </summary>
    public class VoteAccounts
    {
        /// <summary>
        /// Current vote accounts.
        /// </summary>
        public VoteAccount[] Current { get; set; }

        /// <summary>
        /// Delinquent vote accounts.
        /// </summary>
        public VoteAccount[] Delinquent { get; set; }
    }
}