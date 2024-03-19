using Solnet.Programs.Utilities;
using System;

namespace Solnet.Programs.ComputeBudget
{
    /// <summary>
    /// Implements the compute budget program data encodings.
    /// </summary>
    internal static class ComputeBudgetProgramData
    {
        /// <summary>
        /// The offset at which the value which defines the program method begins. 
        /// </summary>
        internal const int MethodOffset = 0;

        /// <summary>
        /// Encode transaction instruction data for the <see cref="ComputeBudgetProgramInstructions.Values.RequestHeapFrame"/> method.
        /// </summary>
        /// <param name="bytes">The heap region size.</param>
        /// <returns>The transaction instruction data.</returns>
        internal static byte[] EncodeRequestHeapFrameData(uint bytes)
        {
            byte[] data = new byte[5];
            
            data.WriteU8((byte)ComputeBudgetProgramInstructions.Values.RequestHeapFrame, MethodOffset);
            data.WriteU32(bytes, 1);
            
            return data;
        }

        /// <summary>
        /// Encode transaction instruction data for the <see cref="ComputeBudgetProgramInstructions.Values.SetComputeUnitLimit"/> method.
        /// </summary>
        /// <param name="units">The compute unit limit.</param>
        /// <returns>The transaction instruction data.</returns>
        internal static byte[] EncodeSetComputeUnitLimitData(uint units)
        {
            byte[] data = new byte[5];
            
            data.WriteU8((byte)ComputeBudgetProgramInstructions.Values.SetComputeUnitLimit, MethodOffset);
            data.WriteU32(units, 1);
            
            return data;
        }

        /// <summary>
        /// Encode transaction instruction data for the <see cref="ComputeBudgetProgramInstructions.Values.SetComputeUnitPrice"/> method.
        /// </summary>
        /// <param name="microLamports">The compute unit price.</param>
        /// <returns>The transaction instruction data.</returns>
        internal static byte[] EncodeSetComputeUnitPriceData(ulong microLamports)
        {
            byte[] data = new byte[9];

            data.WriteU8((byte)ComputeBudgetProgramInstructions.Values.SetComputeUnitPrice, MethodOffset);
            data.WriteU64(microLamports, 1);

            return data;
        }
        
        /// <summary>
        /// Encode transaction instruction data for the <see cref="ComputeBudgetProgramInstructions.Values.SetLoadedAccountsDataSizeLimit"/> method.
        /// </summary>
        /// <param name="bytes">The account data size limit.</param>
        /// <returns>The transaction instruction data.</returns>
        internal static byte[] EncodeSetLoadedAccountsDataSizeLimit(uint bytes)
        {
            byte[] data = new byte[5];

            data.WriteU8((byte)ComputeBudgetProgramInstructions.Values.SetLoadedAccountsDataSizeLimit, MethodOffset);
            data.WriteU32(bytes, 1);

            return data;
        }

        /// <summary>
        /// Decodes the instruction instruction data for the <see cref="ComputeBudgetProgramInstructions.Values.RequestHeapFrame"/> method
        /// </summary>
        /// <param name="decodedInstruction">The decoded instruction to add data to.</param>
        /// <param name="data">The instruction data to decode.</param>
        internal static void DecodeRequestHeapFrameData(DecodedInstruction decodedInstruction, ReadOnlySpan<byte> data)
        {
            decodedInstruction.Values.Add("Bytes", data.GetU32(1));
        }
        
        /// <summary>
        /// Decodes the instruction instruction data for the <see cref="ComputeBudgetProgramInstructions.Values.SetComputeUnitLimit"/> method
        /// </summary>
        /// <param name="decodedInstruction">The decoded instruction to add data to.</param>
        /// <param name="data">The instruction data to decode.</param>
        internal static void DecodeSetComputeUnitLimitData(DecodedInstruction decodedInstruction, ReadOnlySpan<byte> data)
        {
            decodedInstruction.Values.Add("Units", data.GetU32(1));
        }
        
        /// <summary>
        /// Decodes the instruction instruction data for the <see cref="ComputeBudgetProgramInstructions.Values.SetComputeUnitPrice"/> method
        /// </summary>
        /// <param name="decodedInstruction">The decoded instruction to add data to.</param>
        /// <param name="data">The instruction data to decode.</param>
        internal static void DecodeSetComputeUnitPriceData(DecodedInstruction decodedInstruction, ReadOnlySpan<byte> data)
        {
            decodedInstruction.Values.Add("Micro Lamports", data.GetU64(1));
        }
        
        /// <summary>
        /// Decodes the instruction instruction data for the <see cref="ComputeBudgetProgramInstructions.Values.SetLoadedAccountsDataSizeLimit"/> method
        /// </summary>
        /// <param name="decodedInstruction">The decoded instruction to add data to.</param>
        /// <param name="data">The instruction data to decode.</param>
        internal static void DecodeSetLoadedAccountsDataSizeLimitData(DecodedInstruction decodedInstruction, ReadOnlySpan<byte> data)
        {
            decodedInstruction.Values.Add("Bytes", data.GetU32(1));
        }
    }
}