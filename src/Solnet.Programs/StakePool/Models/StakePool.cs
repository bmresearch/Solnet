using Solnet.Programs.TokenSwap.Models;
using Solnet.Wallet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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
        /// Gets or sets the type of the account.
        /// </summary>
        public AccountType AccountType { get; set; }

        /// <summary>
        /// The public key of the stake pool.
        /// </summary>
        public PublicKey Manager { get; set; }

        /// <summary>
        /// The public key of the staker.
        /// </summary>
        public PublicKey Staker { get; set; }

        /// <summary>
        /// The public key of the Deposit Authority.
        /// </summary>
        public PublicKey StakeDepositAuthority { get; set; }

        /// <summary>
        /// Gets or sets the bump seed associated with the stake withdrawal operation.
        /// </summary>
        public PublicKey StakeWithdrawBumpSeed { get; set; }

        /// <summary>
        /// The public key of the validator list.
        /// </summary>
        public PublicKey ValidatorList { get; set; }

        /// <summary>
        /// The public key of the reserve stake account.
        /// </summary>
        public PublicKey ReserveStake { get; set; }

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
        public Fee EpochFee { get; set; }

        /// <summary>
        /// The fee for the next epoch.
        /// </summary>
        public Fee NextEpochFee { get; set; }

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
        public Fee StakeDepositFee { get; set; }

        /// <summary>
        /// The fee for stake withdrawals.
        /// </summary>
        public Fee StakeWithdrawalFee { get; set; }

        /// <summary>
        /// The fee for the next stake withdrawals.
        /// </summary>
        public Fee NextStakeWithdrawalFees { get; set; }

        /// <summary>
        /// The referral fee for stake operations.
        /// </summary>
        public byte StakeReferralFee { get; set; }

        /// <summary>
        /// The public key of the SOL deposit authority.
        /// </summary>
        public PublicKey SolDepositAuthority { get; set; }

        /// <summary>
        /// The fee for SOL deposits.
        /// </summary>
        public Fee SolDepositFee { get; set; }

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
        public Fee SolWithdrawalFee { get; set; }

        /// <summary>
        /// The fee for the next SOL withdrawals.
        /// </summary>
        public Fee NextSolWithdrawalFee { get; set; }

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
        /// Calculates the pool tokens that should be minted for a deposit of stake lamports.
        /// </summary>
        public ulong? CalcPoolTokensForDeposit(ulong stakeLamports)
        {
            if (TotalLamports == 0 || PoolTokenSupply == 0)
                return stakeLamports;

            try
            {
                BigInteger tokens = (BigInteger)stakeLamports * PoolTokenSupply;
                BigInteger result = tokens / TotalLamports;
                if (result < 0 || result > ulong.MaxValue) return null;
                return (ulong)result;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Calculates the lamports amount on withdrawal.
        /// </summary>
        public ulong? CalcLamportsWithdrawAmount(ulong poolTokens)
        {
            BigInteger numerator = (BigInteger)poolTokens * TotalLamports;
            BigInteger denominator = PoolTokenSupply;
            if (denominator == 0 || numerator < denominator)
                return 0;
            try
            {
                BigInteger result = numerator / denominator;
                if (result < 0 || result > ulong.MaxValue) return null;
                return (ulong)result;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Calculates pool tokens to be deducted as stake withdrawal fees.
        /// </summary>
        public ulong? CalcPoolTokensStakeWithdrawalFee(ulong poolTokens)
        {
            return StakeWithdrawalFee?.Apply(poolTokens);
        }

        /// <summary>
        /// Calculates pool tokens to be deducted as SOL withdrawal fees.
        /// </summary>
        public ulong? CalcPoolTokensSolWithdrawalFee(ulong poolTokens)
        {
            return SolWithdrawalFee?.Apply(poolTokens);
        }

        /// <summary>
        /// Calculates pool tokens to be deducted as stake deposit fees.
        /// </summary>
        public ulong? CalcPoolTokensStakeDepositFee(ulong poolTokensMinted)
        {
            return StakeDepositFee?.Apply(poolTokensMinted);
        }

        /// <summary>
        /// Calculates pool tokens to be deducted from deposit fees as referral fees.
        /// </summary>
        public ulong? CalcPoolTokensStakeReferralFee(ulong stakeDepositFee)
        {
            try
            {
                BigInteger result = (BigInteger)stakeDepositFee * StakeReferralFee / 100;
                if (result < 0 || result > ulong.MaxValue) return null;
                return (ulong)result;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Calculates pool tokens to be deducted as SOL deposit fees.
        /// </summary>
        public ulong? CalcPoolTokensSolDepositFee(ulong poolTokensMinted)
        {
            return SolDepositFee?.Apply(poolTokensMinted);
        }

        /// <summary>
        /// Calculates pool tokens to be deducted from SOL deposit fees as referral fees.
        /// </summary>
        public ulong? CalcPoolTokensSolReferralFee(ulong solDepositFee)
        {
            try
            {
                BigInteger result = (BigInteger)solDepositFee * SolReferralFee / 100;
                if (result < 0 || result > ulong.MaxValue) return null;
                return (ulong)result;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Calculates the fee in pool tokens that goes to the manager for a given reward lamports.
        /// </summary>
        public ulong? CalcEpochFeeAmount(ulong rewardLamports)
        {
            if (rewardLamports == 0)
                return 0;

            BigInteger totalLamports = (BigInteger)TotalLamports + rewardLamports;
            var feeLamports = EpochFee?.Apply(rewardLamports) ?? 0;

            if (totalLamports == feeLamports || PoolTokenSupply == 0)
                return rewardLamports;

            try
            {
                BigInteger result = (BigInteger)PoolTokenSupply * feeLamports /
                                    (totalLamports - feeLamports);
                if (result < 0 || result > ulong.MaxValue) return null;
                return (ulong)result;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the current value of pool tokens, rounded up.
        /// </summary>
        public ulong? GetLamportsPerPoolToken()
        {
            try
            {
                BigInteger result = ((BigInteger)TotalLamports + PoolTokenSupply - 1) / PoolTokenSupply;
                if (result < 0 || result > ulong.MaxValue) return null;
                return (ulong)result;
            }
            catch
            {
                return null;
            }
        }
    }
}
