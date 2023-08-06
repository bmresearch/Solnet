using Solnet.Rpc.Utilities;
using Solnet.Wallet;
using Solnet.Wallet.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Solnet.Rpc.Models
{
    public class VersionnedMessage
    {
        /// <summary>
        /// Prefix to determine the message version
        /// </summary>
        public static byte VersionPrefixMask = 0x7f;

        private enum MessageVersion
        {
            Legacy = 0,
            Versionned = 1,
        }

        private (MessageVersion type, int version) DeserializeVersion(byte prefix)
        {
            var maskedPrefix = prefix & VersionPrefixMask;

            // if the highest bit of the prefix is not set, the message is not versioned
            if (maskedPrefix == prefix)
            {
                return new (MessageVersion.Legacy, -1);
            }

            // the lower 7 bits of the prefix indicate the message version
            return new (MessageVersion.Versionned, maskedPrefix);
        }
        
                
    }
    
    /// <summary>
    /// Represents the versionned Message of a Solana <see cref="Transaction"/>.
    /// </summary>
    public class MessageV0
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
        /// Address table lookup for the transaction
        /// </summary>
        /// <value></value>
        public IList<MessageAddressTableLookup> AddressTableLookups { get; set; }

        /// <summary>
        /// The recent block hash for the transaction.
        /// </summary>
        public string RecentBlockhash { get; set; }

        /// <summary>
        /// Return the message version
        /// </summary>
        /// <returns></returns>
        public virtual int Version()
        {
            return 0;
        }

        /// <summary>
        /// Return the number of accounts in the tables lookups
        /// </summary>
        /// <returns></returns>
        public int GetNumberAccountKeysFromLookups() 
        {
            int count = 0;
            foreach(var lookup in AddressTableLookups)
            {
                count += lookup.ReadonlyIndexes.Length + lookup.WritableIndexes.Length;
            }

            return count;
        }

        public MessageAccountKeys GetAccountKey(AccountKeysFromLookups accountKeysFromLookups, AddressLookupTableAccount addressLookupTableAccount)
        {
            if (GetNumberAccountKeysFromLookups() != staticAccount.Count)
        }        

        /// <summary>
        /// Check whether an account is writable.
        /// </summary>
        /// <param name="index">The index of the account in the account keys.</param>
        /// <returns>true if the account is writable, false otherwise.</returns>
        public bool IsAccountWritable(int index)
        {
            int numSignedAccounts = Header.RequiredSignatures;
            int numStaticAccountKeys = AccountKeys.Count;
            
            if (index >= numStaticAccountKeys)
            {
                int lookupAccountKeysIndex = index - numStaticAccountKeys;
                int numWritableLookupAccountKeys = this.AddressTableLookups.Select(p => p.WritableIndexes.Length).DefaultIfEmpty(0).Sum();
                return lookupAccountKeysIndex < numWritableLookupAccountKeys;
            }
            else if (index >= this.Header.RequiredSignatures)
            {
                int unsignedAccountIndex = index - numSignedAccounts;
                int numUnsignedAccounts = numStaticAccountKeys - numSignedAccounts;
                int numWritableUnsignedAccounts = numUnsignedAccounts - this.Header.ReadOnlyUnsignedAccounts;
                return unsignedAccountIndex < numWritableUnsignedAccounts;
            }
            else
            {
                int numWritableSignedAccounts = numSignedAccounts - this.Header.ReadOnlySignedAccounts;
                return index < numWritableSignedAccounts;
            }
        }

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
            byte[] addressTableLookupsLength = ShortVectorEncoding.EncodeLength(AddressTableLookups.Count);
            int accountKeysBufferSize = AccountKeys.Count * 32;

            MemoryStream accountKeysBuffer = new(accountKeysBufferSize);

            foreach (PublicKey key in AccountKeys)
            {
                accountKeysBuffer.Write(key.KeyBytes);
            }

            int messageBufferSize = MessageHeader.Layout.HeaderLength + PublicKey.PublicKeyLength +
                                    accountAddressesLength.Length + addressTableLookupsLength.Length
                                    +instructionsLength.Length + Instructions.Count + accountKeysBufferSize;
            MemoryStream buffer = new(messageBufferSize);
            buffer.Write(Header.ToBytes());
            buffer.Write(accountAddressesLength);
            buffer.Write(accountKeysBuffer.ToArray());
            buffer.Write(Encoders.Base58.DecodeData(RecentBlockhash));
            buffer.Write(instructionsLength);

            foreach (CompiledInstruction compiledInstruction in Instructions)
            {
                buffer.WriteByte(compiledInstruction.ProgramIdIndex);
                buffer.Write(compiledInstruction.KeyIndicesCount);
                buffer.Write(compiledInstruction.KeyIndices);
                buffer.Write(compiledInstruction.DataLength);
                buffer.Write(compiledInstruction.Data);
            }

            buffer.Write(addressTableLookupsLength);

            foreach(var addressTableLookup in AddressTableLookups)
            {
                // buffer.Write
            }


            return buffer.ToArray();
        }

        /// <summary>
        /// Deserialize a compiled message into a Message object.
        /// </summary>
        /// <param name="data">The data to deserialize into the Message object.</param>
        /// <returns>The Message object instance.</returns>
        public static MessageV0 Deserialize(ReadOnlySpan<byte> data)
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

            return new MessageV0
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
        public static MessageV0 Deserialize(string data)
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

    /// <summary>
    /// Address table lookup for a versionned transaction
    /// </summary>
    public class MessageAddressTableLookup
    {
        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public PublicKey AccountKey { get; set; }

        /// <summary>
        /// Indexes for readonly address
        /// </summary>
        /// <value></value>
        public int[] ReadonlyIndexes { get; set; }

        /// <summary>
        /// Indexes for writable address
        /// </summary>
        /// <value></value>
        public int[] WritableIndexes { get; set; }
    }

    /// <summary>
    /// Represent an account key from a lookup
    /// </summary>
    public class AccountKeysFromLookups 
    {
        /// <summary>
        /// Writables accounts
        /// </summary>
        /// <value></value>
        public PublicKey[] Writables { get ; set; }

        /// <summary>
        /// Readonly accounts
        /// </summary>
        /// <value></value>
        public PublicKey[] Readonly { get; set; }
    }
}