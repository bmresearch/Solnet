using System.Collections.Generic;

namespace Solnet.Rpc.Models
{
    public class TransactionInstruction
    {
        public byte[] ProgramId { get; init; }
        public IList<AccountMeta> Keys { get; init; }
        public byte[] Data { get; init; }
    }
}