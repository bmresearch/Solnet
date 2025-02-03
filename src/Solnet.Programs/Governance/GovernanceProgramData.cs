namespace Solnet.Programs.Governance
{
    /// <summary>
    /// Implements the governance program data encodings.
    /// </summary>
    public static class GovernanceProgramData
    {
        /// <summary>
        /// Encode the transaction instruction data for the <see cref="GovernanceProgramInstructions.Values.ExecuteInstruction"/> method.
        /// </summary>
        /// <returns>The byte array with the encoded data.</returns>
        public static byte[] EncodeExecuteInstructionData()
            => new[] { (byte)GovernanceProgramInstructions.Values.ExecuteInstruction };
    }
}
