namespace Solnet.Rpc.Types
{
    /// <summary>
    /// Used to specify which block data to retrieve.
    /// </summary>
    public enum TransactionDetails
    {
        /// <summary>
        /// Retrieve the full block data.
        /// </summary>
        Full,
        /// <summary>
        /// Retrieve only signatures, leaving out detailed transaction data.
        /// </summary>
        Signatures,
        /// <summary>
        /// Retrieve only basic block data.
        /// </summary>
        None
    }
}