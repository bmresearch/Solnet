namespace Solnet.Rpc.Types
{
    /// <summary>
    /// Contains several statistics regarding connection speed and dat usage.
    /// </summary>
    public interface IConnectionStatistics
    {
        /// <summary>
        /// Average throughput in the last 10s. Measured in bytes/s.
        /// </summary>
        ulong AverageThroughput10Seconds { get; set; }

        /// <summary>
        /// Average throughput in the last minute. Measured in bytes/s.
        /// </summary>
        ulong AverageThroughput60Seconds { get; set; }

        /// <summary>
        /// Total bytes downloaded.
        /// </summary>
        ulong TotalReceivedBytes { get; set; }
    }
}