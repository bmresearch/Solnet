namespace Solnet.Rpc.Models
{
    /// <summary>
    /// Represents inflation governor information.
    /// </summary>
    public class InflationGovernor
    {
        /// <summary>
        /// The initial inflation percentage from time zero.
        /// </summary>
        public decimal Initial { get; set; }

        /// <summary>
        /// The terminal inflation percentage.
        /// </summary>
        public decimal Terminal { get; set; }

        /// <summary>
        /// The rate per year at which inflation is lowered.
        /// <remarks>Rate reduction is derived using the target slot time as per genesis config.</remarks>
        /// </summary>
        public decimal Taper { get; set; }

        /// <summary>
        /// Percentage of total inflation allocated to the foundation.
        /// </summary>
        public decimal Foundation { get; set; }

        /// <summary>
        /// Duration of foundation pool inflation in years.
        /// </summary>
        public decimal FoundationTerm { get; set; }
    }

    /// <summary>
    /// Represents the inflation rate information.
    /// </summary>
    public class InflationRate
    {
        /// <summary>
        /// Epoch for which these values are valid.
        /// </summary>
        public decimal Epoch { get; set; }

        /// <summary>
        /// Percentage of total inflation allocated to the foundation.
        /// </summary>
        public decimal Foundation { get; set; }

        /// <summary>
        /// Percentage of total inflation.
        /// </summary>
        public decimal Total { get; set; }

        /// <summary>
        /// Percentage of total inflation allocated to validators.
        /// </summary>
        public decimal Validator { get; set; }
    }

    /// <summary>
    /// Represents the inflation reward for a certain address.
    /// </summary>
    public class InflationReward
    {
        /// <summary>
        /// Epoch for which a reward occurred.
        /// </summary>
        public ulong Epoch { get; set; }

        /// <summary>
        /// The slot in which the rewards are effective.
        /// </summary>
        public ulong EffectiveSlot { get; set; }

        /// <summary>
        /// The reward amount in lamports.
        /// </summary>
        public ulong Amount { get; set; }

        /// <summary>
        /// Post balance of the account in lamports.
        /// </summary>
        public ulong PostBalance { get; set; }
    }
}