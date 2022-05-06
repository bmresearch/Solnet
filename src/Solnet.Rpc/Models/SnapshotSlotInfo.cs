namespace Solnet.Rpc.Models
{
    /// <summary>
    /// The highest snapshot slot info.
    /// </summary>
    public class SnapshotSlotInfo
    {
        /// <summary>
        /// The highest full snapshot slot.
        /// </summary>
        public ulong Full { get; set; }

        /// <summary>
        /// The highest incremental snapshot slot based on <see cref="Full"/>.
        /// </summary>
        public ulong? Incremental { get; set; }
    }
}
