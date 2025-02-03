namespace Solnet.Programs.Governance.Enums
{
    /// <summary>
    /// The type of the vote threshold percentage used to resolve a vote on a Proposal
    /// </summary>
    public enum VoteThresholdPercentage : byte
    {
        /// <summary>
        /// Voting threshold of Yes votes in % required to tip the vote
        /// It's the percentage of tokens out of the entire pool of governance tokens eligible to vote
        /// Note: If the threshold is below or equal to 50% then an even split of votes ex: 50:50 or 40:40 is always resolved as Defeated
        /// In other words a '+1 vote' tie breaker is always required to have a successful vote
        /// </summary>
        YesVote = 0,

        /// <summary>
        /// The minimum number of votes in % out of the entire pool of governance tokens eligible to vote
        /// which must be cast for the vote to be valid
        /// Once the quorum is achieved a simple majority (50%+1) of Yes votes is required for the vote to succeed
        /// Note: Quorum is not implemented in the current version
        /// </summary>
        Quorum = 1,
    }
}
