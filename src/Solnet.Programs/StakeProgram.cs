using Solnet.Programs.Models;
using Solnet.Rpc.Models;
using Solnet.Wallet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solnet.Programs
{
    /// <summary>
    /// Implements the Stake Program methods.
    /// <remarks>
    /// For more information see:
    /// LINK HERE
    /// LINK HERE
    /// </remarks>
    /// </summary>
    public static class StakeProgram
    {
        /// <summary>
        /// The public key of the Stake Program.
        /// </summary>
        public static readonly PublicKey ProgramIdKey = new("stake111111111111111111111111111");
        /// <summary>
        /// The public key of the Recent Block Hashes System Variable. 
        /// </summary>
        public static readonly PublicKey
            SysVarRecentBlockHashesKey = new("SysvarRecentB1ockHashes11111111111111111111");

        /// <summary>
        /// The public key of the Rent System Variable.
        /// </summary>
        public static readonly PublicKey SysVarRentKey = new("SysvarRent111111111111111111111111111111111");

        /// <summary>
        /// The program's name.
        /// </summary>
        private const string ProgramName = "Stake Program";

        public static TransactionInstruction Initialize(PublicKey stake_pubkey, Authorized authorized, Lockup lockup)
        {
            List<AccountMeta> keys = new()
            {
                AccountMeta.Writable(stake_pubkey, false),
                AccountMeta.ReadOnly(SysVarRentKey,false)
            };
            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey.KeyBytes,
                Keys = keys,
                Data = StakeProgramData.EncodeInitializeData()
            };
        }
        public static TransactionInstruction Split(PublicKey stake_pubkey, PublicKey authorized_pubkey, ulong lamports, PublicKey split_stake_pubkey)
        {
            List<AccountMeta> keys = new()
            {
                AccountMeta.Writable(stake_pubkey, false),
                AccountMeta.Writable(split_stake_pubkey, false),
                AccountMeta.ReadOnly(authorized_pubkey, true)
            };
            return new TransactionInstruction
            {
                ProgramId=ProgramIdKey.KeyBytes,
                Keys=keys,
                Data=StakeProgramData.EncodeSplitData()
            }
        }
    }
}
