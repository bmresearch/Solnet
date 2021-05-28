using System.Collections.Generic;

namespace Solnet.Rpc.Models
{
    /// <summary>
    /// Represents a transaction instruction.
    /// </summary>
    public class TransactionInstruction
    {
        /// <summary>
        /// The program ID associated with the instruction.
        /// </summary>
        public byte[] ProgramId { get; init; }
        
        /// <summary>
        /// The keys associated with the instruction.
        /// </summary>
        public IList<AccountMeta> Keys { get; init; }
        
        /// <summary>
        /// The instruction-specific data.
        /// </summary>
        public byte[] Data { get; init; }
    }
}