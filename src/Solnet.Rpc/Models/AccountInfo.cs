using System.Collections.Generic;

namespace Solnet.Rpc.Models
{
    public class AccountInfo
    {
        public ulong Lamports { get; set; }

        public string Owner { get; set; }

        public bool Executable { get; set; }

        public ulong RentEpoch { get; set; }

        public IList<string> Data { get; set; }
    }
}