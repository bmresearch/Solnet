using Solnet.Programs.Abstract;
using Solnet.Programs.TokenLending.Models;
using Solnet.Rpc;
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
    public class TokenLendingProgram : BaseProgram
    {
        /// <summary>
        /// SPL Token Lending Program MainNet Program ID.
        /// <remarks>
        /// As stated in the docs linked below this program is not currently operational.
        /// https://github.com/solana-labs/solana-program-library/tree/master/token-lending
        /// </remarks>
        /// </summary>
        public static readonly PublicKey MainNetProgramIdKey =
            new PublicKey("LendZqTs8gn5CTSJU1jWKhKuVpjJGom45nnwPb2AMTi");

        /// <summary>
        /// SPL Token Lending Program DevNet Program ID.
        /// </summary>
        public static readonly PublicKey DevNetProgramIdKey =
            new PublicKey("6TvznH3B2e3p2mbhufNBpgSrLx6UkgvxtVQvopEZ2kuH");

        /// <summary>
        /// SPL Token Lending Program Name.
        /// </summary>
        public const string DefaultProgramName = "Token Lending Program";

        /// <summary>
        /// Initialize the <see cref="TokenLendingProgram"/> with the given program id key and program name.
        /// </summary>
        /// <param name="programIdKey">The program id key.</param>
        /// <param name="programName">The program name.</param>
        public TokenLendingProgram(PublicKey programIdKey, string programName = DefaultProgramName) 
            : base(programIdKey, programName) { }

        /// <summary>
        /// Initialize the <see cref="TokenLendingProgram"/> for <see cref="Cluster.DevNet"/>.
        /// </summary>
        /// <returns>The <see cref="TokenLendingProgram"/> instance.</returns>
        public static TokenLendingProgram CreateDevNet() => new TokenLendingProgram(DevNetProgramIdKey);

        /// <summary>
        /// Initialize the <see cref="TokenLendingProgram"/> for <see cref="Cluster.MainNet"/>.
        /// </summary>
        /// <returns>The <see cref="TokenLendingProgram"/> instance.</returns>
        public static TokenLendingProgram CreateMainNet() => new TokenLendingProgram(MainNetProgramIdKey);

        /// <summary>
        /// Initializes an instruction to initialize a <see cref="LendingMarket"/>.
        /// </summary>
        /// <param name="owner">The public key of the owner.</param>
        /// <param name="quoteCurrency">The quote currency.</param>
        /// <param name="lendingMarket">The public key of the lending market.</param>
        /// <param name="oracleProgramId">The public key of the oracle program.</param>
        /// <returns>The transaction instruction.</returns>
        public TransactionInstruction InitializeLendingMarket(PublicKey owner,
            byte[] quoteCurrency, PublicKey lendingMarket, PublicKey oracleProgramId)
            => InitializeLendingMarket(ProgramIdKey, owner, quoteCurrency, lendingMarket, oracleProgramId);

        /// <summary>
        /// Initializes an instruction to initialize a <see cref="LendingMarket"/>.
        /// </summary>
        /// <param name="programIdKey">The public key of the program.</param>
        /// <param name="owner">The public key of the owner.</param>
        /// <param name="quoteCurrency">The quote currency.</param>
        /// <param name="lendingMarket">The public key of the lending market.</param>
        /// <param name="oracleProgramId">The public key of the oracle program.</param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction InitializeLendingMarket(PublicKey programIdKey, PublicKey owner,
            byte[] quoteCurrency, PublicKey lendingMarket, PublicKey oracleProgramId)
        {
            List<AccountMeta> keys = new()
            {
                AccountMeta.Writable(lendingMarket, false),
                AccountMeta.ReadOnly(SysVars.RentKey, false),
                AccountMeta.ReadOnly(TokenProgram.ProgramIdKey, false),
                AccountMeta.ReadOnly(oracleProgramId, false),

            };
            return new TransactionInstruction
            {
                ProgramId = programIdKey,
                Data = TokenLendingProgramData.EncodeInitializeLendingMarketData(owner, quoteCurrency),
                Keys = keys
            };
        }

        /// <summary>
        /// Initializes an instruction to set a <see cref="LendingMarket"/> owner.
        /// </summary>
        /// <param name="lendingMarket">The public key of the lending market.</param>
        /// <param name="lendingMarketOwner">The public key of the current lending market owner.</param>
        /// <param name="newOwner">The public key of the new owner.</param>
        /// <returns>The transaction instruction.</returns>
        public virtual TransactionInstruction SetLendingMarketOwner(PublicKey lendingMarket,
            PublicKey lendingMarketOwner, PublicKey newOwner)
            => SetLendingMarketOwner(ProgramIdKey, lendingMarket, lendingMarketOwner, newOwner);


        /// <summary>
        /// Initializes an instruction to set a <see cref="LendingMarket"/> owner.
        /// </summary>
        /// <param name="programIdKey">The public key of the program.</param>
        /// <param name="lendingMarket">The public key of the lending market.</param>
        /// <param name="lendingMarketOwner">The public key of the current lending market owner.</param>
        /// <param name="newOwner">The public key of the new owner.</param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction SetLendingMarketOwner(PublicKey programIdKey, PublicKey lendingMarket,
            PublicKey lendingMarketOwner, PublicKey newOwner)
        {
            List<AccountMeta> keys = new()
            {
                AccountMeta.Writable(lendingMarket, false),
                AccountMeta.ReadOnly(lendingMarketOwner, true),
            };
            return new TransactionInstruction
            {
                ProgramId = programIdKey,
                Data = TokenLendingProgramData.EncodeSetLendingMarketOwnerData(newOwner),
                Keys = keys
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="programIdKey">The public key of the program.</param>
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
        /// <param name="lendingMarket">The public key of the lending market.</param>
        /// <param name="lendingMarketOwner">The public key of the current lending market owner.</param>
        /// <param name="userTransferAuthority"></param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction InitializeReserve(PublicKey programIdKey, ulong liquidityAmount,
            ReserveConfig config, PublicKey sourceLiquidity, PublicKey destinationCollateral,
            PublicKey reserve, PublicKey reserveLiquidityMint, PublicKey reserveLiquiditySupply,
            PublicKey reserveLiquidityFeeReceiver, PublicKey reserveCollateralMint,
            PublicKey reserveCollateralSupply, PublicKey pythProduct, PublicKey pythPrice,
            PublicKey lendingMarket, PublicKey lendingMarketOwner, PublicKey userTransferAuthority)
        {
            PublicKey lendingMarketAuthority = TokenLendingProgramData.DeriveLendingMarketAuthority(lendingMarket, programIdKey);

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
                AccountMeta.ReadOnly(SysVars.ClockKey, false),
                AccountMeta.ReadOnly(SysVars.RentKey, false),
                AccountMeta.ReadOnly(TokenProgram.ProgramIdKey, false)
            };
            return new TransactionInstruction
            {
                ProgramId = programIdKey,
                Data = TokenLendingProgramData.EncodeInitializeReserveData(liquidityAmount, config),
                Keys = keys
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="programIdKey">The public key of the program.</param>
        /// <param name="reserve"></param>
        /// <param name="reserveLiquidityOracle"></param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction RefreshReserve(PublicKey programIdKey, PublicKey reserve, PublicKey reserveLiquidityOracle)
        {

            List<AccountMeta> keys = new()
            {
                AccountMeta.Writable(reserve, false),
                AccountMeta.ReadOnly(reserveLiquidityOracle, false),
                AccountMeta.ReadOnly(SysVars.ClockKey, false),
            };
            return new TransactionInstruction
            {
                ProgramId = programIdKey,
                Data = TokenLendingProgramData.EncodeRefreshReserveData(),
                Keys = keys
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="programIdKey">The public key of the program.</param>
        /// <param name="liquidityAmount"></param>
        /// <param name="sourceLiquidity"></param>
        /// <param name="destinationCollateral"></param>
        /// <param name="reserve"></param>
        /// <param name="reserveLiquiditySupply"></param>
        /// <param name="reserveCollateralMint"></param>
        /// <param name="lendingMarket"></param>
        /// <param name="userTransferAuthority"></param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction DepositReserveLiquidity(PublicKey programIdKey, ulong liquidityAmount,
            PublicKey sourceLiquidity, PublicKey destinationCollateral, PublicKey reserve, PublicKey reserveLiquiditySupply,
            PublicKey reserveCollateralMint, PublicKey lendingMarket, PublicKey userTransferAuthority)
        {
            PublicKey lendingMarketAuthority = TokenLendingProgramData.DeriveLendingMarketAuthority(lendingMarket, programIdKey);

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
                AccountMeta.ReadOnly(SysVars.ClockKey, false),
                AccountMeta.ReadOnly(TokenProgram.ProgramIdKey, false)
            };
            return new TransactionInstruction
            {
                ProgramId = programIdKey,
                Data = TokenLendingProgramData.EncodeDepositReserveLiquidityData(liquidityAmount),
                Keys = keys
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="programIdKey">The public key of the program.</param>
        /// <param name="collateralAmount"></param>
        /// <param name="sourceCollateral"></param>
        /// <param name="destinationLiquidity"></param>
        /// <param name="reserve"></param>
        /// <param name="reserveCollateralMint"></param>
        /// <param name="reserveCollateralSupply"></param>
        /// <param name="lendingMarket"></param>
        /// <param name="userTransferAuthority"></param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction RedeemReserveCollateral(PublicKey programIdKey, ulong collateralAmount,
            PublicKey sourceCollateral, PublicKey destinationLiquidity, PublicKey reserve, PublicKey reserveCollateralMint,
            PublicKey reserveCollateralSupply, PublicKey lendingMarket, PublicKey userTransferAuthority)
        {
            PublicKey lendingMarketAuthority = TokenLendingProgramData.DeriveLendingMarketAuthority(lendingMarket, programIdKey);

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
                AccountMeta.ReadOnly(SysVars.ClockKey, false),
                AccountMeta.ReadOnly(TokenProgram.ProgramIdKey, false)
            };
            return new TransactionInstruction
            {
                ProgramId = programIdKey,
                Data = TokenLendingProgramData.EncodeRedeemReserveCollateralData(collateralAmount),
                Keys = keys
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="programIdKey">The public key of the program.</param>
        /// <param name="obligation"></param>
        /// <param name="lendingMarket"></param>
        /// <param name="obligationOwner"></param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction InitializeObligation(PublicKey programIdKey, PublicKey obligation,
            PublicKey lendingMarket, PublicKey obligationOwner)
        {
            List<AccountMeta> keys = new()
            {
                AccountMeta.Writable(obligation, false),
                AccountMeta.ReadOnly(lendingMarket, false),
                AccountMeta.ReadOnly(obligationOwner, true),
                AccountMeta.ReadOnly(SysVars.ClockKey, false),
                AccountMeta.ReadOnly(SysVars.RentKey, false),
                AccountMeta.ReadOnly(TokenProgram.ProgramIdKey, false)
            };
            return new TransactionInstruction
            {
                ProgramId = programIdKey,
                Data = TokenLendingProgramData.EncodeInitializeObligationData(),
                Keys = keys
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="programIdKey">The public key of the program.</param>
        /// <param name="obligation"></param>
        /// <param name="reserves"></param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction RefreshObligation(PublicKey programIdKey, PublicKey obligation,
            IList<PublicKey> reserves)
        {
            List<AccountMeta> keys = new()
            {
                AccountMeta.Writable(obligation, false),
                AccountMeta.ReadOnly(SysVars.ClockKey, false),
            };

            keys.AddRange(reserves.Select(x => AccountMeta.ReadOnly(x, false)));

            return new TransactionInstruction
            {
                ProgramId = programIdKey,
                Data = TokenLendingProgramData.EncodeRefreshObligationData(),
                Keys = keys
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="programIdKey">The public key of the program.</param>
        /// <param name="collateralAmount"></param>
        /// <param name="sourceCollateral"></param>
        /// <param name="destinationCollateral"></param>
        /// <param name="depositReserve"></param>
        /// <param name="obligation"></param>
        /// <param name="lendingMarket"></param>
        /// <param name="obligationOwner"></param>
        /// <param name="userTransferAuthority"></param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction DepositObligationCollateral(PublicKey programIdKey, ulong collateralAmount,
            PublicKey sourceCollateral, PublicKey destinationCollateral, PublicKey depositReserve, PublicKey obligation,
            PublicKey lendingMarket, PublicKey obligationOwner, PublicKey userTransferAuthority)
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
                AccountMeta.ReadOnly(SysVars.ClockKey, false),
                AccountMeta.ReadOnly(TokenProgram.ProgramIdKey, false)
            };


            return new TransactionInstruction
            {
                ProgramId = programIdKey,
                Data = TokenLendingProgramData.EncodeDepositObligationCollateralData(collateralAmount),
                Keys = keys
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="programIdKey">The public key of the program.</param>
        /// <param name="collateralAmount"></param>
        /// <param name="sourceCollateral"></param>
        /// <param name="destinationCollateral"></param>
        /// <param name="withdrawReserve"></param>
        /// <param name="obligation"></param>
        /// <param name="lendingMarket"></param>
        /// <param name="obligationOwner"></param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction WithdrawObligationCollateral(PublicKey programIdKey, ulong collateralAmount,
            PublicKey sourceCollateral, PublicKey destinationCollateral, PublicKey withdrawReserve, PublicKey obligation,
            PublicKey lendingMarket, PublicKey obligationOwner)
        {
            PublicKey lendingMarketAuthority = TokenLendingProgramData.DeriveLendingMarketAuthority(lendingMarket, programIdKey);

            List<AccountMeta> keys = new()
            {
                AccountMeta.Writable(sourceCollateral, false),
                AccountMeta.Writable(destinationCollateral, false),
                AccountMeta.ReadOnly(withdrawReserve, false),
                AccountMeta.Writable(obligation, false),
                AccountMeta.ReadOnly(lendingMarket, false),
                AccountMeta.ReadOnly(lendingMarketAuthority, false),
                AccountMeta.ReadOnly(obligationOwner, true),
                AccountMeta.ReadOnly(SysVars.ClockKey, false),
                AccountMeta.ReadOnly(TokenProgram.ProgramIdKey, false)
            };


            return new TransactionInstruction
            {
                ProgramId = programIdKey,
                Data = TokenLendingProgramData.EncodeWithdrawObligationCollateralData(collateralAmount),
                Keys = keys
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="programIdKey">The public key of the program.</param>
        /// <param name="liquidityAmount"></param>
        /// <param name="sourceLiquidity"></param>
        /// <param name="destinationLiquidity"></param>
        /// <param name="borrowReserve"></param>
        /// <param name="borrowReserveLiquidityFeeReceiver"></param>
        /// <param name="obligation"></param>
        /// <param name="lendingMarket"></param>
        /// <param name="obligationOwner"></param>
        /// <param name="hostFeeReceiver"></param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction BorrowObligationLiduidity(PublicKey programIdKey, ulong liquidityAmount,
            PublicKey sourceLiquidity, PublicKey destinationLiquidity, PublicKey borrowReserve,
            PublicKey borrowReserveLiquidityFeeReceiver, PublicKey obligation, PublicKey lendingMarket,
            PublicKey obligationOwner, PublicKey hostFeeReceiver = null)
        {
            PublicKey lendingMarketAuthority = TokenLendingProgramData.DeriveLendingMarketAuthority(lendingMarket, programIdKey);

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
                AccountMeta.ReadOnly(SysVars.ClockKey, false),
                AccountMeta.ReadOnly(TokenProgram.ProgramIdKey, false)
            };

            if (hostFeeReceiver != null) keys.Add(AccountMeta.Writable(hostFeeReceiver, false));

            return new TransactionInstruction
            {
                ProgramId = programIdKey,
                Data = TokenLendingProgramData.EncodeBorrowObligationLiduidityData(liquidityAmount),
                Keys = keys
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="programIdKey">The public key of the program.</param>
        /// <param name="liquidityAmount"></param>
        /// <param name="sourceLiquidity"></param>
        /// <param name="destinationLiquidity"></param>
        /// <param name="repayReserve"></param>
        /// <param name="obligation"></param>
        /// <param name="lendingMarket"></param>
        /// <param name="userTransferAuthority"></param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction RepayObligationLiduidity(PublicKey programIdKey, ulong liquidityAmount,
            PublicKey sourceLiquidity, PublicKey destinationLiquidity, PublicKey repayReserve, PublicKey obligation,
            PublicKey lendingMarket, PublicKey userTransferAuthority)
        {
            List<AccountMeta> keys = new()
            {
                AccountMeta.Writable(sourceLiquidity, false),
                AccountMeta.Writable(destinationLiquidity, false),
                AccountMeta.Writable(repayReserve, false),
                AccountMeta.Writable(obligation, false),
                AccountMeta.ReadOnly(lendingMarket, false),
                AccountMeta.ReadOnly(userTransferAuthority, true),
                AccountMeta.ReadOnly(SysVars.ClockKey, false),
                AccountMeta.ReadOnly(TokenProgram.ProgramIdKey, false)
            };


            return new TransactionInstruction
            {
                ProgramId = programIdKey,
                Data = TokenLendingProgramData.EncodeRepayObligationLiduidityData(liquidityAmount),
                Keys = keys
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="programIdKey">The public key of the program.</param>
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
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction LiquidateObligation(PublicKey programIdKey, ulong liquidityAmount,
            PublicKey sourceLiquidity, PublicKey destinationCollateral, PublicKey repayReserve,
            PublicKey repayReserveLiquiditySupply, PublicKey withdrawReserve, PublicKey withdrawReserveCollateralSupply,
            PublicKey obligation, PublicKey lendingMarket, PublicKey userTransferAuthority)
        {
            PublicKey lendingMarketAuthority = TokenLendingProgramData.DeriveLendingMarketAuthority(lendingMarket, programIdKey);

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
                AccountMeta.ReadOnly(SysVars.ClockKey, false),
                AccountMeta.ReadOnly(TokenProgram.ProgramIdKey, false)
            };


            return new TransactionInstruction
            {
                ProgramId = programIdKey,
                Data = TokenLendingProgramData.EncodeLiquidateObligationData(liquidityAmount),
                Keys = keys
            };
        }
    }
}
