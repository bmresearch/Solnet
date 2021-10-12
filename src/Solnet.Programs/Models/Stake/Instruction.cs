using Solnet.Wallet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Solnet.Programs.Models.Stake.State;

namespace Solnet.Programs.Models.Stake
{
    public class Instruction
    {
        public struct AuthorizeCheckedWithSeedArgs
        {
            public StakeAuthorize stake_authorize { get; set; }
            public string authority_seed { get; set; }
            public PublicKey authority_owner { get; set; }

        }
        public struct AuthorizeWithSeedArgs
        {
            public PublicKey new_authorized_pubkey { get; set; }
            public StakeAuthorize stake_authorize { get; set; }
            public string authority_seed { get; set; }
            public PublicKey authority_owner { get; set; }
        }
        public struct LockupArgs
        {
            public Int64 unix_timestamp { get; set; }
            public ulong epoch { get; set; }
            public PublicKey custodian { get; set; }
        }
        public struct LockupChecked
        {
            public Int64 unix_timestamp { get; set; }
            public ulong epoch { get; set; }
        }
    }
}
