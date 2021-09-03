namespace Solnet.Rpc.Types
{
    /// <summary>
    /// Represents the filter type for block data.
    /// </summary>
    public enum TransactionDetailsFilterType
    {
        /// <summary>
        /// Returns no transaction details.
        /// </summary>
        None,

        /// <summary>
        /// Returns only transaction signatures.
        /// </summary>
        Signatures,

        /// <summary>
        /// Returns full transaction details.
        /// </summary>
        Full
    }
}