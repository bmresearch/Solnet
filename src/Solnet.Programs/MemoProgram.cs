using Solnet.Rpc.Models;
using Solnet.Wallet;
using Solnet.Wallet.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
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

        /// <summary>
        /// The public key of the Memo Program V2.
        /// </summary>
        public static readonly PublicKey ProgramIdKeyV2 = new("MemoSq4gqABAXKb96qnH8TysNcWxMyWCqXgDLGmfcHr");

        /// <summary>
        /// The program's name.
        /// </summary>
        private const string ProgramName = "Memo Program";

        /// <summary>
        /// The instruction's name.
        /// </summary>
        private const string InstructionName = "New Memo";

        /// <summary>
        /// Initialize a new transaction instruction which interacts with the Memo Program.
        /// </summary>
        /// <param name="account">The public key of the account associated with the memo.</param>
        /// <param name="memo">The memo to be included in the transaction.</param>
        /// <returns>The <see cref="TransactionInstruction"/> which includes the memo data.</returns>
        public static TransactionInstruction NewMemo(PublicKey account, string memo)
        {
            List<AccountMeta> keys = new()
            {
                AccountMeta.ReadOnly(account, true)
            };
            byte[] memoBytes = Encoding.UTF8.GetBytes(memo);

            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey.KeyBytes,
                Keys = keys,
                Data = memoBytes
            };
        }

        /// <summary>
        /// Initialize a new transaction instruction which interacts with the Memo Program.
        /// </summary>
        /// <param name="account">The public key of the account associated with the memo.</param>
        /// <param name="memo">The memo to be included in the transaction.</param>
        /// <returns>The <see cref="TransactionInstruction"/> which includes the memo data.</returns>
        public static TransactionInstruction NewMemoV2(string memo, PublicKey account = null)
        {
            List<AccountMeta> keys = new();
            if (account != null)
                keys.Add(AccountMeta.ReadOnly(account, true));

            byte[] memoBytes = Encoding.UTF8.GetBytes(memo);

            return new TransactionInstruction
            {
                ProgramId = ProgramIdKeyV2.KeyBytes,
                Keys = keys,
                Data = memoBytes
            };
        }

        /// <summary>
        /// Decodes an instruction created by the Memo Program.
        /// </summary>
        /// <param name="data">The instruction data to decode.</param>
        /// <param name="keys">The account keys present in the transaction.</param>
        /// <param name="keyIndices">The indices of the account keys for the instruction as they appear in the transaction.</param>
        /// <returns>A decoded instruction.</returns>
        public static DecodedInstruction Decode(ReadOnlySpan<byte> data, IList<PublicKey> keys, byte[] keyIndices)
        {
            DecodedInstruction decodedInstruction = new()
            {
                PublicKey = keys.Any(x => x.Key == ProgramIdKey.Key) ? ProgramIdKey : ProgramIdKeyV2,
                InstructionName = InstructionName,
                ProgramName = ProgramName,
                InnerInstructions = new List<DecodedInstruction>(),
                Values = new Dictionary<string, object>()
            };
            if (keyIndices.Length > 0)
            {
                decodedInstruction.Values.Add("Signer", keys[keyIndices[0]]);
                decodedInstruction.Values.Add("Memo", Encoding.UTF8.GetString(data));
            }
            else
            {
                decodedInstruction.Values.Add("Memo", Encoding.UTF8.GetString(data));
            }

            return decodedInstruction;
        }
    }
}