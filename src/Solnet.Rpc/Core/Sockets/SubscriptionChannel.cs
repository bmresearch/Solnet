namespace Solnet.Rpc.Core.Sockets
{
    /// <summary>
    /// Represents the channel of a given subscription.
    /// </summary>
    public enum SubscriptionChannel
    {
        /// <summary>
        /// Account subscription (<c>accountSubscribe</c>). 
        /// </summary>
        TokenAccount,
        /// <summary>
        /// Account subscription (<c>accountSubscribe</c>). 
        /// </summary>
        Account,
        /// <summary>
        /// Logs subscription (<c>logsSubscribe</c>). 
        /// </summary>
        Logs,
        /// <summary>
        /// Program subscription (<c>programSubscribe</c>). 
        /// </summary>
        Program,
        /// <summary>
        /// Signature subscription (<c>signatureSubscribe</c>). 
        /// </summary>
        Signature,
        /// <summary>
        /// Slot subscription (<c>slotSubscribe</c>). 
        /// </summary>
        Slot,
        /// <summary>
        /// Root subscription (<c>rootSubscribe</c>). 
        /// </summary>
        Root
    }
}