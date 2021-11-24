using Solnet.Programs.TokenLending.Models;
using Solnet.Rpc.Models;
using Solnet.Wallet;
using System.Collections.Generic;
using System.Linq;

namespace Solnet.Programs.TokenLending
{
    /// <summary>
    /// Implements the Token Lending Program methods.
    /// <remarks>
    /// For more information see:
    /// https://spl.solana.com/token-lending
    /// https://github.com/solana-labs/solana-program-library/tree/master/token-lending
    /// </remarks>
    /// </summary>
    public class TokenLendingProgram
    {
        /// <summary>
        /// SPL Token Lending Program Mainnet Program ID
        /// </summary>
        public static readonly PublicKey TokenLendingProgramIdKey =
            new PublicKey("LendZqTs8gn5CTSJU1jWKhKuVpjJGom45nnwPb2AMTi");

        /// <summary>
        /// SPL Token Lending Program Name.
        /// </summary>
        public static readonly string TokenLendingProgramName = "Token Lending Program";

        /// <summary>
        /// The public key of the Token Lending Program.
        /// </summary>
        public virtual PublicKey ProgramIdKey => TokenLendingProgramIdKey;

        /// <summary>
        /// The program's name.
        /// </summary>
        public virtual string ProgramName => TokenLendingProgramName;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="quoteCurrency"></param>
        /// <param name="lendingMarket"></param>
        /// <param name="oracleProgramId"></param>
        /// <returns></returns>
        public virtual TransactionInstruction InitializeLendingMarket(PublicKey owner,
            byte[] quoteCurrency, PublicKey lendingMarket, PublicKey oracleProgramId)
        {
            List<AccountMeta> keys = new()
            {
                AccountMeta.Writable(lendingMarket, false),
                AccountMeta.ReadOnly(SystemProgram.SysVarRentKey, false),
                AccountMeta.ReadOnly(TokenProgram.ProgramIdKey, false),
                AccountMeta.ReadOnly(oracleProgramId, false),

            };
            return new TransactionInstruction
            {
                ProgramId = TokenLendingProgramIdKey,
                Data = TokenLendingProgramData.EncodeInitializeLendingMarketData(owner, quoteCurrency),
                Keys = keys
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lendingMarket"></param>
        /// <param name="lendingMarketOwner"></param>
        /// <param name="newOwner"></param>
        /// <returns></returns>
        public virtual TransactionInstruction SetLendingMarketOwner(PublicKey lendingMarket,
            PublicKey lendingMarketOwner, PublicKey newOwner)
        {
            List<AccountMeta> keys = new()
            {
                AccountMeta.Writable(lendingMarket, false),
                AccountMeta.ReadOnly(lendingMarketOwner, true),
            };
            return new TransactionInstruction
            {
                ProgramId = TokenLendingProgramIdKey,
                Data = TokenLendingProgramData.EncodeSetLendingMarketOwnerData(newOwner),
                Keys = keys
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="liquidityAmount"></param>
        /// <param name="config"></param>
        /// <param name="sourceLiquidity"></param>
        /// <param name="destinationCollateral"></param>
        /// <param name="reserve"></param>
        /// <param name="reserveLiquidityMint"></param>
        /// <param name="reserveLiquiditySupply"></param>
        /// <param name="reserveLiquidityFeeReceiver"></param>
        /// <param name="reserveCollateralMint"></param>
        /// <param name="reserveCollateralSupply"></param>
        /// <param name="pythProduct"></param>
        /// <param name="pythPrice"></param>
        /// <param name="lendingMarket"></param>
        /// <param name="lendingMarketOwner"></param>
        /// <param name="userTransferAuthority"></param>
        /// <returns></returns>
        public virtual TransactionInstruction InitializeReserve(ulong liquidityAmount,
            ReserveConfig config, PublicKey sourceLiquidity, PublicKey destinationCollateral,
            PublicKey reserve, PublicKey reserveLiquidityMint, PublicKey reserveLiquiditySupply,
            PublicKey reserveLiquidityFeeReceiver, PublicKey reserveCollateralMint,
            PublicKey reserveCollateralSupply, PublicKey pythProduct, PublicKey pythPrice,
            PublicKey lendingMarket, PublicKey lendingMarketOwner, PublicKey userTransferAuthority)
        {
            PublicKey lendingMarketAuthority = TokenLendingProgramData.DeriveLendingMarketAuthority(lendingMarket, TokenLendingProgramIdKey);

            List<AccountMeta> keys = new()
            {
                AccountMeta.Writable(sourceLiquidity, false),
                AccountMeta.Writable(destinationCollateral, false),
                AccountMeta.Writable(reserve, false),
                AccountMeta.ReadOnly(reserveLiquidityMint, false),
                AccountMeta.Writable(reserveLiquiditySupply, false),
                AccountMeta.Writable(reserveLiquidityFeeReceiver, false),
                AccountMeta.Writable(reserveCollateralMint, false),
                AccountMeta.Writable(reserveCollateralSupply, false),
                AccountMeta.ReadOnly(pythProduct, false),
                AccountMeta.ReadOnly(pythPrice, false),
                AccountMeta.ReadOnly(lendingMarket, false),
                AccountMeta.ReadOnly(lendingMarketAuthority, false),
                AccountMeta.ReadOnly(lendingMarketOwner, true),
                AccountMeta.ReadOnly(userTransferAuthority, true),
                AccountMeta.ReadOnly(SystemProgram.SysVarClockKey, false),
                AccountMeta.ReadOnly(SystemProgram.SysVarRentKey, false),
                AccountMeta.ReadOnly(TokenProgram.ProgramIdKey, false)
            };
            return new TransactionInstruction
            {
                ProgramId = TokenLendingProgramIdKey,
                Data = TokenLendingProgramData.EncodeInitializeReserveData(liquidityAmount, config),
                Keys = keys
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reserve"></param>
        /// <param name="reserveLiquidityOracle"></param>
        /// <returns></returns>
        public virtual TransactionInstruction RefreshReserve(PublicKey reserve, PublicKey reserveLiquidityOracle)
        {

            List<AccountMeta> keys = new()
            {
                AccountMeta.Writable(reserve, false),
                AccountMeta.ReadOnly(reserveLiquidityOracle, false),
                AccountMeta.ReadOnly(SystemProgram.SysVarClockKey, false),
            };
            return new TransactionInstruction
            {
                ProgramId = TokenLendingProgramIdKey,
                Data = TokenLendingProgramData.EncodeRefreshReserveData(),
                Keys = keys
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="liquidityAmount"></param>
        /// <param name="sourceLiquidity"></param>
        /// <param name="destinationCollateral"></param>
        /// <param name="reserve"></param>
        /// <param name="reserveLiquiditySupply"></param>
        /// <param name="reserveCollateralMint"></param>
        /// <param name="lendingMarket"></param>
        /// <param name="userTransferAuthority"></param>
        /// <returns></returns>
        public virtual TransactionInstruction DepositReserveLiquidity(ulong liquidityAmount, PublicKey sourceLiquidity,
            PublicKey destinationCollateral, PublicKey reserve, PublicKey reserveLiquiditySupply, PublicKey reserveCollateralMint,
            PublicKey lendingMarket, PublicKey userTransferAuthority)
        {
            PublicKey lendingMarketAuthority = TokenLendingProgramData.DeriveLendingMarketAuthority(lendingMarket, TokenLendingProgramIdKey);

            List<AccountMeta> keys = new()
            {
                AccountMeta.Writable(sourceLiquidity, false),
                AccountMeta.Writable(destinationCollateral, false),
                AccountMeta.Writable(reserve, false),
                AccountMeta.Writable(reserveLiquiditySupply, false),
                AccountMeta.Writable(reserveCollateralMint, false),
                AccountMeta.ReadOnly(lendingMarket, false),
                AccountMeta.ReadOnly(lendingMarketAuthority, false),
                AccountMeta.ReadOnly(userTransferAuthority, true),
                AccountMeta.ReadOnly(SystemProgram.SysVarClockKey, false),
                AccountMeta.ReadOnly(TokenProgram.ProgramIdKey, false)
            };
            return new TransactionInstruction
            {
                ProgramId = TokenLendingProgramIdKey,
                Data = TokenLendingProgramData.EncodeDepositReserveLiquidityData(liquidityAmount),
                Keys = keys
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="collateralAmount"></param>
        /// <param name="sourceCollateral"></param>
        /// <param name="destinationLiquidity"></param>
        /// <param name="reserve"></param>
        /// <param name="reserveCollateralMint"></param>
        /// <param name="reserveCollateralSupply"></param>
        /// <param name="lendingMarket"></param>
        /// <param name="userTransferAuthority"></param>
        /// <returns></returns>
        public virtual TransactionInstruction RedeemReserveCollateral(ulong collateralAmount, PublicKey sourceCollateral,
            PublicKey destinationLiquidity, PublicKey reserve, PublicKey reserveCollateralMint, PublicKey reserveCollateralSupply,
            PublicKey lendingMarket, PublicKey userTransferAuthority)
        {
            PublicKey lendingMarketAuthority = TokenLendingProgramData.DeriveLendingMarketAuthority(lendingMarket, TokenLendingProgramIdKey);

            List<AccountMeta> keys = new()
            {
                AccountMeta.Writable(sourceCollateral, false),
                AccountMeta.Writable(destinationLiquidity, false),
                AccountMeta.Writable(reserve, false),
                AccountMeta.Writable(reserveCollateralMint, false),
                AccountMeta.Writable(reserveCollateralSupply, false),
                AccountMeta.ReadOnly(lendingMarket, false),
                AccountMeta.ReadOnly(lendingMarketAuthority, false),
                AccountMeta.ReadOnly(userTransferAuthority, true),
                AccountMeta.ReadOnly(SystemProgram.SysVarClockKey, false),
                AccountMeta.ReadOnly(TokenProgram.ProgramIdKey, false)
            };
            return new TransactionInstruction
            {
                ProgramId = TokenLendingProgramIdKey,
                Data = TokenLendingProgramData.EncodeRedeemReserveCollateralData(collateralAmount),
                Keys = keys
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obligation"></param>
        /// <param name="lendingMarket"></param>
        /// <param name="obligationOwner"></param>
        /// <returns></returns>
        public virtual TransactionInstruction InitializeObligation(PublicKey obligation, PublicKey lendingMarket, PublicKey obligationOwner)
        {
            List<AccountMeta> keys = new()
            {
                AccountMeta.Writable(obligation, false),
                AccountMeta.ReadOnly(lendingMarket, false),
                AccountMeta.ReadOnly(obligationOwner, true),
                AccountMeta.ReadOnly(SystemProgram.SysVarClockKey, false),
                AccountMeta.ReadOnly(SystemProgram.SysVarRentKey, false),
                AccountMeta.ReadOnly(TokenProgram.ProgramIdKey, false)
            };
            return new TransactionInstruction
            {
                ProgramId = TokenLendingProgramIdKey,
                Data = TokenLendingProgramData.EncodeInitializeObligationData(),
                Keys = keys
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obligation"></param>
        /// <param name="reserves"></param>
        /// <returns></returns>
        public virtual TransactionInstruction RefreshObligation(PublicKey obligation, IList<PublicKey> reserves)
        {
            List<AccountMeta> keys = new()
            {
                AccountMeta.Writable(obligation, false),
                AccountMeta.ReadOnly(SystemProgram.SysVarClockKey, false),
            };

            keys.AddRange(reserves.Select(x => AccountMeta.ReadOnly(x, false)));

            return new TransactionInstruction
            {
                ProgramId = TokenLendingProgramIdKey,
                Data = TokenLendingProgramData.EncodeRefreshObligationData(),
                Keys = keys
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="collateralAmount"></param>
        /// <param name="sourceCollateral"></param>
        /// <param name="destinationCollateral"></param>
        /// <param name="depositReserve"></param>
        /// <param name="obligation"></param>
        /// <param name="lendingMarket"></param>
        /// <param name="obligationOwner"></param>
        /// <param name="userTransferAuthority"></param>
        /// <returns></returns>
        public virtual TransactionInstruction DepositObligationCollateral(ulong collateralAmount, PublicKey sourceCollateral,
            PublicKey destinationCollateral, PublicKey depositReserve, PublicKey obligation, PublicKey lendingMarket,
            PublicKey obligationOwner, PublicKey userTransferAuthority)
        {
            List<AccountMeta> keys = new()
            {
                AccountMeta.Writable(sourceCollateral, false),
                AccountMeta.Writable(destinationCollateral, false),
                AccountMeta.ReadOnly(depositReserve, false),
                AccountMeta.Writable(obligation, false),
                AccountMeta.ReadOnly(lendingMarket, false),
                AccountMeta.ReadOnly(obligationOwner, true),
                AccountMeta.ReadOnly(userTransferAuthority, true),
                AccountMeta.ReadOnly(SystemProgram.SysVarClockKey, false),
                AccountMeta.ReadOnly(TokenProgram.ProgramIdKey, false)
            };


            return new TransactionInstruction
            {
                ProgramId = TokenLendingProgramIdKey,
                Data = TokenLendingProgramData.EncodeDepositObligationCollateralData(collateralAmount),
                Keys = keys
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="collateralAmount"></param>
        /// <param name="sourceCollateral"></param>
        /// <param name="destinationCollateral"></param>
        /// <param name="withdrawReserve"></param>
        /// <param name="obligation"></param>
        /// <param name="lendingMarket"></param>
        /// <param name="obligationOwner"></param>
        /// <returns></returns>
        public virtual TransactionInstruction WithdrawObligationCollateral(ulong collateralAmount, PublicKey sourceCollateral,
            PublicKey destinationCollateral, PublicKey withdrawReserve, PublicKey obligation, PublicKey lendingMarket,
            PublicKey obligationOwner)
        {
            PublicKey lendingMarketAuthority = TokenLendingProgramData.DeriveLendingMarketAuthority(lendingMarket, TokenLendingProgramIdKey);

            List<AccountMeta> keys = new()
            {
                AccountMeta.Writable(sourceCollateral, false),
                AccountMeta.Writable(destinationCollateral, false),
                AccountMeta.ReadOnly(withdrawReserve, false),
                AccountMeta.Writable(obligation, false),
                AccountMeta.ReadOnly(lendingMarket, false),
                AccountMeta.ReadOnly(lendingMarketAuthority, false),
                AccountMeta.ReadOnly(obligationOwner, true),
                AccountMeta.ReadOnly(SystemProgram.SysVarClockKey, false),
                AccountMeta.ReadOnly(TokenProgram.ProgramIdKey, false)
            };


            return new TransactionInstruction
            {
                ProgramId = TokenLendingProgramIdKey,
                Data = TokenLendingProgramData.EncodeWithdrawObligationCollateralData(collateralAmount),
                Keys = keys
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="liquidityAmount"></param>
        /// <param name="sourceLiquidity"></param>
        /// <param name="destinationLiquidity"></param>
        /// <param name="borrowReserve"></param>
        /// <param name="borrowReserveLiquidityFeeReceiver"></param>
        /// <param name="obligation"></param>
        /// <param name="lendingMarket"></param>
        /// <param name="obligationOwner"></param>
        /// <param name="hostFeeReceiver"></param>
        /// <returns></returns>
        public virtual TransactionInstruction BorrowObligationLiduidity(ulong liquidityAmount, PublicKey sourceLiquidity,
            PublicKey destinationLiquidity, PublicKey borrowReserve, PublicKey borrowReserveLiquidityFeeReceiver, PublicKey obligation,
            PublicKey lendingMarket, PublicKey obligationOwner, PublicKey hostFeeReceiver = null)
        {
            PublicKey lendingMarketAuthority = TokenLendingProgramData.DeriveLendingMarketAuthority(lendingMarket, TokenLendingProgramIdKey);

            List<AccountMeta> keys = new()
            {
                AccountMeta.Writable(sourceLiquidity, false),
                AccountMeta.Writable(destinationLiquidity, false),
                AccountMeta.Writable(borrowReserve, false),
                AccountMeta.Writable(borrowReserveLiquidityFeeReceiver, false),
                AccountMeta.Writable(obligation, false),
                AccountMeta.ReadOnly(lendingMarket, false),
                AccountMeta.ReadOnly(lendingMarketAuthority, false),
                AccountMeta.ReadOnly(obligationOwner, true),
                AccountMeta.ReadOnly(SystemProgram.SysVarClockKey, false),
                AccountMeta.ReadOnly(TokenProgram.ProgramIdKey, false)
            };

            if (hostFeeReceiver != null) keys.Add(AccountMeta.Writable(hostFeeReceiver, false));

            return new TransactionInstruction
            {
                ProgramId = TokenLendingProgramIdKey,
                Data = TokenLendingProgramData.EncodeBorrowObligationLiduidityData(liquidityAmount),
                Keys = keys
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="liquidityAmount"></param>
        /// <param name="sourceLiquidity"></param>
        /// <param name="destinationLiquidity"></param>
        /// <param name="repayReserve"></param>
        /// <param name="obligation"></param>
        /// <param name="lendingMarket"></param>
        /// <returns></returns>
        public virtual TransactionInstruction RepayObligationLiduidity(ulong liquidityAmount, PublicKey sourceLiquidity,
            PublicKey destinationLiquidity, PublicKey repayReserve, PublicKey obligation, PublicKey lendingMarket, PublicKey userTransferAuthority)
        {
            List<AccountMeta> keys = new()
            {
                AccountMeta.Writable(sourceLiquidity, false),
                AccountMeta.Writable(destinationLiquidity, false),
                AccountMeta.Writable(repayReserve, false),
                AccountMeta.Writable(obligation, false),
                AccountMeta.ReadOnly(lendingMarket, false),
                AccountMeta.ReadOnly(userTransferAuthority, true),
                AccountMeta.ReadOnly(SystemProgram.SysVarClockKey, false),
                AccountMeta.ReadOnly(TokenProgram.ProgramIdKey, false)
            };


            return new TransactionInstruction
            {
                ProgramId = TokenLendingProgramIdKey,
                Data = TokenLendingProgramData.EncodeRepayObligationLiduidityData(liquidityAmount),
                Keys = keys
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="liquidityAmount"></param>
        /// <param name="sourceLiquidity"></param>
        /// <param name="destinationCollateral"></param>
        /// <param name="repayReserve"></param>
        /// <param name="repayReserveLiquiditySupply"></param>
        /// <param name="withdrawReserve"></param>
        /// <param name="withdrawReserveCollateralSupply"></param>
        /// <param name="obligation"></param>
        /// <param name="lendingMarket"></param>
        /// <param name="userTransferAuthority"></param>
        /// <returns></returns>
        public virtual TransactionInstruction LiquidateObligation(ulong liquidityAmount, PublicKey sourceLiquidity,
            PublicKey destinationCollateral, PublicKey repayReserve, PublicKey repayReserveLiquiditySupply,
            PublicKey withdrawReserve, PublicKey withdrawReserveCollateralSupply, PublicKey obligation,
            PublicKey lendingMarket, PublicKey userTransferAuthority)
        {
            PublicKey lendingMarketAuthority = TokenLendingProgramData.DeriveLendingMarketAuthority(lendingMarket, TokenLendingProgramIdKey);

            List<AccountMeta> keys = new()
            {
                AccountMeta.Writable(sourceLiquidity, false),
                AccountMeta.Writable(destinationCollateral, false),
                AccountMeta.Writable(repayReserve, false),
                AccountMeta.Writable(repayReserveLiquiditySupply, false),
                AccountMeta.ReadOnly(withdrawReserve, false),
                AccountMeta.Writable(withdrawReserveCollateralSupply, false),
                AccountMeta.Writable(obligation, false),
                AccountMeta.ReadOnly(lendingMarket, false),
                AccountMeta.ReadOnly(lendingMarketAuthority, false),
                AccountMeta.ReadOnly(userTransferAuthority, true),
                AccountMeta.ReadOnly(SystemProgram.SysVarClockKey, false),
                AccountMeta.ReadOnly(TokenProgram.ProgramIdKey, false)
            };


            return new TransactionInstruction
            {
                ProgramId = TokenLendingProgramIdKey,
                Data = TokenLendingProgramData.EncodeLiquidateObligationData(liquidityAmount),
                Keys = keys
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="amount"></param>
        /// <param name="sourceLiquidity"></param>
        /// <param name="destinationLiquidity"></param>
        /// <param name="reserve"></param>
        /// <param name="reserveLiquidityFeeReceiver"></param>
        /// <param name="hostFeeReceiver"></param>
        /// <param name="lendingMarket"></param>
        /// <param name="flashLoanReceiverProgramId"></param>
        /// <param name="flashLoanReceiverProgramAccounts"></param>
        /// <returns></returns>
        public virtual TransactionInstruction FlashLoan(ulong amount, PublicKey sourceLiquidity,
            PublicKey destinationLiquidity, PublicKey reserve, PublicKey reserveLiquidityFeeReceiver,
            PublicKey hostFeeReceiver, PublicKey lendingMarket, PublicKey flashLoanReceiverProgramId,
            IList<AccountMeta> flashLoanReceiverProgramAccounts)
        {
            PublicKey lendingMarketAuthority = TokenLendingProgramData.DeriveLendingMarketAuthority(lendingMarket, TokenLendingProgramIdKey);

            List<AccountMeta> keys = new()
            {
                AccountMeta.Writable(sourceLiquidity, false),
                AccountMeta.Writable(destinationLiquidity, false),
                AccountMeta.Writable(reserve, false),
                AccountMeta.Writable(reserveLiquidityFeeReceiver, false),
                AccountMeta.Writable(hostFeeReceiver, false),
                AccountMeta.ReadOnly(lendingMarket, false),
                AccountMeta.ReadOnly(lendingMarketAuthority, false),
                AccountMeta.ReadOnly(SystemProgram.SysVarClockKey, false),
                AccountMeta.ReadOnly(TokenProgram.ProgramIdKey, false)
            };

            keys.AddRange(flashLoanReceiverProgramAccounts);

            return new TransactionInstruction
            {
                ProgramId = TokenLendingProgramIdKey,
                Data = TokenLendingProgramData.EncodeFlashLoanData(amount),
                Keys = keys
            };
        }

    }
}
