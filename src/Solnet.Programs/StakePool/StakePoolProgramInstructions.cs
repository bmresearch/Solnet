using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solnet.Programs.StakePool
{
    /// <summary>
    /// Represents the instruction types for the <see cref="StakePoolProgram"/> along with a friendly name so as not to use reflection.
    /// <remarks>
    /// For more information see:
    /// https://spl.solana.com/stake-pool
    /// https://docs.rs/spl-stake-pool/latest/spl_stake_pool/
    /// </remarks>
    /// </summary>
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
            Initialize = 0,
            AddValidatorToPool = 1,
            RemoveValidatorFromPool = 2,
            DecreaseValidatorStake = 3,
            IncreaseValidatorStake = 4,
            SetPreferredValidator = 5,
            UpdateValidatorListBalance = 6,
            UpdateStakePoolBalance = 7,
            CleanupRemovedValidatorEntries = 8,
            DepositStake = 9,
            WithdrawStake = 10,
            SetManager = 11,
            SetFee = 12,
            SetStaker = 13,
            DepositSol = 14,
            SetFundingAuthority = 15,
            WithdrawSol = 16,
            CreateTokenMetadata = 17,
            UpdateTokenMetadata = 18,
            IncreaseAdditionalValidatorStake = 19,
            DecreaseAdditionalValidatorStake = 20,
            DecreaseValidatorStakeWithReserve = 21,
            Redelegate = 22,
            DepositStakeWithSlippage = 23,
            WithdrawStakeWithSlippage = 24,
            DepositSolWithSlippage = 25,
            WithdrawSolWithSlippage = 26,
        }
    }
}
