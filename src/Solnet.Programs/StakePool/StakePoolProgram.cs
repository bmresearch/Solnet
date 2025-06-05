using Solnet.Rpc.Models;
using Solnet.Wallet;
using System.Collections.Generic;
using Solnet.Programs.Abstract;
using System;
using System.Text;
using Solnet.Programs.StakePool.Models;
using static Solnet.Programs.Models.Stake.State;
using System.Linq;

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
        /// Mpl Token Metadata Program ID
        /// </summary>
        public static readonly PublicKey MplTokenMetadataProgramIdKey = new("metaqbxxUerdq28cj1RbAWkYQm3ybzjb6a8bt518x1s");

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
        /// Represents the seed for deposit authority.
        /// </summary>
        /// <remarks>This seed identifies the "deposit" authority for a stake pool in a byte array format.</remarks>
        private static readonly byte[] AUTHORITY_DEPOSIT = Encoding.ASCII.GetBytes("deposit");

        /// <summary>
        /// Represents the prefix used for generating transient stake seeds.
        /// </summary>
        /// <remarks>This prefix is encoded as an ASCII byte array and is used in conjunction with other
        /// data  to generate unique transient stake seeds. The value is constant and cannot be modified.</remarks>
        private static readonly byte[] TRANSIENT_STAKE_SEED_PREFIX =
            Encoding.ASCII.GetBytes("transient");

        /// <summary>
        /// Represents the seed for ephemeral stake account.
        /// </summary>
        private static readonly byte[] EPHEMERAL_STAKE_SEED_PREFIX = Encoding.ASCII.GetBytes("ephemeral");

        /// <summary>
        /// Stake Pool account layout size.
        /// </summary>
        public static readonly ulong StakePoolAccountDataSize = 255;

        /// <summary>
        /// Minimum amount of staked lamports required in a validator stake account to
        /// allow for merges without a mismatch on credits observed.
        /// </summary>
        public const ulong MINIMUM_ACTIVE_STAKE = 1_000_000;

        /// <summary>
        /// Minimum amount of lamports in the reserve.
        /// </summary>
        public const ulong MINIMUM_RESERVE_LAMPORTS = 0;

        /// <summary>
        /// Maximum amount of validator stake accounts to update per
        /// <c>UpdateValidatorListBalance</c> instruction, based on compute limits.
        /// </summary>
        public const int MAX_VALIDATORS_TO_UPDATE = 5;

        /// <summary>
        /// Maximum factor by which a withdrawal fee can be increased per epoch,
        /// protecting stakers from malicious users.
        /// If current fee is 0, WITHDRAWAL_BASELINE_FEE is used as the baseline.
        /// </summary>
        public static readonly Fee MAX_WITHDRAWAL_FEE_INCREASE = new Fee(3, 2);

        /// <summary>
        /// Drop-in baseline fee when evaluating withdrawal fee increases when fee is 0.
        /// </summary>
        public static readonly Fee WITHDRAWAL_BASELINE_FEE = new Fee(1, 1000);

        /// <summary>
        /// The maximum number of transient stake accounts respecting transaction account limits.
        /// </summary>
        public const int MAX_TRANSIENT_STAKE_ACCOUNTS = 10;

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
                AccountMeta.Writable(stakePoolAccount, false),
                AccountMeta.ReadOnly(manager, true),
                AccountMeta.ReadOnly(staker, false),
                AccountMeta.ReadOnly(withdrawAuthority, false),
                AccountMeta.Writable(validatorList, false),
                AccountMeta.ReadOnly(reserveStake, false),
                AccountMeta.Writable(poolMint, false),
                AccountMeta.Writable(managerPoolAccount, false),
                AccountMeta.ReadOnly(tokenProgramId, false)
            };

            if (depositAuthority != null)
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
            // dont allow zero seed values
            if (seed == 0)
                throw new ArgumentException("Value must be nonzero.", nameof(seed));

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

        /// <summary>
        /// Creates the 'IncreaseValidatorStake' instruction (rebalance from transient account to validator account).
        /// </summary>
        /// <param name="stakePool"></param>
        /// <param name="staker"></param>
        /// <param name="stakePoolWithdrawAuthority"></param>
        /// <param name="validatorList"></param>
        /// <param name="reserveStake"></param>
        /// <param name="transientStake"></param>
        /// <param name="validatorStake"></param>
        /// <param name="lamports"></param>
        /// <param name="transientStakeSeed"></param>
        /// <returns></returns>
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
                AccountMeta.ReadOnly(StakeProgram.ConfigKey, false),
                AccountMeta.ReadOnly(SystemProgram.ProgramIdKey, false),
                AccountMeta.ReadOnly(StakeProgram.ProgramIdKey, false),
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
            AccountMeta.ReadOnly(StakeProgram.ConfigKey, false),
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
        /// Creates the 'Redelegate' instruction (rebalance from one validator account to another).
        /// </summary>
        /// <remarks>
        /// This instruction is deprecated since 2.0.0. The stake redelegate instruction will not be enabled.
        /// </remarks>
        [Obsolete("The stake redelegate instruction used in this will not be enabled. Since 2.0.0", true)]
        public static TransactionInstruction Redelegate(
            PublicKey stakePool,
            PublicKey staker,
            PublicKey stakePoolWithdrawAuthority,
            PublicKey validatorList,
            PublicKey reserveStake,
            PublicKey sourceValidatorStake,
            PublicKey sourceTransientStake,
            PublicKey ephemeralStake,
            PublicKey destinationTransientStake,
            PublicKey destinationValidatorStake,
            PublicKey validator,
            ulong lamports,
            ulong sourceTransientStakeSeed,
            ulong ephemeralStakeSeed,
            ulong destinationTransientStakeSeed)
        {
            var accounts = new List<AccountMeta>
            {
                AccountMeta.ReadOnly(stakePool, false),
                AccountMeta.ReadOnly(staker, true),
                AccountMeta.ReadOnly(stakePoolWithdrawAuthority, false),
                AccountMeta.Writable(validatorList, false),
                AccountMeta.Writable(reserveStake, false),
                AccountMeta.Writable(sourceValidatorStake, false),
                AccountMeta.Writable(sourceTransientStake, false),
                AccountMeta.Writable(ephemeralStake, false),
                AccountMeta.Writable(destinationTransientStake, false),
                AccountMeta.ReadOnly(destinationValidatorStake, false),
                AccountMeta.ReadOnly(validator, false),
                AccountMeta.ReadOnly(SysVars.ClockKey, false),
                AccountMeta.ReadOnly(SysVars.StakeHistoryKey, false),
                AccountMeta.ReadOnly(StakeProgram.ConfigKey, false),
                AccountMeta.ReadOnly(SystemProgram.ProgramIdKey, false),
                AccountMeta.ReadOnly(StakeProgram.ProgramIdKey, false)
            };

            var data = StakePoolProgramData.EncodeRedelegateData(lamports, sourceTransientStakeSeed, ephemeralStakeSeed, destinationTransientStakeSeed);

            return new TransactionInstruction
            {
                ProgramId = StakePoolProgramIdKey,
                Keys = accounts,
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
        /// <param name="validatorVoteAddress">Optional public key; if provided, it must be in the keys list.</param>
        /// <returns></returns>
        public virtual TransactionInstruction SetPreferredDepositValidator(
            PublicKey stakePoolAddress,
            PublicKey staker,
            PublicKey validatorListAddress,
            PreferredValidatorType validatorType,
            PublicKey? validatorVoteAddress = null)
        {
            // Prepare Account Metas.
            var keys = new List<AccountMeta>
            {
                AccountMeta.Writable(stakePoolAddress, false),
                AccountMeta.ReadOnly(staker, true),
                AccountMeta.ReadOnly(validatorListAddress, false)
            };

            // Fix: If a validator vote address is provided, add it to the keys list instead of encoding into data.
            if (validatorVoteAddress != null)
            {
                keys.Add(AccountMeta.ReadOnly(validatorVoteAddress, false));
            }

            // Encode only the instruction discriminator and validator type.
            var data = StakePoolProgramData.EncodeSetPreferredValidatorData(validatorType);

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
            // dont allow zero seed values
            if (seed == 0)
                throw new ArgumentException("Value must be nonzero.", nameof(seed));

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
            // dont allow zero seed values
            if (validatorStakeSeed == 0)
                throw new ArgumentException("Value must be nonzero.", nameof(validatorStakeSeed));

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
        /// <param name="stakePool"></param>
        /// <param name="stakePoolAddress"></param>
        /// <param name="voteAccountAddress"></param>
        /// <param name="lamports"></param>
        /// <param name="validatorStakeSeed"></param>
        /// <param name="transientStakeSeed"></param>
        /// <returns></returns>
        public static TransactionInstruction IncreaseValidatorStakeWithVote(
            Models.StakePool stakePool,
            PublicKey stakePoolAddress,PublicKey voteAccountAddress,
            ulong lamports,
            uint? validatorStakeSeed,
            ulong transientStakeSeed
        )
        {
            // dont allow zero seed values
            if (validatorStakeSeed == 0)
                throw new ArgumentException("Value must be nonzero.", nameof(validatorStakeSeed));

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
                ProgramId = StakePoolProgramIdKey,
                Keys = accounts,
                Data = data
            };
        }

        /// <summary>
        /// Creates a DecreaseValidatorStake instruction (rebalance from validator account to transient account)
        /// given an existing stake pool and vote account.
        /// </summary>
        /// <param name="stakePool">The stake pool model. Provides staker, validator list, and reserve stake.</param>
        /// <param name="stakePoolAddress">The address of the stake pool.</param>
        /// <param name="voteAccountAddress">The vote account address.</param>
        /// <param name="lamports">The amount of lamports.</param>
        /// <param name="validatorStakeSeed">
        /// An optional nonzero seed for the validator stake account (represented by a uint?; zero is disallowed).
        /// </param>
        /// <param name="transientStakeSeed">The transient stake seed.</param>
        /// <returns>The corresponding transaction instruction.</returns>
        public virtual TransactionInstruction DecreaseValidatorStakeWithVote(
            Models.StakePool stakePool,
            PublicKey stakePoolAddress,
            PublicKey voteAccountAddress,
            ulong lamports,
            uint? validatorStakeSeed,
            ulong transientStakeSeed)
        {
            // Ensure the optional validator stake seed is nonzero.
            if (validatorStakeSeed == 0)
                throw new ArgumentException("Value must be nonzero.", nameof(validatorStakeSeed));

            // Find the pool withdrawal authority.
            var poolWithdrawalAuthority = FindWithdrawAuthorityProgramAddress(stakePoolAddress);

            // Find the validator stake address using the vote account and the seed.
            var validatorStakeAddress = FindStakeProgramAddress(voteAccountAddress, stakePoolAddress, validatorStakeSeed);

            // Find the transient stake address.
            var transientStakeAddress = FindTransientStakeProgramAddress(voteAccountAddress, stakePoolAddress, transientStakeSeed);

            // Construct the instruction by calling the already implemented decrease_validator_stake_with_reserve method.
            return DecreaseValidatorStakeWithReserve(
                stakePoolAddress,
                stakePool.Staker,
                poolWithdrawalAuthority,
                stakePool.ValidatorList,
                stakePool.ReserveStake,
                validatorStakeAddress,
                transientStakeAddress,
                lamports,
                transientStakeSeed);
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

        /// <summary>
        /// Generates the deposit authority program address for the stake pool.
        /// </summary>
        /// <param name="stakePoolAddress">The stake pool public key.</param>
        /// <returns>A tuple of the derived deposit authority public key and the bump seed.</returns>
        public static (PublicKey, byte) FindDepositAuthorityProgramAddress(PublicKey stakePoolAddress)
        {
            // Seeds must be in the exact order: stake pool address bytes followed by AUTHORITY_DEPOSIT.
            var seeds = new[] { stakePoolAddress.KeyBytes, AUTHORITY_DEPOSIT };
            if (!PublicKey.TryFindProgramAddress(seeds, StakePoolProgramIdKey, out PublicKey address, out byte bump))
                throw new InvalidProgramException("Unable to find deposit authority program address");
            return (address, bump);
        }

        /// <summary>
        /// Generates the ephemeral program address for stake pool redelegation.
        /// </summary>
        /// <param name="stakePoolAddress">The stake pool public key.</param>
        /// <param name="seed">The seed used to generate the ephemeral stake address.</param>
        /// <returns>A tuple of the derived ephemeral stake public key and the bump seed.</returns>
        public static (PublicKey, byte) FindEphemeralStakeProgramAddress(PublicKey stakePoolAddress, ulong seed)
        {
            byte[] seedBytes = BitConverter.GetBytes(seed);
            if (!BitConverter.IsLittleEndian)
            {
                Array.Reverse(seedBytes);
            }

            var seeds = new List<byte[]>
            {
                EPHEMERAL_STAKE_SEED_PREFIX,
                stakePoolAddress.KeyBytes,
                seedBytes
            };

            if (!PublicKey.TryFindProgramAddress(seeds, StakePoolProgramIdKey, out PublicKey address, out byte bump))
            {
                throw new InvalidProgramException("Unable to find ephemeral stake program address");
            }

            return (address, bump);
        }

        /// <summary>
        /// Gets the minimum delegation required by a stake account in a stake pool.
        /// </summary>
        /// <param name="stakeProgramMinimumDelegation">The minimum delegation defined by the stake program.</param>
        /// <returns>The greater value between stakeProgramMinimumDelegation and MINIMUM_ACTIVE_STAKE.</returns>
        public static ulong MinimumDelegation(ulong stakeProgramMinimumDelegation)
        {
            return Math.Max(stakeProgramMinimumDelegation, MINIMUM_ACTIVE_STAKE);
        }

        /// <summary>
        /// Gets the stake amount under consideration when calculating pool token conversions.
        /// </summary>
        /// <param name="meta">The metadata instance containing the rent-exempt reserve value.</param>
        /// <returns>
        /// The sum of <c>meta.RentExemptReserve</c> and <c>MINIMUM_RESERVE_LAMPORTS</c>. If addition overflows,
        /// returns <c>ulong.MaxValue</c>.
        /// </returns>
        public static ulong MinimumReserveLamports(Meta meta)
        {
            ulong reserve = meta.RentExemptReserve;
            ulong addition = MINIMUM_RESERVE_LAMPORTS;
            // Implement saturating addition: if adding addition to reserve would overflow, return ulong.MaxValue.
            if (ulong.MaxValue - reserve < addition)
            {
                return ulong.MaxValue;
            }

            return reserve + addition;
        }

        /// <summary>
        /// Creates an IncreaseAdditionalValidatorStake instruction (rebalance from validator account to transient account)
        /// given an existing stake pool, validator list and vote account.
        /// </summary>
        /// <param name="stakePool">The stake pool model containing staker, validator list, and reserve stake.</param>
        /// <param name="validatorList">The validator list used to locate the corresponding validator info.</param>
        /// <param name="stakePoolAddress">The address of the stake pool.</param>
        /// <param name="voteAccountAddress">The vote account address.</param>
        /// <param name="lamports">The amount of lamports.</param>
        /// <param name="ephemeralStakeSeed">The ephemeral stake seed.</param>
        /// <returns>The transaction instruction to increase additional validator stake.</returns>
        public TransactionInstruction IncreaseAdditionalValidatorStakeWithList(
            Models.StakePool stakePool,
            ValidatorList validatorList,
            PublicKey stakePoolAddress,
            PublicKey voteAccountAddress,
            ulong lamports,
            ulong ephemeralStakeSeed)
        {
            var validatorInfo = validatorList.Find(voteAccountAddress);
            if (validatorInfo == null)
                throw new ArgumentException("Invalid instruction data: vote account was not found in the validator list.", nameof(voteAccountAddress));

            ulong transientStakeSeed = (ulong)validatorInfo.TransientSeedSuffix;
            uint? validatorStakeSeed = validatorInfo.ValidatorSeedSuffix; // assume this property returns a uint
            if (validatorStakeSeed == 0)
                throw new ArgumentException("Invalid instruction data: validator stake seed cannot be zero.", nameof(validatorStakeSeed));

            return IncreaseAdditionalValidatorStakeWithVote(
                stakePool,
                stakePoolAddress,
                voteAccountAddress,
                lamports,
                validatorStakeSeed,
                transientStakeSeed,
                ephemeralStakeSeed);
        }

        /// <summary>
        /// Creates an IncreaseAdditionalValidatorStake instruction given an existing stake pool and vote account.
        /// This helper derives the necessary addresses and serializes the instruction data.
        /// </summary>
        /// <param name="stakePool">The stake pool model.</param>
        /// <param name="stakePoolAddress">The stake pool public key.</param>
        /// <param name="voteAccountAddress">The validator vote account public key.</param>
        /// <param name="lamports">The amount of lamports.</param>
        /// <param name="validatorStakeSeed">
        /// An optional nonzero seed for the validator stake account (zero is disallowed).
        /// </param>
        /// <param name="transientStakeSeed">The transient stake seed.</param>
        /// <param name="ephemeralStakeSeed">The ephemeral stake seed.</param>
        /// <returns>The constructed transaction instruction.</returns>
        public static TransactionInstruction IncreaseAdditionalValidatorStakeWithVote(
            Models.StakePool stakePool,
            PublicKey stakePoolAddress,
            PublicKey voteAccountAddress,
            ulong lamports,
            uint? validatorStakeSeed,
            ulong transientStakeSeed,
            ulong ephemeralStakeSeed)
        {
            // Derive the pool withdraw authority.
            PublicKey poolWithdrawAuthority = FindWithdrawAuthorityProgramAddress(stakePoolAddress);

            // Derive the ephemeral stake address using stake pool address and ephemeral seed.
            PublicKey ephemeralStakeAddress = FindEphemeralStakeProgramAddress(stakePoolAddress, ephemeralStakeSeed).Item1;

            // Derive the transient stake address using the vote account and stake pool address.
            PublicKey transientStakeAddress = FindTransientStakeProgramAddress(voteAccountAddress, stakePoolAddress, transientStakeSeed);

            // Derive the validator stake address using the vote account and stake pool address.
            PublicKey validatorStakeAddress = FindStakeProgramAddress(voteAccountAddress, stakePoolAddress, validatorStakeSeed);

            // Build the account metas.
            var accounts = new List<AccountMeta>
            {
                AccountMeta.ReadOnly(stakePoolAddress, false),
                AccountMeta.Writable(stakePool.Staker, true),
                AccountMeta.ReadOnly(poolWithdrawAuthority, false),
                AccountMeta.ReadOnly(stakePool.ValidatorList, false),
                AccountMeta.ReadOnly(stakePool.ReserveStake, false),
                AccountMeta.Writable(ephemeralStakeAddress, false),
                AccountMeta.Writable(transientStakeAddress, false),
                AccountMeta.ReadOnly(validatorStakeAddress, false),
                AccountMeta.ReadOnly(voteAccountAddress, false),
                AccountMeta.ReadOnly(SysVars.ClockKey, false),
                AccountMeta.ReadOnly(SysVars.StakeHistoryKey, false),
                AccountMeta.ReadOnly(StakeProgram.ConfigKey, false),
                AccountMeta.ReadOnly(SystemProgram.ProgramIdKey, false),
                AccountMeta.ReadOnly(StakeProgram.ProgramIdKey, false),
            };

            // Serialize instruction data.
            byte[] data = StakePoolProgramData.EncodeIncreaseAdditionalValidatorStakeData(lamports, transientStakeSeed, ephemeralStakeSeed);

            return new TransactionInstruction
            {
                ProgramId = StakePoolProgramIdKey,
                Keys = accounts,
                Data = data
            };
        }

        /// <summary>
        /// Creates a DecreaseAdditionalValidatorStake instruction given an existing stake pool, validator list and vote account.
        /// </summary>
        /// <param name="stakePool">The stake pool model containing staker, validator list, and reserve stake.</param>
        /// <param name="validatorList">The list of validator stake info.</param>
        /// <param name="stakePoolAddress">The stake pool public key.</param>
        /// <param name="voteAccountAddress">The validator vote account public key.</param>
        /// <param name="lamports">The amount of lamports to withdraw.</param>
        /// <param name="ephemeralStakeSeed">The ephemeral stake seed.</param>
        /// <returns>The constructed transaction instruction.</returns>
        public TransactionInstruction DecreaseAdditionalValidatorStakeWithList(
            Models.StakePool stakePool,
            ValidatorList validatorList,
            PublicKey stakePoolAddress,
            PublicKey voteAccountAddress,
            ulong lamports,
            ulong ephemeralStakeSeed)
        {
            var validatorInfo = validatorList.Find(voteAccountAddress);
            if (validatorInfo == null)
                throw new ArgumentException("Invalid instruction data: vote account was not found in the validator list.", nameof(voteAccountAddress));

            ulong transientStakeSeed = validatorInfo.TransientSeedSuffix;
            uint? validatorStakeSeed = validatorInfo.ValidatorSeedSuffix;
            if (validatorStakeSeed == 0)
                throw new ArgumentException("Invalid instruction data: validator stake seed cannot be zero.", nameof(validatorStakeSeed));

            return DecreaseAdditionalValidatorStakeWithVote(
                stakePool,
                stakePoolAddress,
                voteAccountAddress,
                lamports,
                validatorStakeSeed,
                transientStakeSeed,
                ephemeralStakeSeed);
        }

        /// <summary>
        /// Creates a DecreaseAdditionalValidatorStake instruction given an existing stake pool and vote account.
        /// This helper derives the necessary addresses and serializes the instruction data. Its output is analogous to the Rust
        /// function `decrease_additional_validator_stake_with_vote`.
        /// </summary>
        /// <param name="stakePool">The stake pool model.</param>
        /// <param name="stakePoolAddress">The stake pool public key.</param>
        /// <param name="voteAccountAddress">The validator vote account public key.</param>
        /// <param name="lamports">The amount of lamports to withdraw.</param>
        /// <param name="validatorStakeSeed">
        /// An optional nonzero seed for the validator stake account (zero is disallowed).
        /// </param>
        /// <param name="transientStakeSeed">The transient stake seed.</param>
        /// <param name="ephemeralStakeSeed">The ephemeral stake seed.</param>
        /// <returns>The constructed transaction instruction.</returns>
        public static TransactionInstruction DecreaseAdditionalValidatorStakeWithVote(
            Models.StakePool stakePool,
            PublicKey stakePoolAddress,
            PublicKey voteAccountAddress,
            ulong lamports,
            uint? validatorStakeSeed,
            ulong transientStakeSeed,
            ulong ephemeralStakeSeed)
        {
            // Derive the pool withdraw authority.
            PublicKey poolWithdrawAuthority = FindWithdrawAuthorityProgramAddress(stakePoolAddress);

            // Derive the ephemeral stake address using the stake pool address and ephemeral stake seed.
            PublicKey ephemeralStakeAddress = FindEphemeralStakeProgramAddress(stakePoolAddress, ephemeralStakeSeed).Item1;

            // Derive the transient stake address using the vote account and stake pool address.
            PublicKey transientStakeAddress = FindTransientStakeProgramAddress(voteAccountAddress, stakePoolAddress, transientStakeSeed);

            // Derive the validator stake address using the vote account, stake pool address, and validator stake seed.
            PublicKey validatorStakeAddress = FindStakeProgramAddress(voteAccountAddress, stakePoolAddress, validatorStakeSeed);

            // Build the instruction accounts in the same order as in the Rust version.
            var accounts = new List<AccountMeta>
            {
                AccountMeta.ReadOnly(stakePoolAddress, false),
                AccountMeta.Writable(stakePool.Staker, true),
                AccountMeta.ReadOnly(poolWithdrawAuthority, false),
                AccountMeta.ReadOnly(stakePool.ValidatorList, false),
                AccountMeta.ReadOnly(stakePool.ReserveStake, false),
                AccountMeta.Writable(ephemeralStakeAddress, false),
                AccountMeta.Writable(transientStakeAddress, false),
                AccountMeta.ReadOnly(validatorStakeAddress, false),
                AccountMeta.ReadOnly(voteAccountAddress, false),
                AccountMeta.ReadOnly(SysVars.ClockKey, false),
                AccountMeta.ReadOnly(SysVars.StakeHistoryKey, false),
                AccountMeta.ReadOnly(StakeProgram.ConfigKey, false),
                AccountMeta.ReadOnly(SystemProgram.ProgramIdKey, false),
                AccountMeta.ReadOnly(StakeProgram.ProgramIdKey, false),
            };

            // Serialize instruction data; this method should follow your program's custom layout.
            byte[] data = StakePoolProgramData.EncodeDecreaseAdditionalValidatorStakeData(lamports, transientStakeSeed, ephemeralStakeSeed);

            return new TransactionInstruction
            {
                ProgramId = StakePoolProgramIdKey,
                Keys = accounts,
                Data = data
            };
        }

        /// <summary>
        /// Creates an UpdateValidatorListBalance instruction to update the balance of validators in a stake pool.
        /// </summary>
        /// <param name="stakePool"></param>
        /// <param name="stakePoolWithdrawAuthority"></param>
        /// <param name="validatorListAddress"></param>
        /// <param name="reserveStake"></param>
        /// <param name="validatorList"></param>
        /// <param name="validatorVoteAccounts"></param>
        /// <param name="startIndex"></param>
        /// <param name="noMerge"></param>
        /// <returns></returns>
        [Obsolete("please use UpdateValidatorListBalanceChunk")]
        public static TransactionInstruction UpdateValidatorListBalance(
            PublicKey stakePool,
            PublicKey stakePoolWithdrawAuthority,
            PublicKey validatorListAddress,
            PublicKey reserveStake,
            ValidatorList validatorList,
            IEnumerable<PublicKey> validatorVoteAccounts,
            uint startIndex,
            bool noMerge)
        {
            // Build the fixed part of the account metas.
            var accounts = new List<AccountMeta>
            {
                AccountMeta.ReadOnly(stakePool, false),
                AccountMeta.ReadOnly(stakePoolWithdrawAuthority, false),
                AccountMeta.Writable(validatorListAddress, false),
                AccountMeta.Writable(reserveStake, false),
                AccountMeta.ReadOnly(SysVars.ClockKey, false),
                AccountMeta.ReadOnly(SysVars.StakeHistoryKey, false),
                AccountMeta.ReadOnly(StakeProgram.ProgramIdKey, false)
            };

            // Append each validator's stake and transient stake accounts if found in the validator list.
            foreach (var voteAccount in validatorVoteAccounts)
            {
                var validatorStakeInfo = validatorList.Find(voteAccount);
                if (validatorStakeInfo != null)
                {
                    uint? validatorSeed = validatorStakeInfo.ValidatorSeedSuffix != 0 
                        ? (uint?)validatorStakeInfo.ValidatorSeedSuffix 
                        : null;
                    PublicKey validatorStakeAccount = FindStakeProgramAddress(voteAccount, stakePool, validatorSeed);
                    PublicKey transientStakeAccount = FindTransientStakeProgramAddress(voteAccount, stakePool, validatorStakeInfo.TransientSeedSuffix);
                    accounts.Add(AccountMeta.Writable(validatorStakeAccount, false));
                    accounts.Add(AccountMeta.Writable(transientStakeAccount, false));
                }
            }

            byte[] data = StakePoolProgramData.EncodeUpdateValidatorListBalance(startIndex, noMerge);
            return new TransactionInstruction
            {
                ProgramId = StakePoolProgramIdKey,
                Keys = accounts,
                Data = data
            };
        }

        /// <summary>
        /// Creates an UpdateValidatorListBalanceChunk instruction to update a chunk of validators in a stake pool.
        /// </summary>
        /// <param name="stakePool"></param>
        /// <param name="stakePoolWithdrawAuthority"></param>
        /// <param name="validatorListAddress"></param>
        /// <param name="reserveStake"></param>
        /// <param name="validatorList"></param>
        /// <param name="len"></param>
        /// <param name="startIndex"></param>
        /// <param name="noMerge"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static TransactionInstruction UpdateValidatorListBalanceChunk(
            PublicKey stakePool,
            PublicKey stakePoolWithdrawAuthority,
            PublicKey validatorListAddress,
            PublicKey reserveStake,
            ValidatorList validatorList,
            int len,
            int startIndex,
            bool noMerge)
        {
            // Verify slice bounds.
            if (startIndex < 0 || startIndex + len > validatorList.Validators.Count)
                throw new ArgumentException("Invalid instruction data: slice out of bounds", nameof(validatorList));

            // Build the fixed part of the accounts list.
            var accounts = new List<AccountMeta>
            {
                AccountMeta.ReadOnly(stakePool, false),
                AccountMeta.ReadOnly(stakePoolWithdrawAuthority, false),
                AccountMeta.Writable(validatorListAddress, false),
                AccountMeta.Writable(reserveStake, false),
                AccountMeta.ReadOnly(SysVars.ClockKey, false),
                AccountMeta.ReadOnly(SysVars.StakeHistoryKey, false),
                AccountMeta.ReadOnly(StakeProgram.ProgramIdKey, false)
            };

            // Get the requested slice of validators.
            var subSlice = validatorList.Validators.GetRange(startIndex, len);
            foreach (var validator in subSlice)
            {
                // Ensure the validator stake seed is nonzero.
                uint? seed = validator.ValidatorSeedSuffix;
                if (seed == 0)
                    throw new ArgumentException("Invalid instruction data: validator stake seed cannot be zero", nameof(validator));

                // Derive the validator stake account.
                PublicKey validatorStakeAccount = FindStakeProgramAddress(
                    validator.VoteAccountAddress,
                    stakePool,
                    seed);
                
                // Derive the transient stake account.
                PublicKey transientStakeAccount = FindTransientStakeProgramAddress(
                    validator.VoteAccountAddress,
                    stakePool,
                    validator.TransientSeedSuffix);

                accounts.Add(AccountMeta.Writable(validatorStakeAccount, false));
                accounts.Add(AccountMeta.Writable(transientStakeAccount, false));
            }

            // Serialize the instruction data.
            byte[] data = StakePoolProgramData.EncodeUpdateValidatorListBalance((uint)startIndex, noMerge);

            return new TransactionInstruction
            {
                ProgramId = StakePoolProgramIdKey,
                Keys = accounts,
                Data = data
            };
        }

        /// <summary>
        /// Creates an UpdateStaleValidatorListBalanceChunk instruction to update a chunk of validators in a stake pool
        /// </summary>
        /// <param name="stakePool"></param>
        /// <param name="stakePoolWithdrawAuthority"></param>
        /// <param name="validatorListAddress"></param>
        /// <param name="reserveStake"></param>
        /// <param name="validatorList"></param>
        /// <param name="len"></param>
        /// <param name="startIndex"></param>
        /// <param name="noMerge"></param>
        /// <param name="currentEpoch"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static TransactionInstruction? UpdateStaleValidatorListBalanceChunk(
            PublicKey stakePool,
            PublicKey stakePoolWithdrawAuthority,
            PublicKey validatorListAddress,
            PublicKey reserveStake,
            ValidatorList validatorList,
            int len,
            int startIndex,
            bool noMerge,
            ulong currentEpoch)
        {
            // Verify the requested range is within the validator list bounds.
            if (startIndex < 0 || startIndex + len > validatorList.Validators.Count)
                throw new ArgumentException("Invalid instruction data: slice out of bounds", nameof(validatorList));

            // Get the sub-slice of validators.
            var subSlice = validatorList.Validators.GetRange(startIndex, len);

            // Check if every validator's LastUpdateEpoch is greater than or equal to the current epoch.
            if (subSlice.All(info => info.LastUpdateEpoch >= currentEpoch))
            {
                return null;
            }

            // Otherwise, return the update instruction wrapped in a non-null value.
            return UpdateValidatorListBalanceChunk(
                stakePool,
                stakePoolWithdrawAuthority,
                validatorListAddress,
                reserveStake,
                validatorList,
                len,
                startIndex,
                noMerge);
        }

        /// <summary>
        /// Creates an UpdateStakePoolBalance instruction (update the balance of the stake pool).
        /// </summary>
        /// <param name="stakePool"></param>
        /// <param name="withdrawAuthority"></param>
        /// <param name="validatorListStorage"></param>
        /// <param name="reserveStake"></param>
        /// <param name="managerFeeAccount"></param>
        /// <param name="stakePoolMint"></param>
        /// <param name="tokenProgramId"></param>
        /// <returns></returns>
        public static TransactionInstruction UpdateStakePoolBalance(
            PublicKey stakePool,
            PublicKey withdrawAuthority,
            PublicKey validatorListStorage,
            PublicKey reserveStake,
            PublicKey managerFeeAccount,
            PublicKey stakePoolMint,
            PublicKey tokenProgramId)
        {
            var accounts = new List<AccountMeta>
            {
                AccountMeta.Writable(stakePool, false),
                AccountMeta.ReadOnly(withdrawAuthority, false),
                AccountMeta.Writable(validatorListStorage, false),
                AccountMeta.ReadOnly(reserveStake, false),
                AccountMeta.Writable(managerFeeAccount, false),
                AccountMeta.Writable(stakePoolMint, false),
                AccountMeta.ReadOnly(tokenProgramId, false)
            };

            // Serialize the instruction data. This assumes that the method
            // 'EncodeUpdateStakePoolBalance' encodes the unit variant for this instruction.
            byte[] data = StakePoolProgramData.EncodeUpdateStakePoolBalance();

            return new TransactionInstruction
            {
                ProgramId = StakePoolProgramIdKey,
                Keys = accounts,
                Data = data
            };
        }

        /// <summary>
        /// Creates a CleanupRemovedValidatorEntries instruction (removes entries from the validator list).
        /// </summary>
        /// <param name="stakePool">The stake pool public key.</param>
        /// <param name="validatorListStorage">The validator list storage public key.</param>
        /// <returns>The constructed transaction instruction.</returns>
        public static TransactionInstruction CleanupRemovedValidatorEntries(
            PublicKey stakePool,
            PublicKey validatorListStorage)
        {
            var accounts = new List<AccountMeta>
            {
                AccountMeta.ReadOnly(stakePool, false),
                AccountMeta.Writable(validatorListStorage, false)
            };

            byte[] data = StakePoolProgramData.EncodeCleanupRemovedValidatorEntries();

            return new TransactionInstruction
            {
                ProgramId = StakePoolProgramIdKey,
                Keys = accounts,
                Data = data
            };
        }

        /// <summary>
        /// Creates all UpdateValidatorListBalance and UpdateStakePoolBalance instructions
        /// for fully updating a stake pool each epoch.
        /// </summary>
        /// <param name="stakePool">The stake pool model.</param>
        /// <param name="validatorList">The validator list.</param>
        /// <param name="stakePoolAddress">The address of the stake pool.</param>
        /// <param name="noMerge">Flag indicating whether merging should be bypassed.</param>
        /// <returns>
        /// A tuple where the first element is the list of update validator list instructions
        /// and the second element is the final list of instructions (update stake pool balance and cleanup).
        /// </returns>
        public static (List<TransactionInstruction> updateListInstructions, List<TransactionInstruction> finalInstructions) UpdateStakePool(
            Models.StakePool stakePool,
            ValidatorList validatorList,
            PublicKey stakePoolAddress,
            bool noMerge)
        {
            // Derive the withdraw authority using the helper method.
            var withdrawAuthority = FindWithdrawAuthorityProgramAddress(stakePoolAddress);

            // Build the update list instructions by processing the validator list in chunks.
            var updateListInstructions = new List<TransactionInstruction>();
            const int maxValidatorsToUpdate = MAX_VALIDATORS_TO_UPDATE; // MAX_VALIDATORS_TO_UPDATE is defined in the class.

            // Iterate over the validators in chunks.
            for (int i = 0; i < validatorList.Validators.Count; i += maxValidatorsToUpdate)
            {
                int chunkLen = Math.Min(maxValidatorsToUpdate, validatorList.Validators.Count - i);
                // The start index for this chunk is simply 'i' as validators are grouped in chunks of maxValidatorsToUpdate.
                var instruction = UpdateValidatorListBalanceChunk(
                    stakePoolAddress,
                    withdrawAuthority,
                    stakePool.ValidatorList,
                    stakePool.ReserveStake,
                    validatorList,
                    chunkLen,
                    i,
                    noMerge);
                updateListInstructions.Add(instruction);
            }

            // Create the final instructions:
            // 1. UpdateStakePoolBalance instruction.
            // 2. CleanupRemovedValidatorEntries instruction.
            var finalInstructions = new List<TransactionInstruction>
            {
                UpdateStakePoolBalance(
                    stakePoolAddress,
                    withdrawAuthority,
                    stakePool.ValidatorList,
                    stakePool.ReserveStake,
                    stakePool.ManagerFeeAccount,
                    stakePool.PoolMint,
                    stakePool.TokenProgramId),
                CleanupRemovedValidatorEntries(
                    stakePoolAddress,
                    stakePool.ValidatorList)
            };

            return (updateListInstructions, finalInstructions);
        }

        /// <summary>
        /// Creates the UpdateValidatorListBalance instructions only for validators on the validator list 
        /// that have not been updated for the current epoch, along with the UpdateStakePoolBalance and 
        /// CleanupRemovedValidatorEntries instructions for fully updating the stake pool.
        /// </summary>
        /// <param name="stakePool">The stake pool model.</param>
        /// <param name="validatorList">The validator list.</param>
        /// <param name="stakePoolAddress">The stake pool address.</param>
        /// <param name="noMerge">Indicates whether merging should be bypassed.</param>
        /// <param name="currentEpoch">The current epoch.</param>
        /// <returns>
        /// A tuple where the first element is the list of update instructions for individual validator groups
        /// and the second element contains the final instructions for updating the stake pool balance and cleaning up.
        /// </returns>
        public static (List<TransactionInstruction> updateListInstructions, List<TransactionInstruction> finalInstructions) UpdateStaleStakePool(
            Models.StakePool stakePool,
            ValidatorList validatorList,
            PublicKey stakePoolAddress,
            bool noMerge,
            ulong currentEpoch)
        {
            // Derive the withdraw authority using the helper method.
            var withdrawAuthority = FindWithdrawAuthorityProgramAddress(stakePoolAddress);

            // Build the update list instructions by processing validators in chunks.
            var updateListInstructions = new List<TransactionInstruction>();
            const int maxValidatorsToUpdate = MAX_VALIDATORS_TO_UPDATE; // Defined in this class.

            for (int i = 0; i < validatorList.Validators.Count; i += maxValidatorsToUpdate)
            {
                int chunkLen = Math.Min(maxValidatorsToUpdate, validatorList.Validators.Count - i);
                // Call the stale update instruction helper for the current chunk.
                var instruction = UpdateStaleValidatorListBalanceChunk(
                    stakePoolAddress,
                    withdrawAuthority,
                    stakePool.ValidatorList,
                    stakePool.ReserveStake,
                    validatorList,
                    chunkLen,
                    i,
                    noMerge,
                    currentEpoch);
                // Add the instruction only if it's non-null.
                if (instruction != null)
                {
                    updateListInstructions.Add(instruction);
                }
            }

            // Final instructions: update stake pool balance and clean up removed validator entries.
            var finalInstructions = new List<TransactionInstruction>
            {
                UpdateStakePoolBalance(
                    stakePoolAddress,
                    withdrawAuthority,
                    stakePool.ValidatorList,
                    stakePool.ReserveStake,
                    stakePool.ManagerFeeAccount,
                    stakePool.PoolMint,
                    stakePool.TokenProgramId),
                CleanupRemovedValidatorEntries(
                    stakePoolAddress,
                    stakePool.ValidatorList)
            };

            return (updateListInstructions, finalInstructions);
        }

        /// <summary>
        /// Creates the DepositStake internal instructions. 
        /// 
        /// This method builds the account list and then prepends authorize instructions for deposit stake 
        /// if a deposit authority is provided (or derives it if not), then appends the rest of the accounts required 
        /// for deposit and encodes a DepositStake (or DepositStakeWithSlippage) instruction.
        /// </summary>
        /// <param name="stakePool">The stake pool account.</param>
        /// <param name="validatorListStorage">The account for validator list storage.</param>
        /// <param name="stakePoolDepositAuthority">Optional deposit authority; if null, it is derived.</param>
        /// <param name="stakePoolWithdrawAuthority">The stake pool withdraw authority.</param>
        /// <param name="depositStakeAddress">The deposit stake account address.</param>
        /// <param name="depositStakeWithdrawAuthority">The withdraw authority for the deposit stake account.</param>
        /// <param name="validatorStakeAccount">The validator stake account.</param>
        /// <param name="reserveStakeAccount">The reserve stake account.</param>
        /// <param name="poolTokensTo">The destination account for pool tokens.</param>
        /// <param name="managerFeeAccount">The manager fee pool token account.</param>
        /// <param name="referrerPoolTokensAccount">The referrer’s pool token account.</param>
        /// <param name="poolMint">The pool mint account.</param>
        /// <param name="tokenProgramId">The token program Id.</param>
        /// <param name="minimumPoolTokensOut">Optional minimum pool tokens desired; if provided, creates a slippage‐checking deposit.</param>
        /// <returns>A list of transaction instructions.</returns>
        public static List<TransactionInstruction> DepositStakeInternal(
            PublicKey stakePool,
            PublicKey validatorListStorage,
            PublicKey? stakePoolDepositAuthority,
            PublicKey stakePoolWithdrawAuthority,
            PublicKey depositStakeAddress,
            PublicKey depositStakeWithdrawAuthority,
            PublicKey validatorStakeAccount,
            PublicKey reserveStakeAccount,
            PublicKey poolTokensTo,
            PublicKey managerFeeAccount,
            PublicKey referrerPoolTokensAccount,
            PublicKey poolMint,
            PublicKey tokenProgramId,
            ulong? minimumPoolTokensOut)
        {
            var instructions = new List<TransactionInstruction>();
            // Begin building accounts list with stake pool and validator list storage.
            var accounts = new List<AccountMeta>
    {
        AccountMeta.Writable(stakePool, false),
        AccountMeta.Writable(validatorListStorage, false)
    };

            // Handle deposit authority.
            if (stakePoolDepositAuthority != null)
            {
                // If provided mark it as a signer.
                accounts.Add(AccountMeta.ReadOnly(stakePoolDepositAuthority, true));

                // Add two authorize instructions to set the deposit stake's staker and withdrawer using the provided authority.
                instructions.Add(StakeProgram.Authorize(
                    depositStakeAddress,
                    depositStakeWithdrawAuthority,
                    stakePoolDepositAuthority,
                    StakeAuthorize.Staker,
                    null));

                instructions.Add(StakeProgram.Authorize(
                    depositStakeAddress,
                    depositStakeWithdrawAuthority,
                    stakePoolDepositAuthority,
                    StakeAuthorize.Withdrawer,
                    null));
            }
            else
            {
                // Otherwise, derive the deposit authority for the stake pool.
                var (derivedDepositAuthority, _) = FindDepositAuthorityProgramAddress(stakePool);
                accounts.Add(AccountMeta.ReadOnly(derivedDepositAuthority, false));

                instructions.Add(StakeProgram.Authorize(
                    depositStakeAddress,
                    depositStakeWithdrawAuthority,
                    derivedDepositAuthority,
                    StakeAuthorize.Staker,
                    null));

                instructions.Add(StakeProgram.Authorize(
                    depositStakeAddress,
                    depositStakeWithdrawAuthority,
                    derivedDepositAuthority,
                    StakeAuthorize.Withdrawer,
                    null));
            }

            // Append the remaining accounts.
            accounts.AddRange(new List<AccountMeta>
            {
                AccountMeta.ReadOnly(stakePoolWithdrawAuthority, false),
                AccountMeta.Writable(depositStakeAddress, false),
                AccountMeta.Writable(validatorStakeAccount, false),
                AccountMeta.Writable(reserveStakeAccount, false),
                AccountMeta.Writable(poolTokensTo, false),
                AccountMeta.Writable(managerFeeAccount, false),
                AccountMeta.Writable(referrerPoolTokensAccount, false),
                AccountMeta.Writable(poolMint, false),
                AccountMeta.ReadOnly(SysVars.ClockKey, false),
                AccountMeta.ReadOnly(SysVars.StakeHistoryKey, false),
                AccountMeta.ReadOnly(tokenProgramId, false),
                AccountMeta.ReadOnly(StakeProgram.ProgramIdKey, false)
            });

            // Depending on whether a minimum pool token output is required, encode the appropriate instruction.
            TransactionInstruction depositInstruction;
            if (minimumPoolTokensOut.HasValue)
            {
                depositInstruction = new TransactionInstruction
                {
                    ProgramId = StakePoolProgramIdKey,
                    Keys = accounts,
                    Data = StakePoolProgramData.EncodeDepositStakeWithSlippage(minimumPoolTokensOut.Value)
                };
            }
            else
            {
                depositInstruction = new TransactionInstruction
                {
                    ProgramId = StakePoolProgramIdKey,
                    Keys = accounts,
                    Data = StakePoolProgramData.EncodeDepositStake()
                };
            }

            instructions.Add(depositInstruction);
            return instructions;
        }

        /// <summary>
        /// Creates instructions required to deposit into a stake pool, given a stake account owned by the user.
        /// </summary>
        /// <param name="stakePool">The stake pool account.</param>
        /// <param name="validatorListStorage">The account for validator list storage.</param>
        /// <param name="stakePoolWithdrawAuthority">The stake pool withdraw authority.</param>
        /// <param name="depositStakeAddress">The deposit stake account address.</param>
        /// <param name="depositStakeWithdrawAuthority">The withdraw authority for the deposit stake account.</param>
        /// <param name="validatorStakeAccount">The validator stake account.</param>
        /// <param name="reserveStakeAccount">The reserve stake account.</param>
        /// <param name="poolTokensTo">The destination account for pool tokens.</param>
        /// <param name="managerFeeAccount">The manager fee pool token account.</param>
        /// <param name="referrerPoolTokensAccount">The referrer's pool token account.</param>
        /// <param name="poolMint">The pool mint account.</param>
        /// <param name="tokenProgramId">The token program Id.</param>
        /// <returns>A list of transaction instructions.</returns>
        public static List<TransactionInstruction> DepositStake(
            PublicKey stakePool,
            PublicKey validatorListStorage,
            PublicKey stakePoolWithdrawAuthority,
            PublicKey depositStakeAddress,
            PublicKey depositStakeWithdrawAuthority,
            PublicKey validatorStakeAccount,
            PublicKey reserveStakeAccount,
            PublicKey poolTokensTo,
            PublicKey managerFeeAccount,
            PublicKey referrerPoolTokensAccount,
            PublicKey poolMint,
            PublicKey tokenProgramId)
        {
            return DepositStakeInternal(
                stakePool,
                validatorListStorage,
                null, // no deposit authority provided
                stakePoolWithdrawAuthority,
                depositStakeAddress,
                depositStakeWithdrawAuthority,
                validatorStakeAccount,
                reserveStakeAccount,
                poolTokensTo,
                managerFeeAccount,
                referrerPoolTokensAccount,
                poolMint,
                tokenProgramId,
                null  // no minimum pool tokens out (no slippage check)
            );
        }

        /// <summary>
        /// Creates instructions to deposit into a stake pool with slippage.
        /// </summary>
        /// <param name="stakePool">The stake pool account.</param>
        /// <param name="validatorListStorage">The account for validator list storage.</param>
        /// <param name="stakePoolWithdrawAuthority">The stake pool withdraw authority.</param>
        /// <param name="depositStakeAddress">The deposit stake account address.</param>
        /// <param name="depositStakeWithdrawAuthority">The withdraw authority for the deposit stake account.</param>
        /// <param name="validatorStakeAccount">The validator stake account.</param>
        /// <param name="reserveStakeAccount">The reserve stake account.</param>
        /// <param name="poolTokensTo">The destination account for pool tokens.</param>
        /// <param name="managerFeeAccount">The manager fee pool token account.</param>
        /// <param name="referrerPoolTokensAccount">The referrer's pool token account.</param>
        /// <param name="poolMint">The pool mint account.</param>
        /// <param name="tokenProgramId">The token program Id.</param>
        /// <param name="minimumPoolTokensOut">The minimum pool tokens desired.</param>
        /// <returns>A list of transaction instructions.</returns>
        public static List<TransactionInstruction> DepositStakeWithSlippage(
            PublicKey stakePool,
            PublicKey validatorListStorage,
            PublicKey stakePoolWithdrawAuthority,
            PublicKey depositStakeAddress,
            PublicKey depositStakeWithdrawAuthority,
            PublicKey validatorStakeAccount,
            PublicKey reserveStakeAccount,
            PublicKey poolTokensTo,
            PublicKey managerFeeAccount,
            PublicKey referrerPoolTokensAccount,
            PublicKey poolMint,
            PublicKey tokenProgramId,
            ulong minimumPoolTokensOut)
        {
            return DepositStakeInternal(
                stakePool,
                validatorListStorage,
                null, // no deposit authority provided
                stakePoolWithdrawAuthority,
                depositStakeAddress,
                depositStakeWithdrawAuthority,
                validatorStakeAccount,
                reserveStakeAccount,
                poolTokensTo,
                managerFeeAccount,
                referrerPoolTokensAccount,
                poolMint,
                tokenProgramId,
                minimumPoolTokensOut
            );
        }

        /// <summary>
        /// Creates an instruction to deposit SOL directly into a stake pool with a slippage constraint,
        /// requiring the deposit authority's signature (as needed for private pools).
        /// </summary>
        /// <param name="stakePool">The stake pool account.</param>
        /// <param name="solDepositAuthority">The SOL deposit authority which must sign the instruction.</param>
        /// <param name="stakePoolWithdrawAuthority">The stake pool withdraw authority.</param>
        /// <param name="reserveStakeAccount">The reserve stake account.</param>
        /// <param name="lamportsFrom">The source account from which SOL lamports will be deducted.</param>
        /// <param name="poolTokensTo">The destination pool tokens account.</param>
        /// <param name="managerFeeAccount">The manager fee pool token account.</param>
        /// <param name="referrerPoolTokensAccount">The referrer’s pool token account.</param>
        /// <param name="poolMint">The pool mint account.</param>
        /// <param name="tokenProgramId">The token program Id.</param>
        /// <param name="lamportsIn">The amount of lamports to deposit.</param>
        /// <param name="minimumPoolTokensOut">The minimum number of pool tokens expected (slippage constraint).</param>
        /// <returns>A transaction instruction for the SOL deposit operation.</returns>
        public static TransactionInstruction DepositSolWithAuthorityAndSlippage(
            PublicKey stakePool,
            PublicKey solDepositAuthority,
            PublicKey stakePoolWithdrawAuthority,
            PublicKey reserveStakeAccount,
            PublicKey lamportsFrom,
            PublicKey poolTokensTo,
            PublicKey managerFeeAccount,
            PublicKey referrerPoolTokensAccount,
            PublicKey poolMint,
            PublicKey tokenProgramId,
            ulong lamportsIn,
            ulong minimumPoolTokensOut)
        {
            return DepositSolInternal(
                stakePool,
                stakePoolWithdrawAuthority,
                reserveStakeAccount,
                lamportsFrom,
                poolTokensTo,
                managerFeeAccount,
                referrerPoolTokensAccount,
                poolMint,
                tokenProgramId,
                solDepositAuthority, // deposit authority provided
                lamportsIn,
                minimumPoolTokensOut
            );
        }

        /// <summary>
        /// Creates instructions required to withdraw from a stake pool by splitting a stake account.
        /// When a minimum lamports output is provided, a slippage check is enforced.
        /// </summary>
        /// <param name="stakePool">The stake pool account.</param>
        /// <param name="validatorListStorage">The validator list storage account.</param>
        /// <param name="stakePoolWithdraw">The stake pool withdraw authority.</param>
        /// <param name="stakeToSplit">The stake account to split.</param>
        /// <param name="stakeToReceive">The stake account to receive the split stake.</param>
        /// <param name="userStakeAuthority">The user's stake authority.</param>
        /// <param name="userTransferAuthority">The user's transfer authority (signer).</param>
        /// <param name="userPoolTokenAccount">The user's pool token account.</param>
        /// <param name="managerFeeAccount">The manager fee account.</param>
        /// <param name="poolMint">The pool mint account.</param>
        /// <param name="tokenProgramId">The token program Id.</param>
        /// <param name="poolTokensIn">The amount of pool tokens to redeem.</param>
        /// <param name="minimumLamportsOut">Optional minimum lamports expected on withdrawal (slippage check).</param>
        /// <returns>A transaction instruction for the stake withdrawal.</returns>
        public static TransactionInstruction WithdrawStakeInternal(
            PublicKey stakePool,
            PublicKey validatorListStorage,
            PublicKey stakePoolWithdraw,
            PublicKey stakeToSplit,
            PublicKey stakeToReceive,
            PublicKey userStakeAuthority,
            PublicKey userTransferAuthority,
            PublicKey userPoolTokenAccount,
            PublicKey managerFeeAccount,
            PublicKey poolMint,
            PublicKey tokenProgramId,
            ulong poolTokensIn,
            ulong? minimumLamportsOut)
        {
            var accounts = new List<AccountMeta>
            {
                AccountMeta.Writable(stakePool, false),
                AccountMeta.Writable(validatorListStorage, false),
                AccountMeta.ReadOnly(stakePoolWithdraw, false),
                AccountMeta.Writable(stakeToSplit, false),
                AccountMeta.Writable(stakeToReceive, false),
                AccountMeta.ReadOnly(userStakeAuthority, false),
                AccountMeta.ReadOnly(userTransferAuthority, true),
                AccountMeta.Writable(userPoolTokenAccount, false),
                AccountMeta.Writable(managerFeeAccount, false),
                AccountMeta.Writable(poolMint, false),
                AccountMeta.ReadOnly(SysVars.ClockKey, false),
                AccountMeta.ReadOnly(tokenProgramId, false),
                AccountMeta.ReadOnly(StakeProgram.ProgramIdKey, false)
            };

            byte[] data;
            if (minimumLamportsOut.HasValue)
            {
                data = StakePoolProgramData.EncodeWithdrawStakeWithSlippage(poolTokensIn, minimumLamportsOut.Value);
            }
            else
            {
                data = StakePoolProgramData.EncodeWithdrawStake(poolTokensIn);
            }

            return new TransactionInstruction
            {
                ProgramId = StakePoolProgramIdKey,
                Keys = accounts,
                Data = data
            };
        }

        /// <summary>
        /// Creates a 'WithdrawStake' instruction.
        /// </summary>
        /// <param name="stakePool">The stake pool account.</param>
        /// <param name="validatorListStorage">The validator list storage account.</param>
        /// <param name="stakePoolWithdrawAuthority">The stake pool withdraw authority.</param>
        /// <param name="stakeToSplit">The stake account to split.</param>
        /// <param name="stakeToReceive">The stake account to receive the split stake.</param>
        /// <param name="userStakeAuthority">The user's stake authority.</param>
        /// <param name="userTransferAuthority">The user's transfer authority (signer).</param>
        /// <param name="userPoolTokenAccount">The user's pool token account.</param>
        /// <param name="managerFeeAccount">The manager fee account.</param>
        /// <param name="poolMint">The pool mint account.</param>
        /// <param name="tokenProgramId">The token program Id.</param>
        /// <param name="poolTokensIn">The amount of pool tokens to redeem.</param>
        /// <returns>A transaction instruction for stake withdrawal.</returns>
        public static TransactionInstruction WithdrawStake(
            PublicKey stakePool,
            PublicKey validatorListStorage,
            PublicKey stakePoolWithdrawAuthority,
            PublicKey stakeToSplit,
            PublicKey stakeToReceive,
            PublicKey userStakeAuthority,
            PublicKey userTransferAuthority,
            PublicKey userPoolTokenAccount,
            PublicKey managerFeeAccount,
            PublicKey poolMint,
            PublicKey tokenProgramId,
            ulong poolTokensIn)
        {
            return WithdrawStakeInternal(
                stakePool,
                validatorListStorage,
                stakePoolWithdrawAuthority,
                stakeToSplit,
                stakeToReceive,
                userStakeAuthority,
                userTransferAuthority,
                userPoolTokenAccount,
                managerFeeAccount,
                poolMint,
                tokenProgramId,
                poolTokensIn,
                null // no minimum lamports out (no slippage check)
            );
        }

        /// <summary>
        /// Creates a 'WithdrawStakeWithSlippage' instruction.
        /// </summary>
        /// <param name="stakePool">The stake pool account.</param>
        /// <param name="validatorListStorage">The validator list storage account.</param>
        /// <param name="stakePoolWithdrawAuthority">The stake pool withdraw authority.</param>
        /// <param name="stakeToSplit">The stake account to split.</param>
        /// <param name="stakeToReceive">The stake account to receive the split stake.</param>
        /// <param name="userStakeAuthority">The user's stake authority.</param>
        /// <param name="userTransferAuthority">The user's transfer authority (signer).</param>
        /// <param name="userPoolTokenAccount">The user's pool token account.</param>
        /// <param name="managerFeeAccount">The manager fee account.</param>
        /// <param name="poolMint">The pool mint account.</param>
        /// <param name="tokenProgramId">The token program Id.</param>
        /// <param name="poolTokensIn">The amount of pool tokens to redeem.</param>
        /// <param name="minimumLamportsOut">The minimum lamports expected on withdrawal.</param>
        /// <returns>A transaction instruction for stake withdrawal with slippage check.</returns>
        public static TransactionInstruction WithdrawStakeWithSlippage(
            PublicKey stakePool,
            PublicKey validatorListStorage,
            PublicKey stakePoolWithdrawAuthority,
            PublicKey stakeToSplit,
            PublicKey stakeToReceive,
            PublicKey userStakeAuthority,
            PublicKey userTransferAuthority,
            PublicKey userPoolTokenAccount,
            PublicKey managerFeeAccount,
            PublicKey poolMint,
            PublicKey tokenProgramId,
            ulong poolTokensIn,
            ulong minimumLamportsOut)
        {
            return WithdrawStakeInternal(
                stakePool,
                validatorListStorage,
                stakePoolWithdrawAuthority,
                stakeToSplit,
                stakeToReceive,
                userStakeAuthority,
                userTransferAuthority,
                userPoolTokenAccount,
                managerFeeAccount,
                poolMint,
                tokenProgramId,
                poolTokensIn,
                minimumLamportsOut
            );
        }

        /// <summary>
        /// Creates instructions required to withdraw SOL directly from a stake pool,
        /// optionally enforcing a slippage constraint.
        /// </summary>
        /// <param name="stakePool">The stake pool account.</param>
        /// <param name="stakePoolWithdrawAuthority">The stake pool withdraw authority.</param>
        /// <param name="userTransferAuthority">The user's transfer authority (signer).</param>
        /// <param name="poolTokensFrom">The account from which pool tokens will be deducted.</param>
        /// <param name="reserveStakeAccount">The reserve stake account.</param>
        /// <param name="lamportsTo">The destination account for SOL lamports.</param>
        /// <param name="managerFeeAccount">The manager fee account.</param>
        /// <param name="poolMint">The pool mint account.</param>
        /// <param name="tokenProgramId">The token program Id.</param>
        /// <param name="solWithdrawAuthority">
        /// Optional SOL withdraw authority; if provided, it must sign the instruction.
        /// </param>
        /// <param name="poolTokensIn">The amount of pool tokens to redeem.</param>
        /// <param name="minimumLamportsOut">
        /// Optional minimum lamports expected on withdrawal (for slippage check).
        /// </param>
        /// <returns>A transaction instruction for SOL withdrawal.</returns>
        public static TransactionInstruction WithdrawSolInternal(
            PublicKey stakePool,
            PublicKey stakePoolWithdrawAuthority,
            PublicKey userTransferAuthority,
            PublicKey poolTokensFrom,
            PublicKey reserveStakeAccount,
            PublicKey lamportsTo,
            PublicKey managerFeeAccount,
            PublicKey poolMint,
            PublicKey tokenProgramId,
            PublicKey? solWithdrawAuthority,
            ulong poolTokensIn,
            ulong? minimumLamportsOut)
        {
            var accounts = new List<AccountMeta>
            {
                AccountMeta.Writable(stakePool, false),
                AccountMeta.ReadOnly(stakePoolWithdrawAuthority, false),
                AccountMeta.ReadOnly(userTransferAuthority, true),
                AccountMeta.Writable(poolTokensFrom, false),
                AccountMeta.Writable(reserveStakeAccount, false),
                AccountMeta.Writable(lamportsTo, false),
                AccountMeta.Writable(managerFeeAccount, false),
                AccountMeta.Writable(poolMint, false),
                AccountMeta.ReadOnly(SysVars.ClockKey, false),
                AccountMeta.ReadOnly(SysVars.StakeHistoryKey, false),
                // Assuming StakeProgram.ProgramIdKey returns a PublicKey for the stake program.
                AccountMeta.ReadOnly(StakeProgram.ProgramIdKey, false),
                AccountMeta.ReadOnly(tokenProgramId, false)
            };

            if (solWithdrawAuthority != null)
            {
                accounts.Add(AccountMeta.ReadOnly(solWithdrawAuthority, true));
            }

            byte[] data;
            if (minimumLamportsOut.HasValue)
            {
                data = StakePoolProgramData.EncodeWithdrawSolWithSlippage(poolTokensIn, minimumLamportsOut.Value);
            }
            else
            {
                data = StakePoolProgramData.EncodeWithdrawSol(poolTokensIn);
            }

            return new TransactionInstruction
            {
                ProgramId = StakePoolProgramIdKey,
                Keys = accounts,
                Data = data
            };
        }

        /// <summary>
        /// Creates instruction required to withdraw SOL directly from a stake pool.
        /// </summary>
        /// <param name="stakePool">The stake pool account.</param>
        /// <param name="stakePoolWithdrawAuthority">The stake pool withdraw authority.</param>
        /// <param name="userTransferAuthority">The user's transfer authority (signer).</param>
        /// <param name="poolTokensFrom">The account from which pool tokens will be deducted.</param>
        /// <param name="reserveStakeAccount">The reserve stake account.</param>
        /// <param name="lamportsTo">The destination account for SOL lamports.</param>
        /// <param name="managerFeeAccount">The manager fee account.</param>
        /// <param name="poolMint">The pool mint account.</param>
        /// <param name="tokenProgramId">The token program Id.</param>
        /// <param name="poolTokensIn">The amount of pool tokens to redeem.</param>
        /// <returns>A transaction instruction for SOL withdrawal.</returns>
        public static TransactionInstruction WithdrawSol(
            PublicKey stakePool,
            PublicKey stakePoolWithdrawAuthority,
            PublicKey userTransferAuthority,
            PublicKey poolTokensFrom,
            PublicKey reserveStakeAccount,
            PublicKey lamportsTo,
            PublicKey managerFeeAccount,
            PublicKey poolMint,
            PublicKey tokenProgramId,
            ulong poolTokensIn)
        {
            return WithdrawSolInternal(
                stakePool,
                stakePoolWithdrawAuthority,
                userTransferAuthority,
                poolTokensFrom,
                reserveStakeAccount,
                lamportsTo,
                managerFeeAccount,
                poolMint,
                tokenProgramId,
                null,         // SOL withdraw authority: not provided
                poolTokensIn,
                null          // minimum lamports out: not provided
            );
        }

        /// <summary>
        /// Creates an instruction required to withdraw SOL directly from a stake pool with slippage constraints.
        /// </summary>
        /// <param name="stakePool">The stake pool account.</param>
        /// <param name="stakePoolWithdrawAuthority">The stake pool withdraw authority.</param>
        /// <param name="userTransferAuthority">The user's transfer authority (signer).</param>
        /// <param name="poolTokensFrom">The account from which pool tokens will be deducted.</param>
        /// <param name="reserveStakeAccount">The reserve stake account.</param>
        /// <param name="lamportsTo">The destination account for SOL lamports.</param>
        /// <param name="managerFeeAccount">The manager fee account.</param>
        /// <param name="poolMint">The pool mint account.</param>
        /// <param name="tokenProgramId">The token program Id.</param>
        /// <param name="poolTokensIn">The amount of pool tokens to redeem.</param>
        /// <param name="minimumLamportsOut">The minimum lamports expected on withdrawal (slippage constraint).</param>
        /// <returns>A transaction instruction for stake withdrawal with slippage check.</returns>
        public static TransactionInstruction WithdrawSolWithSlippage(
            PublicKey stakePool,
            PublicKey stakePoolWithdrawAuthority,
            PublicKey userTransferAuthority,
            PublicKey poolTokensFrom,
            PublicKey reserveStakeAccount,
            PublicKey lamportsTo,
            PublicKey managerFeeAccount,
            PublicKey poolMint,
            PublicKey tokenProgramId,
            ulong poolTokensIn,
            ulong minimumLamportsOut)
        {
            return WithdrawSolInternal(
                stakePool,
                stakePoolWithdrawAuthority,
                userTransferAuthority,
                poolTokensFrom,
                reserveStakeAccount,
                lamportsTo,
                managerFeeAccount,
                poolMint,
                tokenProgramId,
                null,                   // SOL withdraw authority: not provided
                poolTokensIn,
                minimumLamportsOut
            );
        }

        /// <summary>
        /// Creates an instruction required to withdraw SOL directly from a stake pool.
        /// The difference with WithdrawSol() is that the SOL withdraw authority must sign this instruction.
        /// </summary>
        /// <param name="stakePool">The stake pool account.</param>
        /// <param name="solWithdrawAuthority">The SOL withdraw authority (must sign).</param>
        /// <param name="stakePoolWithdrawAuthority">The stake pool withdraw authority.</param>
        /// <param name="userTransferAuthority">The user's transfer authority (signer).</param>
        /// <param name="poolTokensFrom">The account from which pool tokens will be deducted.</param>
        /// <param name="reserveStakeAccount">The reserve stake account.</param>
        /// <param name="lamportsTo">The destination account for SOL lamports.</param>
        /// <param name="managerFeeAccount">The manager fee account.</param>
        /// <param name="poolMint">The pool mint account.</param>
        /// <param name="tokenProgramId">The token program Id.</param>
        /// <param name="poolTokensIn">The amount of pool tokens to redeem.</param>
        /// <returns>A transaction instruction for SOL withdrawal with the required SOL withdraw authority signature.</returns>
        public static TransactionInstruction WithdrawSolWithAuthority(
            PublicKey stakePool,
            PublicKey solWithdrawAuthority,
            PublicKey stakePoolWithdrawAuthority,
            PublicKey userTransferAuthority,
            PublicKey poolTokensFrom,
            PublicKey reserveStakeAccount,
            PublicKey lamportsTo,
            PublicKey managerFeeAccount,
            PublicKey poolMint,
            PublicKey tokenProgramId,
            ulong poolTokensIn)
        {
            return WithdrawSolInternal(
                stakePool,
                stakePoolWithdrawAuthority,
                userTransferAuthority,
                poolTokensFrom,
                reserveStakeAccount,
                lamportsTo,
                managerFeeAccount,
                poolMint,
                tokenProgramId,
                solWithdrawAuthority,  // Provide the SOL withdraw authority which must sign
                poolTokensIn,
                null                   // No minimum lamports out (no slippage check)
            );
        }

        /// <summary>
        /// Creates an instruction required to withdraw SOL directly from a stake pool with a slippage constraint.
        /// The difference with WithdrawSol() is that the SOL withdraw authority must sign this instruction.
        /// </summary>
        /// <param name="stakePool">The stake pool account.</param>
        /// <param name="solWithdrawAuthority">The SOL withdraw authority which must sign the instruction.</param>
        /// <param name="stakePoolWithdrawAuthority">The stake pool withdraw authority.</param>
        /// <param name="userTransferAuthority">The user's transfer authority (signer).</param>
        /// <param name="poolTokensFrom">The account from which pool tokens will be deducted.</param>
        /// <param name="reserveStakeAccount">The reserve stake account.</param>
        /// <param name="lamportsTo">The destination account for SOL lamports.</param>
        /// <param name="managerFeeAccount">The manager fee account.</param>
        /// <param name="poolMint">The pool mint account.</param>
        /// <param name="tokenProgramId">The token program Id.</param>
        /// <param name="poolTokensIn">The amount of pool tokens to redeem.</param>
        /// <param name="minimumLamportsOut">The minimum lamports expected on withdrawal (slippage constraint).</param>
        /// <returns>A transaction instruction for SOL withdrawal with the required SOL withdraw authority signature and slippage check.</returns>
        public static TransactionInstruction WithdrawSolWithAuthorityAndSlippage(
            PublicKey stakePool,
            PublicKey solWithdrawAuthority,
            PublicKey stakePoolWithdrawAuthority,
            PublicKey userTransferAuthority,
            PublicKey poolTokensFrom,
            PublicKey reserveStakeAccount,
            PublicKey lamportsTo,
            PublicKey managerFeeAccount,
            PublicKey poolMint,
            PublicKey tokenProgramId,
            ulong poolTokensIn,
            ulong minimumLamportsOut)
        {
            return WithdrawSolInternal(
                stakePool,
                stakePoolWithdrawAuthority,
                userTransferAuthority,
                poolTokensFrom,
                reserveStakeAccount,
                lamportsTo,
                managerFeeAccount,
                poolMint,
                tokenProgramId,
                solWithdrawAuthority,       // SOL withdraw authority must sign
                poolTokensIn,
                minimumLamportsOut          // enforce slippage check
            );
        }

        /// <summary>
        /// Creates a 'Set Manager' instruction.
        /// </summary>
        /// <param name="stakePool">The stake pool account.</param>
        /// <param name="manager">The current manager (must sign).</param>
        /// <param name="newManager">The new manager (must sign).</param>
        /// <param name="newFeeReceiver">The new fee receiver account.</param>
        /// <returns>A transaction instruction to set the manager.</returns>
        public static TransactionInstruction SetManager(
            PublicKey stakePool,
            PublicKey manager,
            PublicKey newManager,
            PublicKey newFeeReceiver)
        {
            var accounts = new List<AccountMeta>
            {
                AccountMeta.Writable(stakePool, false),
                AccountMeta.ReadOnly(manager, true),
                AccountMeta.ReadOnly(newManager, true),
                AccountMeta.ReadOnly(newFeeReceiver, false)
            };

            return new TransactionInstruction
            {
                ProgramId = StakePoolProgramIdKey,
                Keys = accounts,
                Data = StakePoolProgramData.EncodeSetManager() // encode the SetManager variant
            };
        }

        /// <summary>
        /// Creates a 'Set Fee' instruction.
        /// </summary>
        /// <param name="stakePool">The stake pool account.</param>
        /// <param name="manager">The manager account (must sign).</param>
        /// <param name="fee">The fee to be set.</param>
        /// <returns>A transaction instruction to set the fee.</returns>
        public static TransactionInstruction SetFee(
            PublicKey stakePool,
            PublicKey manager,
            Fee fee)
        {
            var accounts = new List<AccountMeta>
            {
                AccountMeta.Writable(stakePool, false),
                AccountMeta.ReadOnly(manager, true)
            };

            return new TransactionInstruction
            {
                ProgramId = StakePoolProgramIdKey,
                Keys = accounts,
                Data = StakePoolProgramData.EncodeSetFee(fee)
            };
        }

        /// <summary>
        /// Creates a 'Set Staker' instruction.
        /// </summary>
        /// <param name="stakePool">The stake pool account.</param>
        /// <param name="setStakerAuthority">The current staker (must sign).</param>
        /// <param name="newStaker">The new staker to be set.</param>
        /// <returns>A transaction instruction to set the staker.</returns>
        public static TransactionInstruction SetStaker(
            PublicKey stakePool,
            PublicKey setStakerAuthority,
            PublicKey newStaker)
        {
            var accounts = new List<AccountMeta>
            {
                AccountMeta.Writable(stakePool, false),
                AccountMeta.ReadOnly(setStakerAuthority, true),
                AccountMeta.ReadOnly(newStaker, false)
            };

            // Fix: Remove the public key from the data since it is provided in keys
            return new TransactionInstruction
            {
                ProgramId = StakePoolProgramIdKey,
                Keys = accounts,
                Data = StakePoolProgramData.EncodeSetStaker() // now only encodes the discriminator
            };
        }

        /// <summary>
        /// Creates a 'SetFundingAuthority' instruction.
        /// </summary>
        /// <param name="stakePool">The stake pool account.</param>
        /// <param name="manager">The manager account (must sign).</param>
        /// <param name="newSolDepositAuthority">
        /// The new SOL deposit authority (optional). If provided, it is added as a read-only account.
        /// </param>
        /// <param name="fundingType">The funding type to be set.</param>
        /// <returns>A transaction instruction to set the funding authority.</returns>
        public static TransactionInstruction SetFundingAuthority(
            PublicKey stakePool,
            PublicKey manager,
            PublicKey? newSolDepositAuthority,
            FundingType fundingType)
        {
            var accounts = new List<AccountMeta>
            {
                AccountMeta.Writable(stakePool, false),
                AccountMeta.ReadOnly(manager, true)
            };

            if(newSolDepositAuthority != null)
            {
                accounts.Add(AccountMeta.ReadOnly(newSolDepositAuthority, false));
            }

            return new TransactionInstruction
            {
                ProgramId = StakePoolProgramIdKey,
                Keys = accounts,
                Data = StakePoolProgramData.EncodeSetFundingAuthority(fundingType)
            };
        }

        /// <summary>
        /// Creates an instruction to update metadata in the MPL token metadata program
        /// account for the pool token.
        /// </summary>
        /// <param name="stakePool">The stake pool account.</param>
        /// <param name="manager">The manager account (must sign).</param>
        /// <param name="poolMint">The pool mint account.</param>
        /// <param name="name">The new name for the pool token.</param>
        /// <param name="symbol">The new symbol for the pool token.</param>
        /// <param name="uri">The new URI for the pool token metadata.</param>
        /// <returns>A transaction instruction for updating token metadata.</returns>
        public static TransactionInstruction UpdateTokenMetadata(
            PublicKey stakePool,
            PublicKey manager,
            PublicKey poolMint,
            string name,
            string symbol,
            string uri)
        {
            // Derive the stake pool withdraw authority.
            PublicKey stakePoolWithdrawAuthority = FindWithdrawAuthorityProgramAddress(stakePool);
            // Derive the metadata account for the pool mint.
            (PublicKey tokenMetadata, byte bump) = FindMetadataAccount(poolMint);

            var accounts = new List<AccountMeta>
            {
                AccountMeta.ReadOnly(stakePool, false),
                AccountMeta.ReadOnly(manager, true),
                AccountMeta.ReadOnly(stakePoolWithdrawAuthority, false),
                AccountMeta.Writable(tokenMetadata, false),
                // InlineMplTokenMetadata.Id() returns the MPL token metadata program ID.
                AccountMeta.ReadOnly(MplTokenMetadataProgramIdKey, false)
            };

            byte[] data = StakePoolProgramData.EncodeUpdateTokenMetadata(name, symbol, uri);
            return new TransactionInstruction
            {
                ProgramId = StakePoolProgramIdKey,
                Keys = accounts,
                Data = data
            };
        }

        /// <summary>
        /// Creates an instruction to create metadata using the MPL token metadata
        /// program for the pool token.
        /// </summary>
        /// <param name="stakePool">The stake pool account.</param>
        /// <param name="manager">The manager account (must sign).</param>
        /// <param name="poolMint">The pool mint account.</param>
        /// <param name="payer">The account paying for the transaction (must sign).</param>
        /// <param name="name">The name for the pool token.</param>
        /// <param name="symbol">The symbol for the pool token.</param>
        /// <param name="uri">The URI for the pool token metadata.</param>
        /// <returns>A transaction instruction for creating token metadata.</returns>
        public static TransactionInstruction CreateTokenMetadata(
            PublicKey stakePool,
            PublicKey manager,
            PublicKey poolMint,
            PublicKey payer,
            string name,
            string symbol,
            string uri)
        {
            // Derive the stake pool withdraw authority.
            PublicKey stakePoolWithdrawAuthority = FindWithdrawAuthorityProgramAddress(stakePool);
            
            // Derive the metadata account for the pool mint.
            (PublicKey tokenMetadata, byte bump) = FindMetadataAccount(poolMint);
            
            // Build the accounts as required by the MPL Token Metadata program.
            var accounts = new List<AccountMeta>
            {
                // The stake pool account (read-only)
                AccountMeta.ReadOnly(stakePool, false),
                // The manager must sign (read-only)
                AccountMeta.ReadOnly(manager, true),
                // The derived withdraw authority (read-only)
                AccountMeta.ReadOnly(stakePoolWithdrawAuthority, false),
                // The pool mint (read-only)
                AccountMeta.ReadOnly(poolMint, false),
                // The payer (writable and signer)
                AccountMeta.Writable(payer, true),
                // The token metadata account (writable)
                AccountMeta.Writable(tokenMetadata, false),
                // The MPL Token Metadata program
                AccountMeta.ReadOnly(MplTokenMetadataProgramIdKey, false),
                // The system program
                AccountMeta.ReadOnly(SystemProgram.ProgramIdKey, false)
            };

            // Encode the instruction data for creating token metadata.
            byte[] data = StakePoolProgramData.EncodeCreateTokenMetadata(name, symbol, uri);

            return new TransactionInstruction
            {
                ProgramId = StakePoolProgramIdKey,
                Keys = accounts,
                Data = data
            };
        }

        /// <summary>
        /// Creates an instruction required to deposit SOL directly into a stake pool.
        /// </summary>
        /// <param name="stakePool">The stake pool account.</param>
        /// <param name="stakePoolWithdrawAuthority">The stake pool withdraw authority.</param>
        /// <param name="reserveStakeAccount">The reserve stake account.</param>
        /// <param name="lamportsFrom">The source account for SOL lamports (must sign).</param>
        /// <param name="poolTokensTo">The account to receive pool tokens.</param>
        /// <param name="managerFeeAccount">The manager fee account.</param>
        /// <param name="referrerPoolTokensAccount">The referrer’s pool token account.</param>
        /// <param name="poolMint">The pool mint account.</param>
        /// <param name="tokenProgramId">The token program account.</param>
        /// <param name="solDepositAuthority">
        /// Optional SOL deposit authority; if provided, it must sign.
        /// </param>
        /// <param name="lamportsIn">The amount of SOL lamports to deposit.</param>
        /// <param name="minimumPoolTokensOut">
        /// Optional minimum pool tokens expected (for slippage protection).
        /// </param>
        /// <returns>A transaction instruction for SOL deposit.</returns>
        public static TransactionInstruction DepositSolInternal(
            PublicKey stakePool,
            PublicKey stakePoolWithdrawAuthority,
            PublicKey reserveStakeAccount,
            PublicKey lamportsFrom,
            PublicKey poolTokensTo,
            PublicKey managerFeeAccount,
            PublicKey referrerPoolTokensAccount,
            PublicKey poolMint,
            PublicKey tokenProgramId,
            PublicKey? solDepositAuthority,
            ulong lamportsIn,
            ulong? minimumPoolTokensOut)
        {
            var accounts = new List<AccountMeta>
            {
                AccountMeta.Writable(stakePool, false),
                AccountMeta.ReadOnly(stakePoolWithdrawAuthority, false),
                AccountMeta.Writable(reserveStakeAccount, false),
                AccountMeta.Writable(lamportsFrom, true),
                AccountMeta.Writable(poolTokensTo, false),
                AccountMeta.Writable(managerFeeAccount, false),
                AccountMeta.Writable(referrerPoolTokensAccount, false),
                AccountMeta.Writable(poolMint, false),
                AccountMeta.ReadOnly(SystemProgram.ProgramIdKey, false),
                AccountMeta.ReadOnly(tokenProgramId, false)
            };

            if (solDepositAuthority != null)
            {
                accounts.Add(AccountMeta.ReadOnly(solDepositAuthority, true));
            }

            byte[] data = minimumPoolTokensOut.HasValue
                ? StakePoolProgramData.EncodeDepositSolWithSlippage(lamportsIn, minimumPoolTokensOut.Value)
                : StakePoolProgramData.EncodeDepositSol(lamportsIn);

            return new TransactionInstruction
            {
                ProgramId = StakePoolProgramIdKey.KeyBytes,
                Keys = accounts,
                Data = data
            };
        }

        /// <summary>
        /// Finds the metadata account for a given mint using the MPL Token Metadata program.
        /// </summary>
        /// <param name="mint">The mint public key.</param>
        /// <returns>A tuple containing the metadata account public key and the bump seed.</returns>
        public static (PublicKey, byte) FindMetadataAccount(PublicKey mint)
        {
            // Seeds must be in the same order as in the Rust function:
            // 1. The UTF8 bytes for "metadata"
            // 2. The MPL Token Metadata Program ID bytes.
            // 3. The mint's public key bytes.
            var seeds = new List<byte[]>
            {
                Encoding.ASCII.GetBytes("metadata"),
                MplTokenMetadataProgramIdKey.KeyBytes,
                mint.KeyBytes
            };

            if (!PublicKey.TryFindProgramAddress(seeds, MplTokenMetadataProgramIdKey, out PublicKey metadataAccount, out byte bump))
            {
                throw new InvalidProgramException("Unable to find metadata account for the provided mint.");
            }

            return (metadataAccount, bump);
        }

        #endregion
    }
}
