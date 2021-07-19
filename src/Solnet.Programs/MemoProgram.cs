using Solnet.Rpc.Models;
using Solnet.Wallet;
using Solnet.Wallet.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Solnet.Programs
{
    /// <summary>
    /// Implements the Memo Program methods.
    /// <remarks>
    /// For more information see: https://spl.solana.com/memo
    /// </remarks>
    /// </summary>
    public static class MemoProgram
    {
        /// <summary>
        /// The public key of the Memo Program.
        /// </summary>
        public static readonly PublicKey ProgramIdKey = new("Memo1UhkJRfHyvLMcVucJwxXeuD728EqVDDwQDxFMNo");

        private const string ProgramName = "Memo";

        private const string InstructionName = "Memo";

        /// <summary>
        /// Initialize a new transaction instruction which interacts with the Memo Program.
        /// </summary>
        /// <param name="account">The public key of the account associated with the memo.</param>
        /// <param name="memo">The memo to be included in the transaction.</param>
        /// <returns>The <see cref="TransactionInstruction"/> which includes the memo data.</returns>
        public static TransactionInstruction NewMemo(PublicKey account, string memo)
        {
            List<AccountMeta> keys = new ()
            {
                AccountMeta.ReadOnly(account, false)
            };
            byte[] memoBytes = Encoding.UTF8.GetBytes(memo);

            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey.KeyBytes,
                Keys = keys,
                Data = memoBytes
            };
        }

        public static DecodedInstruction Decode(ReadOnlySpan<byte> data)
        {
            DecodedInstruction decodedInstruction = new();
            decodedInstruction.PublicKey = ProgramIdKey;
            decodedInstruction.InstructionName = InstructionName;
            decodedInstruction.ProgramName = ProgramName;
            decodedInstruction.Values =
                new Dictionary<string, object>() {{"memo", Encoding.UTF8.GetString(data)}};
            
            return decodedInstruction;
        }

    }
}