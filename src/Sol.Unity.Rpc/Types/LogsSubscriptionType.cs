namespace Sol.Unity.Rpc.Types
{
    /// <summary>
    /// Enum with the possible vote selection parameter for the log subscription method.
    /// </summary>
    public enum LogsSubscriptionType
    {
        /// <summary>
        /// Subscribes to All logs.
        /// </summary>
        All,

        /// <summary>
        /// Subscribes to All logs including votes.
        /// </summary>
        AllWithVotes
    }
}