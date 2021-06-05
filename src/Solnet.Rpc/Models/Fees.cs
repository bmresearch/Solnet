namespace Solnet.Rpc.Models
{
    /// <summary>
    /// Represents the fee rate governor.
    /// </summary>
    public class FeeRateGovernor
    {
        /// <summary>
        /// Percentage of fees collected to be destroyed.
        /// </summary>
        public decimal BurnPercent { get; set; }

        /// <summary>
        /// Highest value LamportsPerSignature can attain for the next slot.
        /// </summary>
        public ulong MaxLamportsPerSignature { get; set; }

        /// <summary>
        /// Smallest value LamportsPerSignature can attain for the next slot.
        /// </summary>
        public ulong MinLamportsPerSignature { get; set; }

        /// <summary>
        /// Desired fee rate for the cluster.
        /// </summary>
        public ulong TargetLamportsPerSignature { get; set; }

        /// <summary>
        /// Desired signature rate for the cluster.
        /// </summary>
        public ulong TargetSignaturesPerSlot { get; set; }
    }

    /// <summary>
    /// Represents the fee rate governor info.
    /// </summary>
    public class FeeRateGovernorInfo
    {
        /// <summary>
        /// The fee rate governor.
        /// </summary>
        public FeeRateGovernor FeeRateGovernor { get; set; }
    }

    /// <summary>
    /// Represents information about the fees.
    /// </summary>
    public class FeesInfo
    {
        /// <summary>
        /// A block hash as base-58 encoded string.
        /// </summary>
        public string Blockhash { get; set; }

        /// <summary>
        /// The fee calculator for this block hash.
        /// </summary>
        public FeeCalculator FeeCalculator { get; set; }

        /// <summary>
        /// DEPRECATED - this value is inaccurate and should not be relied upon
        /// </summary>
        public ulong LastValidSlot { get; set; }

        /// <summary>
        /// Last block height at which a blockhash will be valid.
        /// </summary>
        public ulong LastValidBlockHeight { get; set; }
    }
}