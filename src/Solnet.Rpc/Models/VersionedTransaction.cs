using Solnet.Rpc.Builders;
using Solnet.Rpc.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static Solnet.Rpc.Models.Message;

namespace Solnet.Rpc.Models
{
    /// <summary>
    /// Represents a Transaction in Solana.
    /// </summary>
    public class VersionedTransaction : Transaction
    {

        /// <summary>
        /// Address Lookup Table for the message. This is used to resolve addresses in the message.
        /// </summary>
        public List<MessageAddressLookupTable> AddressLookupTable { get; set; }

        /// <summary>
        /// The transaction configuration to use when compiling the versioned transaction.
        /// </summary>
        public TransactionConfig TransactionConfig { get; set; }

        /// <summary>
        /// The message version to use when compiling the versioned transaction.
        /// </summary>
        public byte Version { get; set; }


        /// <summary>
        /// Compile the transaction data.
        /// </summary>
        public override byte[] CompileMessage()
        {
            VersionedMessageBuilder messageBuilder = new()
            {
                FeePayer = FeePayer,
                AccountKeys = _accountKeys,
                Version = Version
            };

            if (RecentBlockHash != null) messageBuilder.RecentBlockHash = RecentBlockHash;
            if (NonceInformation != null) messageBuilder.NonceInformation = NonceInformation;

            foreach (TransactionInstruction instruction in Instructions)
            {
                messageBuilder.AddInstruction(instruction);
            }
            if(TransactionConfig != null) 
                messageBuilder.TransactionConfig = TransactionConfig;
            
            if(AddressLookupTable != null)
                messageBuilder.AddressLookupTable = AddressLookupTable ;

            return messageBuilder.Build();
        }

        /// <summary>
        /// Serialize the transaction into wire format.
        /// </summary>
        /// <returns>A byte array containing the serialized transaction data.</returns>
        public new byte[] Serialize()
        {
            return Version switch
            {
                1 => SerializeV1(),
                _ => SerializeV0()
            };
        }

        /// <summary>
        /// Serialize the transaction into wire format for version 0.
        /// </summary>
        /// <returns>A byte array containing the serialized transaction data.</returns>
        private byte[] SerializeV0()
        {
            byte[] message = CompileMessage();

            byte[] signaturesLength = ShortVectorEncoding.EncodeLength(Signatures.Count);

            MemoryStream buffer =
                new(
                    signaturesLength.Length +
                    (Signatures.Count *
                     TransactionBuilder.SignatureLength) +
                    message.Length);

            buffer.Write(signaturesLength);

            foreach (var signature in Signatures)
            {
                buffer.Write(signature.Signature);
            }

            buffer.Write(message);

            return buffer.ToArray();
        }

        /// <summary>
        /// Serialize the transaction into wire format for version 1.
        /// </summary>
        /// <returns>A byte array containing the serialized transaction data.</returns>
        private byte[] SerializeV1()
        {
            byte[] message = CompileMessage();

            MemoryStream buffer =
                new(
                    message.Length +
                    (
                        Signatures.Count *
                        TransactionBuilder.SignatureLength
                    ));

            buffer.Write(message);

            foreach (var signature in Signatures)
            {
                buffer.Write(signature.Signature);
            }

            return buffer.ToArray();
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
                Version = message.Version,
                RecentBlockHash = message.RecentBlockhash,
                Signatures = new List<SignaturePubKeyPair>(),
                Instructions = new List<TransactionInstruction>(),
                AddressLookupTable = message.AddressLookupTable,
                TransactionConfig = message.TransactionConfig,
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
        /// Deserialize a transaction encoded as bytes into a Transaction object.
        /// </summary>
        /// <param name="data">The byte array containing the serialized transaction data.</param>
        /// <returns>The deserialized VersionedTransaction object.</returns>
        /// <exception cref="ArgumentException">Thrown when the input data is empty.</exception>
        public static new VersionedTransaction Deserialize(ReadOnlySpan<byte> data)
        {
            if (data.Length == 0)
                throw new ArgumentException(nameof(data));

            // V1 transaction starts with 0x81
            if (data[0] == 0x81)
            {
                return DeserializeV1(data);
            }

            return DeserializeV0(data);
        }

        /// <summary>
        /// Deserialize a version 0 transaction encoded as bytes into a VersionedTransaction object.
        /// </summary>
        /// <param name="data">The byte array containing the serialized version 0 transaction data.</param>
        /// <returns>The deserialized VersionedTransaction object.</returns>
        private static VersionedTransaction DeserializeV0(ReadOnlySpan<byte> data)
        {
            (int signaturesLength, int encodedLength) =
                ShortVectorEncoding.DecodeLength(
                    data[..ShortVectorEncoding.SpanLength]);

            List<byte[]> signatures = new(signaturesLength);

            for (int i = 0; i < signaturesLength; i++)
            {
                ReadOnlySpan<byte> signature =
                    data.Slice(
                        encodedLength +
                        (i * TransactionBuilder.SignatureLength),
                        TransactionBuilder.SignatureLength);

                signatures.Add(signature.ToArray());
            }

            VersionedMessage message =
                VersionedMessage.Deserialize(
                    data[
                        (
                            encodedLength +
                            (
                                signaturesLength *
                                TransactionBuilder.SignatureLength
                            )
                        )..]);
            message.Version = 0;
            return Populate(message, signatures);
        }
        /// <summary>
        /// Deserialize a version 1 transaction encoded as bytes into a VersionedTransaction object.
        /// </summary>
        /// <param name="data">The byte array containing the serialized version 1 transaction data.</param>
        /// <returns>The deserialized VersionedTransaction object.</returns>
        private static VersionedTransaction DeserializeV1(ReadOnlySpan<byte> data)
        {
            VersionedMessage message = VersionedMessage.DeserializeV1(data);

            int signatureCount =
                message.Header.RequiredSignatures;

            int signatureBytes =
                signatureCount *
                TransactionBuilder.SignatureLength;

            int messageLength =
                data.Length - signatureBytes;

            List<byte[]> signatures =
                new(signatureCount);

            ReadOnlySpan<byte> signatureData =
                data[messageLength..];

            for (int i = 0; i < signatureCount; i++)
            {
                signatures.Add(
                    signatureData.Slice(
                        i * TransactionBuilder.SignatureLength,
                        TransactionBuilder.SignatureLength)
                    .ToArray());
            }

            return Populate(message, signatures);
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

