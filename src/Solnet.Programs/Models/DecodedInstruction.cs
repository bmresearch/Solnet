using Solnet.Wallet;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

        /// <summary>
        /// Converts the decoded instructions to a string
        /// </summary>
        /// <returns>A string representation of the decoded instructions</returns>
        public override string ToString() => ToString(0);

        /// <summary>
        /// Converts the decoded instructions to a string, indented a certain amount
        /// </summary>
        /// <returns>A string representation of the decoded instructions, indented a certain amount</returns>
        public string ToString(int indent)
        {
            var sb = new StringBuilder();
            sb.Append($"{new string(Enumerable.Repeat(' ', indent * 4).ToArray())}[{indent}] {PublicKey}:{ProgramName}:{InstructionName}\n");
            sb.Append($"{new string(Enumerable.Repeat(' ', indent * 4).ToArray())}[{indent}] [{string.Join(',', Values.Select(a=>a))}]\n");
            sb.Append($"{new string(Enumerable.Repeat(' ', indent * 4).ToArray())}[{indent}] InnerInstructions ({InnerInstructions.Count})\n");
            foreach (var item in InnerInstructions)
                sb.Append(item.ToString(indent + 1));
            return sb.ToString();
        }
    }
}