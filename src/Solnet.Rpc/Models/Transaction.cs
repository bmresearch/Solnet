// unset

using Solnet.Rpc.Builders;
using Solnet.Wallet;
using System;
using System.Collections.Generic;

namespace Solnet.Rpc.Models
{
    /// <summary>
    /// Nonce information to be used to build an offline transaction.
    /// </summary>
    public class NonceInformation
    {
        /// <summary>
        /// The current blockhash stored in the nonce account.
        /// </summary>
        public string Nonce { get; set; }
        
        /// <summary>
        /// An AdvanceNonceAccount instruction.
        /// </summary>
        public TransactionInstruction Instruction { get; set; }
    }

    /// <summary>
    /// Represents a Transaction in Solana.
    /// </summary>
    public class Transaction
    {
        /// <summary>
        /// The transaction's fee payer.
        /// </summary>
        public PublicKey FeePayer { get; init; }
        
        /// <summary>
        /// The list of <see cref="TransactionInstruction"/>s present in the transaction.
        /// </summary>
        public IList<TransactionInstruction> Instructions { get; set; }
        
        /// <summary>
        /// The recent block hash for the transaction.
        /// </summary>
        public string RecentBlockhash { get; set; }
        
        /// <summary>
        /// The nonce information of the transaction.
        /// <remarks>
        /// When this is set, the <see cref="NonceInformation"/>'s Nonce is used as the <c>RecentBlockhash</c>.
        /// </remarks>
        /// </summary>
        public NonceInformation NonceInformation { get; set; }
        
        /// <summary>
        /// The signatures for the transaction.
        /// <remarks>
        /// These are typically created by invoking the <c>Build(IList{Account} signers)</c> method of the <see cref="TransactionBuilder"/>,
        /// but can be created deserializing a Transaction and adding signatures manually.
        /// </remarks>
        /// </summary>
        public IList<byte[]> Signatures { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data">The data to deserialize into the Transaction object.</param>
        /// <returns>The Transaction object instance.</returns>
        public static Transaction Deserialize(ReadOnlySpan<byte> data)
        {
            return new Transaction() { };
        }
    }
}