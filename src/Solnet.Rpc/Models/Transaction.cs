using Solnet.Rpc.Builders;
using Solnet.Rpc.Utilities;
using Solnet.Wallet;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Solnet.Rpc.Models
{
    /// <summary>
    /// A pair corresponding of a public key and it's verifiable signature. 
    /// </summary>
    public class SignaturePubKeyPair
    {
        /// <summary>
        /// The public key to verify the signature against.
        /// </summary>
        public PublicKey PublicKey { get; set; }

        /// <summary>
        /// The signature created by the corresponding <see cref="PrivateKey"/> of this pair's <see cref="PublicKey"/>.
        /// </summary>
        public byte[] Signature { get; set; }
    }

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
        public PublicKey FeePayer { get; set; }

        /// <summary>
        /// The list of <see cref="TransactionInstruction"/>s present in the transaction.
        /// </summary>
        public List<TransactionInstruction> Instructions { get; set; }

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
        /// but can be created by deserializing a Transaction and adding signatures manually.
        /// </remarks>
        /// </summary>
        public List<SignaturePubKeyPair> Signatures { get; set; }

        /// <summary>
        /// Populate the Transaction from the given message and signatures.
        /// </summary>
        /// <param name="message">The <see cref="Message"/> object.</param>
        /// <param name="signatures">The list of signatures.</param>
        /// <returns>The Transaction object.</returns>
        public static Transaction Populate(Message message, IList<byte[]> signatures = null)
        {
            Transaction tx = new() {RecentBlockhash = message.RecentBlockhash};

            if (message.Header.RequiredSignatures > 0)
            {
                tx.FeePayer = message.AccountKeys[0];
            }

            if (signatures == null)
            {
                return tx;
            }

            for (int i = 0; i < signatures.Count; i++)
            {
                tx.Signatures.Add(new SignaturePubKeyPair
                {
                    PublicKey = message.AccountKeys[i], Signature = signatures[i]
                });
            }

            foreach (CompiledInstruction compiledInstruction in message.Instructions)
            {
                int accountLength = ShortVectorEncoding.DecodeLength(compiledInstruction.KeyIndicesCount);
                List<AccountMeta> accounts = new(accountLength);
                for (int i = 0; i < accountLength; i++)
                {
                    accounts.Add(new AccountMeta(message.AccountKeys[i], message.IsAccountWritable(i),
                        tx.Signatures.Any(pair => pair.PublicKey.Key == message.AccountKeys[i].Key)));
                }

                tx.Instructions.Add(new TransactionInstruction
                {
                    Keys = accounts,
                    ProgramId = message.AccountKeys[compiledInstruction.ProgramIdIndex],
                    Data = compiledInstruction.Data
                });
            }

            return tx;
        }

        /// <summary>
        /// Populate the Transaction from the given compiled message and signatures.
        /// </summary>
        /// <param name="message">The compiled message, as base-64 encoded string.</param>
        /// <param name="signatures">The list of signatures.</param>
        /// <returns>The Transaction object.</returns>
        public static Transaction Populate(string message, IList<byte[]> signatures = null)
            => Populate(Message.Deserialize(message), signatures);

        /// <summary>
        /// Deserialize a wire format transaction into a Transaction object.
        /// </summary>
        /// <param name="data">The data to deserialize into the Transaction object.</param>
        /// <returns>The Transaction object.</returns>
        public static Transaction Deserialize(ReadOnlySpan<byte> data)
        {
            // Read number of signatures
            int signaturesLength = ShortVectorEncoding.DecodeLength(data[..ShortVectorEncoding.SpanLength]);
            List<byte[]> signatures = new(signaturesLength);

            for (int i = 0; i < signaturesLength; i++)
            {
                ReadOnlySpan<byte> signature =
                    data.Slice(ShortVectorEncoding.SpanLength + (i * TransactionBuilder.SignatureLength),
                        PublicKey.PublicKeyLength);
                signatures.Add(signature.ToArray());
            }

            return Populate(
                Message.Deserialize(data[
                    (ShortVectorEncoding.SpanLength + (signaturesLength * TransactionBuilder.SignatureLength))..]),
                signatures);
        }

        /// <summary>
        /// Deserialize a transaction encoded as base-64 into a Transaction object.
        /// </summary>
        /// <param name="data">The data to deserialize into the Transaction object.</param>
        /// <returns>The Transaction object.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the given string is null.</exception>
        public static Transaction Deserialize(string data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            byte[] decodedBytes;

            try
            {
                decodedBytes = Convert.FromBase64String(data);
            }
            catch (Exception ex)
            {
                throw new Exception("could not decode transaction data from base64", ex);
            }

            return Deserialize(decodedBytes);
        }
    }
}