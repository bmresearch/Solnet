using System;
using System.Collections.Generic;
using Solnet.Wallet;
using Solnet.Rpc.Models;

namespace Solnet.Rpc.Builders
{
    /// <summary>
    /// Methods to create instances of the TransactionInstruction for languages that do not support init-only setters
    /// </summary>
    public static class TransactionInstructionFactory
    {

        /// <summary>
        /// Helper method to make TransactionInstruction objects available to languages
        /// that do not currently support immutable init-only setters like VB.Net
        /// </summary>
        /// <param name="programId"></param>
        /// <param name="keys"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static TransactionInstruction Create(PublicKey programId, 
                                                    IList<AccountMeta> keys, 
                                                    byte[] data) {

            if (programId == null) throw new ArgumentNullException(nameof(programId));
            if (keys == null) throw new ArgumentNullException(nameof(keys));
            if (data == null) throw new ArgumentNullException(nameof(data));

            return new TransactionInstruction
            {
                ProgramId = programId.KeyBytes,
                Keys = keys,
                Data = data
            };

        }

    }

}
