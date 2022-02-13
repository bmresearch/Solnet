using Solnet.Programs.TokenLending.Models;
using Solnet.Programs.Utilities;
using Solnet.Rpc.Utilities;
using Solnet.Wallet;
using System;
using System.Collections.Generic;

namespace Solnet.Programs.TokenLending
{
    /// <summary>
    /// Implements the token lending program data encodings.
    /// </summary>
    public static class TokenLendingProgramData
    {
        /// <summary>
        /// The offset at which to encode the value that specifies the instruction.
        /// </summary>
        internal const int MethodOffset = 0;

        /// <summary>
        /// Encode the transaction instruction data for the <see cref="TokenLendingProgramInstructions.Values.InitializeLendingMarket"/> method.
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="quoteCurrency"></param>
        /// <returns>The byte array with the encoded data.</returns>
        internal static byte[] EncodeInitializeLendingMarketData(PublicKey owner, byte[] quoteCurrency)
        {
            byte[] buffer = new byte[65];
            buffer.WriteU8((byte)TokenLendingProgramInstructions.Values.InitializeLendingMarket, MethodOffset);
            buffer.WritePubKey(owner, 1);
            buffer.WriteSpan(quoteCurrency, 33);
            return buffer;
        }

        /// <summary>
        /// Decodes the instruction instruction data  for the <see cref="TokenLendingProgramInstructions.Values.InitializeLendingMarket"/> method.
        /// </summary>
        /// <param name="decodedInstruction">The decoded instruction to add data to.</param>
        /// <param name="data">The instruction data to decode.</param>
        /// <param name="keys">The account keys present in the transaction.</param>
        /// <param name="keyIndices">The indices of the account keys for the instruction as they appear in the transaction.</param>
        internal static void DecodeInitializeLendingMarketData(DecodedInstruction decodedInstruction, ReadOnlySpan<byte> data,
            IList<PublicKey> keys, byte[] keyIndices)
        {

        }

        /// <summary>
        /// Encode the transaction instruction data for the <see cref="TokenLendingProgramInstructions.Values.SetLendingMarketOwner"/> method.
        /// </summary>
        /// <param name="newOwner"></param>
        /// <returns>The byte array with the encoded data.</returns>
        internal static byte[] EncodeSetLendingMarketOwnerData(PublicKey newOwner)
        {
            byte[] buffer = new byte[33];
            buffer.WriteU8((byte)TokenLendingProgramInstructions.Values.SetLendingMarketOwner, MethodOffset);
            buffer.WritePubKey(newOwner, 1);
            return buffer;
        }

        /// <summary>
        /// Decodes the instruction instruction data  for the <see cref="TokenLendingProgramInstructions.Values.SetLendingMarketOwner"/> method.
        /// </summary>
        /// <param name="decodedInstruction">The decoded instruction to add data to.</param>
        /// <param name="data">The instruction data to decode.</param>
        /// <param name="keys">The account keys present in the transaction.</param>
        /// <param name="keyIndices">The indices of the account keys for the instruction as they appear in the transaction.</param>
        internal static void DecodeSetLendingMarketOwnerData(DecodedInstruction decodedInstruction, ReadOnlySpan<byte> data,
            IList<PublicKey> keys, byte[] keyIndices)
        {

        }

        /// <summary>
        /// Encode the transaction instruction data for the <see cref="TokenLendingProgramInstructions.Values.InitializeReserve"/> method.
        /// </summary>
        /// <param name="liquidityAmount">The amount of liquidity.</param>
        /// <param name="config">The <see cref="ReserveConfig"/>.</param>
        /// <returns>The byte array with the encoded data.</returns>
        internal static byte[] EncodeInitializeReserveData(ulong liquidityAmount, ReserveConfig config)
        {
            byte[] buffer = new byte[33];
            buffer.WriteU8((byte)TokenLendingProgramInstructions.Values.InitializeReserve, MethodOffset);
            buffer.WriteU64(liquidityAmount, 1);
            config.Serialize(buffer, 9);
            return buffer;
        }

        /// <summary>
        /// Decodes the instruction instruction data  for the <see cref="TokenLendingProgramInstructions.Values.InitializeReserve"/> method.
        /// </summary>
        /// <param name="decodedInstruction">The decoded instruction to add data to.</param>
        /// <param name="data">The instruction data to decode.</param>
        /// <param name="keys">The account keys present in the transaction.</param>
        /// <param name="keyIndices">The indices of the account keys for the instruction as they appear in the transaction.</param>
        internal static void DecodeInitializeReserveData(DecodedInstruction decodedInstruction, ReadOnlySpan<byte> data,
            IList<PublicKey> keys, byte[] keyIndices)
        {

        }

        /// <summary>
        /// Encode the transaction instruction data for the <see cref="TokenLendingProgramInstructions.Values.RefreshReserve"/> method.
        /// </summary>
        /// <returns>The byte array with the encoded data.</returns>
        internal static byte[] EncodeRefreshReserveData()
        {
            byte[] buffer = new byte[1];
            buffer.WriteU8((byte)TokenLendingProgramInstructions.Values.RefreshReserve, MethodOffset);
            return buffer;
        }

        /// <summary>
        /// Decodes the instruction instruction data  for the <see cref="TokenLendingProgramInstructions.Values.RefreshReserve"/> method.
        /// </summary>
        /// <param name="decodedInstruction">The decoded instruction to add data to.</param>
        /// <param name="keys">The account keys present in the transaction.</param>
        /// <param name="keyIndices">The indices of the account keys for the instruction as they appear in the transaction.</param>
        internal static void DecodeRefreshReserveData(DecodedInstruction decodedInstruction, IList<PublicKey> keys,
            byte[] keyIndices)
        {

        }

        /// <summary>
        /// Encode the transaction instruction data for the <see cref="TokenLendingProgramInstructions.Values.DepositReserveLiquidity"/> method.
        /// </summary>
        /// <param name="liquidityAmount">The amount of liquidity.</param>
        /// <returns>The byte array with the encoded data.</returns>
        internal static byte[] EncodeDepositReserveLiquidityData(ulong liquidityAmount)
        {
            byte[] buffer = new byte[9];
            buffer.WriteU8((byte)TokenLendingProgramInstructions.Values.DepositReserveLiquidity, MethodOffset);
            buffer.WriteU64(liquidityAmount, 1);
            return buffer;
        }

        /// <summary>
        /// Decodes the instruction instruction data  for the <see cref="TokenLendingProgramInstructions.Values.DepositReserveLiquidity"/> method.
        /// </summary>
        /// <param name="decodedInstruction">The decoded instruction to add data to.</param>
        /// <param name="data">The instruction data to decode.</param>
        /// <param name="keys">The account keys present in the transaction.</param>
        /// <param name="keyIndices">The indices of the account keys for the instruction as they appear in the transaction.</param>
        internal static void DecodeDepositReserveLiquidityData(DecodedInstruction decodedInstruction, ReadOnlySpan<byte> data,
            IList<PublicKey> keys, byte[] keyIndices)
        {

        }

        /// <summary>
        /// Encode the transaction instruction data for the <see cref="TokenLendingProgramInstructions.Values.RedeemReserveCollateral"/> method.
        /// </summary>
        /// <param name="collateralAmount">The amount of collateral.</param>
        /// <returns>The byte array with the encoded data.</returns>
        internal static byte[] EncodeRedeemReserveCollateralData(ulong collateralAmount)
        {
            byte[] buffer = new byte[9];
            buffer.WriteU8((byte)TokenLendingProgramInstructions.Values.RedeemReserveCollateral, MethodOffset);
            buffer.WriteU64(collateralAmount, 1);
            return buffer;
        }

        /// <summary>
        /// Decodes the instruction instruction data  for the <see cref="TokenLendingProgramInstructions.Values.RedeemReserveCollateral"/> method.
        /// </summary>
        /// <param name="decodedInstruction">The decoded instruction to add data to.</param>
        /// <param name="data">The instruction data to decode.</param>
        /// <param name="keys">The account keys present in the transaction.</param>
        /// <param name="keyIndices">The indices of the account keys for the instruction as they appear in the transaction.</param>
        internal static void DecodeRedeemReserveCollateralData(DecodedInstruction decodedInstruction, ReadOnlySpan<byte> data,
            IList<PublicKey> keys, byte[] keyIndices)
        {

        }

        /// <summary>
        /// Encode the transaction instruction data for the <see cref="TokenLendingProgramInstructions.Values.InitializeObligation"/> method.
        /// </summary>
        /// <returns>The byte array with the encoded data.</returns>
        internal static byte[] EncodeInitializeObligationData()
        {
            byte[] buffer = new byte[1];
            buffer.WriteU8((byte)TokenLendingProgramInstructions.Values.InitializeObligation, MethodOffset);
            return buffer;
        }

        /// <summary>
        /// Decodes the instruction instruction data  for the <see cref="TokenLendingProgramInstructions.Values.InitializeObligation"/> method.
        /// </summary>
        /// <param name="decodedInstruction">The decoded instruction to add data to.</param>
        /// <param name="keys">The account keys present in the transaction.</param>
        /// <param name="keyIndices">The indices of the account keys for the instruction as they appear in the transaction.</param>
        internal static void DecodeInitializeObligationData(DecodedInstruction decodedInstruction, IList<PublicKey> keys,
            byte[] keyIndices)
        {

        }

        /// <summary>
        /// Encode the transaction instruction data for the <see cref="TokenLendingProgramInstructions.Values.RefreshObligation"/> method.
        /// </summary>
        /// <returns>The byte array with the encoded data.</returns>
        internal static byte[] EncodeRefreshObligationData()
        {
            byte[] buffer = new byte[1];
            buffer.WriteU8((byte)TokenLendingProgramInstructions.Values.RefreshObligation, MethodOffset);
            return buffer;
        }

        /// <summary>
        /// Decodes the instruction instruction data  for the <see cref="TokenLendingProgramInstructions.Values.RefreshObligation"/> method.
        /// </summary>
        /// <param name="decodedInstruction">The decoded instruction to add data to.</param>
        /// <param name="keys">The account keys present in the transaction.</param>
        /// <param name="keyIndices">The indices of the account keys for the instruction as they appear in the transaction.</param>
        internal static void DecodeRefreshObligationData(DecodedInstruction decodedInstruction, IList<PublicKey> keys,
            byte[] keyIndices)
        {

        }

        /// <summary>
        /// Encode the transaction instruction data for the <see cref="TokenLendingProgramInstructions.Values.DepositObligationCollateral"/> method.
        /// </summary>
        /// <param name="collateralAmount">The amount of collateral.</param>
        /// <returns>The byte array with the encoded data.</returns>
        internal static byte[] EncodeDepositObligationCollateralData(ulong collateralAmount)
        {
            byte[] buffer = new byte[9];
            buffer.WriteU8((byte)TokenLendingProgramInstructions.Values.DepositObligationCollateral, MethodOffset);
            buffer.WriteU64(collateralAmount, 1);
            return buffer;
        }

        /// <summary>
        /// Decodes the instruction instruction data  for the <see cref="TokenLendingProgramInstructions.Values.DepositObligationCollateral"/> method.
        /// </summary>
        /// <param name="decodedInstruction">The decoded instruction to add data to.</param>
        /// <param name="data">The instruction data to decode.</param>
        /// <param name="keys">The account keys present in the transaction.</param>
        /// <param name="keyIndices">The indices of the account keys for the instruction as they appear in the transaction.</param>
        internal static void DecodeDepositObligationCollateralData(DecodedInstruction decodedInstruction, ReadOnlySpan<byte> data,
            IList<PublicKey> keys, byte[] keyIndices)
        {

        }

        /// <summary>
        /// Encode the transaction instruction data for the <see cref="TokenLendingProgramInstructions.Values.WithdrawObligationCollateral"/> method.
        /// </summary>
        /// <param name="collateralAmount">The amount of collateral.</param>
        /// <returns>The byte array with the encoded data.</returns>
        internal static byte[] EncodeWithdrawObligationCollateralData(ulong collateralAmount)
        {
            byte[] buffer = new byte[9];
            buffer.WriteU8((byte)TokenLendingProgramInstructions.Values.WithdrawObligationCollateral, MethodOffset);
            buffer.WriteU64(collateralAmount, 1);
            return buffer;
        }

        /// <summary>
        /// Decodes the instruction instruction data  for the <see cref="TokenLendingProgramInstructions.Values.WithdrawObligationCollateral"/> method.
        /// </summary>
        /// <param name="decodedInstruction">The decoded instruction to add data to.</param>
        /// <param name="data">The instruction data to decode.</param>
        /// <param name="keys">The account keys present in the transaction.</param>
        /// <param name="keyIndices">The indices of the account keys for the instruction as they appear in the transaction.</param>
        internal static void DecodeWithdrawObligationCollateralData(DecodedInstruction decodedInstruction, ReadOnlySpan<byte> data,
            IList<PublicKey> keys, byte[] keyIndices)
        {

        }

        /// <summary>
        /// Encode the transaction instruction data for the <see cref="TokenLendingProgramInstructions.Values.BorrowObligationLiquidity"/> method.
        /// </summary>
        /// <param name="liquidityAmount">The amount of liquidity.</param>
        /// <returns>The byte array with the encoded data.</returns>
        internal static byte[] EncodeBorrowObligationLiduidityData(ulong liquidityAmount)
        {
            byte[] buffer = new byte[9];
            buffer.WriteU8((byte)TokenLendingProgramInstructions.Values.BorrowObligationLiquidity, MethodOffset);
            buffer.WriteU64(liquidityAmount, 1);
            return buffer;
        }

        /// <summary>
        /// Decodes the instruction instruction data  for the <see cref="TokenLendingProgramInstructions.Values.BorrowObligationLiquidity"/> method.
        /// </summary>
        /// <param name="decodedInstruction">The decoded instruction to add data to.</param>
        /// <param name="data">The instruction data to decode.</param>
        /// <param name="keys">The account keys present in the transaction.</param>
        /// <param name="keyIndices">The indices of the account keys for the instruction as they appear in the transaction.</param>
        internal static void DecodeBorrowObligationLiduidityData(DecodedInstruction decodedInstruction, ReadOnlySpan<byte> data,
            IList<PublicKey> keys, byte[] keyIndices)
        {

        }

        /// <summary>
        /// Encode the transaction instruction data for the <see cref="TokenLendingProgramInstructions.Values.RepayObligationLiquidity"/> method.
        /// </summary>
        /// <param name="liquidityAmount">The amount of liquidity.</param>
        /// <returns>The byte array with the encoded data.</returns>
        internal static byte[] EncodeRepayObligationLiduidityData(ulong liquidityAmount)
        {
            byte[] buffer = new byte[9];
            buffer.WriteU8((byte)TokenLendingProgramInstructions.Values.RepayObligationLiquidity, MethodOffset);
            buffer.WriteU64(liquidityAmount, 1);
            return buffer;
        }

        /// <summary>
        /// Decodes the instruction instruction data  for the <see cref="TokenLendingProgramInstructions.Values.RepayObligationLiquidity"/> method.
        /// </summary>
        /// <param name="decodedInstruction">The decoded instruction to add data to.</param>
        /// <param name="data">The instruction data to decode.</param>
        /// <param name="keys">The account keys present in the transaction.</param>
        /// <param name="keyIndices">The indices of the account keys for the instruction as they appear in the transaction.</param>
        internal static void DecodeRepayObligationLiduidityData(DecodedInstruction decodedInstruction, ReadOnlySpan<byte> data,
            IList<PublicKey> keys, byte[] keyIndices)
        {

        }

        /// <summary>
        /// Encode the transaction instruction data for the <see cref="TokenLendingProgramInstructions.Values.LiquidateObligation"/> method.
        /// </summary>
        /// <param name="liquidityAmount">The amount of liquidity.</param>
        /// <returns>The byte array with the encoded data.</returns>
        internal static byte[] EncodeLiquidateObligationData(ulong liquidityAmount)
        {
            byte[] buffer = new byte[9];
            buffer.WriteU8((byte)TokenLendingProgramInstructions.Values.LiquidateObligation, MethodOffset);
            buffer.WriteU64(liquidityAmount, 1);
            return buffer;
        }

        /// <summary>
        /// Decodes the instruction instruction data  for the <see cref="TokenLendingProgramInstructions.Values.LiquidateObligation"/> method.
        /// </summary>
        /// <param name="decodedInstruction">The decoded instruction to add data to.</param>
        /// <param name="data">The instruction data to decode.</param>
        /// <param name="keys">The account keys present in the transaction.</param>
        /// <param name="keyIndices">The indices of the account keys for the instruction as they appear in the transaction.</param>
        internal static void DecodeLiquidateObligationData(DecodedInstruction decodedInstruction, ReadOnlySpan<byte> data,
            IList<PublicKey> keys, byte[] keyIndices)
        {

        }

        /// <summary>
        /// Encode the transaction instruction data for the <see cref="TokenLendingProgramInstructions.Values.FlashLoan"/> method.
        /// </summary>
        /// <param name="liquidityAmount">The amount of liquidity.</param>
        /// <returns>The byte array with the encoded data.</returns>
        internal static byte[] EncodeFlashLoanData(ulong liquidityAmount)
        {
            byte[] buffer = new byte[9];
            buffer.WriteU8((byte)TokenLendingProgramInstructions.Values.FlashLoan, MethodOffset);
            buffer.WriteU64(liquidityAmount, 1);
            return buffer;
        }

        /// <summary>
        /// Decodes the instruction instruction data  for the <see cref="TokenLendingProgramInstructions.Values.FlashLoan"/> method.
        /// </summary>
        /// <param name="decodedInstruction">The decoded instruction to add data to.</param>
        /// <param name="data">The instruction data to decode.</param>
        /// <param name="keys">The account keys present in the transaction.</param>
        /// <param name="keyIndices">The indices of the account keys for the instruction as they appear in the transaction.</param>
        internal static void DecodeFlashLoanData(DecodedInstruction decodedInstruction, ReadOnlySpan<byte> data,
            IList<PublicKey> keys, byte[] keyIndices)
        {

        }

        /// <summary>
        /// Derive the vault signer address for the given market.
        /// </summary>
        /// <param name="lendingMarket">The lending market public key.</param>
        /// <param name="programId">The program id.</param>
        /// <returns>The vault signer address.</returns>
        /// <exception cref="Exception">Throws exception when unable to derive the vault signer address.</exception>
        public static PublicKey DeriveLendingMarketAuthority(PublicKey lendingMarket, PublicKey programId)
        {
            List<byte[]> seeds = new() { lendingMarket.KeyBytes };

            bool success = AddressExtensions.TryFindProgramAddress(seeds, programId.KeyBytes,
                out byte[] lendingMarketAuthority, out _);

            return !success ? null : new(lendingMarketAuthority);
        }
    }
}
