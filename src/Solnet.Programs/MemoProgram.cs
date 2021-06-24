using Solnet.Rpc.Models;
using Solnet.Wallet;
using Solnet.Wallet.Utilities;
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
        private static readonly PublicKey ProgramIdKey = new("Memo1UhkJRfHyvLMcVucJwxXeuD728EqVDDwQDxFMNo");

        /// <summary>
        /// Initialize a new transaction instruction which interacts with the Memo Program.
        /// </summary>
        /// <param name="account">The account associated with the memo.</param>
        /// <param name="memo">The memo to be included in the transaction.</param>
        /// <returns>The <see cref="TransactionInstruction"/> which includes the memo data.</returns>
        public static TransactionInstruction NewMemo(Account account, string memo)
        {
            var keys = new List<AccountMeta>
            {
                new (account, false)
            };
            var memoBytes = Encoding.UTF8.GetBytes(memo);

            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey.KeyBytes,
                Keys = keys,
                Data = memoBytes
            };
        }

    }
}