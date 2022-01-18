// unset

namespace Solnet.Rpc.Models
{
    /// <summary>
    /// Represents the <c>memcmp</c> filter for the <see cref="IRpcClient.GetProgramAccounts"/> method.
    /// </summary>
    public class MemCmp
    {
        /// <summary>
        /// The offset into program account data at which to start the comparison.
        /// </summary>
        public int Offset { get; set; }

        /// <summary>
        /// The data to match against the program data, as base-58 encoded string and limited to 129 bytes.
        /// </summary>
        public string Bytes { get; set; }
    }
}