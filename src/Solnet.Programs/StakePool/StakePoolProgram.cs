using Solnet.Rpc.Models;
using Solnet.Wallet;
using System.Collections.Generic;
using Solnet.Programs.Abstract;
using System;
using System.Text;
using Solnet.Programs.StakePool.Models;

namespace Solnet.Programs.StakePool
{
    /// <summary>
    /// Implements the Stake Pool Program methods.
    /// <remarks>
    /// For more information see:
    /// https://spl.solana.com/stake-pool
    /// https://docs.rs/spl-stake-pool/latest/spl_stake_pool/
    /// </remarks>
    /// </summary>
    public class StakePoolProgram: BaseProgram
    {
        /// <summary>
        /// SPL Stake Pool Program ID
        /// </summary>
        public static readonly PublicKey StakePoolProgramIdKey = new("SPoo1Ku8WFXoNDMHPsrGSTSG1Y47rzgn41SLUNakuHy");

        /// <summary>
        /// SPL Stake Pool Program Name
        /// </summary>
        public static readonly string StakePoolProgramName = "Stake Pool Program";

        // Instance vars

        /// <summary>
        /// The owner key required to use as the fee account owner.  
        /// </summary>
        public virtual PublicKey OwnerKey => new("HfoTxFR1Tm6kGmWgYWD6J7YHVy1UwqSULUGVLXkJqaKN");

        /// <summary>
        /// Represents the byte array encoding of the ASCII string "withdraw".
        /// </summary>
        /// <remarks>This field is used to identify the "withdraw" authority in a byte array format. It is
        /// encoded using ASCII encoding.</remarks>
        private static readonly byte[] AUTHORITY_WITHDRAW = Encoding.ASCII.GetBytes("withdraw");

        /// <summary>
        /// Represents the prefix used for generating transient stake seeds.
        /// </summary>
        /// <remarks>This prefix is encoded as an ASCII byte array and is used in conjunction with other
        /// data  to generate unique transient stake seeds. The value is constant and cannot be modified.</remarks>
        private static readonly byte[] TRANSIENT_STAKE_SEED_PREFIX =
            Encoding.ASCII.GetBytes("transient");

        /// <summary>
        /// Stake Pool account layout size.
        /// </summary>
        public static readonly ulong StakePoolAccountDataSize = 255;

        /// <summary>
        /// Represents the Stake Pool Program, a specialized program for managing stake pools on the Solana blockchain.
        /// </summary>
        /// <remarks>This program provides functionality for interacting with stake pools, including
        /// creating, managing, and querying stake pool accounts. It is identified by the program ID key and name
        /// specific to the Stake Pool Program.</remarks>
        public StakePoolProgram() : base(StakePoolProgramIdKey, StakePoolProgramName) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="StakePoolProgram"/> class with the specified program ID.
        /// </summary>
        /// <param name="programId">The public key identifying the stake pool program.</param>
        public StakePoolProgram(PublicKey programId) : base(programId, StakePoolProgramName) { }

        #region Transaction Methods

        /// <summary>
        /// Creates an Initialize instruction (initialize a new stake pool).
        /// </summary>
        /// <param name="programId"></param>
        /// <param name="stakePoolAccount"></param>
        /// <param name="manager"></param>
        /// <param name="staker"></param>
        /// <param name="withdrawAuthority"></param>
        /// <param name="validatorList"></param>
        /// <param name="reserveStake"></param>
        /// <param name="poolMint"></param>
        /// <param name="managerPoolAccount"></param>
        /// <param name="tokenProgramId"></param>
        /// <param name="fee"></param>
        /// <param name="withdrawalFee"></param>
        /// <param name="depositFee"></param>
        /// <param name="referralFee"></param>
        /// <param name="depositAuthority"></param>
        /// <param name="maxValidators"></param>
        /// <returns></returns>
        public virtual TransactionInstruction Initialize(
            PublicKey stakePoolAccount,
            PublicKey manager,
            PublicKey staker,
            PublicKey withdrawAuthority,
            PublicKey validatorList,
            PublicKey reserveStake,
            PublicKey poolMint,
            PublicKey managerPoolAccount,
            PublicKey tokenProgramId,
            Fee fee,
            Fee withdrawalFee,
            Fee depositFee,
            Fee referralFee,
            PublicKey depositAuthority = null,
            uint? maxValidators = null
        )
        {
            // Prepare the instruction data
            var data = StakePoolProgramData.EncodeInitializeData(fee, withdrawalFee, depositFee, referralFee, maxValidators);
            // Prepare the accounts for the instruction
            var keys = new List<AccountMeta>
            {
                AccountMeta.ReadOnly(StakePoolProgramIdKey, false),
                AccountMeta.ReadOnly(SysVars.ClockKey, false),
                AccountMeta.ReadOnly(SysVars.RentKey, false),
                AccountMeta.ReadOnly(SystemProgram.ProgramIdKey, false),
            };

            if(depositAuthority != null)
            {
                keys.Add(AccountMeta.ReadOnly(depositAuthority, false));
            }

            // Return the transaction instruction
            return new TransactionInstruction
            {
                ProgramId = StakePoolProgramIdKey.KeyBytes,
                Keys = keys,
                Data = data
            };
        }

        /// <summary>
        /// Creates an AddValidatorToPool instruction (add validator stake account to the pool).
        /// </summary>
        /// <param name="stakePoolAccount"></param>
        /// <param name="staker"></param>
        /// <param name="reserve"></param>
        /// <param name="stakePoolWithdraw"></param>
        /// <param name="validatorList"></param>
        /// <param name="stakeAccount"></param>
        /// <param name="validatorAccount"></param>
        /// <param name="seed"></param>
        /// <returns></returns>
        public virtual TransactionInstruction AddValidatorToPool(
            PublicKey stakePoolAccount,
            PublicKey staker,
            PublicKey reserve,
            PublicKey stakePoolWithdraw,
            PublicKey validatorList,
            PublicKey stakeAccount,
            PublicKey validatorAccount,
            uint? seed = null
        )
        {
            // Prepare the instruction data
            var data = StakePoolProgramData.EncodeAddValidatorToPoolData(seed);

            // Prepare the accounts for the instruction
            List<AccountMeta> keys = new()
            {
                AccountMeta.Writable(stakePoolAccount, false),
                AccountMeta.ReadOnly(staker, true),
                AccountMeta.Writable(reserve, false),
                AccountMeta.ReadOnly(stakePoolWithdraw, false),
                AccountMeta.Writable(validatorList, false),
                AccountMeta.Writable(stakeAccount, false),
                AccountMeta.ReadOnly(validatorAccount, false),
                AccountMeta.ReadOnly(SysVars.RentKey, false),
                AccountMeta.ReadOnly(SysVars.ClockKey, false),
                AccountMeta.ReadOnly(StakeProgram.ProgramIdKey, false),
                AccountMeta.ReadOnly(SystemProgram.ProgramIdKey, false),
                AccountMeta.ReadOnly(StakeProgram.ProgramIdKey, false),
            };

            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey.KeyBytes,
                Keys = keys,
                Data = data
            };
        }

        /// <summary>
        /// Creates an RemoveValidatorFromPool instruction (remove validator stake account from the pool).
        /// </summary>
        /// <param name="stakePool"></param>
        /// <param name="staker"></param>
        /// <param name="stakePoolWithdraw"></param>
        /// <param name="validatorList"></param>
        /// <param name="stakeAccount"></param>
        /// <param name="transientStakeAccount"></param>
        /// <returns></returns>
        public virtual TransactionInstruction RemoveValidatorFromPool(
            PublicKey stakePool,
            PublicKey staker,
            PublicKey stakePoolWithdraw,
            PublicKey validatorList,
            PublicKey stakeAccount,
            PublicKey transientStakeAccount
        )
        {
            // Prepare the instruction data
            var data = StakePoolProgramData.EncodeRemoveValidatorFromPoolData();

            // Prepare the accounts that will be involved in this instruction
            List<AccountMeta> keys = new()
            {
                AccountMeta.Writable(stakePool, false),
                AccountMeta.ReadOnly(staker, true),
                AccountMeta.ReadOnly(stakePoolWithdraw, false),
                AccountMeta.Writable(validatorList, false),
                AccountMeta.Writable(stakeAccount, false),
                AccountMeta.Writable(transientStakeAccount, false),
                AccountMeta.ReadOnly(SysVars.ClockKey, false),
                AccountMeta.ReadOnly(StakeProgram.ProgramIdKey, false),
            };

            // Return the TransactionInstruction
            return new TransactionInstruction
            {
                ProgramId = StakePoolProgramIdKey.KeyBytes,
                Keys = keys,
                Data = data
            };
        }

        /// <summary>
        /// Creates the 'DecreaseValidatorStake' instruction (rebalance from validator account to transient account).
        /// </summary>
        /// <param name="stakePool"></param>
        /// <param name="staker"></param>
        /// <param name="stakePoolWithdrawAuthority"></param>
        /// <param name="validatorList"></param>
        /// <param name="validatorStake"></param>
        /// <param name="transientStake"></param>
        /// <param name="lamports"></param>
        /// <param name="transientStakeSeed"></param>
        /// <returns></returns>
        [Obsolete("Use 'decrease_validator_stake_with_reserve' instead")]
        public virtual TransactionInstruction DecreaseValidatorStake(
            PublicKey stakePool,
            PublicKey staker,
            PublicKey stakePoolWithdrawAuthority,
            PublicKey validatorList,
            PublicKey validatorStake,
            PublicKey transientStake,
            ulong lamports,
            ulong transientStakeSeed)
        {
            // Prepare the instruction data
            var data = StakePoolProgramData.EncodeDecreaseValidatorStakeData(lamports, transientStakeSeed);

            // Prepare the accounts for the instruction
            var keys = new List<AccountMeta>
            {
                AccountMeta.ReadOnly(stakePool, false),
                AccountMeta.ReadOnly(staker, true),
                AccountMeta.ReadOnly(stakePoolWithdrawAuthority, false),
                AccountMeta.Writable(validatorList, false),
                AccountMeta.Writable(validatorStake, false),
                AccountMeta.Writable(transientStake, false),
                AccountMeta.ReadOnly(SysVars.ClockKey, false),
                AccountMeta.ReadOnly(SysVars.RentKey, false),
                AccountMeta.ReadOnly(SystemProgram.ProgramIdKey, false),
                AccountMeta.ReadOnly(StakeProgram.ProgramIdKey, false),
            };

            // Return the transaction instruction
            return new TransactionInstruction
            {
                ProgramId = StakePoolProgramIdKey.KeyBytes,
                Keys = keys,
                Data = data
            };
        }

        /// <summary>
        /// Creates the 'DecreaseAdditionalValidatorStake' instruction (rebalance from validator account to transient account).
        /// </summary>
        /// <param name="stakePool"></param>
        /// <param name="staker"></param>
        /// <param name="stakePoolWithdrawAuthority"></param>
        /// <param name="validatorList"></param>
        /// <param name="reserveStake"></param>
        /// <param name="validatorStake"></param>
        /// <param name="ephemeralStake"></param>
        /// <param name="transientStake"></param>
        /// <param name="lamports"></param>
        /// <param name="transientStakeSeed"></param>
        /// <param name="ephemeralStakeSeed"></param>
        /// <returns></returns>
        public virtual TransactionInstruction DecreaseAdditionalValidatorStake(
            PublicKey stakePool,
            PublicKey staker,
            PublicKey stakePoolWithdrawAuthority,
            PublicKey validatorList,
            PublicKey reserveStake,
            PublicKey validatorStake,
            PublicKey ephemeralStake,
            PublicKey transientStake,
            ulong lamports,
            ulong transientStakeSeed,
            ulong ephemeralStakeSeed)
        {
            // Prepare the instruction data
            var data = StakePoolProgramData.EncodeDecreaseAdditionalValidatorStakeData(lamports, transientStakeSeed, ephemeralStakeSeed);

            // Prepare the accounts for the instruction
            var keys = new List<AccountMeta>
        {
            AccountMeta.ReadOnly(stakePool, false),
            AccountMeta.ReadOnly(staker, true),
            AccountMeta.ReadOnly(stakePoolWithdrawAuthority, false),
            AccountMeta.Writable(validatorList, false),
            AccountMeta.Writable(reserveStake, false),
            AccountMeta.Writable(validatorStake, false),
            AccountMeta.Writable(ephemeralStake, false),
            AccountMeta.Writable(transientStake, false),
            AccountMeta.ReadOnly(SysVars.ClockKey, false),
            AccountMeta.ReadOnly(SysVars.StakeHistoryKey, false),
            AccountMeta.ReadOnly(SystemProgram.ProgramIdKey, false),
            AccountMeta.ReadOnly(StakeProgram.ProgramIdKey, false),
        };

            // Return the transaction instruction
            return new TransactionInstruction
            {
                ProgramId = StakePoolProgramIdKey.KeyBytes,
                Keys = keys,
                Data = data
            };
        }

        /// <summary>
        /// Creates the 'DecreaseValidatorStakeWithReserve' instruction (rebalance from validator account to transient account).
        /// </summary>
        /// <param name="stakePool"></param>
        /// <param name="staker"></param>
        /// <param name="stakePoolWithdrawAuthority"></param>
        /// <param name="validatorList"></param>
        /// <param name="reserveStake"></param>
        /// <param name="validatorStake"></param>
        /// <param name="transientStake"></param>
        /// <param name="lamports"></param>
        /// <param name="transientStakeSeed"></param>
        /// <returns></returns>
        public virtual TransactionInstruction DecreaseValidatorStakeWithReserve(
            PublicKey stakePool,
            PublicKey staker,
            PublicKey stakePoolWithdrawAuthority,
            PublicKey validatorList,
            PublicKey reserveStake,
            PublicKey validatorStake,
            PublicKey transientStake,
            ulong lamports,
            ulong transientStakeSeed)
        {
            // Prepare the instruction data
            var data = StakePoolProgramData.EncodeDecreaseValidatorStakeWithReserveData(lamports, transientStakeSeed);

            // Prepare the accounts for the instruction
            var keys = new List<AccountMeta>
        {
            AccountMeta.ReadOnly(stakePool, false),
            AccountMeta.ReadOnly(staker, true),
            AccountMeta.ReadOnly(stakePoolWithdrawAuthority, false),
            AccountMeta.Writable(validatorList, false),
            AccountMeta.Writable(reserveStake, false),
            AccountMeta.Writable(validatorStake, false),
            AccountMeta.Writable(transientStake, false),
            AccountMeta.ReadOnly(SysVars.ClockKey, false),
            AccountMeta.ReadOnly(SysVars.StakeHistoryKey, false),
            AccountMeta.ReadOnly(SystemProgram.ProgramIdKey, false),
            AccountMeta.ReadOnly(StakeProgram.ProgramIdKey, false),
        };

            // Return the transaction instruction
            return new TransactionInstruction
            {
                ProgramId = StakePoolProgramIdKey.KeyBytes,
                Keys = keys,
                Data = data
            };
        }

        public virtual TransactionInstruction IncreaseValidatorStake
        (
            PublicKey stakePool,
            PublicKey staker,
            PublicKey stakePoolWithdrawAuthority,
            PublicKey validatorList,
            PublicKey reserveStake,
            PublicKey transientStake,
            PublicKey validatorStake,
            ulong lamports,
            ulong transientStakeSeed
        )
        { 
            var data = StakePoolProgramData.EncodeIncreaseValidatorStakeData(lamports, transientStakeSeed);

            var keys = new List<AccountMeta>
            {
                AccountMeta.ReadOnly(stakePool, false),
                AccountMeta.ReadOnly(staker, true),
                AccountMeta.ReadOnly(stakePoolWithdrawAuthority, false),
                AccountMeta.Writable(validatorList, false),
                AccountMeta.Writable(reserveStake, false),
                AccountMeta.Writable(transientStake, false),
                AccountMeta.ReadOnly(validatorStake, false),
                AccountMeta.ReadOnly(SysVars.ClockKey, false),
                AccountMeta.ReadOnly(SysVars.RentKey, false),
                AccountMeta.ReadOnly(SysVars.StakeHistoryKey, false),
            };

            return new TransactionInstruction
            {
                ProgramId = StakePoolProgramIdKey.KeyBytes,
                Keys = keys,
                Data = data
            };
        }

        /// <summary>
        /// Creates the 'IncreaseAdditionalValidatorStake' instruction (rebalance from validator account to transient account).
        /// </summary>
        /// <param name="stakePool"></param>
        /// <param name="staker"></param>
        /// <param name="stakePoolWithdrawAuthority"></param>
        /// <param name="validatorList"></param>
        /// <param name="reserveStake"></param>
        /// <param name="ephemeralStake"></param>
        /// <param name="transientStake"></param>
        /// <param name="validatorStake"></param>
        /// <param name="validator"></param>
        /// <param name="lamports"></param>
        /// <param name="transientStakeSeed"></param>
        /// <param name="ephemeralStakeSeed"></param>
        /// <returns></returns>
        public virtual TransactionInstruction IncreaseAdditionalValidatorStake(
            PublicKey stakePool,
            PublicKey staker,
            PublicKey stakePoolWithdrawAuthority,
            PublicKey validatorList,
            PublicKey reserveStake,
            PublicKey ephemeralStake,
            PublicKey transientStake,
            PublicKey validatorStake,
            PublicKey validator,
            ulong lamports,
            ulong transientStakeSeed,
            ulong ephemeralStakeSeed)
        {
            // Prepare the instruction data
            var data = StakePoolProgramData.EncodeIncreaseAdditionalValidatorStakeData(lamports, transientStakeSeed, ephemeralStakeSeed);

            // Prepare the accounts for the instruction
            var keys = new List<AccountMeta>
        {
            AccountMeta.ReadOnly(stakePool, false),
            AccountMeta.ReadOnly(staker, true),
            AccountMeta.ReadOnly(stakePoolWithdrawAuthority, false),
            AccountMeta.Writable(validatorList, false),
            AccountMeta.Writable(reserveStake, false),
            AccountMeta.Writable(ephemeralStake, false),
            AccountMeta.Writable(transientStake, false),
            AccountMeta.ReadOnly(validatorStake, false),
            AccountMeta.ReadOnly(validator, false),
            AccountMeta.ReadOnly(SysVars.ClockKey, false),
            AccountMeta.ReadOnly(SysVars.StakeHistoryKey, false),
            AccountMeta.ReadOnly(SystemProgram.ProgramIdKey, false),
            AccountMeta.ReadOnly(StakeProgram.ProgramIdKey, false),
        };

            // Return the transaction instruction
            return new TransactionInstruction
            {
                ProgramId = StakePoolProgramIdKey.KeyBytes,
                Keys = keys,
                Data = data
            };
        }



#nullable enable

        /// <summary>
        /// Creates the 'SetPreferredDepositValidator' instruction (set preferred deposit validator).
        /// </summary>
        /// <param name="stakePoolAddress"></param>
        /// <param name="staker"></param>
        /// <param name="validatorListAddress"></param>
        /// <param name="validatorType"></param>
        /// <param name="validatorVoteAddress"></param>
        /// <returns></returns>
        public virtual TransactionInstruction SetPreferredDepositValidator(
           PublicKey stakePoolAddress,
           PublicKey staker,
           PublicKey validatorListAddress,
           PreferredValidatorType validatorType,
           PublicKey? validatorVoteAddress = null)
        {
            // Prepare the instruction data for setting the preferred deposit validator
            var data = StakePoolProgramData.EncodeSetPreferredValidatorData(validatorType, validatorVoteAddress);

            // Prepare the accounts for the instruction
            var keys = new List<AccountMeta>
           {
               AccountMeta.Writable(stakePoolAddress, false),
               AccountMeta.ReadOnly(staker, true),
               AccountMeta.ReadOnly(validatorListAddress, false)
           };

            // Return the transaction instruction
            return new TransactionInstruction
            {
                ProgramId = StakePoolProgramIdKey.KeyBytes,
                Keys = keys,
                Data = data
            };
        }

        /// <summary>
        /// Creates an 'AddValidatorToPoolWithVote' instruction given an existing stake pool and vote account.
        /// </summary>
        /// <param name="stakePool"></param>
        /// <param name="stakePoolAddress"></param>
        /// <param name="voteAccountAddress"></param>
        /// <param name="seed"></param>
        /// <returns></returns>
        /// <exception cref="InvalidProgramException"></exception>
        public virtual TransactionInstruction AddValidatorToPoolWithVote(
            Models.StakePool stakePool,
            PublicKey stakePoolAddress,
            PublicKey voteAccountAddress,
            uint? seed = null)
        {
            // Find the program address for the withdraw authority
            if (!PublicKey.TryFindProgramAddress([stakePoolAddress.KeyBytes], ProgramIdKey, out var poolWithdrawalAuthority, out var nonce))
                throw new InvalidProgramException();
            // Find the stake account address for the validator using the vote account and seed
            if (!PublicKey.TryFindProgramAddress([voteAccountAddress.KeyBytes], ProgramIdKey, out var stakeAccountAddress, out var _))
                throw new InvalidProgramException();

            // Generate the instruction to add the validator to the pool
            return AddValidatorToPool(
                StakePoolProgramIdKey,
                stakePool.Staker,
                stakePool.ReserveStake,
                poolWithdrawalAuthority,
                stakePool.ValidatorList,
                stakeAccountAddress,
                voteAccountAddress,
                seed
            );
        }


        /// <summary>
        /// Creates an 'RemoveValidatorFromPoolWithVote' instruction given an existing stake pool and vote account.
        /// </summary>
        /// <param name="programId"></param>
        /// <param name="stakePool"></param>
        /// <param name="stakePoolAddress"></param>
        /// <param name="voteAccountAddress"></param>
        /// <param name="validatorStakeSeed"></param>
        /// <param name="transientStakeSeed"></param>
        /// <returns></returns>
        public virtual TransactionInstruction RemoveValidatorFromPoolWithVote(
            Models.StakePool stakePool, // Fully qualify the type to avoid ambiguity
            PublicKey stakePoolAddress,
            PublicKey voteAccountAddress,
            uint? validatorStakeSeed = null,
            ulong transientStakeSeed = 0)
        {
            // Find the program address for the withdraw authority
            if (!PublicKey.TryFindProgramAddress([stakePoolAddress.KeyBytes], ProgramIdKey, out var poolWithdrawalAuthority, out var nonce))
                throw new InvalidProgramException();
            
            // Find the stake account address for the validator using the vote account and seed
            if (!PublicKey.TryFindProgramAddress([voteAccountAddress.KeyBytes], ProgramIdKey, out var stakeAccountAddress, out var _))
                throw new InvalidProgramException();

            // Find the transient stake account using the vote account, stake pool address, and transient stake seed
            if (!PublicKey.TryFindProgramAddress([voteAccountAddress.KeyBytes], ProgramIdKey, out var transientStakeAccount, out var _))
                throw new InvalidProgramException();

            // Create the RemoveValidatorFromPool instruction
            return RemoveValidatorFromPool(
                StakePoolProgramIdKey,
                stakePool.Staker,
                poolWithdrawalAuthority,
                stakePool.ValidatorList,
                stakeAccountAddress,
                transientStakeAccount
            );
        }

        /// <summary>
        /// Creates an 'IncreaseValidatorStakeWithVote' instruction given an existing stake pool and vote account.
        /// </summary>
        /// <param name="programId"></param>
        /// <param name="stakePool"></param>
        /// <param name="stakePoolAddress"></param>
        /// <param name="voteAccountAddress"></param>
        /// <param name="lamports"></param>
        /// <param name="validatorStakeSeed"></param>
        /// <param name="transientStakeSeed"></param>
        /// <returns></returns>
        public static TransactionInstruction IncreaseValidatorStakeWithVote(
            PublicKey programId,
            Models.StakePool stakePool,
            PublicKey stakePoolAddress,
            PublicKey voteAccountAddress,
            ulong lamports,
            uint? validatorStakeSeed,
            ulong transientStakeSeed
        )
        {
            // Find the addresses using helper methods
            var poolWithdrawAuthority = FindWithdrawAuthorityProgramAddress(stakePoolAddress);
            var transientStakeAddress = FindTransientStakeProgramAddress(voteAccountAddress, stakePoolAddress, transientStakeSeed);
            var validatorStakeAddress = FindStakeProgramAddress(voteAccountAddress, stakePoolAddress, validatorStakeSeed);

            // Create the instruction accounts (same structure as the Rust version)
            var accounts = new List<AccountMeta>
        {
            AccountMeta.ReadOnly(stakePoolAddress, false),
            AccountMeta.Writable(stakePool.Staker, true),
            AccountMeta.ReadOnly(poolWithdrawAuthority, false),
            AccountMeta.ReadOnly(stakePool.ValidatorList, false),
            AccountMeta.ReadOnly(stakePool.ReserveStake, false),
            AccountMeta.ReadOnly(transientStakeAddress, false),
            AccountMeta.ReadOnly(validatorStakeAddress, false),
            AccountMeta.ReadOnly(voteAccountAddress, false)
        };

            // Serialize the instruction data (this is similar to `borsh::to_vec` in Rust)
            var data = SerializeIncreaseValidatorStakeData(lamports, transientStakeSeed);

            // Return the instruction
            return new TransactionInstruction
            {
                ProgramId = programId,
                Keys = accounts,
                Data = data
            };
        }

        /// <summary>
        /// Finds the program-derived address for the withdraw authority.
        /// </summary>
        /// <param name="stakePoolAddress"></param>
        /// <returns></returns>
        public static PublicKey FindWithdrawAuthorityProgramAddress(PublicKey stakePoolAddress)
        {
            // Seeds must be provided in the exact same order used in Rust.
            var seeds = new[]
            {
                stakePoolAddress.KeyBytes, // stake pool pubkey as bytes
                AUTHORITY_WITHDRAW         // "withdraw"
            };

            // Solnet exposes FindProgramAddress; it yields the PDA and out‑param bump.
            if (!PublicKey.TryFindProgramAddress(seeds, StakePoolProgramIdKey, out PublicKey address, out byte bump))
            {
                throw new InvalidProgramException();
            }

            return address;
        }

        /// <summary>
        /// Serializes the data for the 'IncreaseValidatorStake' instruction.
        /// </summary>
        /// <param name="lamports"></param>
        /// <param name="transientStakeSeed"></param>
        /// <returns></returns>
        public static byte[] SerializeIncreaseValidatorStakeData(ulong lamports, ulong transientStakeSeed)
        {
            // Placeholder: actual serialization would be required based on your program's structure.
            return new byte[] { (byte)lamports, (byte)transientStakeSeed };
        }

        /// <summary>
        /// Finds the stake program address for a validator's vote account.
        /// </summary>
        /// <param name="voteAccountAddress"></param>
        /// <param name="stakePoolAddress"></param>
        /// <param name="validatorStakeSeed"></param>
        /// <returns></returns>
        public static PublicKey FindStakeProgramAddress(
            PublicKey voteAccountAddress,
            PublicKey stakePoolAddress,
            uint? validatorStakeSeed
        )
        {
            if (validatorStakeSeed.HasValue && validatorStakeSeed.Value == 0)
                throw new ArgumentException("Seed must be non‑zero (Rust NonZeroU32).", nameof(validatorStakeSeed));

            // Convert the seed (if provided) to little‑endian bytes.
            byte[] seedBytes = Array.Empty<byte>();
            if (validatorStakeSeed.HasValue)
            {
                seedBytes = BitConverter.GetBytes(validatorStakeSeed.Value);              // platform‑endian -> little‑endian
                if (!BitConverter.IsLittleEndian) Array.Reverse(seedBytes); // ensure LE on big‑endian CPUs
            }

            // Seeds must be passed in the exact order used in Rust.
            var seeds = new[]
            {
                voteAccountAddress.KeyBytes,    // vote‑account pubkey
                stakePoolAddress.KeyBytes,
                seedBytes                // may be empty
            };

            // Fix: Correctly call TryFindProgramAddress with all required parameters
            if (!PublicKey.TryFindProgramAddress(seeds, StakePoolProgramIdKey, out PublicKey pda, out byte bump))
            {
                throw new InvalidProgramException();
            }

            return pda;
        }

        /// <summary>
        /// Finds the transient stake program address for a given vote account and stake pool.
        /// </summary>
        /// <param name="voteAccountAddress"></param>
        /// <param name="stakePoolAddress"></param>
        /// <param name="transientStakeSeed"></param>
        /// <returns></returns>
        public static PublicKey FindTransientStakeProgramAddress(
            PublicKey voteAccountAddress,
            PublicKey stakePoolAddress,
            ulong transientStakeSeed)
        {
            // Convert the u64 seed to little‑endian bytes (8 bytes).
            byte[] seedBytes = BitConverter.GetBytes(transientStakeSeed);
            if (!BitConverter.IsLittleEndian)
                Array.Reverse(seedBytes); // ensure LE on big‑endian machines

            // Build seed list in the exact order used in Rust.
            var seeds = new List<byte[]>
            {
                TRANSIENT_STAKE_SEED_PREFIX,
                voteAccountAddress.KeyBytes,
                stakePoolAddress.KeyBytes,
                seedBytes
            };

            // Fix: Correctly call TryFindProgramAddress with all required parameters
            if (!PublicKey.TryFindProgramAddress(seeds, StakePoolProgramIdKey, out PublicKey pda, out byte bump))
            {
                throw new InvalidProgramException();
            }

            return pda;
        }


        #endregion
    }
}
