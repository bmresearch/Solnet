using Solnet.Programs.TokenSwap.Models;
using Solnet.Programs.Utilities;
using Solnet.Wallet;
using System;
using System.Collections.Generic;

namespace Solnet.Programs.TokenSwap
{
    /// <summary>
    /// Implements the token swap program data encodings.
    /// </summary>
    internal static class TokenSwapProgramData
    {
        /// <summary>
        /// Encode the transaction instruction data for the <see cref="TokenSwapProgramInstructions.Values.Initialize"/> method.
        /// </summary>
        /// <param name="nonce">nonce used to create valid program address.</param>
        /// <param name="fees">all swap fees.</param>
        /// <param name="swapCurve">swap curve info for pool, including CurveType and anything else that may be required.</param>
        /// <remarks>The <c>freezeAuthorityOption</c> parameter is related to the existence or not of a freeze authority.</remarks>
        /// <returns>The byte array with the encoded data.</returns>
        internal static byte[] EncodeInitializeData(byte nonce, Fees fees, SwapCurve swapCurve)
        {
            byte[] methodBuffer = new byte[99];

            methodBuffer.WriteU8((byte)TokenSwapProgramInstructions.Values.Initialize, 0);
            methodBuffer.WriteU8(nonce, 1);
            methodBuffer.WriteSpan(fees.Serialize(), 2);
            methodBuffer.WriteSpan(swapCurve.Serialize(), 66);

            return methodBuffer;
        }

        /// <summary>
        /// Encode the transaction instruction data for the <see cref="TokenSwapProgramInstructions.Values.Swap"/> method.
        /// </summary>
        /// <param name="amountIn">The amount of tokens in.</param>
        /// <param name="amountOut">The amount of tokens out.</param>
        /// <returns>The byte array with the encoded data.</returns>
        internal static byte[] EncodeSwapData(ulong amountIn, ulong amountOut)
        {
            byte[] methodBuffer = new byte[17];

            methodBuffer.WriteU8((byte)TokenSwapProgramInstructions.Values.Swap, 0);
            methodBuffer.WriteU64(amountIn, 1);
            methodBuffer.WriteU64(amountOut, 9);

            return methodBuffer;
        }

        /// <summary>
        /// Encode the transaction instruction data for the <see cref="TokenSwapProgramInstructions.Values.DepositAllTokenTypes"/> method.
        /// </summary>
        /// <param name="poolTokenAmount">The amount of tokens out.</param>
        /// <param name="maxTokenAAmount">The max amount of tokens A.</param>
        /// <param name="maxTokenBAmount">The max amount of tokens B.</param>
        /// <returns>The byte array with the encoded data.</returns>
        internal static byte[] EncodeDepositAllTokenTypesData(ulong poolTokenAmount, ulong maxTokenAAmount, ulong maxTokenBAmount)
        {
            byte[] methodBuffer = new byte[25];

            methodBuffer.WriteU8((byte)TokenSwapProgramInstructions.Values.DepositAllTokenTypes, 0);
            methodBuffer.WriteU64(poolTokenAmount, 1);
            methodBuffer.WriteU64(maxTokenAAmount, 9);
            methodBuffer.WriteU64(maxTokenBAmount, 17);

            return methodBuffer;
        }

        /// <summary>
        /// Encode the transaction instruction data for the <see cref="TokenSwapProgramInstructions.Values.WithdrawAllTokenTypes"/> method.
        /// </summary>
        /// <param name="poolTokenAmount">The amount of tokens in.</param>
        /// <param name="minTokenAAmount">The maminx amount of tokens A.</param>
        /// <param name="minTokenBAmount">The min amount of tokens B.</param>
        /// <returns>The byte array with the encoded data.</returns>
        internal static byte[] EncodeWithdrawAllTokenTypesData(ulong poolTokenAmount, ulong minTokenAAmount, ulong minTokenBAmount)
        {
            byte[] methodBuffer = new byte[25];

            methodBuffer.WriteU8((byte)TokenSwapProgramInstructions.Values.WithdrawAllTokenTypes, 0);
            methodBuffer.WriteU64(poolTokenAmount, 1);
            methodBuffer.WriteU64(minTokenAAmount, 9);
            methodBuffer.WriteU64(minTokenBAmount, 17);

            return methodBuffer;
        }

        /// <summary>
        /// Encode the transaction instruction data for the <see cref="TokenSwapProgramInstructions.Values.DepositSingleTokenTypeExactAmountIn"/> method.
        /// </summary>
        /// <param name="sourceTokenAmount">The amount of tokens in.</param>
        /// <param name="minPoolTokenAmount">The min amount of pool tokens out.</param>
        /// <returns>The byte array with the encoded data.</returns>
        internal static byte[] EncodeDepositSingleTokenTypeExactAmountInData(ulong sourceTokenAmount, ulong minPoolTokenAmount)
        {
            byte[] methodBuffer = new byte[17];

            methodBuffer.WriteU8((byte)TokenSwapProgramInstructions.Values.DepositSingleTokenTypeExactAmountIn, 0);
            methodBuffer.WriteU64(sourceTokenAmount, 1);
            methodBuffer.WriteU64(minPoolTokenAmount, 9);

            return methodBuffer;
        }

        /// <summary>
        /// Encode the transaction instruction data for the <see cref="TokenSwapProgramInstructions.Values.WithdrawSingleTokenTypeExactAmountOut"/> method.
        /// </summary>
        /// <param name="destTokenAmount">The amount of tokens out.</param>
        /// <param name="maxPoolTokenAmount">The max amount of pool tokens in.</param>
        /// <returns>The byte array with the encoded data.</returns>
        internal static byte[] EncodeWithdrawSingleTokenTypeExactAmountOutData(ulong destTokenAmount, ulong maxPoolTokenAmount)
        {
            byte[] methodBuffer = new byte[17];

            methodBuffer.WriteU8((byte)TokenSwapProgramInstructions.Values.WithdrawSingleTokenTypeExactAmountOut, 0);
            methodBuffer.WriteU64(destTokenAmount, 1);
            methodBuffer.WriteU64(maxPoolTokenAmount, 9);

            return methodBuffer;
        }

        /// <summary>
        /// Decodes the instruction instruction data  for the <see cref="TokenSwapProgramInstructions.Values.Initialize"/> method
        /// </summary>
        /// <param name="decodedInstruction">The decoded instruction to add data to.</param>
        /// <param name="data">The instruction data to decode.</param>
        /// <param name="keys">The account keys present in the transaction.</param>
        /// <param name="keyIndices">The indices of the account keys for the instruction as they appear in the transaction.</param>
        internal static void DecodeInitializeData(DecodedInstruction decodedInstruction, ReadOnlySpan<byte> data,
            IList<PublicKey> keys, byte[] keyIndices)
        {
            decodedInstruction.Values.Add("Token Swap Account", keys[keyIndices[0]]);
            decodedInstruction.Values.Add("Swap Authority", keys[keyIndices[1]]);
            decodedInstruction.Values.Add("Token A Account", keys[keyIndices[2]]);
            decodedInstruction.Values.Add("Token B Account", keys[keyIndices[3]]);
            decodedInstruction.Values.Add("Pool Token Mint", keys[keyIndices[4]]);
            decodedInstruction.Values.Add("Pool Token Fee Account", keys[keyIndices[5]]);
            decodedInstruction.Values.Add("Pool Token Account", keys[keyIndices[6]]);
            decodedInstruction.Values.Add("Token Program ID", keys[keyIndices[7]]);

            decodedInstruction.Values.Add("Nonce", data.GetU8(1));
            decodedInstruction.Values.Add("Trade Fee Numerator", data.GetU64(2));
            decodedInstruction.Values.Add("Trade Fee Denominator", data.GetU64(10));
            decodedInstruction.Values.Add("Owner Trade Fee Numerator", data.GetU64(18));
            decodedInstruction.Values.Add("Owner Trade Fee Denominator", data.GetU64(26));
            decodedInstruction.Values.Add("Owner Withraw Fee Numerator", data.GetU64(34));
            decodedInstruction.Values.Add("Owner Withraw Fee Denominator", data.GetU64(42));
            decodedInstruction.Values.Add("Host Fee Numerator", data.GetU64(50));
            decodedInstruction.Values.Add("Host Fee Denominator", data.GetU64(58));
            decodedInstruction.Values.Add("Curve Type", data.GetU64(59));
            //nothing to show for calculator unless hardcoding the switch stmt
        }

        /// <summary>
        /// Decodes the instruction instruction data  for the <see cref="TokenSwapProgramInstructions.Values.Swap"/> method
        /// </summary>
        /// <param name="decodedInstruction">The decoded instruction to add data to.</param>
        /// <param name="data">The instruction data to decode.</param>
        /// <param name="keys">The account keys present in the transaction.</param>
        /// <param name="keyIndices">The indices of the account keys for the instruction as they appear in the transaction.</param>
        internal static void DecodeSwapData(DecodedInstruction decodedInstruction, ReadOnlySpan<byte> data,
            IList<PublicKey> keys, byte[] keyIndices)
        {
            decodedInstruction.Values.Add("Token Swap Account", keys[keyIndices[0]]);
            decodedInstruction.Values.Add("Swap Authority", keys[keyIndices[1]]);
            decodedInstruction.Values.Add("User Transfer Authority", keys[keyIndices[2]]);
            decodedInstruction.Values.Add("User Source Account", keys[keyIndices[3]]);
            decodedInstruction.Values.Add("Token Base Into Account", keys[keyIndices[4]]);
            decodedInstruction.Values.Add("Token Base From Account", keys[keyIndices[5]]);
            decodedInstruction.Values.Add("User Destination Account", keys[keyIndices[6]]);
            decodedInstruction.Values.Add("Pool Token Mint", keys[keyIndices[7]]);
            decodedInstruction.Values.Add("Fee Account", keys[keyIndices[8]]);
            decodedInstruction.Values.Add("Token Program ID", keys[keyIndices[9]]);
            if (keyIndices.Length >= 11)
            {
                decodedInstruction.Values.Add("Host Fee Account", keys[keyIndices[10]]);
            }

            decodedInstruction.Values.Add("Amount In", data.GetU64(1));
            decodedInstruction.Values.Add("Amount Out", data.GetU64(9));
        }

        /// <summary>
        /// Decodes the instruction instruction data  for the <see cref="TokenSwapProgramInstructions.Values.DepositAllTokenTypes"/> method
        /// </summary>
        /// <param name="decodedInstruction">The decoded instruction to add data to.</param>
        /// <param name="data">The instruction data to decode.</param>
        /// <param name="keys">The account keys present in the transaction.</param>
        /// <param name="keyIndices">The indices of the account keys for the instruction as they appear in the transaction.</param>
        internal static void DecodeDepositAllTokenTypesData(DecodedInstruction decodedInstruction, ReadOnlySpan<byte> data,
            IList<PublicKey> keys, byte[] keyIndices)
        {
            decodedInstruction.Values.Add("Token Swap Account", keys[keyIndices[0]]);
            decodedInstruction.Values.Add("Swap Authority", keys[keyIndices[1]]);
            decodedInstruction.Values.Add("User Transfer Authority", keys[keyIndices[2]]);
            decodedInstruction.Values.Add("User Token A Account", keys[keyIndices[3]]);
            decodedInstruction.Values.Add("User Token B Account", keys[keyIndices[4]]);
            decodedInstruction.Values.Add("Pool Token A Account", keys[keyIndices[5]]);
            decodedInstruction.Values.Add("Pool Token B Account", keys[keyIndices[6]]);
            decodedInstruction.Values.Add("Pool Token Mint", keys[keyIndices[7]]);
            decodedInstruction.Values.Add("User Pool Token Account", keys[keyIndices[8]]);
            decodedInstruction.Values.Add("Token Program ID", keys[keyIndices[9]]);

            decodedInstruction.Values.Add("Pool Tokens", data.GetU64(1));
            decodedInstruction.Values.Add("Max Token A", data.GetU64(9));
            decodedInstruction.Values.Add("Max Token B", data.GetU64(17));
        }

        /// <summary>
        /// Decodes the instruction instruction data  for the <see cref="TokenSwapProgramInstructions.Values.WithdrawAllTokenTypes"/> method
        /// </summary>
        /// <param name="decodedInstruction">The decoded instruction to add data to.</param>
        /// <param name="data">The instruction data to decode.</param>
        /// <param name="keys">The account keys present in the transaction.</param>
        /// <param name="keyIndices">The indices of the account keys for the instruction as they appear in the transaction.</param>
        internal static void DecodeWithdrawAllTokenTypesData(DecodedInstruction decodedInstruction, ReadOnlySpan<byte> data,
            IList<PublicKey> keys, byte[] keyIndices)
        {
            decodedInstruction.Values.Add("Token Swap Account", keys[keyIndices[0]]);
            decodedInstruction.Values.Add("Swap Authority", keys[keyIndices[1]]);
            decodedInstruction.Values.Add("User Transfer Authority", keys[keyIndices[2]]);
            decodedInstruction.Values.Add("Pool Token Account", keys[keyIndices[3]]);
            decodedInstruction.Values.Add("User Pool Token Account", keys[keyIndices[4]]);
            decodedInstruction.Values.Add("Pool Token A Account", keys[keyIndices[5]]);
            decodedInstruction.Values.Add("Pool Token B Account", keys[keyIndices[6]]);
            decodedInstruction.Values.Add("User Token A Account", keys[keyIndices[7]]);
            decodedInstruction.Values.Add("User Token B Account", keys[keyIndices[8]]);
            decodedInstruction.Values.Add("Fee Account", keys[keyIndices[9]]);
            decodedInstruction.Values.Add("Token Program ID", keys[keyIndices[10]]);

            decodedInstruction.Values.Add("Pool Tokens", data.GetU64(1));
            decodedInstruction.Values.Add("Min Token A", data.GetU64(9));
            decodedInstruction.Values.Add("Min Token B", data.GetU64(17));
        }

        /// <summary>
        /// Decodes the instruction instruction data  for the <see cref="TokenSwapProgramInstructions.Values.DepositSingleTokenTypeExactAmountIn"/> method
        /// </summary>
        /// <param name="decodedInstruction">The decoded instruction to add data to.</param>
        /// <param name="data">The instruction data to decode.</param>
        /// <param name="keys">The account keys present in the transaction.</param>
        /// <param name="keyIndices">The indices of the account keys for the instruction as they appear in the transaction.</param>
        internal static void DecodeDepositSingleTokenTypeExactAmountInData(DecodedInstruction decodedInstruction, ReadOnlySpan<byte> data,
            IList<PublicKey> keys, byte[] keyIndices)
        {
            decodedInstruction.Values.Add("Token Swap Account", keys[keyIndices[0]]);
            decodedInstruction.Values.Add("Swap Authority", keys[keyIndices[1]]);
            decodedInstruction.Values.Add("User Transfer Authority", keys[keyIndices[2]]);
            decodedInstruction.Values.Add("User Source Token Account", keys[keyIndices[3]]);
            decodedInstruction.Values.Add("Token A Swap Account", keys[keyIndices[4]]);
            decodedInstruction.Values.Add("Token B Swap Account", keys[keyIndices[5]]);
            decodedInstruction.Values.Add("Pool Mint Account", keys[keyIndices[6]]);
            decodedInstruction.Values.Add("User Pool Token Account", keys[keyIndices[7]]);
            decodedInstruction.Values.Add("Token Program ID", keys[keyIndices[8]]);

            decodedInstruction.Values.Add("Source Token Amount", data.GetU64(1));
            decodedInstruction.Values.Add("Min Pool Token Amount", data.GetU64(9));
        }

        /// <summary>
        /// Decodes the instruction instruction data  for the <see cref="TokenSwapProgramInstructions.Values.WithdrawSingleTokenTypeExactAmountOut"/> method
        /// </summary>
        /// <param name="decodedInstruction">The decoded instruction to add data to.</param>
        /// <param name="data">The instruction data to decode.</param>
        /// <param name="keys">The account keys present in the transaction.</param>
        /// <param name="keyIndices">The indices of the account keys for the instruction as they appear in the transaction.</param>
        internal static void DecodeWithdrawSingleTokenTypeExactAmountOutData(DecodedInstruction decodedInstruction, ReadOnlySpan<byte> data,
            IList<PublicKey> keys, byte[] keyIndices)
        {
            decodedInstruction.Values.Add("Token Swap Account", keys[keyIndices[0]]);
            decodedInstruction.Values.Add("Swap Authority", keys[keyIndices[1]]);
            decodedInstruction.Values.Add("User Transfer Authority", keys[keyIndices[2]]);
            decodedInstruction.Values.Add("Pool Mint Account", keys[keyIndices[3]]);
            decodedInstruction.Values.Add("User Pool Token Account", keys[keyIndices[4]]);
            decodedInstruction.Values.Add("Token A Swap Account", keys[keyIndices[5]]);
            decodedInstruction.Values.Add("Token B Swap Account", keys[keyIndices[6]]);
            decodedInstruction.Values.Add("User Token Account", keys[keyIndices[7]]);
            decodedInstruction.Values.Add("Fee Account", keys[keyIndices[8]]);
            decodedInstruction.Values.Add("Token Program ID", keys[keyIndices[9]]);

            decodedInstruction.Values.Add("Destination Token Amount", data.GetU64(1));
            decodedInstruction.Values.Add("Max Pool Token Amount", data.GetU64(9));
        }
    }
}