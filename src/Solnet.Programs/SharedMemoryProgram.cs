using Solnet.Programs.Utilities;
using Solnet.Rpc.Models;
using Solnet.Wallet;
using Solnet.Wallet.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Solnet.Programs
{
    /// <summary>
    /// Helper class for the Shared Memory Program.
    /// <remarks>
    /// Used to write data to a given account data.
    /// Note: this program, as of writing this, was inactive in some clusters.
    /// </remarks>
    /// </summary>
    public static class SharedMemoryProgram
    {
        /// <summary>
        /// The address of the Shared Memory Program.
        /// </summary>
        public static readonly PublicKey ProgramId = new("shmem4EWT2sPdVGvTZCzXXRAURL9G5vpPxNwSeKhHUL");

        /// <summary>
        /// Creates an instruction used to interact with the Shared memory program.
        /// This instruction writes data to a given program starting at a specific offset.
        /// </summary>
        /// <param name="dest">The account where the data is to be written.</param>
        /// <param name="payload">The data to be written.</param>
        /// <param name="offset">The offset of the account data to write to.</param>
        /// <returns>The <see cref="TransactionInstruction"/> encoded that interacts with the shared memory program..</returns>
        public static TransactionInstruction Write(PublicKey dest, ReadOnlySpan<byte> payload, ulong offset)
        {
            List<AccountMeta> keys = new ()
            {
                new AccountMeta(dest, true)
            };

            byte[] transactionData = new byte[payload.Length + 8];

            transactionData.WriteU64(offset, 0);
            transactionData.WriteSpan(payload, 8);

            return new TransactionInstruction
            {
                ProgramId = ProgramId.KeyBytes,
                Keys = keys,
                Data = transactionData
            };
        }

    }
}