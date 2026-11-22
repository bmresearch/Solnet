using System.Text.Json;

namespace Solnet.Rpc.Models
{
    /// <summary>
    /// A block subscription update, including any error reported by the validator.
    /// </summary>
    public class BlockUpdate
    {
        /// <summary>The slot of the block.</summary>
        public ulong Slot { get; set; }

        /// <summary>The block, or null if the validator could not return it.</summary>
        public BlockInfo Block { get; set; }

        /// <summary>The validator's error, or null on success.</summary>
        public JsonElement? Err { get; set; }
    }
}
