using Sol.Unity.Rpc.Utilities;
using Sol.Unity.Wallet;
using Sol.Unity.Wallet.Utilities;
using System;
using System.Collections.Generic;
using System.IO;

namespace Sol.Unity.Rpc.Models
{
    /// <summary>
    /// The message header.
    /// </summary>
    public class MessageHeader
    {
        #region Layout

        /// <summary>
        /// Represents the layout of the <see cref="MessageHeader"/> encoded values.
        /// </summary>
        internal static class Layout
        {
            /// <summary>
            /// The offset at which the byte that defines the number of required signatures begins.
            /// </summary>
            internal const int RequiredSignaturesOffset = 0;

            /// <summary>
            /// The offset at which the byte that defines the number of read-only signer accounts begins.
            /// </summary>
            internal const int ReadOnlySignedAccountsOffset = 1;

            /// <summary>
            /// The offset at which the byte that defines the number of read-only non-signer accounts begins.
            /// </summary>
            internal const int ReadOnlyUnsignedAccountsOffset = 2;

            /// <summary>
            /// The message header length.
            /// </summary>
            internal const int HeaderLength = 3;
        }

        #endregion Layout

        /// <summary>
        /// The number of required signatures.
        /// </summary>
        public byte RequiredSignatures { get; set; }

        /// <summary>
        /// The number of read-only signed accounts.
        /// </summary>
        public byte ReadOnlySignedAccounts { get; set; }

        /// <summary>
        /// The number of read-only non-signed accounts.
        /// </summary>
        public byte ReadOnlyUnsignedAccounts { get; set; }

        /// <summary>
        /// Convert the message header to byte array format.
        /// </summary>
        /// <returns>The byte array.</returns>
        internal byte[] ToBytes()
        {
            return new[] { RequiredSignatures, ReadOnlySignedAccounts, ReadOnlyUnsignedAccounts };
        }
    }

    /// <summary>
    /// Represents the Message of a Solana <see cref="Transaction"/>.
    /// </summary>
    public class Message
    {
        /// <summary>
        /// The header of the <see cref="Message"/>.
        /// </summary>
        public MessageHeader Header { get; set; }

        /// <summary>
        /// The list of account <see cref="PublicKey"/>s present in the transaction.
        /// </summary>
        public IList<PublicKey> AccountKeys { get; set; }

        /// <summary>
        /// The list of <see cref="TransactionInstruction"/>s present in the transaction.
        /// </summary>
        public IList<CompiledInstruction> Instructions { get; set; }

        /// <summary>
        /// The recent block hash for the transaction.
        /// </summary>
        public string RecentBlockhash { get; set; }

        /// <summary>
        /// Check whether an account is writable.
        /// </summary>
        /// <param name="index">The index of the account in the account keys.</param>
        /// <returns>true if the account is writable, false otherwise.</returns>
        public bool IsAccountWritable(int index) => index < Header.RequiredSignatures - Header.ReadOnlySignedAccounts ||
                                                    (index >= Header.RequiredSignatures &&
                                                     index < AccountKeys.Count - Header.ReadOnlyUnsignedAccounts);

        /// <summary>
        /// Check whether an account is a signer.
        /// </summary>
        /// <param name="index">The index of the account in the account keys.</param>
        /// <returns>true if the account is an expected signer, false otherwise.</returns>
        public bool IsAccountSigner(int index) => index < Header.RequiredSignatures;

        /// <summary>
        /// Serialize the message into the wire format.
        /// </summary>
        /// <returns>A byte array corresponding to the serialized message.</returns>
        public byte[] Serialize()
        {
            byte[] accountAddressesLength = ShortVectorEncoding.EncodeLength(AccountKeys.Count);
            byte[] instructionsLength = ShortVectorEncoding.EncodeLength(Instructions.Count);
            int accountKeysBufferSize = AccountKeys.Count * 32;

            MemoryStream accountKeysBuffer = new(accountKeysBufferSize);

            foreach (PublicKey key in AccountKeys)
            {
                accountKeysBuffer.Write(key.KeyBytes, 0, key.KeyBytes.Length);
            }

            int messageBufferSize = MessageHeader.Layout.HeaderLength + PublicKey.PublicKeyLength +
                                    accountAddressesLength.Length +
                                    +instructionsLength.Length + Instructions.Count + accountKeysBufferSize;
            MemoryStream buffer = new(messageBufferSize);
            buffer.Write(Header.ToBytes(), 0, Header.ToBytes().Length);
            buffer.Write(accountAddressesLength, 0, accountAddressesLength.Length);
            buffer.Write(accountKeysBuffer.ToArray(), 0, accountKeysBuffer.ToArray().Length);
            var decodedRecentBlockHash = Encoders.Base58.DecodeData(RecentBlockhash);
            buffer.Write(decodedRecentBlockHash, 0, decodedRecentBlockHash.Length);
            buffer.Write(instructionsLength, 0, instructionsLength.Length);

            foreach (CompiledInstruction compiledInstruction in Instructions)
            {
                buffer.WriteByte(compiledInstruction.ProgramIdIndex);
                buffer.Write(compiledInstruction.KeyIndicesCount, 0, compiledInstruction.KeyIndicesCount.Length);
                buffer.Write(compiledInstruction.KeyIndices, 0, compiledInstruction.KeyIndices.Length);
                buffer.Write(compiledInstruction.DataLength, 0 ,compiledInstruction.DataLength.Length);
                buffer.Write(compiledInstruction.Data, 0, compiledInstruction.Data.Length);
            }
            return buffer.ToArray();
        }

        /// <summary>
        /// Deserialize a compiled message into a Message object.
        /// </summary>
        /// <param name="data">The data to deserialize into the Message object.</param>
        /// <returns>The Message object instance.</returns>
        public static Message Deserialize(ReadOnlySpan<byte> data)
        {
            // Read message header
            byte numRequiredSignatures = data[MessageHeader.Layout.RequiredSignaturesOffset];
            byte numReadOnlySignedAccounts = data[MessageHeader.Layout.ReadOnlySignedAccountsOffset];
            byte numReadOnlyUnsignedAccounts = data[MessageHeader.Layout.ReadOnlyUnsignedAccountsOffset];

            // Read account keys
            (int accountAddressLength, int accountAddressLengthEncodedLength) =
                ShortVectorEncoding.DecodeLength(data.Slice(MessageHeader.Layout.HeaderLength,
                    ShortVectorEncoding.SpanLength));
            List<PublicKey> accountKeys = new(accountAddressLength);
            for (int i = 0; i < accountAddressLength; i++)
            {
                ReadOnlySpan<byte> keyBytes = data.Slice(
                    MessageHeader.Layout.HeaderLength + accountAddressLengthEncodedLength +
                    i * PublicKey.PublicKeyLength,
                    PublicKey.PublicKeyLength);
                accountKeys.Add(new PublicKey(keyBytes));
            }

            // Read block hash
            string blockHash =
                Encoders.Base58.EncodeData(data.Slice(
                    MessageHeader.Layout.HeaderLength + accountAddressLengthEncodedLength +
                    accountAddressLength * PublicKey.PublicKeyLength,
                    PublicKey.PublicKeyLength).ToArray());

            // Read the number of instructions in the message
            (int instructionsLength, int instructionsLengthEncodedLength) =
                ShortVectorEncoding.DecodeLength(
                    data.Slice(
                        MessageHeader.Layout.HeaderLength + accountAddressLengthEncodedLength +
                        (accountAddressLength * PublicKey.PublicKeyLength) + PublicKey.PublicKeyLength,
                        ShortVectorEncoding.SpanLength));

            List<CompiledInstruction> instructions = new(instructionsLength);
            int instructionsOffset =
                MessageHeader.Layout.HeaderLength + accountAddressLengthEncodedLength +
                (accountAddressLength * PublicKey.PublicKeyLength) + PublicKey.PublicKeyLength +
                instructionsLengthEncodedLength;
            ReadOnlySpan<byte> instructionsData = data[instructionsOffset..];

            // Read the instructions in the message
            for (int i = 0; i < instructionsLength; i++)
            {
                (CompiledInstruction compiledInstruction, int instructionLength) =
                    CompiledInstruction.Deserialize(instructionsData);
                instructions.Add(compiledInstruction);
                instructionsData = instructionsData[instructionLength..];
            }

            return new Message
            {
                Header = new MessageHeader
                {
                    RequiredSignatures = numRequiredSignatures,
                    ReadOnlySignedAccounts = numReadOnlySignedAccounts,
                    ReadOnlyUnsignedAccounts = numReadOnlyUnsignedAccounts
                },
                RecentBlockhash = blockHash,
                AccountKeys = accountKeys,
                Instructions = instructions,
            };
        }

        /// <summary>
        /// Deserialize a compiled message encoded as base-64 into a Message object.
        /// </summary>
        /// <param name="data">The data to deserialize into the Message object.</param>
        /// <returns>The Transaction object.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the given string is null.</exception>
        public static Message Deserialize(string data)
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
                throw new Exception("could not decode message data from base64", ex);
            }

            return Deserialize(decodedBytes);
        }
    }
}