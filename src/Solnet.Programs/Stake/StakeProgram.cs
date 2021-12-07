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
    /// https://docs.rs/solana-program/latest/src/solana_program/stake/instruction.rs.html
    /// https://github.com/solana-labs/solana/blob/master/sdk/program/src/stake/instruction.rs
    /// </remarks>
    /// </summary>
    public static class StakeProgram
    {
        /// <summary>
        /// The public key of the Stake Program.
        /// </summary>
        public static readonly PublicKey ProgramIdKey = new("Stake11111111111111111111111111111111111111");
        /// <summary>
        /// Stake Account Layout Size
        /// </summary>
        public const int StakeAccountDataSize = 200;
        /// <summary>
        /// Stake Config ID
        /// </summary>
         public static readonly PublicKey ConfigKey = new("StakeConfig11111111111111111111111111111111");
        /// <summary>
        /// The program's name.
        /// </summary>
        private const string ProgramName = "Stake Program";
        /// <summary>
        /// Initialize a stake with lockup and authorization information
        /// </summary>
        /// <param name="stakePubkey">Uninitialized stake account</param>
        /// <param name="authorized">Carries pubkeys that must sign staker transactions and withdrawer transactions</param>
        /// <param name="lockup">Carries information about withdrawal restrictions</param>
        /// <returns>The transaction instruction.</returns>
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
        /// <param name="stakePubkey">Stake account to be updated</param>
        /// <param name="authorizedPubkey">Stake or withdraw authority</param>
        /// <param name="newAuthorizedPubkey">Key to be authorized by the transaction</param>
        /// <param name="stakeAuthorize">Type of authority</param>
        /// <param name="custodianPubkey">Lockup authority pubkey if updated withdrawer before lockup expiration</param>
        /// <returns>The transaction instruction.</returns>
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
        /// <param name="stakePubkey">Initialized stake account to be delegated</param>
        /// <param name="authorizedPubkey">Stake authority</param>
        /// <param name="votePubkey">Vote account to which this stake will be delegated</param>
        /// <returns>The transaction instruction.</returns>
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
        /// <param name="stakePubkey">Stake account to be split; must be in the Initialized or Stake state</param>
        /// <param name="authorizedPubkey">Stake authority</param>
        /// <param name="lamports">Amount to be split</param>
        /// <param name="splitStakePubkey">Uninitialized stake account that will take the split-off amount</param>
        /// <returns>The transaction instruction.</returns>
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
        /// <param name="stakePubkey">Stake account from which to withdraw</param>
        /// <param name="withdrawerPubkey">Withdraw authority</param>
        /// <param name="toPubkey">Recipient account</param>
        /// <param name="lamports">Amount to withdraw</param>
        /// <param name="custodianPubkey">Lockup authority</param>
        /// <returns>The transaction instruction.</returns>
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
        /// <param name="stakePubkey">Delegated stake account</param>
        /// <param name="authorizedPubkey">Stake authority</param>
        /// <returns>The transaction instruction.</returns>
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
        /// <param name="stakePubkey">Initialized stake account</param>
        /// <param name="lockup">Lockup information</param>
        /// <param name="custodianPubkey">Lockup authority or withdraw authority</param>
        /// <returns>The transaction instruction.</returns>
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
        /// <param name="destinationStakePubkey">Destination stake account for the merge</param>
        /// <param name="sourceStakePubkey">Source stake account for merge, will be drained</param>
        /// <param name="authorizedPubkey">Stake authority</param>
        /// <returns>The transaction instruction.</returns>
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
        /// <param name="stakePubkey">Stake account to be updated</param>
        /// <param name="authorityBase">Base key of stake or withdraw authority</param>
        /// <param name="authoritySeed">Seed</param>
        /// <param name="authorityOwner">Authority owner</param>
        /// <param name="newAuthorizedPubkey">Pubkey authorized by the transaction</param>
        /// <param name="stakeAuthorize">Type of stake authority</param>
        /// <param name="custodianPubkey">Custodian pubkey</param>
        /// <returns>The transaction instruction.</returns>
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
        /// <param name="stakePubkey">Uninitialized stake account</param>
        /// <param name="authorized">Carries pubkeys that must sign staker transactions and withdrawer transactions</param>
        /// <returns>The transaction instruction.</returns>
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
        /// <param name="stakePubkey">Stake account to be updated</param>
        /// <param name="authorizedPubkey">Stake or withdraw authority</param>
        /// <param name="newAuthorizedPubkey">Key to be authorized by the transaction</param>
        /// <param name="stakeAuthorize">Type of authority</param>
        /// <param name="custodianPubkey">Lockup authority pubkey if updated withdrawer before lockup expiration</param>
        /// <returns>The transaction instruction.</returns>
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
        /// <param name="stakePubkey">Stake account to be updated</param>
        /// <param name="authorityBase">Base key of stake or withdraw authority</param>
        /// <param name="authoritySeed">Seed</param>
        /// <param name="authorityOwner">Authority owner</param>
        /// <param name="newAuthorizedPubkey">Key to be authorized by the transaction</param>
        /// <param name="stakeAuthorize">Type of stake authority</param>
        /// <param name="custodianPubkey">Custodian pubkey</param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction AuthorizeCheckedWithSeed(PublicKey stakePubkey, PublicKey authorityBase, string authoritySeed, PublicKey authorityOwner, PublicKey newAuthorizedPubkey, StakeAuthorize stakeAuthorize, PublicKey custodianPubkey)
        {
            List<AccountMeta> keys = new()
            {
                AccountMeta.Writable(stakePubkey, false),
                AccountMeta.ReadOnly(authorityBase, true),
                AccountMeta.ReadOnly(SysVars.ClockKey, false),
                AccountMeta.ReadOnly(newAuthorizedPubkey, true)
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
        /// <param name="stakePubkey">Initialized stake account</param>
        /// <param name="lockup">Lockup information</param>
        /// <param name="custodianPubkey">Lockup authority or withdraw authority</param>
        /// <returns>The transaction instruction.</returns>
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
