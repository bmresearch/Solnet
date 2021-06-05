namespace Solnet.Rpc.Models
{
    /// <summary>
    /// Represents a performance sample.
    /// </summary>
    public class PerformanceSample
    {
        /// <summary>
        /// Slot in which sample was taken at.
        /// </summary>
        public ulong Slot { get; set; }

        /// <summary>
        /// Number of transactions in sample.
        /// </summary>
        public ulong NumTransactions { get; set; }

        /// <summary>
        /// Number of slots in sample
        /// </summary>
        public ulong NumSlots { get; set; }

        /// <summary>
        /// Number of seconds in a sample window.
        /// </summary>
        public int SamplePeriodSecs { get; set; }
    }
}