// ReSharper disable ClassNeverInstantiated.Global

namespace Solnet.Rpc.Models
{
    /// <summary>
    /// Represents the slot info.
    /// </summary>
    public class SlotInfo
    {
        /// <summary>
        /// The parent slot.
        /// </summary>
        public int Parent { get; set; }

        /// <summary>
        /// The root as set by the validator.
        /// </summary>
        public int Root { get; set; }

        /// <summary>
        /// The current slot.
        /// </summary>
        public int Slot { get; set; }
    }
}