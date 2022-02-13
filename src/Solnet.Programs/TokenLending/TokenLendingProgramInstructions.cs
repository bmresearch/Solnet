using System.Collections.Generic;

namespace Solnet.Programs.TokenLending
{
    /// <summary>
    /// Represents the instruction types for the <see cref="TokenLendingProgram"/> along with a friendly name so as not to use reflection.
    /// <remarks>
    /// For more information see:
    /// https://spl.solana.com/token-lending
    /// https://github.com/solana-labs/solana-program-library/tree/master/token-lending
    /// </remarks>
    /// </summary>
    internal static class TokenLendingProgramInstructions
    {
        /// <summary>
        /// Represents the user-friendly names for the instruction types for the <see cref="TokenLendingProgram"/>.
        /// </summary>
        internal static readonly Dictionary<Values, string> Names = new()
        {
            { Values.InitializeLendingMarket, "Initialize Lending Market" },
            { Values.SetLendingMarketOwner, "Set LendingMarket Owner" },
            { Values.InitializeReserve, "Initialize Reserve" },
            { Values.RefreshReserve, "Refresh Reserve" },
            { Values.DepositReserveLiquidity, "Deposit Reserve Liquidity" },
            { Values.RedeemReserveCollateral, "Redeem Reserve Collateral" },
            { Values.InitializeObligation, "Initialize Obligation" },
            { Values.RefreshObligation, "Refresh Obligation" },
            { Values.DepositObligationCollateral, "Deposit Obligation Collateral" },
            { Values.WithdrawObligationCollateral, "Withdraw Obligation Collateral" },
            { Values.BorrowObligationLiquidity, "Borrow Obligation Liquidity" },
            { Values.RepayObligationLiquidity, "Repay Obligation Liquidity" },
            { Values.LiquidateObligation, "Liquidate Obligation" },
            { Values.FlashLoan, "Flash Loan" },
        };

        /// <summary>
        /// Represents the instruction types for the <see cref="TokenLendingProgram"/>.
        /// </summary>
        internal enum Values : byte
        {
            /// <summary>
            /// Initializes a new lending market.
            /// </summary>
            InitializeLendingMarket = 0,

            /// <summary>
            /// Sets the new owner of a lending market.
            /// </summary>
            SetLendingMarketOwner = 1,

            /// <summary>
            /// Initializes a new lending market reserve.
            /// </summary>
            InitializeReserve = 2,

            /// <summary>
            /// Accrue interest and update market price of liquidity on a reserve.
            /// </summary>
            RefreshReserve = 3,

            /// <summary>
            /// Deposit liquidity into a reserve in exchange for collateral. Collateral represents a share
            /// of the reserve liquidity pool.
            /// </summary>
            DepositReserveLiquidity = 4,

            /// <summary>
            /// Redeem collateral from a reserve in exchange for liquidity.
            /// </summary>
            RedeemReserveCollateral = 5,

            /// <summary>
            /// Initializes a new lending market obligation.
            /// </summary>
            InitializeObligation = 6,

            /// <summary>
            /// Refresh an obligation's accrued interest and collateral and liquidity prices. Requires
            /// refreshed reserves, as all obligation collateral deposit reserves in order, followed by all
            /// liquidity borrow reserves in order.
            /// </summary>
            RefreshObligation = 7,

            /// <summary>
            /// Deposit collateral to an obligation. Requires a refreshed reserve.
            /// </summary>
            DepositObligationCollateral = 8,

            /// <summary>
            /// Withdraw collateral from an obligation. Requires a refreshed obligation and reserve.
            /// </summary>
            WithdrawObligationCollateral = 9,

            /// <summary>
            /// Borrow liquidity from a reserve by depositing collateral tokens. Requires a refreshed
            /// obligation and reserve.
            /// </summary>
            BorrowObligationLiquidity = 10,

            /// <summary>
            /// Repay borrowed liquidity to a reserve. Requires a refreshed obligation and reserve.
            /// </summary>
            RepayObligationLiquidity = 11,

            /// <summary>
            /// Repay borrowed liquidity to a reserve to receive collateral at a discount from an unhealthy
            /// obligation. Requires a refreshed obligation and reserves.
            /// </summary>
            LiquidateObligation = 12,

            /// <summary>
            /// Make a flash loan.
            /// </summary>
            FlashLoan = 13,
        }
    }
}
