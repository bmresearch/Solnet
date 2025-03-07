namespace Solnet.Programs.Governance.Enums
{
    /// <summary>
    /// Instruction execution flags defining how instructions are executed for a Proposal
    /// </summary>
    public enum InstructionExecutionFlags : byte
    {
        /// <summary>
        /// No execution flags are specified
        /// Instructions can be executed individually, in any order, as soon as they hold_up time expires
        /// </summary>
        None = 0,

        /// <summary>
        /// Instructions are executed in a specific order
        /// Note: Ordered execution is not supported in the current version
        /// The implementation requires another account type to track deleted instructions
        /// </summary>
        Ordered = 1,

        /// <summary>
        /// Multiple instructions can be executed as a single transaction
        /// Note: Transactions are not supported in the current version
        /// The implementation requires another account type to group instructions within a transaction
        /// </summary>
        UseTransaction = 2,
    }
}
