using System.Numerics;

namespace Solnet.Programs.StakePool.Models
{
    /// <summary>
    /// Fee rate as a ratio, minted on UpdateStakePoolBalance as a proportion of the rewards.
    /// If either the numerator or the denominator is 0, the fee is considered to be 0.
    /// </summary>
    public class Fee
    {
        /// <summary>
        /// Denominator of the fee ratio.
        /// </summary>
        public ulong Denominator { get; set; }

        /// <summary>
        /// Numerator of the fee ratio.
        /// </summary>
        public ulong Numerator { get; set; }

        /// <summary>
        /// Returns true if the fee is considered zero (either numerator or denominator is zero).
        /// </summary>
        public bool IsZero => Denominator == 0 || Numerator == 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="Fee"/> class.
        /// </summary>
        public Fee() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Fee"/> class with the specified numerator and denominator.
        /// </summary>
        /// <param name="numerator"></param>
        /// <param name="denominator"></param>
        public Fee(ulong numerator, ulong denominator)
        {
            Numerator = numerator;
            Denominator = denominator;
        }

        /// <summary>
        /// Applies the fee's rates to a given amount, returning the amount to be subtracted as fees.
        /// Returns 0 if denominator is 0 or amount is 0, or null if overflow occurs.
        /// </summary>
        public ulong? Apply(ulong amount)
        {
            if (Denominator == 0 || amount == 0)
                return 0;

            try
            {
                // Use BigInteger to avoid overflow
                BigInteger amt = new BigInteger(amount);
                BigInteger numerator = new BigInteger(Numerator);
                BigInteger denominator = new BigInteger(Denominator);

                BigInteger feeNumerator = amt * numerator;
                // Ceiling division: (feeNumerator + denominator - 1) / denominator
                BigInteger result = (feeNumerator + denominator - 1) / denominator;

                if (result < 0 || result > ulong.MaxValue)
                    return null;

                return (ulong)result;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Checks withdrawal fee restrictions, throws StakePoolFeeException if not met.
        /// </summary>
        public void CheckWithdrawal(Fee oldWithdrawalFee)
        {
            // Constants as per SPL Stake Pool program
            var WITHDRAWAL_BASELINE_FEE = new Fee(1, 1000); // 0.1%
            var MAX_WITHDRAWAL_FEE_INCREASE = new Fee(3, 2); // 1.5x

            ulong oldNum, oldDenom;
            if (oldWithdrawalFee.Denominator == 0 || oldWithdrawalFee.Numerator == 0)
            {
                oldNum = WITHDRAWAL_BASELINE_FEE.Numerator;
                oldDenom = WITHDRAWAL_BASELINE_FEE.Denominator;
            }
            else
            {
                oldNum = oldWithdrawalFee.Numerator;
                oldDenom = oldWithdrawalFee.Denominator;
            }

            // Check that new_fee / old_fee <= MAX_WITHDRAWAL_FEE_INCREASE
            try
            {
                BigInteger left = (BigInteger)oldNum * Denominator * MAX_WITHDRAWAL_FEE_INCREASE.Numerator;
                BigInteger right = (BigInteger)Numerator * oldDenom * MAX_WITHDRAWAL_FEE_INCREASE.Denominator;

                if (left < right)
                {
                    throw new StakePoolFeeException("Fee increase exceeds maximum allowed.");
                }
            }
            catch
            {
                throw new StakePoolFeeException("Calculation failure in withdrawal fee check.");
            }
        }

        /// <summary>
        /// Returns a string representation of the fee.
        /// </summary>
        public override string ToString()
        {
            if (Numerator > 0 && Denominator > 0)
                return $"{Numerator}/{Denominator}";
            return "none";
        }
    }

    /// <summary>
    /// Exception for stake pool fee errors.
    /// </summary>
    public class StakePoolFeeException : System.Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StakePoolFeeException"/> class with a specified error message.
        /// </summary>
        /// <param name="message"></param>
        public StakePoolFeeException(string message) : base(message) { }
    }
}
