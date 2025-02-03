namespace Solnet.Programs.Governance.Enums
{
    /// <summary>
    /// What state a Proposal is in
    /// </summary>
    public enum ProposalState : byte
    {
        /// <summary>
        /// Draft - Proposal enters Draft state when it's created
        /// </summary>
        Draft = 0,

        /// <summary>
        /// SigningOff - The Proposal is being signed off by Signatories
        /// Proposal enters the state when first Signatory Sings and leaves it when last Signatory signs
        /// </summary>
        SigningOff = 1,

        /// <summary>
        /// Taking votes
        /// </summary>
        Voting = 2,

        /// <summary>
        /// Voting ended with success
        /// </summary>
        Succeeded = 3,

        /// <summary>
        /// Voting on Proposal succeeded and now instructions are being executed
        /// Proposal enter this state when first instruction is executed and leaves when the last instruction is executed
        /// </summary>
        Executing = 4,

        /// <summary>
        /// Completed
        /// </summary>
        Completed = 5,

        /// <summary>
        /// Cancelled
        /// </summary>
        Cancelled = 6,

        /// <summary>
        /// Defeated
        /// </summary>
        Defeated = 7,

        /// <summary>
        /// Same as Executing but indicates some instructions failed to execute
        /// Proposal can't be transitioned from ExecutingWithErrors to Completed state
        /// </summary>
        ExecutingWithErrors = 8,
    }
}
