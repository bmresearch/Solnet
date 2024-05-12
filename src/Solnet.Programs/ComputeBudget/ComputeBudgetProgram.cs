using Solnet.Programs.Abstract;
using Solnet.Rpc.Models;
using Solnet.Wallet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solnet.Programs.ComputeBudget
{
    public class ComputeBudgetProgram : BaseProgram
    {
        public static readonly PublicKey ProgramIdKey = new("ComputeBudget111111111111111111111111111111");

        /// <summary>
        /// The program's name.
        /// </summary>
        private const string ProgramName = "Compute Budget Program";

        public ComputeBudgetProgram(PublicKey programIdKey, string programName) : base(programIdKey, programName)
        {
        }

        public static TransactionInstruction SetComputeUnitLimit(
             ulong amount)
        {
            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey.KeyBytes,
                Keys = new List<AccountMeta>(),
                Data = ComputeBudgetProgramData.SetComputeUnitLimit(amount) 
            };
        }
        /// <summary>
        ///  Instruction to add fee in microlamports
        /// </summary> 
        /// <param name="source"></param>
        /// <param name="microlamports">The amount in microlamports.</param>
        /// <returns></returns>
        public static TransactionInstruction SetComputeUnitPrice(
            UInt64 microlamports)
        { 
            var instruction = new TransactionInstruction
            {
                ProgramId = ProgramIdKey.KeyBytes,
                Keys = new List<AccountMeta>(),
                Data = ComputeBudgetProgramData.SetComputeUnitPrice(microlamports)
            };
            return instruction;
        }

         
    }
}
