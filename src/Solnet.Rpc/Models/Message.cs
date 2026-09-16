using Solnet.Rpc.Utilities;
using Solnet.Wallet;
using Solnet.Wallet.Utilities;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Solnet.Rpc.Models
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
        public virtual byte[] Serialize()
        {
            byte[] accountAddressesLength = ShortVectorEncoding.EncodeLength(AccountKeys.Count);
            byte[] instructionsLength = ShortVectorEncoding.EncodeLength(Instructions.Count);
            int accountKeysBufferSize = AccountKeys.Count * 32;

            MemoryStream accountKeysBuffer = new(accountKeysBufferSize);

            foreach (PublicKey key in AccountKeys)
            {
                accountKeysBuffer.Write(key.KeyBytes);
            }

            int messageBufferSize = MessageHeader.Layout.HeaderLength + PublicKey.PublicKeyLength +
                                    accountAddressesLength.Length +
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
            return buffer.ToArray();
        }

        /// <summary>
        /// Deserialize a compiled message into a Message object.
        /// </summary>
        /// <param name="data">The data to deserialize into the Message object.</param>
        /// <returns>The Message object instance.</returns>
        public static Message Deserialize(ReadOnlySpan<byte> data)
        {

            // Check that the message is not a VersionedMessage
            byte prefix = data[0];
            byte maskedPrefix = (byte)(prefix & VersionedMessage.VersionPrefixMask);
            if (prefix != maskedPrefix)
                throw new NotSupportedException("The message is a VersionedMessage, use VersionedMessage." +
                                                    "Deserialize instead.");

            // Read message header
            byte numRequiredSignatures = data[MessageHeader.Layout.RequiredSignaturesOffset];
            byte numReadOnlySignedAccounts = data[MessageHeader.Layout.ReadOnlySignedAccountsOffset];
            byte numReadOnlyUnsignedAccounts = data[MessageHeader.Layout.ReadOnlyUnsignedAccountsOffset];

            // Read account keys
            (int accountAddressLength, int accountAddressLengthEncodedLength) =
                ShortVectorEncoding.DecodeLength(GetShortVectorSpan(data, MessageHeader.Layout.HeaderLength));
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
                    GetShortVectorSpan(
                        data,
                        MessageHeader.Layout.HeaderLength + accountAddressLengthEncodedLength +
                        (accountAddressLength * PublicKey.PublicKeyLength) + PublicKey.PublicKeyLength));

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

        /// <summary>
        /// Versioned Message
        /// </summary>
        public class VersionedMessage : Message
        {
            /// <summary>
            /// Version prefix Mask.
            /// </summary>
            public const byte VersionPrefixMask = 0x7F;

            /// <summary>
            /// The message version encoded in the low 7 bits of the versioned prefix.
            /// </summary>
            public byte Version { get; set; }

            /// <summary>
            /// Address table lookup
            /// </summary>
            public List<MessageAddressLookupTable> AddressLookupTable { get; set; }

            /// <summary>
            /// The transaction configuration for the versioned message.
            /// </summary>
            public TransactionConfig TransactionConfig { get; set; }

            /// <summary>
            /// Serialize the versioned message into the wire format.
            /// </summary>
            /// <returns>A byte array corresponding to the serialized versioned message.</returns>
            public byte[] SerializeV0()
            {
                byte[] accountAddressesLength = ShortVectorEncoding.EncodeLength(AccountKeys.Count);
                byte[] instructionsLength = ShortVectorEncoding.EncodeLength(Instructions.Count);
                int accountKeysBufferSize = AccountKeys.Count * PublicKey.PublicKeyLength;

                MemoryStream accountKeysBuffer = new(accountKeysBufferSize);

                foreach (PublicKey key in AccountKeys)
                {
                    accountKeysBuffer.Write(key.KeyBytes);
                }

                byte[] addressLookupTableBytes = AddressTableLookupUtils.SerializeAddressLookupTable(AddressLookupTable);
                int messageBufferSize = 1 + MessageHeader.Layout.HeaderLength + PublicKey.PublicKeyLength +
                                        accountAddressesLength.Length + instructionsLength.Length + Instructions.Count +
                                        accountKeysBufferSize + addressLookupTableBytes.Length;
                MemoryStream buffer = new(messageBufferSize);
                byte[] messageHeaderBytes = Header.ToBytes();

                buffer.WriteByte((byte)(0x80 | Version));
                buffer.Write(messageHeaderBytes);
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

                buffer.Write(addressLookupTableBytes);
                return buffer.ToArray();
            }
            /// <summary>
            /// Serialize the versioned message into the wire format for version 1.
            /// </summary>
            /// <returns>A byte array corresponding to the serialized versioned message.</returns>
            public byte[] SerializeV1()
            {
                MemoryStream buffer = new(GetV1Size());

                TransactionConfigMask mask = CreateMask(TransactionConfig);

                Span<byte> scratch = stackalloc byte[8];

                // Version Prefix
                buffer.WriteByte((byte)(0x80 | Version));
                // Message Header
                buffer.Write(Header.ToBytes());

                // Config Mask (u32 LE)
                BinaryPrimitives.WriteUInt32LittleEndian(
                    scratch[..4],
                    mask.Value);

                buffer.Write(scratch[..4]);

                // Lifetime Specifier / Blockhash
                buffer.Write(Encoders.Base58.DecodeData(RecentBlockhash));

                // Counts
                buffer.WriteByte((byte)Instructions.Count);
                buffer.WriteByte((byte)AccountKeys.Count);

                // Account Keys
                foreach (PublicKey key in AccountKeys)
                {
                    Console.WriteLine(key.ToString());
                    buffer.Write(key.KeyBytes);
                }
                // Config Values

                if (TransactionConfig.PriorityFee.HasValue)
                {
                    BinaryPrimitives.WriteUInt64LittleEndian(scratch, TransactionConfig.PriorityFee.Value);

                    buffer.Write(scratch);
                }

                if (TransactionConfig.ComputeUnitLimit.HasValue)
                {
                    BinaryPrimitives.WriteUInt32LittleEndian(scratch[..4], TransactionConfig.ComputeUnitLimit.Value);
                    buffer.Write(scratch[..4]);
                }

                if (TransactionConfig.LoadedAccountsDataSizeLimit.HasValue)
                {
                    BinaryPrimitives.WriteUInt32LittleEndian(scratch[..4], TransactionConfig.LoadedAccountsDataSizeLimit.Value);
                    buffer.Write(scratch[..4]);
                }

                if (TransactionConfig.HeapSize.HasValue)
                {
                    BinaryPrimitives.WriteUInt32LittleEndian(scratch[..4], TransactionConfig.HeapSize.Value);
                    buffer.Write(scratch[..4]);
                }

                // Instruction Headers
                foreach (CompiledInstruction ix in Instructions)
                {
                    buffer.WriteByte(ix.ProgramIdIndex);
                    buffer.WriteByte((byte)ix.KeyIndices.Length);
                    BinaryPrimitives.WriteUInt16LittleEndian(scratch[..2], (ushort)ix.Data.Length);
                    buffer.Write(scratch[..2]);
                }

                // Instruction Payloads
                foreach (CompiledInstruction ix in Instructions)
                {
                    buffer.Write(ix.KeyIndices);
                    buffer.Write(ix.Data);
                }
                
                return buffer.ToArray();
            }
            private int GetV1Size()
            {
                int size = 0;

                // version prefix
                size += 1;

                // header
                size += 3;

                // config mask
                size += sizeof(uint);

                // blockhash
                size += 32;

                // instruction count
                size += sizeof(byte);

                // account count
                size += sizeof(byte);

                // account keys
                size += AccountKeys.Count * PublicKey.PublicKeyLength;

                // config values
                size += TransactionConfig?.Size() ?? 0;

                // instruction headers
                size += Instructions.Count * 4;

                // instruction payloads
                foreach (CompiledInstruction ix in Instructions)
                {
                    size += ix.KeyIndices.Length;
                    size += ix.Data.Length;
                }

                return size;
            }

            /// <summary>
            /// Serialize the versioned message into the wire format based on the version.
            /// </summary>
            /// <returns>A byte array containing the serialized message data.</returns>
            /// <exception cref="NotSupportedException"></exception>
            public override byte[] Serialize() 
            {
                switch(Version)
                {
                    case 0:
                        return SerializeV0();
                    case 1:
                        return SerializeV1();
                    default:
                        throw new NotSupportedException($"Version {Version} is not supported for serialization.");
                }
                
            }

            /// <summary>
            /// Deserialize a compiled message into a VersionedMessage object.
            /// </summary>
            /// <param name="data">The data to deserialize into the VersionedMessage object.</param>
            /// <returns>The VersionedMessage object instance.</returns>
            /// <exception cref="NotSupportedException">Thrown when the data represents a legacy message instead of a versioned message.</exception>
            public static new VersionedMessage Deserialize(ReadOnlySpan<byte> data)
            {

                byte prefix = data[0];
                byte maskedPrefix = (byte)(prefix & VersionPrefixMask);

                if (prefix == maskedPrefix)
                    throw new NotSupportedException("Expected versioned message but received legacy message");

                byte version = maskedPrefix;
                
                switch(version)
                {
                    case 0:
                        return DeserializeV0(data);
                    case 1:
                        return DeserializeV1(data);
                    default:
                        throw new NotSupportedException($"Version {version} is not supported for deserialization.");
                }
            }

            /// <summary>
            /// Deserialize a compiled message into a VersionedMessage object for version 1.
            /// </summary>
            /// <param name="data">The byte span containing the serialized message data.</param>
            /// <returns>A <see cref="VersionedMessage"/> object deserialized from the provided data.</returns>
            /// <exception cref="NotSupportedException"></exception>
            /// <exception cref="FormatException"></exception>
            public static VersionedMessage DeserializeV1(ReadOnlySpan<byte> data)
            {
                int offset = 0;

                byte prefix = data[offset++];

                byte version =
                    (byte)(prefix & VersionPrefixMask);

                if (version != 1)
                    throw new NotSupportedException(
                        $"Expected V1 message, got V{version}");

                MessageHeader header = new()
                {
                    RequiredSignatures = data[offset++],
                    ReadOnlySignedAccounts = data[offset++],
                    ReadOnlyUnsignedAccounts = data[offset++]
                };

                TransactionConfigMask configMask =
                    new(BinaryPrimitives.ReadUInt32LittleEndian(
                        data.Slice(offset, 4)));

                offset += 4;

                if (configMask.HasUnknownBits() ||
                    configMask.HasInvalidPriorityFeeBits())
                {
                    throw new FormatException(
                        "Invalid transaction config mask.");
                }

                string recentBlockHash =
                    Encoders.Base58.EncodeData(
                        data.Slice(offset, 32).ToArray());

                offset += 32;

                byte instructionCount = data[offset++];
                byte accountCount = data[offset++];

                List<PublicKey> accountKeys =
                    new(accountCount);

                for (int i = 0; i < accountCount; i++)
                {
                    accountKeys.Add(
                        new PublicKey(
                            data.Slice(
                                offset,
                                PublicKey.PublicKeyLength)));

                    offset += PublicKey.PublicKeyLength;
                }

                TransactionConfig config = new();

                if (configMask.HasPriorityFee())
                {
                    config.PriorityFee =
                        BinaryPrimitives.ReadUInt64LittleEndian(
                            data.Slice(offset, 8));

                    offset += 8;
                }

                if (configMask.HasComputeUnitLimit())
                {
                    config.ComputeUnitLimit =
                        BinaryPrimitives.ReadUInt32LittleEndian(
                            data.Slice(offset, 4));

                    offset += 4;
                }

                if (configMask.HasLoadedAccountsDataSize())
                {
                    config.LoadedAccountsDataSizeLimit =
                        BinaryPrimitives.ReadUInt32LittleEndian(
                            data.Slice(offset, 4));

                    offset += 4;
                }

                if (configMask.HasHeapSize())
                {
                    config.HeapSize =
                        BinaryPrimitives.ReadUInt32LittleEndian(
                            data.Slice(offset, 4));

                    offset += 4;
                }

                // Read instruction headers first.

                List<(byte ProgramId,
                      byte AccountCount,
                      ushort DataLength)> headers =
                    new(instructionCount);

                for (int i = 0; i < instructionCount; i++)
                {
                    byte programId =
                        data[offset++];

                    byte numAccounts =
                        data[offset++];

                    ushort dataLength =
                        BinaryPrimitives.ReadUInt16LittleEndian(
                            data.Slice(offset, 2));

                    offset += 2;

                    headers.Add(
                        (
                            programId,
                            numAccounts,
                            dataLength
                        ));
                }

                // Read payloads after all headers.

                List<CompiledInstruction> instructions =
                    new(instructionCount);

                foreach (var headerInfo in headers)
                {
                    byte[] accounts =
                        data.Slice(
                            offset,
                            headerInfo.AccountCount)
                            .ToArray();

                    offset += headerInfo.AccountCount;

                    byte[] instructionData =
                        data.Slice(
                            offset,
                            headerInfo.DataLength)
                            .ToArray();

                    offset += headerInfo.DataLength;

                    instructions.Add(
                        new CompiledInstruction
                        {
                            ProgramIdIndex =
                                headerInfo.ProgramId,

                            KeyIndices = accounts,

                            Data = instructionData,

                            KeyIndicesCount =
                                ShortVectorEncoding.EncodeLength(
                                    accounts.Length),

                            DataLength =
                                ShortVectorEncoding.EncodeLength(
                                    instructionData.Length)
                        });
                }

                return new VersionedMessage
                {
                    Version = 1,
                    Header = header,
                    TransactionConfig = config,
                    RecentBlockhash = recentBlockHash,
                    AccountKeys = accountKeys,
                    Instructions = instructions
                };
            }

            

            /// <summary>
            /// Deserialize a compiled message into a VersionedMessage object.
            /// </summary>
            /// <param name="data">The data to deserialize into the VersionedMessage object.</param>
            /// <returns>The VersionedMessage object instance.</returns>
            /// <exception cref="NotSupportedException">Thrown when the data represents a legacy message instead of a versioned message.</exception>
            public static VersionedMessage DeserializeV0(ReadOnlySpan<byte> data)
            {
                byte prefix = data[0];
                byte maskedPrefix = (byte)(prefix & VersionPrefixMask);

                if (prefix == maskedPrefix)
                    throw new NotSupportedException("Expected versioned message but received legacy message");

                byte version = maskedPrefix;

                data = data.Slice(1, data.Length - 1); // Remove the processed prefix byte

                // Read message header
                byte numRequiredSignatures = data[MessageHeader.Layout.RequiredSignaturesOffset];
                byte numReadOnlySignedAccounts = data[MessageHeader.Layout.ReadOnlySignedAccountsOffset];
                byte numReadOnlyUnsignedAccounts = data[MessageHeader.Layout.ReadOnlyUnsignedAccountsOffset];

                // Read account keys
                (int accountAddressLength, int accountAddressLengthEncodedLength) =
                    ShortVectorEncoding.DecodeLength(GetShortVectorSpan(data, MessageHeader.Layout.HeaderLength));
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
                        GetShortVectorSpan(
                            data,
                            MessageHeader.Layout.HeaderLength + accountAddressLengthEncodedLength +
                            (accountAddressLength * PublicKey.PublicKeyLength) + PublicKey.PublicKeyLength));

                List<CompiledInstruction> instructions = new(instructionsLength);
                int instructionsOffset =
                    MessageHeader.Layout.HeaderLength + accountAddressLengthEncodedLength +
                    (accountAddressLength * PublicKey.PublicKeyLength) + PublicKey.PublicKeyLength +
                    instructionsLengthEncodedLength;
                ReadOnlySpan<byte> instructionsData = data[instructionsOffset..];

                int instructionsDataLength = 0;

                // Read the instructions in the message
                for (int i = 0; i < instructionsLength; i++)
                {
                    (CompiledInstruction compiledInstruction, int instructionLength) =
                        CompiledInstruction.Deserialize(instructionsData);
                    instructions.Add(compiledInstruction);
                    instructionsData = instructionsData[instructionLength..];
                    instructionsDataLength += instructionLength;
                }

                // Read the address table lookups
                int tableLookupOffset =
                    MessageHeader.Layout.HeaderLength + accountAddressLengthEncodedLength +
                    (accountAddressLength * PublicKey.PublicKeyLength) + PublicKey.PublicKeyLength +
                    instructionsLengthEncodedLength + instructionsDataLength;

                List<MessageAddressLookupTable> addressLookupTable = new();
                if (tableLookupOffset >= data.Length)
                {
                    return new VersionedMessage()
                    {
                        Version = version,
                        Header = new MessageHeader()
                        {
                            RequiredSignatures = numRequiredSignatures,
                            ReadOnlySignedAccounts = numReadOnlySignedAccounts,
                            ReadOnlyUnsignedAccounts = numReadOnlyUnsignedAccounts
                        },
                        RecentBlockhash = blockHash,
                        AccountKeys = accountKeys,
                        Instructions = instructions,
                        AddressLookupTable = addressLookupTable
                    };
                }

                ReadOnlySpan<byte> tableLookupData = data[tableLookupOffset..];

                (int addressLookupTableCount, int addressLookupTableEncodedCount) = ShortVectorEncoding.DecodeLength(tableLookupData);
                tableLookupData = tableLookupData[addressLookupTableEncodedCount..];

                for (int i = 0; i < addressLookupTableCount; i++)
                {
                    byte[] accountKeyBytes = tableLookupData.Slice(0, PublicKey.PublicKeyLength).ToArray();
                    PublicKey accountKey = new(accountKeyBytes);
                    tableLookupData = tableLookupData.Slice(PublicKey.PublicKeyLength);

                    (int writableIndexesLength, int writableIndexesEncodedLength) = ShortVectorEncoding.DecodeLength(tableLookupData);
                    List<byte> writableIndexes = tableLookupData.Slice(writableIndexesEncodedLength, writableIndexesLength).ToArray().ToList();
                    tableLookupData = tableLookupData.Slice(writableIndexesEncodedLength + writableIndexesLength);

                    (int readonlyIndexesLength, int readonlyIndexesEncodedLength) = ShortVectorEncoding.DecodeLength(tableLookupData);
                    List<byte> readonlyIndexes = tableLookupData.Slice(readonlyIndexesEncodedLength, readonlyIndexesLength).ToArray().ToList();
                    tableLookupData = tableLookupData.Slice(readonlyIndexesEncodedLength + readonlyIndexesLength);

                    addressLookupTable.Add(new MessageAddressLookupTable
                    {
                        AccountKey = accountKey,
                        WritableIndexes = writableIndexes.ToArray(),
                        ReadonlyIndexes = readonlyIndexes.ToArray()
                    });
                }

                return new VersionedMessage()
                {
                    Version = version,
                    Header = new MessageHeader()
                    {
                        RequiredSignatures = numRequiredSignatures,
                        ReadOnlySignedAccounts = numReadOnlySignedAccounts,
                        ReadOnlyUnsignedAccounts = numReadOnlyUnsignedAccounts
                    },
                    RecentBlockhash = blockHash,
                    AccountKeys = accountKeys,
                    Instructions = instructions,
                    AddressLookupTable = addressLookupTable
                };
            }

            /// <summary>
            /// Deserialize a compiled message encoded as base-64 into a Message object.
            /// </summary>
            /// <param name="data">The data to deserialize into the Message object.</param>
            /// <returns>The Transaction object.</returns>
            /// <exception cref="ArgumentNullException">Thrown when the given string is null.</exception>
            public static new VersionedMessage Deserialize(string data)
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

            /// <summary>
            /// Deserialize the message version
            /// </summary>
            /// <param name="serializedMessage"></param>
            /// <returns></returns>
            public static string DeserializeMessageVersion(byte[] serializedMessage)
            {
                byte prefix = serializedMessage[0];
                byte maskedPrefix = (byte)(prefix & VersionPrefixMask);

                // If the highest bit of the prefix is not set, the message is not versioned
                if (maskedPrefix == prefix)
                {
                    return "legacy";
                }

                // The lower 7 bits of the prefix indicate the message version
                return maskedPrefix.ToString();
            }

            /// <summary>
            /// Represents a version 0 message.
            /// </summary>
            public class MessageV0 : VersionedMessage
            {
                /// <summary>
                /// Initializes a version 0 message.
                /// </summary>
                public MessageV0()
                {
                    Version = 0;
                }
            }

            /// <summary>
            /// Represents a version 1 message.
            /// </summary>
            public class MessageV1 : VersionedMessage
            {
                /// <summary>
                /// Initializes a version 1 message.
                /// </summary>
                public MessageV1()
                {
                    Version = 1;
                }
            }

            /// <summary>
            /// Creates a TransactionConfigMask from a TransactionConfig object.
            /// </summary>
            /// <param name="config"></param>
            /// <returns></returns>
            public static TransactionConfigMask CreateMask(TransactionConfig config)
            {
                uint mask = 0;

                if (config.PriorityFee.HasValue)
                    mask |= TransactionConfigMask.PriorityFee;

                if (config.ComputeUnitLimit.HasValue)
                    mask |= TransactionConfigMask.ComputeUnitLimit;

                if (config.LoadedAccountsDataSizeLimit.HasValue)
                    mask |= TransactionConfigMask.LoadedAccountsDataSize;

                if (config.HeapSize.HasValue)
                    mask |= TransactionConfigMask.HeapSize;

                return new TransactionConfigMask(mask);
            }
        }

        /// <summary>
        /// Message Address Lookup table
        /// </summary>
        public class MessageAddressLookupTable
        {
            /// <summary>
            /// Account Key
            /// </summary>
            public PublicKey AccountKey { get; set; }

            /// <summary>
            /// Writable indexes
            /// </summary>
            public byte[] WritableIndexes { get; set; }

            /// <summary>
            /// Read only indexes
            /// </summary>
            public byte[] ReadonlyIndexes { get; set; }
        }

        /// <summary>
        /// Message Address Lookup table
        /// </summary>
        public static class AddressTableLookupUtils
        {
            /// <summary>
            /// Serialize the address table lookups
            /// </summary>
            /// <param name="addressLookupTable"></param>
            /// <returns></returns>
            public static byte[] SerializeAddressLookupTable(List<MessageAddressLookupTable> addressLookupTable)
            {
                addressLookupTable ??= new List<MessageAddressLookupTable>();

                MemoryStream buffer = new();

                var encodedAddressLookupTableLength = ShortVectorEncoding.EncodeLength(addressLookupTable.Count);
                buffer.Write(encodedAddressLookupTableLength, 0, encodedAddressLookupTableLength.Length);

                foreach (var lookup in addressLookupTable)
                {
                    // Write the Account Key
                    buffer.Write(lookup.AccountKey, 0, PublicKey.PublicKeyLength);

                    // Write the Writable Indexes
                    var encodedWritableIndexesLength = ShortVectorEncoding.EncodeLength(lookup.WritableIndexes.Length);
                    buffer.Write(encodedWritableIndexesLength, 0, encodedWritableIndexesLength.Length);
                    buffer.Write(lookup.WritableIndexes, 0, lookup.WritableIndexes.Length);

                    // Write the Readonly Indexes
                    var encodedReadonlyIndexesLength = ShortVectorEncoding.EncodeLength(lookup.ReadonlyIndexes.Length);
                    buffer.Write(encodedReadonlyIndexesLength, 0, encodedReadonlyIndexesLength.Length);
                    buffer.Write(lookup.ReadonlyIndexes, 0, lookup.ReadonlyIndexes.Length);
                }

                return buffer.ToArray();
            }
        }

        private static ReadOnlySpan<byte> GetShortVectorSpan(ReadOnlySpan<byte> data, int offset)
        {
            int remaining = data.Length - offset;
            return remaining <= ShortVectorEncoding.SpanLength
                ? data.Slice(offset, remaining)
                : data.Slice(offset, ShortVectorEncoding.SpanLength);
        }
    }
}
