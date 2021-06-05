using System.Collections.Generic;

namespace Solnet.Rpc.Models
{
    /// <summary>
    /// Holds the block production information.
    /// </summary>
    public class BlockProductionInfo
    {
        /// <summary>
        /// The block production as a map from the validator to a list 
        /// of the number of leader slots and number of blocks produced
        /// </summary>
        public Dictionary<string, List<int>> ByIdentity { get; set; }

        /// <summary>
        /// The block production range by slots.
        /// </summary>
        public SlotRange Range { get; set; }
    }

    /// <summary>
    /// Represents a slot range.
    /// </summary>
    public class SlotRange
    {
        /// <summary>
        /// The first slot of the range (inclusive).
        /// </summary>
        public ulong FirstSlot { get; set; }

        /// <summary>
        /// The last slot of the range (inclusive).
        /// </summary>
        public ulong LastSlot { get; set; }

    }
}