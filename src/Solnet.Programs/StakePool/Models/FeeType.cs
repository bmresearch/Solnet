using System;

namespace Solnet.Programs.StakePool.Models
{
    /// <summary>
    /// The type of fees that can be set on the stake pool.
    /// </summary>
    public abstract class FeeType
    {
        /// <summary>
        /// Represents a referral fee type with a specified percentage.
        /// </summary>
        /// <remarks>This class is used to define a referral fee as a percentage value.  The percentage is
        /// immutable and must be specified at the time of instantiation.</remarks>
        public class SolReferral : FeeType
        {
            /// <summary>
            /// Gets the percentage value represented as a byte.
            /// </summary>
            public byte Percentage { get; }
            /// <summary>
            /// Initializes a new instance of the <see cref="SolReferral"/> class with the specified referral
            /// percentage.
            /// </summary>
            /// <param name="percentage">The referral percentage to be applied. Must be a value between 0 and 100, inclusive.</param>
            public SolReferral(byte percentage) => Percentage = percentage;
        }

        /// <summary>
        /// Represents a referral fee type with a specified percentage for staking rewards.
        /// </summary>
        /// <remarks>This class is used to define a referral fee as a percentage of staking rewards. The
        /// percentage value is immutable and must be specified at the time of instantiation.</remarks>
        public class StakeReferral : FeeType
        {
            /// <summary>
            /// Gets the percentage value represented as a byte.
            /// </summary>
            public byte Percentage { get; }
            /// <summary>
            /// Represents a referral with a specified stake percentage.
            /// </summary>
            /// <remarks>The <paramref name="percentage"/> parameter defines the proportion of the
            /// stake allocated to the referral.</remarks>
            /// <param name="percentage">The percentage of the stake associated with the referral. Must be a value between 0 and 100.</param>
            public StakeReferral(byte percentage) => Percentage = percentage;
        }

        /// <summary>
        /// Represents an epoch in the fee structure, associated with a specific fee.
        /// </summary>
        /// <remarks>This class is used to define a specific epoch and its corresponding fee.  It inherits
        /// from the <see cref="FeeType"/> base class.</remarks>
        public class Epoch : FeeType
        {
            /// <summary>
            /// Gets the fee associated with the transaction.
            /// </summary>
            public Fee Fee { get; }
            /// <summary>
            /// Represents a specific epoch with an associated fee.
            /// </summary>
            /// <param name="fee">The fee associated with the epoch. Cannot be null.</param>
            public Epoch(Fee fee) => Fee = fee;
        }

        /// <summary>
        /// Represents a withdrawal of staked funds, including an associated fee.
        /// </summary>
        /// <remarks>This class encapsulates the details of a stake withdrawal operation,  including the
        /// fee applied to the withdrawal. It inherits from <see cref="FeeType"/>.</remarks>
        public class StakeWithdrawal : FeeType
        {
            /// <summary>
            /// Gets the fee associated with the transaction.
            /// </summary>
            public Fee Fee { get; }
            /// <summary>
            /// Initializes a new instance of the <see cref="StakeWithdrawal"/> class with the specified fee.
            /// </summary>
            /// <param name="fee">The fee associated with the stake withdrawal. This value cannot be null.</param>
            public StakeWithdrawal(Fee fee) => Fee = fee;
        }

        /// <summary>
        /// Represents a deposit fee type specific to Solana transactions.
        /// </summary>
        /// <remarks>This class encapsulates the fee information for a Solana deposit transaction.  It
        /// inherits from the <see cref="FeeType"/> base class.</remarks>
        public class SolDeposit : FeeType
        {
            /// <summary>
            /// Gets the fee associated with the transaction.
            /// </summary>
            public Fee Fee { get; }
            /// <summary>
            /// Initializes a new instance of the <see cref="SolDeposit"/> class with the specified fee.
            /// </summary>
            /// <param name="fee">The fee associated with the deposit. This value cannot be null.</param>
            public SolDeposit(Fee fee) => Fee = fee;
        }

        /// <summary>
        /// Represents a deposit required for staking, associated with a specific fee.
        /// </summary>
        /// <remarks>This class encapsulates the concept of a staking deposit, which includes a fee that
        /// must be paid. It inherits from <see cref="FeeType"/>, providing additional context for fee-related
        /// operations.</remarks>
        public class StakeDeposit : FeeType
        {
            /// <summary>
            /// Gets the fee associated with the transaction.
            /// </summary>
            public Fee Fee { get; }
            /// <summary>
            /// Initializes a new instance of the <see cref="StakeDeposit"/> class with the specified fee.
            /// </summary>
            /// <param name="fee">The fee associated with the stake deposit. Cannot be null.</param>
            public StakeDeposit(Fee fee) => Fee = fee;
        }

        /// <summary>
        /// Represents a withdrawal operation for Solana (SOL) that includes an associated fee.
        /// </summary>
        /// <remarks>This class encapsulates the details of a Solana withdrawal, including the fee
        /// required for the transaction.</remarks>
        public class SolWithdrawal : FeeType
        {
            /// <summary>
            /// Gets the fee associated with the transaction.
            /// </summary>
            public Fee Fee { get; }
            /// <summary>
            /// Initializes a new instance of the <see cref="SolWithdrawal"/> class with the specified fee.
            /// </summary>
            /// <param name="fee">The fee associated with the withdrawal. This value cannot be null.</param>
            public SolWithdrawal(Fee fee) => Fee = fee;
        }

        /// <summary>
        /// Checks if the provided fee is too high.
        /// </summary>
        public bool IsTooHigh()
        {
            return this switch
            {
                SolReferral s => s.Percentage > 100,
                StakeReferral s => s.Percentage > 100,
                Epoch e => e.Fee.Numerator > e.Fee.Denominator,
                StakeWithdrawal s => s.Fee.Numerator > s.Fee.Denominator,
                SolWithdrawal s => s.Fee.Numerator > s.Fee.Denominator,
                SolDeposit s => s.Fee.Numerator > s.Fee.Denominator,
                StakeDeposit s => s.Fee.Numerator > s.Fee.Denominator,
                _ => false
            };
        }

        /// <summary>
        /// Returns true if the contained fee can only be updated earliest on the next epoch.
        /// </summary>
        public bool CanOnlyChangeNextEpoch()
        {
            return this is StakeWithdrawal or SolWithdrawal or Epoch;
        }
    }
}
