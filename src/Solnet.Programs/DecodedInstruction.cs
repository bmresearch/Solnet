// unset

using Solnet.Wallet;
using System.Collections.Generic;

namespace Solnet.Programs
{
    public class DecodedInstruction
    {
        public PublicKey PublicKey { get; set; }
        public string ProgramName { get; set; }
        public string InstructionName { get; set; }
        public Dictionary<string, object> Values { get; set; }
    }
}