namespace Solnet.Rpc.Core.Sockets
{
    /// <summary>
    /// Represents the status of a subscription.
    /// </summary>
    public enum SubscriptionStatus
    {
        /// <summary>
        /// Waiting for the subscription message to be handled.
        /// </summary>
        WaitingResult,

        /// <summary>
        /// The subscription was terminated.
        /// </summary>
        Unsubscribed,

        /// <summary>
        /// The subscription is still alive.
        /// </summary>
        Subscribed,

        /// <summary>
        /// There was an error during subscription.
        /// </summary>
        ErrorSubscribing
    }
}