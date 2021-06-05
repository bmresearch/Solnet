namespace Solnet.Rpc.Models
{
    /// <summary>
    /// Represents information about the current epoch.
    /// </summary>
    public class EpochInfo
    {
        /// <summary>
        /// The current slot.
        /// </summary>
        public ulong AbsoluteSlot { get; set; }

        /// <summary>
        /// The current block height.
        /// </summary>
        public ulong BlockHeight { get; set; }

        /// <summary>
        /// The current epoch.
        /// </summary>
        public ulong Epoch { get; set; }

        /// <summary>
        /// The current slot relative to the start of the current epoch.
        /// </summary>
        public ulong SlotIndex { get; set; }

        /// <summary>
        /// The number of slots in this epoch
        /// </summary>
        public ulong SlotsInEpoch { get; set; }
    }

    /// <summary>
    /// Represents information about the epoch schedule.
    /// </summary>
    public class EpochScheduleInfo
    {
        /// <summary>
        /// The maximum number of slots in each epoch.
        /// </summary>
        public ulong SlotsPerEpoch { get; set; }

        /// <summary>
        /// The number of slots before beginning of an epoch to calculate a leader schedule for that epoch.
        /// </summary>
        public ulong LeaderScheduleSlotOffset { get; set; }

        /// <summary>
        /// The first normal-length epoch.
        /// </summary>
        public ulong FirstNormalEpoch { get; set; }

        /// <summary>
        /// The first normal-length slot.
        /// </summary>
        public ulong FirstNormalSlot { get; set; }

        /// <summary>
        /// Whether epochs start short and grow.
        /// </summary>
        public bool Warmup { get; set; }
    }
}