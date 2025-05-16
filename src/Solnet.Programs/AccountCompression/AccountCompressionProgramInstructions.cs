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
    /// https://docs.rs/spl-account-compression/latest/spl_account_compression/instruction/index.html
    /// https://github.com/solana-program/account-compression/blob/ac-mainnet-tag/account-compression/sdk/src/instructions/index.ts
    /// </remarks>
    /// </summary>
    internal static class AccountCompressionProgramInstructions
    {
        /// <summary>
        /// Represents the user-friendly names for the instruction types for the <see cref="AccountCompressionProgram"/>.
        /// </summary>
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
        /// <summary>
        /// Represents the instruction types for the <see cref="AccountCompressionProgram"/>.
        /// </summary>
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
