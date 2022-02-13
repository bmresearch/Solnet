using Solnet.Programs.Utilities;
using Solnet.Wallet;
using System;
using System.Numerics;

namespace Solnet.Programs.TokenLending.Models
{
    /// <summary>
    /// Reserve liquidity
    /// </summary>
    public class ReserveLiquidity
    {
        /// <summary>
        /// The layout of the <see cref="ReserveLiquidity"/> structure.
        /// </summary>
        public static class Layout
        {
            /// <summary>
            /// The length of the <see cref="ReserveLiquidity"/> structure.
            /// </summary>
            public const int Length = 185;

            /// <summary>
            /// The offset at which the mint value begins.
            /// </summary>
            public const int MintOffset = 0;

            /// <summary>
            /// The offset at which the decimals value begins.
            /// </summary>
            public const int DecimalsOffset = 32;

            /// <summary>
            /// The offset at which the supply value begins.
            /// </summary>
            public const int SupplyOffset = 33;

            /// <summary>
            /// The offset at which the fee receiver value begins.
            /// </summary>
            public const int FeeReceiverOffset = 65;

            /// <summary>
            /// The offset at which the oracle value begins.
            /// </summary>
            public const int OracleOffset = 97;

            /// <summary>
            /// The offset at which the available amount offset begins.
            /// </summary>
            public const int AvailableAmountOffset = 129;

            /// <summary>
            /// The offset at which the borrow amount value begins.
            /// </summary>
            public const int BorrowedAmountOffset = 137;

            /// <summary>
            /// The offset at which the cumulative borrow amount value begins.
            /// </summary>
            public const int CumulativeBorrowAmountOffset = 153;

            /// <summary>
            /// The offset at which the market price value begins.
            /// </summary>
            public const int MarketPriceOffset = 169;
        }
        /// <summary>
        /// Reserve liquidity mint address
        /// </summary>
        public PublicKey Mint;

        /// <summary>
        /// Reserve liquidity mint decimals
        /// </summary>
        public byte Decimals;

        /// <summary>
        /// Reserve liquidity supply address
        /// </summary>
        public PublicKey Supply;

        /// <summary>
        /// Reserve liquidity fee receiver address
        /// </summary>
        public PublicKey FeeReceiver;

        /// <summary>
        /// Reserve liquidity oracle account
        /// </summary>
        public PublicKey Oracle;

        /// <summary>
        /// Reserve liquidity available
        /// </summary>
        public ulong AvailableAmount;

        /// <summary>
        /// Reserve liquidity borrowed
        /// </summary>
        public BigInteger BorrowedAmountWads;

        /// <summary>
        /// Reserve liquidity cumulative borrow rate
        /// </summary>
        public BigInteger CumulativeBorrowAmountWads;

        /// <summary>
        /// Reserve liquidity market price in quote currency
        /// </summary>
        public BigInteger MarketPrice;

        /// <summary>
        /// Initiaize the <see cref="ReserveLiquidity"/> with the given data.
        /// </summary>
        /// <param name="data">The byte array.</param>
        public ReserveLiquidity(ReadOnlySpan<byte> data)
        {
            if (data.Length != Layout.Length)
                throw new ArgumentException($"{nameof(data)} has wrong size. Expected {Layout.Length} bytes, actual {data.Length} bytes.");
            Mint = data.GetPubKey(Layout.MintOffset);
            Decimals = data.GetU8(Layout.DecimalsOffset);
            Supply = data.GetPubKey(Layout.SupplyOffset);
            FeeReceiver = data.GetPubKey(Layout.FeeReceiverOffset);
            Oracle = data.GetPubKey(Layout.OracleOffset);
            AvailableAmount = data.GetU64(Layout.AvailableAmountOffset);
            BorrowedAmountWads = data.GetBigInt(Layout.BorrowedAmountOffset, 16, true);
            CumulativeBorrowAmountWads = data.GetBigInt(Layout.CumulativeBorrowAmountOffset, 16, true);
            MarketPrice = data.GetBigInt(Layout.MarketPriceOffset, 16, true);
        }

        /// <summary>
        /// Deserialize the given byte array into the <see cref="ReserveLiquidity"/> structure.
        /// </summary>
        /// <param name="data">The byte array.</param>
        /// <returns>The <see cref="ReserveLiquidity"/> instance.</returns>
        public static ReserveLiquidity Deserialize(byte[] data) => new(data.AsSpan());
    }

    /// <summary>
    /// Reserve collateral
    /// </summary>
    public class ReserveCollateral
    {
        /// <summary>
        /// The layout of the <see cref="ReserveCollateral"/> structure.
        /// </summary>
        public static class Layout
        {
            /// <summary>
            /// The length of the <see cref="ReserveCollateral"/> structure.
            /// </summary>
            public const int Length = 72;

            /// <summary>
            /// The offset at which the borrow fee value begins.
            /// </summary>
            public const int MintOffset = 0;

            /// <summary>
            /// The offset at which the flash loan fee value begins.
            /// </summary>
            public const int TotalSupplyOffset = 32;

            /// <summary>
            /// The offset at which the host fee percentage value begins.
            /// </summary>
            public const int SupplyOffset = 40;
        }

        /// <summary>
        /// Reserve collateral mint address
        /// </summary>
        public PublicKey Mint;

        /// <summary>
        /// Reserve collateral mint supply, used for exchange rate
        /// </summary>
        public ulong TotalSupply;

        /// <summary>
        /// Reserve collateral supply address
        /// </summary>
        public PublicKey Supply;

        /// <summary>
        /// Initialize the <see cref="ReserveCollateral"/> with the given data.
        /// </summary>
        /// <param name="data">The byte array.</param>
        public ReserveCollateral(ReadOnlySpan<byte> data)
        {
            if (data.Length != Layout.Length)
                throw new ArgumentException($"{nameof(data)} has wrong size. Expected {Layout.Length} bytes, actual {data.Length} bytes.");
            Mint = data.GetPubKey(Layout.MintOffset);
            TotalSupply = data.GetU64(Layout.TotalSupplyOffset);
            Supply = data.GetPubKey(Layout.SupplyOffset);
        }

        /// <summary>
        /// Deserialize the given byte array into the <see cref="ReserveCollateral"/> structure.
        /// </summary>
        /// <param name="data">The byte array.</param>
        /// <returns>The <see cref="ReserveCollateral"/> instance.</returns>
        public static ReserveCollateral Deserialize(byte[] data) => new(data.AsSpan());
    }

    /// <summary>
    /// Additional fee information on a reserve.
    ///
    /// These exist separately from interest accrual fees, and are specifically for the program owner
    /// and frontend host. The fees are paid out as a percentage of liquidity token amounts during
    /// repayments and liquidations.
    /// </summary>
    public class ReserveFees
    {
        /// <summary>
        /// The layout of the <see cref="ReserveFees"/> structure.
        /// </summary>
        public static class Layout
        {
            /// <summary>
            /// The length of the <see cref="ReserveFees"/>  structure.
            /// </summary>
            public const int Length = 17;

            /// <summary>
            /// The offset at which the borrow fee value begins.
            /// </summary>
            public const int BorrowFeeWadOffset = 0;

            /// <summary>
            /// The offset at which the flash loan fee value begins.
            /// </summary>
            public const int FlashLoanFeeWadOffset = 8;

            /// <summary>
            /// The offset at which the host fee percentage value begins.
            /// </summary>
            public const int HostFeePercentageOffset = 16;
        }

        /// <summary>
        /// Fee assessed on `BorrowObligationLiquidity`, expressed as a Wad.
        /// Must be between 0 and 10^18, such that 10^18 = 1.  A few examples for
        /// clarity:
        /// 1% = 10_000_000_000_000_000
        /// 0.01% (1 basis point) = 100_000_000_000_000
        /// 0.00001% (Aave borrow fee) = 100_000_000_000
        /// </summary>
        public ulong BorrowFeeWad;

        /// <summary>
        /// Fee for flash loan, expressed as a Wad.
        /// 0.3% (Aave flash loan fee) = 3_000_000_000_000_000
        /// </summary>
        public ulong FlashLoanFeeWad;

        /// <summary>
        /// Amount of fee going to host account, if provided in liquidate and repay
        /// </summary>
        public byte HostFeePercentage;

        /// <summary>
        /// Initialize the <see cref="ReserveFees"/> with the given data.
        /// </summary>
        /// <param name="data">The byte array.</param>
        public ReserveFees(ReadOnlySpan<byte> data)
        {
            if (data.Length != Layout.Length)
                throw new ArgumentException($"{nameof(data)} has wrong size. Expected {Layout.Length} bytes, actual {data.Length} bytes.");

            BorrowFeeWad = data.GetU64(Layout.BorrowFeeWadOffset);
            FlashLoanFeeWad = data.GetU64(Layout.FlashLoanFeeWadOffset);
            HostFeePercentage = data.GetU8(Layout.HostFeePercentageOffset);
        }

        /// <summary>
        /// Deserialize the given byte array into the <see cref="ReserveFees"/> structure.
        /// </summary>
        /// <param name="data">The byte array.</param>
        /// <returns>The <see cref="ReserveFees"/> instance.</returns>
        public static ReserveFees Deserialize(byte[] data)
            => new(data.AsSpan());

        /// <summary>
        /// Serializes the <see cref="ReserveFees"/> object to the given buffer at the desired offset.
        /// </summary>
        /// <param name="buffer">The buffer to serialize into.</param>
        /// <param name="offset">The offset at which to begin serialization.</param>
        public void Serialize(byte[] buffer, int offset = 0)
        {
            buffer.WriteU64(BorrowFeeWad, offset + Layout.BorrowFeeWadOffset);
            buffer.WriteU64(FlashLoanFeeWad, offset + Layout.FlashLoanFeeWadOffset);
            buffer.WriteU8(HostFeePercentage, offset + Layout.HostFeePercentageOffset);
        }
    }

    /// <summary>
    /// Reserve configuration values
    /// </summary>
    public class ReserveConfig
    {
        /// <summary>
        /// The layout of the <see cref="ReserveConfig"/> structure.
        /// </summary>
        public static class Layout
        {
            /// <summary>
            /// The length of the <see cref="ReserveConfig"/> structure.
            /// </summary>
            public const int Length = 24;

            /// <summary>
            /// The offset at which the optimal utilization rate value begins.
            /// </summary>
            public const int OptimalUtilizationRateOffset = 0;

            /// <summary>
            /// The offset at which the loan to value ratio value begins.
            /// </summary>
            public const int LoanToValueRatioOffset = 1;

            /// <summary>
            /// The offset at which the liquidation bonus value begins.
            /// </summary>
            public const int LiquidationBonusOffset = 2;

            /// <summary>
            /// The offset at which the liquidation threshold value begins-
            /// </summary>
            public const int LiquidationThresholdOffset = 3;

            /// <summary>
            /// The offset at which the minimum borrow rate value begins.
            /// </summary>
            public const int MinBorrowRateOffset = 4;

            /// <summary>
            /// The offset at which the optimal borrow rate value begins.
            /// </summary>
            public const int OptimalBorrowRateOffset = 5;

            /// <summary>
            /// The offset at which the maximum borrow rate value begins.
            /// </summary>
            public const int MaxBorrowRateOffset = 6;

            /// <summary>
            /// The offset at which the fees value begins.
            /// </summary>
            public const int FeesOffset = 7;
        }

        /// <summary>
        /// Optimal utilization rate, as a percentage
        /// </summary>
        public byte OptimalUtilizationRate;

        /// <summary>
        /// Target ratio of the value of borrows to deposits, as a percentage
        /// 0 if use as collateral is disabled
        /// </summary>
        public byte LoanToValueRatio;

        /// <summary>
        /// Bonus a liquidator gets when repaying part of an unhealthy obligation, as a percentage
        /// </summary>
        public byte LiquidationBonus;

        /// <summary>
        /// Loan to value ratio at which an obligation can be liquidated, as a percentage
        /// </summary>
        public byte LiquidationThreshold;

        /// <summary>
        /// Min borrow APY
        /// </summary>
        public byte MinBorrowRate;

        /// <summary>
        /// Optimal (utilization) borrow APY
        /// </summary>
        public byte OptimalBorrowRate;

        /// <summary>
        /// Max borrow APY
        /// </summary>
        public byte MaxBorrowRate;

        /// <summary>
        /// Program owner fees assessed, separate from gains due to interest accrual
        /// </summary>
        public ReserveFees Fees;

        /// <summary>
        /// Initialize the <see cref="ReserveConfig"/> with the given data.
        /// </summary>
        /// <param name="data">The byte array.</param>
        public ReserveConfig(ReadOnlySpan<byte> data)
        {
            if (data.Length != Layout.Length)
                throw new ArgumentException($"{nameof(data)} has wrong size. Expected {Layout.Length} bytes, actual {data.Length} bytes.");

            OptimalUtilizationRate = data.GetU8(Layout.OptimalUtilizationRateOffset);
            LoanToValueRatio = data.GetU8(Layout.LoanToValueRatioOffset);
            LiquidationBonus = data.GetU8(Layout.LiquidationBonusOffset);
            LiquidationThreshold = data.GetU8(Layout.LiquidationThresholdOffset);
            MinBorrowRate = data.GetU8(Layout.MinBorrowRateOffset);
            OptimalBorrowRate = data.GetU8(Layout.OptimalBorrowRateOffset);
            MaxBorrowRate = data.GetU8(Layout.MaxBorrowRateOffset);
            Fees = new(data.Slice(Layout.FeesOffset, ReserveFees.Layout.Length));
        }

        /// <summary>
        /// Deserialize the given byte array into the <see cref="ReserveConfig"/> structure.
        /// </summary>
        /// <param name="data">The byte array.</param>
        /// <returns>The <see cref="ReserveConfig"/> instance.</returns>
        public static ReserveConfig Deserialize(byte[] data)
            => new(data.AsSpan());

        /// <summary>
        /// Serializes the <see cref="ReserveConfig"/> object to the given buffer at the desired offset.
        /// </summary>
        /// <param name="buffer">The buffer to serialize into.</param>
        /// <param name="offset">The offset at which to begin serialization.</param>
        public void Serialize(byte[] buffer, int offset = 0)
        {
            buffer.WriteU8(OptimalUtilizationRate, offset + Layout.OptimalUtilizationRateOffset);
            buffer.WriteU8(LoanToValueRatio, offset + Layout.LoanToValueRatioOffset);
            buffer.WriteU8(LiquidationBonus, offset + Layout.LiquidationBonusOffset);
            buffer.WriteU8(LiquidationThreshold, offset + Layout.LiquidationThresholdOffset);
            buffer.WriteU8(MinBorrowRate, offset + Layout.MinBorrowRateOffset);
            buffer.WriteU8(OptimalBorrowRate, offset + Layout.OptimalBorrowRateOffset);
            buffer.WriteU8(MaxBorrowRate, offset + Layout.MaxBorrowRateOffset);
            Fees.Serialize(buffer, offset + Layout.FeesOffset);
        }
    }

    /// <summary>
    /// Lending market reserve state
    /// </summary>
    public class Reserve
    {
        /// <summary>
        /// The layout of the <see cref="Reserve"/> structure.
        /// </summary>
        public static class Layout
        {
            /// <summary>
            /// The length of the <see cref="Reserve"/> structure.
            /// </summary>
            public const int Length = 571;

            /// <summary>
            /// The offset at which the version value begins.
            /// </summary>
            public const int VersionOffset = 0;

            /// <summary>
            /// The offset at which the last update value begins.
            /// </summary>
            public const int LastUpdateOffset = 1;

            /// <summary>
            /// The offset at which the lending market value begins.
            /// </summary>
            public const int LendingMarketOffset = 10;

            /// <summary>
            /// The offset at which the liquidity value begins.
            /// </summary>
            public const int LiquidityOffset = 42;

            /// <summary>
            /// The offset at which the collateral value begins.
            /// </summary>
            public const int CollateralOffset = 227;

            /// <summary>
            /// The offset at which the config value begins.
            /// </summary>
            public const int ConfigOffset = 299;
        }

        /// <summary>
        /// Version of the struct
        /// </summary>
        public byte Version;

        /// <summary>
        /// Last slot when supply and rates updated
        /// </summary>
        public LastUpdate LastUpdate;

        /// <summary>
        /// Lending market public key
        /// </summary>
        public PublicKey LendingMarket;

        /// <summary>
        /// Reserve liquidity
        /// </summary>
        public ReserveLiquidity Liquidity;

        /// <summary>
        /// Reserve collateral
        /// </summary>
        public ReserveCollateral Collateral;

        /// <summary>
        /// Reserve configuration values
        /// </summary>
        public ReserveConfig Config;

        /// <summary>
        /// Initialize the <see cref="Reserve"/> with the given data.
        /// </summary>
        /// <param name="data">The byte array.</param>
        public Reserve(ReadOnlySpan<byte> data)
        {
            if (data.Length != Layout.Length)
                throw new ArgumentException($"{nameof(data)} has wrong size. Expected {Layout.Length} bytes, actual {data.Length} bytes.");
            Version = data.GetU8(Layout.VersionOffset);
            LendingMarket = data.GetPubKey(Layout.LendingMarketOffset);
            Liquidity = new(data.Slice(Layout.LiquidityOffset, ReserveLiquidity.Layout.Length));
            Collateral = new(data.Slice(Layout.CollateralOffset, ReserveCollateral.Layout.Length));
            Config = new(data.Slice(Layout.ConfigOffset, ReserveConfig.Layout.Length));
        }

        /// <summary>
        /// Deserialize the given byte array into the <see cref="Reserve"/> structure.
        /// </summary>
        /// <param name="data">The byte array.</param>
        /// <returns>The <see cref="Reserve"/> instance.</returns>
        public static Reserve Deserialize(byte[] data) => new(data.AsSpan());
    }
}
