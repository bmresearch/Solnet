namespace Solnet.Programs.Governance.Enums
{
    /// <summary>
    /// The status of instruction execution
    /// </summary>
    public enum InstructionExecutionStatus : byte
    {
        /// <summary>
        /// Instruction was not executed yet
        /// </summary>
        None = 0,

        /// <summary>
        /// Instruction was executed successfully
        /// </summary>
        Success = 1,

        /// <summary>
        /// Instruction execution failed
        /// </summary>
        Error = 2,
    }
}
