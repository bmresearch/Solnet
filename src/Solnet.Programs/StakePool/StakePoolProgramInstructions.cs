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
            { Values.AddValidatorToPoolWithVote, "Add Validator To Pool With Vote" },
            { Values.RemoveValidatorFromPoolWithVote, "Remove Validator From Pool With Vote" },
            { Values.IncreaseValidatorStakeWithVote, "Increase Validator Stake With Vote" },
            { Values.DecreaseValidatorStakeWithReserve, "Decrease Validator Stake With Reserve" },
            { Values.CreateTokenMetadata, "Create Token Metadata" },
            { Values.UpdateTokenMetadata, "Update Token Metadata" },
            { Values.WithdrawStake, "Withdraw Stake" },
            { Values.DecreaseAdditionalValidatorStake, "Decrease Additional Validator Stake" },
            { Values.SetManager, "Set Manager" },
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
            AddValidatorToPoolWithVote = 9,
            RemoveValidatorFromPoolWithVote = 10,
            IncreaseValidatorStakeWithVote = 11,
            DecreaseValidatorStakeWithReserve = 12,
            DecreaseAdditionalValidatorStake = 13,
            CreateTokenMetadata = 14,
            UpdateTokenMetadata = 15,
            WithdrawStake = 16,
            SetManager = 17,
            SetFee = 18,
            SetStaker = 19,
            DepositSol = 20,
            SetFundingAuthority = 21,
            WithdrawSol = 22,
            IncreaseAdditionalValidatorStake = 23,
        }
    }
}
