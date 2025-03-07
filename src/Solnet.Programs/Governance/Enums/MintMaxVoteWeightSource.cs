namespace Solnet.Programs.Governance.Enums
{
    /// <summary>
    /// The source of max vote weight used for voting
    /// Values below 100% mint supply can be used when the governing token is fully minted but not distributed yet
    /// </summary>
    public enum MintMaxVoteWeightSource : byte
    {
        /// <summary>
        /// Fraction (10^10 precision) of the governing mint supply is used as max vote weight
        /// The default is 100% (10^10) to use all available mint supply for voting
        /// </summary>
        SupplyFraction = 0,

        /// <summary>
        /// Absolute value, irrelevant of the actual mint supply, is used as max vote weight
        /// Note: this option is not implemented in the current version
        /// </summary>
        Absolute = 0,
    }
}
