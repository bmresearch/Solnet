using Solnet.Programs.Utilities;
using Solnet.Rpc.Models;
using Solnet.Wallet;
using System;
using System.Collections.Generic;

namespace Solnet.Programs.ComputeBudget
{
    /// <summary>
    /// Implements the Compute Budget Program methods.
    /// <remarks>
    /// For more information see:
    /// https://docs.rs/solana-sdk/1.18.7/solana_sdk/compute_budget/enum.ComputeBudgetInstruction.html
    /// </remarks>
    /// </summary>
    public static class ComputeBudgetProgram
    {
        /// <summary>
        /// The public key of the Compute Budget Program.
        /// </summary>
        public static readonly PublicKey ProgramIdKey = new("ComputeBudget111111111111111111111111111111");

        /// <summary>
        /// The program's name.
        /// </summary>
        private const string ProgramName = "Compute Budget Program";

        /// <summary>
        /// Request a specific transaction-wide program heap region size in bytes. The value requested must be a multiple of 1024. This new heap region size applies to each program executed in the transaction, including all calls to CPIs.
        /// </summary>
        /// <param name="bytes">The heap region size.</param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction RequestHeapFrame(uint bytes)
        {
            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey.KeyBytes,
                Keys = new List<AccountMeta>(),
                Data = ComputeBudgetProgramData.EncodeRequestHeapFrameData(bytes)
            };
        }

        /// <summary>
        /// Set a specific compute unit limit that the transaction is allowed to consume.
        /// </summary>
        /// <param name="units">The compute unit limit.</param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction SetComputeUnitLimit(uint units)
        {
            return new TransactionInstruction()
            {
                ProgramId = ProgramIdKey.KeyBytes,
                Keys = new List<AccountMeta>(),
                Data = ComputeBudgetProgramData.EncodeSetComputeUnitLimitData(units)
            };
        }

        /// <summary>
        /// Set a compute unit price in “micro-lamports” to pay a higher transaction fee for higher transaction prioritization.
        /// </summary>
        /// <param name="microLamports">The compute unit price.</param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction SetComputeUnitPrice(ulong microLamports)
        {
            return new TransactionInstruction()
            {
                ProgramId = ProgramIdKey.KeyBytes,
                Keys = new List<AccountMeta>(),
                Data = ComputeBudgetProgramData.EncodeSetComputeUnitPriceData(microLamports)
            };
        }
        
        /// <summary>
        /// Set a specific transaction-wide account data size limit, in bytes, is allowed to load.
        /// </summary>
        /// <param name="bytes">The account data size limit.</param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction SetLoadedAccountsDataSizeLimit(uint bytes)
        {
            return new TransactionInstruction()
            {
                ProgramId = ProgramIdKey.KeyBytes,
                Keys = new List<AccountMeta>(),
                Data = ComputeBudgetProgramData.EncodeSetLoadedAccountsDataSizeLimit(bytes)
            };
        }

        /// <summary>
        /// Decodes an instruction created by the Compute Budget Program.
        /// </summary>
        /// <param name="data">The instruction data to decode.</param>
        /// <returns>A decoded instruction.</returns>
        public static DecodedInstruction Decode(ReadOnlySpan<byte> data, IList<PublicKey> keys, byte[] keyIndices)
        {
            uint instruction = data.GetU8(ComputeBudgetProgramData.MethodOffset);

            if (!Enum.IsDefined(typeof(ComputeBudgetProgramInstructions.Values), instruction))
            {
                return new()
                {
                    PublicKey = ProgramIdKey,
                    InstructionName = "Unknown Instruction",
                    ProgramName = ProgramName,
                    Values = new Dictionary<string, object>(),
                    InnerInstructions = new List<DecodedInstruction>()
                };
            }

            ComputeBudgetProgramInstructions.Values instructionValue = (ComputeBudgetProgramInstructions.Values)instruction;


            DecodedInstruction decodedInstruction = new()
            {
                PublicKey = ProgramIdKey,
                InstructionName = ComputeBudgetProgramInstructions.Names[instructionValue],
                ProgramName = ProgramName,
                Values = new Dictionary<string, object>(),
                InnerInstructions = new List<DecodedInstruction>()
            };

            switch (instructionValue)
            {
                case ComputeBudgetProgramInstructions.Values.RequestHeapFrame:
                    ComputeBudgetProgramData.DecodeRequestHeapFrameData(decodedInstruction, data);
                    break;
                case ComputeBudgetProgramInstructions.Values.SetComputeUnitLimit:
                    ComputeBudgetProgramData.DecodeSetComputeUnitLimitData(decodedInstruction, data);
                    break;
                case ComputeBudgetProgramInstructions.Values.SetComputeUnitPrice:
                    ComputeBudgetProgramData.DecodeSetComputeUnitPriceData(decodedInstruction, data);
                    break;
                case ComputeBudgetProgramInstructions.Values.SetLoadedAccountsDataSizeLimit:
                    ComputeBudgetProgramData.DecodeSetLoadedAccountsDataSizeLimitData(decodedInstruction, data);
                    break;
            }

            return decodedInstruction;
        }
    }
}