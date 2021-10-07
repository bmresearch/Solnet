using Solnet.Wallet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solnet.Programs.Models
{
    public class Lockup
    {
        public Int64 unix_timestamp { get; set; }
        public ulong epoch { get; set; }
        public PublicKey custodian { get; set; }
    }
}
