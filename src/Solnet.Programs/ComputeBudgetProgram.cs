using Solnet.Programs.Utilities;
using Solnet.Rpc.Models;
using Solnet.Wallet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solnet.Programs
{
        /// <summary>
        /// Implements the ComputeBudget Program methods.
        /// <remarks>
        /// For more information see: https://spl.solana.com/memo
        /// </remarks>
        /// </summary>
 
    public class ComputeBudgetProgram
    {
        
            /// <summary>
            /// The public key of the ComputeBudget Program.
            /// </summary>
            public static readonly PublicKey ProgramIdKey = new("ComputeBudget111111111111111111111111111111");


            /// <summary>
            /// The program's name.
            /// </summary>
            private const string ProgramName = "Compute Budget Program";


  
        /// <summary>
        /// Request HeapFrame Instruction related to Priority Fees
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static TransactionInstruction RequestHeapFrame(uint bytes)
        {
            List<AccountMeta> keys = new();

            byte[] instructionBytes = new byte[17];
            instructionBytes.WriteU8(1, 0);
            instructionBytes.WriteU32(bytes, 1);

            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey.KeyBytes,
                Keys = keys,
                Data = instructionBytes
            };
        }
        /// <summary>
        /// Set Compute Unit Limit Instruction for Priority Fees
        /// </summary>
        /// <param name="units"></param>
        /// <returns></returns>
        public static TransactionInstruction SetComputeUnitLimit(uint units)
        {
            List<AccountMeta> keys = new();

            byte[] instructionBytes = new byte[9];
            instructionBytes.WriteU8(2, 0);
            instructionBytes.WriteU64(units, 1);

            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey.KeyBytes,
                Keys = keys,
                Data = instructionBytes
            };
        }
        /// <summary>
        /// Set Compute Unit Price Instruction for Priority Fees
        /// </summary>
        /// <param name="priority_rate"></param>
        /// <returns></returns>
        public static TransactionInstruction SetComputeUnitPrice(ulong priority_rate)
        {
            List<AccountMeta> keys = new();
            
            byte[] instructionBytes = new byte[9];
            instructionBytes.WriteU8(3, 0);
            instructionBytes.WriteU64(priority_rate, 1);

            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey.KeyBytes,
                Keys = keys,
                Data = instructionBytes
            };
        }

    }
}
