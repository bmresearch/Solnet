using Solnet.Programs.Models;
using Solnet.Programs.Utilities;
using Solnet.Rpc.Models;
using Solnet.Wallet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Solnet.Programs.Models.Stake.Instruction;
using static Solnet.Programs.Models.Stake.State;

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
        public static readonly PublicKey ProgramIdKey = new("Stake11111111111111111111111111111111111111");
        /// <summary>
        /// The public key of the Recent Block Hashes System Variable. 
        /// </summary>
        public static readonly PublicKey
            SysVarRecentBlockHashesKey = new("SysvarRecentB1ockHashes11111111111111111111");

        /// <summary>
        /// The public key of the Rent System Variable.
        /// </summary>
        public static readonly PublicKey SysVarRentKey = new("SysvarRent111111111111111111111111111111111");

        public const int StakeAccountDataSize = 200;

        /// <summary>
        /// The public key of the Clock System Variable.
        /// </summary>
        public static readonly PublicKey SysVarClockKey = new("SysvarC1ock11111111111111111111111111111111");

        public static readonly PublicKey SysVarStakeHistoryKey = new("SysvarStakeHistory1111111111111111111111111");
        public static readonly PublicKey ConfigKey = new("StakeConfig11111111111111111111111111111111");
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
                Data = StakeProgramData.EncodeInitializeData(authorized,lockup)
            };
        }
        public static TransactionInstruction Authorize(PublicKey stake_pubkey, PublicKey authorized_pubkey, PublicKey new_authorized_pubkey, StakeAuthorize stake_authorize, PublicKey custodian_pubkey)
        {
            List<AccountMeta> keys = new()
            {
                AccountMeta.Writable(stake_pubkey,false),
                AccountMeta.ReadOnly(SysVarClockKey,false),
                AccountMeta.ReadOnly(authorized_pubkey,true)
            };
            if (custodian_pubkey != null)
            {
                keys.Add(AccountMeta.ReadOnly(custodian_pubkey, true));
            }
            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey.KeyBytes,
                Keys = keys,
                Data = StakeProgramData.EncodeAuthorizeData(new_authorized_pubkey, stake_authorize)
            };
        }
        public static TransactionInstruction DelegateStake(PublicKey stake_pubkey, PublicKey authorized_pubkey, PublicKey vote_pubkey)
        {
            List<AccountMeta> keys = new()
            {
                AccountMeta.Writable(stake_pubkey, false),
                AccountMeta.ReadOnly(vote_pubkey, false),
                AccountMeta.ReadOnly(SysVarClockKey, false),
                AccountMeta.ReadOnly(SysVarStakeHistoryKey, false),
                AccountMeta.ReadOnly(ConfigKey, false),
                AccountMeta.ReadOnly(authorized_pubkey, true)
            };
            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey.KeyBytes,
                Keys = keys,
                Data=StakeProgramData.EncodeDelegateStakeData()
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
                ProgramId = ProgramIdKey.KeyBytes,
                Keys = keys,
                Data = StakeProgramData.EncodeSplitData(lamports)
            };
        }
        public static TransactionInstruction Withdraw(PublicKey stake_pubkey, PublicKey withdrawer_pubkey, PublicKey to_pubkey, ulong lamports, PublicKey custodian_pubkey)
        {
            List<AccountMeta> keys = new()
            {
                AccountMeta.Writable(stake_pubkey, false),
                AccountMeta.Writable(to_pubkey, false),
                AccountMeta.ReadOnly(SysVarClockKey, false),
                AccountMeta.ReadOnly(SysVarStakeHistoryKey,false),
                AccountMeta.ReadOnly(withdrawer_pubkey,true)
            };
            if (custodian_pubkey != null)
            {
                keys.Add(AccountMeta.ReadOnly(custodian_pubkey, true));
            }
            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey.KeyBytes,
                Keys = keys,
                Data = StakeProgramData.EncodeWithdrawData(lamports)
            };
        }
        public static TransactionInstruction Deactivate(PublicKey stake_pubkey, PublicKey authorized_pubkey)
        {
            List<AccountMeta> keys = new()
            {
                AccountMeta.Writable(stake_pubkey, false),
                AccountMeta.ReadOnly(SysVarClockKey, false),
                AccountMeta.ReadOnly(authorized_pubkey, true)
            };
            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey.KeyBytes,
                Keys = keys,
                Data = StakeProgramData.EncodeDeactivateData()
            };
        }
        public static TransactionInstruction SetLockup(PublicKey stake_pubkey, Lockup lockup, PublicKey custodian_pubkey)
        {
            List<AccountMeta> keys = new()
            {
                AccountMeta.Writable(stake_pubkey, false),
                AccountMeta.ReadOnly(custodian_pubkey, true)
            };
            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey.KeyBytes,
                Keys = keys,
                Data = StakeProgramData.EncodeSetLockupData(lockup)
            };
        }
        public static TransactionInstruction Merge(PublicKey destination_stake_pubkey, PublicKey source_stake_pubkey, PublicKey authorized_pubkey)
        {
            List<AccountMeta> keys = new()
            {
                AccountMeta.Writable(destination_stake_pubkey, false),
                AccountMeta.Writable(source_stake_pubkey, false),
                AccountMeta.ReadOnly(SysVarClockKey, false),
                AccountMeta.ReadOnly(SysVarStakeHistoryKey, false),
                AccountMeta.ReadOnly(authorized_pubkey, true)
            };
            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey.KeyBytes,
                Keys = keys,
                Data = StakeProgramData.EncodeMergeData()
            };
        }
        public static TransactionInstruction AuthorizeWithSeed(PublicKey stake_pubkey, PublicKey authority_base, string authority_seed, PublicKey authority_owner, PublicKey new_authorized_pubkey, StakeAuthorize stake_authorize, PublicKey custodian_pubkey)
        {
            List<AccountMeta> keys = new()
            {
                AccountMeta.Writable(stake_pubkey, false),
                AccountMeta.ReadOnly(authority_base, true),
                AccountMeta.ReadOnly(SysVarClockKey, false)
            };
            if (custodian_pubkey != null)
            {
                keys.Add(AccountMeta.ReadOnly(custodian_pubkey, true));
            }
            AuthorizeWithSeedArgs authorizeWithSeed = new AuthorizeWithSeedArgs
            {
                new_authorized_pubkey = new_authorized_pubkey,
                stake_authorize = stake_authorize,
                authority_seed = authority_seed,
                authority_owner = authority_owner
            };
            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey.KeyBytes,
                Keys = keys,
                Data = StakeProgramData.EncodeAuthorizeWithSeedData(authorizeWithSeed)
            };
        }
        public static TransactionInstruction InitializeChecked(PublicKey stake_pubkey, Authorized authorized)
        {
            List<AccountMeta> keys = new()
            {
                AccountMeta.Writable(stake_pubkey, false),
                AccountMeta.ReadOnly(SysVarRentKey, false),
                AccountMeta.ReadOnly(authorized.staker, false),
                AccountMeta.ReadOnly(authorized.withdrawer,true)
            };
            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey.KeyBytes,
                Keys = keys,
                Data = StakeProgramData.EncodeInitializeCheckedData()
            };
        }
        public static TransactionInstruction AuthorizeChecked(PublicKey stake_pubkey, PublicKey authorized_pubkey, PublicKey new_authorized_pubkey, StakeAuthorize stake_authorize, PublicKey custodian_pubkey)
        {
            List<AccountMeta> keys = new()
            {
                AccountMeta.Writable(stake_pubkey, false),
                AccountMeta.ReadOnly(SysVarClockKey, false),
                AccountMeta.ReadOnly(authorized_pubkey, true),
                AccountMeta.ReadOnly(new_authorized_pubkey,true)
            };
            if (custodian_pubkey != null)
            {
                keys.Add(AccountMeta.ReadOnly(custodian_pubkey, true));
            }
            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey.KeyBytes,
                Keys = keys,
                Data = StakeProgramData.EncodeAuthorizeCheckedData(stake_authorize)
            };
        }
        public static TransactionInstruction AuthorizeCheckedWithSeed(PublicKey stake_pubkey, PublicKey authority_base, string authority_seed, PublicKey authority_owner, PublicKey new_authorized_pubkey, StakeAuthorize stake_authorize, PublicKey custodian_pubkey)
        {
            List<AccountMeta> keys = new()
            {
                AccountMeta.Writable(stake_pubkey, false),
                AccountMeta.ReadOnly(authority_base, true),
                AccountMeta.ReadOnly(SysVarClockKey, false),
                AccountMeta.ReadOnly(new_authorized_pubkey, true)
            };
            if (custodian_pubkey != null)
            {
                keys.Add(AccountMeta.ReadOnly(custodian_pubkey, true));
            }
            AuthorizeCheckedWithSeedArgs authorizeWithSeed = new AuthorizeCheckedWithSeedArgs
            {
                stake_authorize = stake_authorize,
                authority_seed = authority_seed,
                authority_owner = authority_owner
            };
            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey.KeyBytes,
                Keys = keys,
                Data = StakeProgramData.EncodeAuthorizeCheckedWithSeedData(authorizeWithSeed)
            };
        }
        public static TransactionInstruction SetLockupChecked(PublicKey stake_pubkey, Lockup lockup, PublicKey custodian_pubkey)
        {
            List<AccountMeta> keys = new()
            {
                AccountMeta.Writable(stake_pubkey, false),
                AccountMeta.ReadOnly(custodian_pubkey, true)
            };
            if (lockup.custodian != null)
            {
                keys.Add(AccountMeta.ReadOnly(lockup.custodian, true));
            }
            LockupChecked lockupChecked = new LockupChecked
            {
                unix_timestamp = lockup.unix_timestamp,
                epoch = lockup.epoch
            };
            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey.KeyBytes,
                Keys = keys,
                Data = StakeProgramData.EncodeSetLockupCheckedData(lockupChecked)
            };
        }
        /// <summary>
        /// Decodes an instruction created by the System Program.
        /// </summary>
        /// <param name="data">The instruction data to decode.</param>
        /// <param name="keys">The account keys present in the transaction.</param>
        /// <param name="keyIndices">The indices of the account keys for the instruction as they appear in the transaction.</param>
        /// <returns>A decoded instruction.</returns>
        public static DecodedInstruction Decode(ReadOnlySpan<byte> data, IList<PublicKey> keys, byte[] keyIndices)
        {
            uint instruction = data.GetU32(StakeProgramData.MethodOffset);
            StakeProgramInstructions.Values instructionValue = 
                (StakeProgramInstructions.Values)Enum.Parse(typeof(StakeProgramInstructions.Values), instruction.ToString());
            
            DecodedInstruction decodedInstruction = new ()
            {
                PublicKey = ProgramIdKey,
                InstructionName = StakeProgramInstructions.Names[instructionValue],
                ProgramName = ProgramName,
                Values = new Dictionary<string, object>(){},
                InnerInstructions = new List<DecodedInstruction>()
            };

            switch (instructionValue)
            {
                case StakeProgramInstructions.Values.Initialize:
                    StakeProgramData.DecodeInitializeData(decodedInstruction, data, keys, keyIndices);
                    break;
                case StakeProgramInstructions.Values.Authorize:
                    StakeProgramData.DecodeAuthorizeData(decodedInstruction, data, keys, keyIndices);
                    break;
                case StakeProgramInstructions.Values.DelegateStake:
                    StakeProgramData.DecodeDelegateStakeData(decodedInstruction, data, keys, keyIndices);
                    break;
                case StakeProgramInstructions.Values.Split:
                    StakeProgramData.DecodeSplitStakeData(decodedInstruction, data, keys, keyIndices);
                    break;
                case StakeProgramInstructions.Values.Withdraw:
                    StakeProgramData.DecodeWithdrawStakeData(decodedInstruction, data, keys, keyIndices);
                    break;
                case StakeProgramInstructions.Values.Deactivate:
                    StakeProgramData.DecodeDeactivateStakeData(decodedInstruction, data, keys, keyIndices);
                    break;
                case StakeProgramInstructions.Values.SetLockup:
                    StakeProgramData.DecodeSetLockupStakeData(decodedInstruction, data, keys, keyIndices);
                    break;
                case StakeProgramInstructions.Values.Merge:
                    StakeProgramData.DecodeMergeStakeData(decodedInstruction, data, keys, keyIndices);
                    break;
                case StakeProgramInstructions.Values.AuthorizeWithSeed:
                    StakeProgramData.DecodeAuthorizeWithSeedStakeData(decodedInstruction, data, keys, keyIndices);
                    break;
                case StakeProgramInstructions.Values.InitializeChecked:
                    StakeProgramData.DecodeInitializeCheckedStakeData(decodedInstruction, data, keys, keyIndices);
                    break;
                case StakeProgramInstructions.Values.AuthorizeChecked:
                    StakeProgramData.DecodeAuthorizeCheckedStakeData(decodedInstruction, data, keys, keyIndices);
                    break;
                case StakeProgramInstructions.Values.AuthorizeCheckedWithSeed:
                    StakeProgramData.DecodeAuthorizeCheckedWithSeedStakeData(decodedInstruction, data, keys, keyIndices);
                    break;
                case StakeProgramInstructions.Values.SetLockupChecked:
                    StakeProgramData.DecodeSetLockupCheckedStakeData(decodedInstruction, data, keys, keyIndices);
                    break;
            }
            return decodedInstruction;
        }
    }
}
