namespace Solnet.Programs.Governance.Enums
{
    /// <summary>
    /// Proposal option vote result
    /// </summary>
    public enum OptionVoteResult : byte
    {
        /// <summary>
        /// Vote on the option is not resolved yet
        /// </summary>
        None = 0,

        /// <summary>
        /// Vote on the option is completed and the option passed
        /// </summary>
        Succeeded = 1,

        /// <summary>
        /// Vote on the option is completed and the option was defeated
        /// </summary>
        Defeated = 2
    }
}
