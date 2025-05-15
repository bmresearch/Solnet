using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solnet.Programs
{
    /// <summary>
    /// Represents the instruction types for the <see cref="AccountCompressionProgram"/> ??? along with a friendly name so as not to use reflection ???.
    /// <remarks>
    /// For more information see:
    /// https://docs.solana.com/developing/runtime-facilities/programs#stake-program
    /// https://docs.rs/solana-sdk/1.7.14/solana_sdk/stake/instruction/enum.StakeInstruction.html
    /// </remarks>
    /// </summary>
    internal static class AccountCompressionProgramInstructions
    {
        internal static readonly Dictionary<Values, string> Names = new()
        {
            { Values.Append, "Append Merkle Tree" },
            { Values.CloseEmptyTree, "Close Empty Tree" },
            { Values.InitEmptyMerkleTree, "Init Empty Merkle Tree" },
            { Values.ReplaceLeaf, "Replace Leaf" },
            { Values.InsertOrAppend, "Insert Or Append Leaf" },
            { Values.TransferAuthority, "Transfer Authority" },
            { Values.VerifyLeaf, "Verify Leaf" }
        };

       internal enum Values: uint
        {
            Append = 0,
            CloseEmptyTree = 1,
            InitEmptyMerkleTree = 2,
            ReplaceLeaf = 3,
            InsertOrAppend = 4,
            TransferAuthority = 5,
            VerifyLeaf = 6
        }
    }
}
