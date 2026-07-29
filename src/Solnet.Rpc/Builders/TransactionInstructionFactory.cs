using Solnet.Rpc.Models;
using Solnet.Wallet;
using System;
using System.Collections.Generic;

namespace Solnet.Rpc.Builders
{
    /// <summary>
    /// Methods to create instances of the TransactionInstruction for languages that do not support init-only setters
    /// </summary>
    public static class TransactionInstructionFactory
    {

        /// <summary>
        /// Helper method to make TransactionInstruction objects available to outside Program implementations in 
        /// languages that do not currently support immutable init-only setters like VB.Net
        /// </summary>
        /// <param name="programId">The program ID associated with the instruction.</param>
        /// <param name="keys">The keys associated with the instruction.</param>
        /// <param name="data">The instruction-specific data.</param>
        /// <returns></returns>
        public static TransactionInstruction Create(PublicKey programId,
                                                    IList<AccountMeta> keys,
                                                    byte[] data)
        {

            ArgumentNullException.ThrowIfNull(programId);
            ArgumentNullException.ThrowIfNull(keys);
            ArgumentNullException.ThrowIfNull(data);

            return new TransactionInstruction
            {
                ProgramId = programId.KeyBytes,
                Keys = keys,
                Data = data
            };

        }

    }

}