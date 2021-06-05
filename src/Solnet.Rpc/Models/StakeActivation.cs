namespace Solnet.Rpc.Models
{
    /// <summary>
    /// Represents the stake activation info.
    /// </summary>
    public class StakeActivationInfo
    {
        /// <summary>
        /// Stake active during the epoch.
        /// </summary>
        public ulong Active { get; set; }

        /// <summary>
        /// Stake inactive during the epoch.
        /// </summary>
        public ulong Inactive { get; set; }

        /// <summary>
        /// The stake account's activation state, one of "active", "inactive", "activating", "deactivating".
        /// </summary>
        public string State { get; set; }
    }
}