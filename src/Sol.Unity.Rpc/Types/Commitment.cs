namespace Sol.Unity.Rpc.Types
{
    /// <summary>
    /// The commitment describes how finalized a block is at that point in time.
    /// </summary>
    public enum Commitment
    {
        /// <summary>
        /// The node will query the most recent block confirmed by supermajority of the cluster as having reached maximum lockout, meaning the cluster has recognized this block as finalized.
        /// </summary>
        Finalized,
        /// <summary>
        /// The node will query the most recent block that has been voted on by supermajority of the cluster.
        /// </summary>
        Confirmed,

        /// <summary>
        /// The node will query its most recent block. Note that the block may not be complete.
        /// </summary>
        Processed
    }
}