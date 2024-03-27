using Solnet.Rpc.Builders;
using Solnet.Rpc.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Solnet.Rpc.Models.Message;

namespace Solnet.Rpc.Models
{
    /// <summary>
    /// Represents a Transaction in Solana.
    /// </summary>
    public class VersionedTransaction : Transaction
    {

        /// <summary>
        /// Address Table Lookups
        /// </summary>
        public List<MessageAddressTableLookup> AddressTableLookups { get; set; }


        /// <summary>
        /// Compile the transaction data.
        /// </summary>
        public override byte[] CompileMessage()
        {
            VersionedMessageBuilder messageBuilder = new() { FeePayer = FeePayer, AccountKeys = _accountKeys };

            if (RecentBlockHash != null) messageBuilder.RecentBlockHash = RecentBlockHash;
            if (NonceInformation != null) messageBuilder.NonceInformation = NonceInformation;

            foreach (TransactionInstruction instruction in Instructions)
            {
                messageBuilder.AddInstruction(instruction);
            }

            messageBuilder.AddressTableLookups = AddressTableLookups;

            return messageBuilder.Build();
        }

        /// <summary>
        /// Populate the Transaction from the given message and signatures.
        /// </summary>
        /// <param name="message">The <see cref="Message"/> object.</param>
        /// <param name="signatures">The list of signatures.</param>
        /// <returns>The Transaction object.</returns>
        public static VersionedTransaction Populate(VersionedMessage message, IList<byte[]> signatures = null)
        {
            VersionedTransaction tx = new()
            {
                RecentBlockHash = message.RecentBlockhash,
                Signatures = new List<SignaturePubKeyPair>(),
                Instructions = new List<TransactionInstruction>(),
                AddressTableLookups = message.AddressTableLookups,
                _accountKeys = message.AccountKeys
            };

            if (message.Header.RequiredSignatures > 0)
            {
                tx.FeePayer = message.AccountKeys[0];
            }

            if (signatures != null)
            {
                for (int i = 0; i < signatures.Count; i++)
                {
                    tx.Signatures.Add(new SignaturePubKeyPair
                    {
                        PublicKey = message.AccountKeys[i],
                        Signature = signatures[i]
                    });
                }
            }

            for (int i = 0; i < message.Instructions.Count; i++)
            {
                CompiledInstruction compiledInstruction = message.Instructions[i];
                (int accountLength, _) = ShortVectorEncoding.DecodeLength(compiledInstruction.KeyIndicesCount);

                List<AccountMeta> accounts = new(accountLength);
                for (int j = 0; j < accountLength; j++)
                {
                    int k = compiledInstruction.KeyIndices[j];
                    if (k >= message.AccountKeys.Count) continue;
                    accounts.Add(new AccountMeta(message.AccountKeys[k], message.IsAccountWritable(k),
                    tx.Signatures.Any(pair => pair.PublicKey.Key == message.AccountKeys[k].Key) || message.IsAccountSigner(k)));
                }

                VersionedTransactionInstruction instruction = new()
                {
                    Keys = accounts,
                    KeyIndices = compiledInstruction.KeyIndices,
                    ProgramId = message.AccountKeys[compiledInstruction.ProgramIdIndex],
                    Data = compiledInstruction.Data
                };
                if (i == 0 && accounts.Any(a => a.PublicKey == "SysvarRecentB1ockHashes11111111111111111111"))
                {
                    tx.NonceInformation = new NonceInformation { Instruction = instruction, Nonce = tx.RecentBlockHash };
                    continue;
                }
                tx.Instructions.Add(instruction);
            }

            return tx;
        }

        /// <summary>
        /// Populate the Transaction from the given compiled message and signatures.
        /// </summary>
        /// <param name="message">The compiled message, as base-64 encoded string.</param>
        /// <param name="signatures">The list of signatures.</param>
        /// <returns>The Transaction object.</returns>
        public static new VersionedTransaction Populate(string message, IList<byte[]> signatures = null)
            => Populate(VersionedMessage.Deserialize(message), signatures);

        /// <summary>
        /// Deserialize a wire format transaction into a Transaction object.
        /// </summary>
        /// <param name="data">The data to deserialize into the Transaction object.</param>
        /// <returns>The Transaction object.</returns>
        public static new VersionedTransaction Deserialize(ReadOnlySpan<byte> data)
        {
            // Read number of signatures
            (int signaturesLength, int encodedLength) =
                ShortVectorEncoding.DecodeLength(data[..ShortVectorEncoding.SpanLength]);
            List<byte[]> signatures = new(signaturesLength);

            for (int i = 0; i < signaturesLength; i++)
            {
                ReadOnlySpan<byte> signature =
                    data.Slice(encodedLength + (i * TransactionBuilder.SignatureLength),
                        TransactionBuilder.SignatureLength);
                signatures.Add(signature.ToArray());
            }

            var message = VersionedMessage.Deserialize(data[(encodedLength + (signaturesLength * TransactionBuilder.SignatureLength))..]);
            return Populate(message, signatures);
        }

        /// <summary>
        /// Deserialize a transaction encoded as base-64 into a Transaction object.
        /// </summary>
        /// <param name="data">The data to deserialize into the Transaction object.</param>
        /// <returns>The Transaction object.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the given string is null.</exception>
        public static new VersionedTransaction Deserialize(string data)
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

