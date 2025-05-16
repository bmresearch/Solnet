using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solnet.Programs.StakePool
{
    /// <summary>
    /// Represents the instruction types for the <see cref="StakePoolProgram"/> along with a friendly name so as not to use reflection.
    /// </summary>
    /// <remarks>
    /// For more information see:
    /// https://spl.solana.com/stake-pool
    /// https://docs.rs/spl-stake-pool/latest/spl_stake_pool/
    /// </remarks>
    internal static class StakePoolProgramInstructions
    {
        /// <summary>
        /// Represents the user-friendly names for the instruction types for the <see cref="StakePoolProgram"/>.
        /// </summary>
        internal static readonly Dictionary<Values, string> Names = new()
        {
            { Values.Initialize, "Initialize" },
            { Values.AddValidatorToPool, "Add Validator To Pool" },
            { Values.RemoveValidatorFromPool, "Remove Validator From Pool" },
            { Values.DecreaseValidatorStake, "Decrease Validator Stake" },
            { Values.IncreaseValidatorStake, "Increase Validator Stake" },
            { Values.SetPreferredValidator, "Set Preferred Deposit Validator" },
            { Values.UpdateValidatorListBalance, "Update Validator List Balance" },
            { Values.UpdateStakePoolBalance, "Update Stake Pool Balance" },
            { Values.CleanupRemovedValidatorEntries, "Cleanup Removed Validator Entries" },
            { Values.DecreaseValidatorStakeWithReserve, "Decrease Validator Stake With Reserve" },
            { Values.CreateTokenMetadata, "Create Token Metadata" },
            { Values.UpdateTokenMetadata, "Update Token Metadata" },
            { Values.DepositStake, "Deposit some stake into the pool" },
            { Values.WithdrawStake, "Withdraw Stake" },
            { Values.DecreaseAdditionalValidatorStake, "Decrease Additional Validator Stake" },
            { Values.SetManager, "Set Manager" },
            { Values.Redelegate, "Redelegate active stake on a validator" },
            { Values.DepositStakeWithSlippage, "Deposit some stake into the pool, with a specified slippage" },
            { Values.WithdrawStakeWithSlippage, "Withdraw the token from the pool at the current ratio" },
            { Values.DepositSolWithSlippage, "Deposit SOL directly into the pool's reserve account, with a specified slippage constraint." },
            { Values.SetFee, "Set Fee" },
            { Values.SetStaker, "Set Staker" },
            { Values.DepositSol, "Deposit Sol" },
            { Values.SetFundingAuthority, "Set Funding Authority" },
            { Values.WithdrawSol, "Withdraw Sol" },
            { Values.IncreaseAdditionalValidatorStake, "Increase Additional Validator Stake" },
        };

        /// <summary>
        /// Represents the instruction types for the <see cref="StakePoolProgram"/>.
        /// </summary>
        internal enum Values : uint
        {            
            /// <summary>
            /// Initializes a new StakePool.
            /// </summary>
            Initialize = 0,

            /// <summary>
            /// (Staker only) Adds stake account delegated to validator to the pool's list of managed validators.
            /// The stake account will have the rent-exempt amount plus max(crate::MINIMUM_ACTIVE_STAKE, solana_program::stake::tools::get_minimum_delegation()).
            /// It is funded from the stake pool reserve.
            /// Userdata: optional non-zero u32 seed used for generating the validator stake address.
            /// </summary>
            AddValidatorToPool = 1,

            /// <summary>
            /// (Staker only) Removes validator from the pool, deactivating its stake.
            /// Only succeeds if the validator stake account has the minimum of max(crate::MINIMUM_ACTIVE_STAKE, solana_program::stake::tools::get_minimum_delegation()) plus the rent-exempt amount.
            /// </summary>
            RemoveValidatorFromPool = 2,

            /// <summary>
            /// (Deprecated since v0.7.0, use <see cref="DecreaseValidatorStakeWithReserve"/> instead)
            /// (Staker only) Decrease active stake on a validator, eventually moving it to the reserve.
            /// Internally, this instruction splits a validator stake account into its corresponding transient stake account and deactivates it.
            /// </summary>
            DecreaseValidatorStake = 3,

            /// <summary>
            /// (Staker only) Increase stake on a validator from the reserve account.
            /// Internally, this instruction splits reserve stake into a transient stake account and delegates to the appropriate validator.
            /// <see cref="UpdateValidatorListBalance"/> will do the work of merging once it's ready.
            /// Userdata: amount of lamports to increase on the given validator.
            /// The actual amount split into the transient stake account is: lamports + stake_rent_exemption.
            /// The rent-exemption of the stake account is withdrawn back to the reserve after it is merged.
            /// </summary>
            IncreaseValidatorStake = 4,

            /// <summary>
            /// (Staker only) Set the preferred deposit or withdraw stake account for the stake pool.
            /// In order to avoid users abusing the stake pool as a free conversion between SOL staked on different validators,
            /// the staker can force all deposits and/or withdraws to go to one chosen account, or unset that account.
            /// </summary>
            SetPreferredValidator = 5,

            /// <summary>
            /// Updates balances of validator and transient stake accounts in the pool.
            /// While going through the pairs of validator and transient stake accounts, if the transient stake is inactive,
            /// it is merged into the reserve stake account. If the transient stake is active and has matching credits observed,
            /// it is merged into the canonical validator stake account. In all other states, nothing is done, and the balance is simply added to the canonical stake account balance.
            /// </summary>
            UpdateValidatorListBalance = 6,

            /// <summary>
            /// Updates total pool balance based on balances in the reserve and validator list.
            /// </summary>
            UpdateStakePoolBalance = 7,

            /// <summary>
            /// Cleans up validator stake account entries marked as ReadyForRemoval.
            /// </summary>
            CleanupRemovedValidatorEntries = 8,

            /// <summary>
            /// Deposit some stake into the pool. The output is a "pool" token representing ownership into the pool. Inputs are converted to the current ratio.
            /// </summary>
            DepositStake = 9,

            /// <summary>
            /// Withdraw the token from the pool at the current ratio.
            /// Succeeds if the stake account has enough SOL to cover the desired amount of pool tokens, and if the withdrawal keeps the total staked amount above the minimum of rent-exempt amount + max(crate::MINIMUM_ACTIVE_STAKE, solana_program::stake::tools::get_minimum_delegation()).
            /// When allowing withdrawals, the order of priority goes: preferred withdraw validator stake account (if set), validator stake accounts, transient stake accounts, reserve stake account OR totally remove validator stake accounts.
            /// Userdata: amount of pool tokens to withdraw.
            /// </summary>
            WithdrawStake = 10,

            /// <summary>
            /// (Manager only) Update manager.
            /// </summary>
            SetManager = 11,

            /// <summary>
            /// (Manager only) Update fee.
            /// </summary>
            SetFee = 12,

            /// <summary>
            /// (Manager or staker only) Update staker.
            /// </summary>
            SetStaker = 13,

            /// <summary>
            /// Deposit SOL directly into the pool's reserve account. The output is a "pool" token representing ownership into the pool. Inputs are converted to the current ratio.
            /// </summary>
            DepositSol = 14,

            /// <summary>
            /// (Manager only) Update SOL deposit, stake deposit, or SOL withdrawal authority.
            /// </summary>
            SetFundingAuthority = 15,

            /// <summary>
            /// Withdraw SOL directly from the pool's reserve account. Fails if the reserve does not have enough SOL.
            /// </summary>
            WithdrawSol = 16,

            /// <summary>
            /// Create token metadata for the stake-pool token in the metaplex-token program.
            /// </summary>
            CreateTokenMetadata = 17,

            /// <summary>
            /// Update token metadata for the stake-pool token in the metaplex-token program.
            /// </summary>
            UpdateTokenMetadata = 18,

            /// <summary>
            /// (Staker only) Increase stake on a validator again in an epoch.
            /// Works regardless if the transient stake account exists.
            /// Internally, this instruction splits reserve stake into an ephemeral stake account, activates it, then merges or splits it into the transient stake account delegated to the appropriate validator.
            /// <see cref="UpdateValidatorListBalance"/> will do the work of merging once it's ready.
            /// Userdata: amount of lamports to increase on the given validator.
            /// The actual amount split into the transient stake account is: lamports + stake_rent_exemption.
            /// The rent-exemption of the stake account is withdrawn back to the reserve after it is merged.
            /// </summary>
            IncreaseAdditionalValidatorStake = 19,

            /// <summary>
            /// (Staker only) Decrease active stake again from a validator, eventually moving it to the reserve.
            /// Works regardless if the transient stake account already exists.
            /// Internally, this instruction: withdraws rent-exempt reserve lamports from the reserve into the ephemeral stake, splits a validator stake account into an ephemeral stake account, deactivates the ephemeral account, merges or splits the ephemeral account into the transient stake account delegated to the appropriate validator.
            /// </summary>
            DecreaseAdditionalValidatorStake = 20,

            /// <summary>
            /// (Staker only) Decrease active stake on a validator, eventually moving it to the reserve.
            /// Internally, this instruction: withdraws enough lamports to make the transient account rent-exempt, splits from a validator stake account into a transient stake account, deactivates the transient stake account.
            /// </summary>
            DecreaseValidatorStakeWithReserve = 21,

            /// <summary>
            /// (Staker only) Redelegate active stake on a validator, eventually moving it to another.
            /// Internally, this instruction splits a validator stake account into its corresponding transient stake account, redelegates it to an ephemeral stake account, then merges that stake into the destination transient stake account.
            /// </summary>
            Redelegate = 22,

            /// <summary>
            /// Deposit some stake into the pool, with a specified slippage constraint. The output is a "pool" token representing ownership into the pool. Inputs are converted at the current ratio.
            /// </summary>
            DepositStakeWithSlippage = 23,

            /// <summary>
            /// Withdraw the token from the pool at the current ratio, specifying a minimum expected output lamport amount.
            /// Succeeds if the stake account has enough SOL to cover the desired amount of pool tokens, and if the withdrawal keeps the total staked amount above the minimum of rent-exempt amount + max(crate::MINIMUM_ACTIVE_STAKE, solana_program::stake::tools::get_minimum_delegation()).
            /// Userdata: amount of pool tokens to withdraw.
            /// </summary>
            WithdrawStakeWithSlippage = 24,

            /// <summary>
            /// Deposit SOL directly into the pool's reserve account, with a specified slippage constraint. The output is a "pool" token representing ownership into the pool. Inputs are converted at the current ratio.
            /// </summary>
            DepositSolWithSlippage = 25,

            /// <summary>
            /// Withdraw SOL directly from the pool's reserve account. Fails if the reserve does not have enough SOL or if the slippage constraint is not met.
            /// </summary>
            WithdrawSolWithSlippage = 26,
        }
    }
}
