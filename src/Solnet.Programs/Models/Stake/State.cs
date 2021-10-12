using Solnet.Wallet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solnet.Programs.Models.Stake
{
    public class State
    {
        public struct Authorized
        {
            public PublicKey staker { get; set; }
            public PublicKey withdrawer { get; set; }
        }
        public struct Delegation
        {
            public PublicKey voter_pubkey { get; set; }
            public ulong stake { get; set; }
            public ulong activation_epoch { get; set; }
            public ulong deactivation_epoch { get; set; }
            public float warmup_cooldown_rate { get; set; }
        }
        public struct Lockup
        {
            public Int64 unix_timestamp { get; set; }
            public ulong epoch { get; set; }
            public PublicKey custodian { get; set; }
        }
        public struct Meta
        {
            public ulong rent_exempt_reserve { get; set; }
            public Authorized authorized { get; set; }
            public Lockup lockup { get; set; }
        }
        public struct Stake
        {
            public Delegation delegation { get; set; }
            public ulong credits_observed { get; set; }
        }
        public enum StakeAuthorize
        {
            Staker = 0,
            Withdrawer = 1
        }
    }
}
