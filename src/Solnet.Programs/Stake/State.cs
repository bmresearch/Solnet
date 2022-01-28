using Solnet.Wallet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solnet.Programs.Models.Stake
{
    /// <summary>
    /// Represents <see cref="StakeProgram"/> State Structs in Solana.
    /// </summary>
    public class State
    {
        /// <summary>
        /// A public key pair passed as an Authorized struct with staking and withdrawing authorities
        /// </summary>
        public struct Authorized
        {
            /// The staking authority
            public PublicKey Staker { get; set; }
            /// The withdrawing authority
            public PublicKey Withdrawer { get; set; }
        }
        /// <summary>
        /// A structure containing the information for delegating a stake
        /// </summary>
        public struct Delegation
        {
            /// <summary>
            /// to whom the stake is delegated
            /// </summary>
            public PublicKey VoterPubkey { get; set; }
            /// <summary>
            /// activated stake amount, set at delegate() time
            /// </summary>
            public ulong Stake { get; set; }
            ///<summary>
            /// epoch at which this stake was activated, std::Epoch::MAX if is a bootstrap stake
            /// </summary>
            public ulong ActivationEpoch { get; set; }
            /// <summary>
            /// epoch the stake was deactivated, std::Epoch::MAX if not deactivated
            /// </summary>
            public ulong DeactivationEpoch { get; set; }
            /// <summary>
            /// how much stake we can activate per-epoch as a fraction of currently effective stake
            /// </summary>
            public float WarmupCooldownRate { get; set; }
        }
        /// <summary>
        /// A structure containing the information for setting Lockup information for a stake
        /// </summary>
        public struct Lockup
        {
            /// <summary>
            /// UnixTimestamp at which this stake will allow withdrawal, unless the
            ///   transaction is signed by the custodian
            ///   </summary>
            public Int64 UnixTimestamp { get; set; }
            /// <summary>
            /// epoch height at which this stake will allow withdrawal, unless the
            /// transaction is signed by the custodian
            /// </summary>
            public ulong Epoch { get; set; }
            /// <summary>
            /// custodian signature on a transaction exempts the operation from
            /// lockup constraints
            /// </summary>
            public PublicKey Custodian { get; set; }
        }
        /// <summary>
        /// A structure containing metadata for a stake
        /// </summary>
        public struct Meta
        {
            /// <summary>
            /// The Rent Exempt Reserve amount
            /// </summary>
            public ulong RentExemptReserve { get; set; }
            /// <summary>
            /// An Authorized struct
            /// </summary>
            public Authorized Authorized { get; set; }
            /// <summary>
            /// A Lockup struct
            /// </summary>
            public Lockup Lockup { get; set; }
        }
        /// <summary>
        /// A structure containing information about a redeemed or delegated vote account stake
        /// </summary>
        public struct Stake
        {
            /// <summary>
            /// A Delegation struct
            /// </summary>
            public Delegation Delegation { get; set; }
            /// <summary>
            /// credits observed is credits from vote account state when delegated or redeemed
            /// </summary>
            public ulong CreditsObserved { get; set; }
        }
        /// <summary>
        /// An enum representing Authority type
        /// </summary>
        public enum StakeAuthorize : byte
        {
            /// <summary>
            /// Is staker
            /// </summary>
            Staker = 0,
            /// <summary>
            /// Is withdrawer
            /// </summary>
            Withdrawer = 1
        }
    }
}
