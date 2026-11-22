namespace Solnet.Rpc.Types
{
    /// <summary>
    /// The encodings used for binary data to interact with the Solana nodes.
    /// </summary>
    public enum BinaryEncoding
    {
        /// <summary>
        /// Request json parsed data, when a parser is available.
        /// </summary>
        JsonParsed,
        /// <summary>
        /// Base64 encoding.
        /// </summary>
        Base64,
        /// <summary>
        /// Base64+Zstd encoding.
        /// </summary>
        Base64Zstd,
        /// <summary>
        /// Base58 encoding, for account data within the node's size limit.
        /// </summary>
        Base58
    }
}
