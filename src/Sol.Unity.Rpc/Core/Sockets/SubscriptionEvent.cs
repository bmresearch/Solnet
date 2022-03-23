using System;

namespace Sol.Unity.Rpc.Core.Sockets
{
    /// <summary>
    /// Represents an event related to a given subscription.
    /// </summary>
    public class SubscriptionEvent : EventArgs
    {
        /// <summary>
        /// The new status of the subscription.
        /// </summary>
        public SubscriptionStatus Status { get; }

        /// <summary>
        /// A possible error mssage for this event.
        /// </summary>
        public string Error { get; }

        /// <summary>
        /// A possible error code for this event.
        /// </summary>
        public string Code { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="status">The new status.</param>
        /// <param name="error">The possible error message.</param>
        /// <param name="code">The possible error code.</param>
        public SubscriptionEvent(SubscriptionStatus status, string error = default, string code = default)
        {
            Status = status;
            Error = error;
            Code = code;
        }
    }
}