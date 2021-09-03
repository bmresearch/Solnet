using Solnet.Wallet;
using System.Collections.Generic;

namespace Solnet.Programs
{
    /// <summary>
    /// Represents a decoded instruction.
    /// </summary>
    public class DecodedInstruction
    {
        /// <summary>
        /// The public key of the program.
        /// </summary>
        public PublicKey PublicKey { get; set; }

        /// <summary>
        /// The program name.
        /// </summary>
        public string ProgramName { get; set; }

        /// <summary>
        /// The instruction name.
        /// </summary>
        public string InstructionName { get; set; }

        /// <summary>
        /// Values decoded from the instruction.
        /// </summary>
        public Dictionary<string, object> Values { get; set; }

        /// <summary>
        /// The inner instructions related to this decoded instruction.
        /// </summary>
        public List<DecodedInstruction> InnerInstructions { get; set; }
    }
}