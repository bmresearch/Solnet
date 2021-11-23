using Solnet.Programs.Utilities;
using Solnet.Rpc.Models;
using Solnet.Wallet;
using System;
using System.Collections.Generic;
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
        /// 
        /// </summary>
        public const int StakeAccountDataSize = 200;
        /// <summary>
        /// 
        /// </summary>
         public static readonly PublicKey ConfigKey = new("StakeConfig11111111111111111111111111111111");
        /// <summary>
        /// The program's name.
        /// </summary>
        private const string ProgramName = "Stake Program";
        /// <summary>
        /// Initialize a stake with lockup and authorization information
        /// </summary>
        /// <param name="stakePubkey"></param>
        /// <param name="authorized"></param>
        /// <param name="lockup"></param>
        /// <returns></returns>
        public static TransactionInstruction Initialize(PublicKey stakePubkey, Authorized authorized, Lockup lockup)
        {
            List<AccountMeta> keys = new()
            {
                AccountMeta.Writable(stakePubkey, false),
                AccountMeta.ReadOnly(SysVars.RentKey,false)
            };
            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey.KeyBytes,
                Keys = keys,
                Data = StakeProgramData.EncodeInitializeData(authorized,lockup)
            };
        }
        /// <summary>
        /// Authorize a key to manage stake or withdrawal
        /// </summary>
        /// <param name="stakePubkey"></param>
        /// <param name="authorizedPubkey"></param>
        /// <param name="newAuthorizedPubkey"></param>
        /// <param name="stakeAuthorize"></param>
        /// <param name="custodianPubkey"></param>
        /// <returns></returns>
        public static TransactionInstruction Authorize(PublicKey stakePubkey, PublicKey authorizedPubkey, PublicKey newAuthorizedPubkey, StakeAuthorize stakeAuthorize, PublicKey custodianPubkey)
        {
            List<AccountMeta> keys = new()
            {
                AccountMeta.Writable(stakePubkey,false),
                AccountMeta.ReadOnly(SysVars.ClockKey,false),
                AccountMeta.ReadOnly(authorizedPubkey,true)
            };
            if (custodianPubkey != null)
            {
                keys.Add(AccountMeta.ReadOnly(custodianPubkey, true));
            }
            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey.KeyBytes,
                Keys = keys,
                Data = StakeProgramData.EncodeAuthorizeData(newAuthorizedPubkey, stakeAuthorize)
            };
        }
        /// <summary>
        /// Delegate a stake to a particular vote account
        /// </summary>
        /// <param name="stakePubkey"></param>
        /// <param name="authorizedPubkey"></param>
        /// <param name="votePubkey"></param>
        /// <returns></returns>
        public static TransactionInstruction DelegateStake(PublicKey stakePubkey, PublicKey authorizedPubkey, PublicKey votePubkey)
        {
            List<AccountMeta> keys = new()
            {
                AccountMeta.Writable(stakePubkey, false),
                AccountMeta.ReadOnly(votePubkey, false),
                AccountMeta.ReadOnly(SysVars.ClockKey, false),
                AccountMeta.ReadOnly(SysVars.StakeHistoryKey, false),
                AccountMeta.ReadOnly(ConfigKey, false),
                AccountMeta.ReadOnly(authorizedPubkey, true)
            };
            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey.KeyBytes,
                Keys = keys,
                Data=StakeProgramData.EncodeDelegateStakeData()
            };
        }
        /// <summary>
        /// Split u64 tokens and stake off a stake account into another stake account.
        /// </summary>
        /// <param name="stakePubkey"></param>
        /// <param name="authorizedPubkey"></param>
        /// <param name="lamports"></param>
        /// <param name="splitStakePubkey"></param>
        /// <returns></returns>
        public static TransactionInstruction Split(PublicKey stakePubkey, PublicKey authorizedPubkey, ulong lamports, PublicKey splitStakePubkey)
        {
            List<AccountMeta> keys = new()
            {
                AccountMeta.Writable(stakePubkey, false),
                AccountMeta.Writable(splitStakePubkey, false),
                AccountMeta.ReadOnly(authorizedPubkey, true)
            };
            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey.KeyBytes,
                Keys = keys,
                Data = StakeProgramData.EncodeSplitData(lamports)
            };
        }
        /// <summary>
        /// Withdraw unstaked lamports from the stake account
        /// </summary>
        /// <param name="stakePubkey"></param>
        /// <param name="withdrawerPubkey"></param>
        /// <param name="toPubkey"></param>
        /// <param name="lamports"></param>
        /// <param name="custodianPubkey"></param>
        /// <returns></returns>
        public static TransactionInstruction Withdraw(PublicKey stakePubkey, PublicKey withdrawerPubkey, PublicKey toPubkey, ulong lamports, PublicKey custodianPubkey)
        {
            List<AccountMeta> keys = new()
            {
                AccountMeta.Writable(stakePubkey, false),
                AccountMeta.Writable(toPubkey, false),
                AccountMeta.ReadOnly(SysVars.ClockKey, false),
                AccountMeta.ReadOnly(SysVars.StakeHistoryKey,false),
                AccountMeta.ReadOnly(withdrawerPubkey,true)
            };
            if (custodianPubkey != null)
            {
                keys.Add(AccountMeta.ReadOnly(custodianPubkey, true));
            }
            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey.KeyBytes,
                Keys = keys,
                Data = StakeProgramData.EncodeWithdrawData(lamports)
            };
        }
        /// <summary>
        /// Deactivates the stake in the account
        /// </summary>
        /// <param name="stakePubkey"></param>
        /// <param name="authorizedPubkey"></param>
        /// <returns></returns>
        public static TransactionInstruction Deactivate(PublicKey stakePubkey, PublicKey authorizedPubkey)
        {
            List<AccountMeta> keys = new()
            {
                AccountMeta.Writable(stakePubkey, false),
                AccountMeta.ReadOnly(SysVars.ClockKey, false),
                AccountMeta.ReadOnly(authorizedPubkey, true)
            };
            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey.KeyBytes,
                Keys = keys,
                Data = StakeProgramData.EncodeDeactivateData()
            };
        }
        /// <summary>
        /// Set stake lockup
        /// If a lockup is not active, the withdraw authority may set a new lockup
        /// If a lockup is active, the lockup custodian may update the lockup parameters
        /// </summary>
        /// <param name="stakePubkey"></param>
        /// <param name="lockup"></param>
        /// <param name="custodianPubkey"></param>
        /// <returns></returns>
        public static TransactionInstruction SetLockup(PublicKey stakePubkey, Lockup lockup, PublicKey custodianPubkey)
        {
            List<AccountMeta> keys = new()
            {
                AccountMeta.Writable(stakePubkey, false),
                AccountMeta.ReadOnly(custodianPubkey, true)
            };
            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey.KeyBytes,
                Keys = keys,
                Data = StakeProgramData.EncodeSetLockupData(lockup)
            };
        }
        /// <summary>
        /// Merge two stake accounts.
        ///
        /// Both accounts must have identical lockup and authority keys. A merge
        /// is possible between two stakes in the following states with no additional
        /// conditions:
        ///
        /// * two deactivated stakes
        /// * an inactive stake into an activating stake during its activation epoch
        ///
        /// For the following cases, the voter pubkey and vote credits observed must match:
        ///
        /// * two activated stakes
        /// * two activating accounts that share an activation epoch, during the activation epoch
        ///
        /// All other combinations of stake states will fail to merge, including all
        /// "transient" states, where a stake is activating or deactivating with a
        /// non-zero effective stake.
        ///
        /// </summary>
        /// <param name="destinationStakePubkey"></param>
        /// <param name="sourceStakePubkey"></param>
        /// <param name="authorizedPubkey"></param>
        /// <returns></returns>
        public static TransactionInstruction Merge(PublicKey destinationStakePubkey, PublicKey sourceStakePubkey, PublicKey authorizedPubkey)
        {
            List<AccountMeta> keys = new()
            {
                AccountMeta.Writable(destinationStakePubkey, false),
                AccountMeta.Writable(sourceStakePubkey, false),
                AccountMeta.ReadOnly(SysVars.ClockKey, false),
                AccountMeta.ReadOnly(SysVars.StakeHistoryKey, false),
                AccountMeta.ReadOnly(authorizedPubkey, true)
            };
            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey.KeyBytes,
                Keys = keys,
                Data = StakeProgramData.EncodeMergeData()
            };
        }
        /// <summary>
        /// Authorize a key to manage stake or withdrawal with a derived key
        /// </summary>
        /// <param name="stakePubkey"></param>
        /// <param name="authorityBase"></param>
        /// <param name="authoritySeed"></param>
        /// <param name="authorityOwner"></param>
        /// <param name="newAuthorizedPubkey"></param>
        /// <param name="stakeAuthorize"></param>
        /// <param name="custodianPubkey"></param>
        /// <returns></returns>
        public static TransactionInstruction AuthorizeWithSeed(PublicKey stakePubkey, PublicKey authorityBase, string authoritySeed, PublicKey authorityOwner, PublicKey newAuthorizedPubkey, StakeAuthorize stakeAuthorize, PublicKey custodianPubkey)
        {
            List<AccountMeta> keys = new()
            {
                AccountMeta.Writable(stakePubkey, false),
                AccountMeta.ReadOnly(authorityBase, true),
                AccountMeta.ReadOnly(SysVars.ClockKey, false)
            };
            if (custodianPubkey != null && custodianPubkey != authorityBase)
            {
                keys.Add(AccountMeta.ReadOnly(custodianPubkey, true));
            }
            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey.KeyBytes,
                Keys = keys,
                Data = StakeProgramData.EncodeAuthorizeWithSeedData(authoritySeed, newAuthorizedPubkey, stakeAuthorize, authorityOwner)
            };
        }
        /// <summary>
        /// Initialize a stake with authorization information
        ///
        /// This instruction is similar to `Initialize` except that the withdraw authority
        /// must be a signer, and no lockup is applied to the account.
        /// 
        /// </summary>
        /// <param name="stakePubkey"></param>
        /// <param name="authorized"></param>
        /// <returns></returns>
        public static TransactionInstruction InitializeChecked(PublicKey stakePubkey, Authorized authorized)
        {
            List<AccountMeta> keys = new()
            {
                AccountMeta.Writable(stakePubkey, false),
                AccountMeta.ReadOnly(SysVars.RentKey, false),
                AccountMeta.ReadOnly(authorized.Staker, false),
                AccountMeta.ReadOnly(authorized.Withdrawer,true)
            };
            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey.KeyBytes,
                Keys = keys,
                Data = StakeProgramData.EncodeInitializeCheckedData()
            };
        }
        /// <summary>
        /// Authorize a key to manage stake or withdrawal
        ///
        /// This instruction behaves like `Authorize` with the additional requirement that the new
        /// stake or withdraw authority must also be a signer.
        ///
        /// </summary>
        /// <param name="stakePubkey"></param>
        /// <param name="authorizedPubkey"></param>
        /// <param name="newAuthorizedPubkey"></param>
        /// <param name="stakeAuthorize"></param>
        /// <param name="custodianPubkey"></param>
        /// <returns></returns>
        public static TransactionInstruction AuthorizeChecked(PublicKey stakePubkey, PublicKey authorizedPubkey, PublicKey newAuthorizedPubkey, StakeAuthorize stakeAuthorize, PublicKey custodianPubkey)
        {
            List<AccountMeta> keys = new()
            {
                AccountMeta.Writable(stakePubkey, false),
                AccountMeta.ReadOnly(SysVars.ClockKey, false),
                AccountMeta.ReadOnly(authorizedPubkey, true),
                AccountMeta.ReadOnly(newAuthorizedPubkey,true)
            };
            if (custodianPubkey != null)
            {
                keys.Add(AccountMeta.ReadOnly(custodianPubkey, true));
            }
            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey.KeyBytes,
                Keys = keys,
                Data = StakeProgramData.EncodeAuthorizeCheckedData(stakeAuthorize)
            };
        }
        /// <summary>
        /// Authorize a key to manage stake or withdrawal with a derived key
        ///
        /// This instruction behaves like `AuthorizeWithSeed` with the additional requirement that
        /// the new stake or withdraw authority must also be a signer.
        ///
        /// </summary>
        /// <param name="stakePubkey"></param>
        /// <param name="authorityBase"></param>
        /// <param name="authoritySeed"></param>
        /// <param name="authorityOwner"></param>
        /// <param name="newAuthoritzedPubkey"></param>
        /// <param name="stakeAuthorize"></param>
        /// <param name="custodianPubkey"></param>
        /// <returns></returns>
        public static TransactionInstruction AuthorizeCheckedWithSeed(PublicKey stakePubkey, PublicKey authorityBase, string authoritySeed, PublicKey authorityOwner, PublicKey newAuthoritzedPubkey, StakeAuthorize stakeAuthorize, PublicKey custodianPubkey)
        {
            List<AccountMeta> keys = new()
            {
                AccountMeta.Writable(stakePubkey, false),
                AccountMeta.ReadOnly(authorityBase, true),
                AccountMeta.ReadOnly(SysVars.ClockKey, false),
                AccountMeta.ReadOnly(newAuthoritzedPubkey, true)
            };
            if (custodianPubkey != null)
            {
                keys.Add(AccountMeta.ReadOnly(custodianPubkey, true));
            }
            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey.KeyBytes,
                Keys = keys,
                Data = StakeProgramData.EncodeAuthorizeCheckedWithSeedData(authoritySeed, authorityOwner, stakeAuthorize)
            };
        }
        /// <summary>
        /// This instruction behaves like `SetLockup` with the additional requirement that
        /// the new lockup authority also be a signer.
        ///
        /// If a lockup is not active, the withdraw authority may set a new lockup
        /// If a lockup is active, the lockup custodian may update the lockup parameters
        ///
        /// </summary>
        /// <param name="stakePubkey"></param>
        /// <param name="lockup"></param>
        /// <param name="custodianPubkey"></param>
        /// <returns></returns>
        public static TransactionInstruction SetLockupChecked(PublicKey stakePubkey, Lockup lockup, PublicKey custodianPubkey)
        {
            List<AccountMeta> keys = new()
            {
                AccountMeta.Writable(stakePubkey, false),
                AccountMeta.ReadOnly(custodianPubkey, true)
            };
            if (lockup.Custodian != null)
            {
                keys.Add(AccountMeta.ReadOnly(lockup.Custodian, true));
            }
            Lockup lockupChecked = new Lockup
            {
                UnixTimestamp = lockup.UnixTimestamp,
                Epoch = lockup.Epoch
            };
            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey.KeyBytes,
                Keys = keys,
                Data = StakeProgramData.EncodeSetLockupCheckedData(lockupChecked)
            };
        }
        /// <summary>
        /// Decodes an instruction created by the Stake Program.
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
