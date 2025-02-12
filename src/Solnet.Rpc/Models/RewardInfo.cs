using System.Text.Json.Serialization;

namespace Solnet.Rpc.Models
{
    /// <summary>
    /// Represents the reward information related to a given account.
    /// </summary>
    public class RewardInfo
    {
        /// <summary>
        /// The account pubkey as base58 encoded string.
        /// </summary>
        public string Pubkey { get; set; }
        /// <summary>
        /// Number of reward lamports credited or debited by the account.
        /// </summary>
        public long Lamports { get; set; }

        /// <summary>
        /// Account balance in lamports after the reward was applied.
        /// </summary>
        public ulong PostBalance { get; set; }

       
        public RewardType RewardType {  get; set; }
    }
  
    /// <summary>
    /// The type of the reward.
    /// </summary>
    public enum RewardType
    {
        /// <summary>
        /// Default value in case the returned value is undefined.
        /// </summary>
        Unknown,

        /// <summary>
        /// Fee reward.
        /// </summary>
        Fee,

        /// <summary>
        /// Rent reward.
        /// </summary>
        Rent,

        /// <summary>
        /// Voting reward.
        /// </summary>
        Voting,

        /// <summary>
        /// Staking reward.
        /// </summary>
        Staking
    }
}
