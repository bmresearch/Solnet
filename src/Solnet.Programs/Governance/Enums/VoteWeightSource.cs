namespace Solnet.Programs.Governance.Enums
{
    /// <summary>
    /// The source of voter weights used to vote on proposals.
    /// </summary>
    public enum VoteWeightSource : byte
    {
        /// <summary>
        /// Governing token deposits into the Realm are used as voter weights
        /// </summary>
        Deposit = 0,

        /// <summary>
        /// Governing token account snapshots as of the time a proposal entered voting state are used as voter weights
        /// Note: Snapshot source is not supported in the current version
        /// Support for account snapshots are required in solana and/or arweave as a prerequisite
        /// </summary>
        Snapshot = 1,
    }
}
