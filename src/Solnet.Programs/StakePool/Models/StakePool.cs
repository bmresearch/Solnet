using Solnet.Programs.TokenSwap.Models;
using Solnet.Wallet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Solnet.Programs.Models.Stake.State;

namespace Solnet.Programs.StakePool.Models
{
    /// <summary>
    /// Represents a Stake Pool in the Solana blockchain.
    /// </summary>
    public class StakePool
    {
        /// <summary>
        /// The public key of the staker.
        /// </summary>
        public PublicKey Staker { get; set; }

        /// <summary>
        /// The public key of the reserve stake account.
        /// </summary>
        public PublicKey ReserveStake { get; set; }

        /// <summary>
        /// The public key of the validator list.
        /// </summary>
        public PublicKey ValidatorList { get; set; }

        /// <summary>
        /// The public key of the pool mint.
        /// </summary>
        public PublicKey PoolMint { get; set; }

        /// <summary>
        /// The public key of the manager fee account.
        /// </summary>
        public PublicKey ManagerFeeAccount { get; set; }

        /// <summary>
        /// The public key of the token program ID.
        /// </summary>
        public PublicKey TokenProgramId { get; set; }

        /// <summary>
        /// The total lamports in the stake pool.
        /// </summary>
        public ulong TotalLamports { get; set; }

        /// <summary>
        /// The total supply of pool tokens.
        /// </summary>
        public ulong PoolTokenSupply { get; set; }

        /// <summary>
        /// The epoch of the last update.
        /// </summary>
        public ulong LastUpdateEpoch { get; set; }

        /// <summary>
        /// The lockup configuration for the stake pool.
        /// </summary>
        public Lockup Lockup { get; set; }

        /// <summary>
        /// The fee for the current epoch.
        /// </summary>
        public Fees EpochFee { get; set; }

        /// <summary>
        /// The fee for the next epoch.
        /// </summary>
        public Fees NextEpochFee { get; set; }

        /// <summary>
        /// The preferred validator vote address for deposits.
        /// </summary>
        public PublicKey PreferredDepositValidatorVoteAddress { get; set; }

        /// <summary>
        /// The preferred validator vote address for withdrawals.
        /// </summary>
        public PublicKey PreferredWithdrawValidatorVoteAddress { get; set; }

        /// <summary>
        /// The fee for stake deposits.
        /// </summary>
        public Fees StakeDepositFee { get; set; }

        /// <summary>
        /// The fee for stake withdrawals.
        /// </summary>
        public Fees StakeWithdrawalFee { get; set; }

        /// <summary>
        /// The fee for the next stake withdrawals.
        /// </summary>
        public Fees NextStakeWithdrawalFees { get; set; }

        /// <summary>
        /// The referral fee for stake operations.
        /// </summary>
        public byte StakeReferralFees { get; set; }

        /// <summary>
        /// The public key of the SOL deposit authority.
        /// </summary>
        public PublicKey SolDepositAuthority { get; set; }

        /// <summary>
        /// The fee for SOL deposits.
        /// </summary>
        public Fees SolDepositFee { get; set; }

        /// <summary>
        /// The referral fee for SOL operations.
        /// </summary>
        public byte SolReferralFee { get; set; }

        /// <summary>
        /// The public key of the SOL withdrawal authority.
        /// </summary>
        public PublicKey SolWithdrawAuthority { get; set; }

        /// <summary>
        /// The fee for SOL withdrawals.
        /// </summary>
        public Fees SolWithdrawalFee { get; set; }

        /// <summary>
        /// The fee for the next SOL withdrawals.
        /// </summary>
        public Fees NextSolWithdrawalFee { get; set; }

        /// <summary>
        /// The pool token supply at the last epoch.
        /// </summary>
        public ulong LastEpochPoolTokenSupply { get; set; }

        /// <summary>
        /// The total lamports at the last epoch.
        /// </summary>
        public ulong LastEpochTotalLamports { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="StakePool"/> class.
        /// </summary>
        public StakePool()
        {
            // Initialize properties
        }

        /// <summary>
        /// Calculates the number of pool tokens for a given deposit of lamports.
        /// </summary>
        /// <param name="stakeLamports">The amount of lamports to deposit.</param>
        /// <returns>The calculated pool tokens.</returns>
        public ulong CalcPoolTokensForDeposit(ulong stakeLamports)
        {
            if (TotalLamports == 0 || PoolTokenSupply == 0)
            {
                return stakeLamports;
            }

            return (stakeLamports * PoolTokenSupply) / TotalLamports;
        }

        /// <summary>
        /// Calculates the amount of lamports to withdraw for a given number of pool tokens.
        /// </summary>
        /// <param name="poolTokens">The number of pool tokens to withdraw.</param>
        /// <returns>The calculated lamports.</returns>
        public ulong CalcLamportsWithdrawAmount(ulong poolTokens)
        {
            return (poolTokens * TotalLamports) / PoolTokenSupply;
        }
    }
}
