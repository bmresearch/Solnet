using Solnet.Rpc.Models;
using Solnet.Rpc.Utilities;
using Solnet.Wallet;
using Solnet.Wallet.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static Solnet.Rpc.Models.Message;

namespace Solnet.Rpc.Builders
{
    /// <summary>
    /// A compiled instruction within the message.
    /// </summary>
    public class VersionedMessageBuilder : MessageBuilder
    {
        /// <summary>
        /// The version to encode in the message prefix.
        /// </summary>
        public byte Version { get; set; }

        /// <summary>
        /// Address Table Lookups
        /// </summary>
        public List<MessageAddressTableLookup> AddressTableLookups { get; set; }

        /// <summary>
        /// Transaction Config
        /// </summary>

        private TransactionConfig _transactionConfig = new();

        /// <summary>
        /// Transaction Config
        /// </summary>
        public override TransactionConfig TransactionConfig
        {
            get => _transactionConfig;
            set => _transactionConfig = value;
        }

        /// <summary>
        /// Account Keys
        /// </summary>
        public IList<PublicKey> AccountKeys { get; internal set; }

        /// <summary>
        /// Builds the message into the wire format.
        /// </summary>
        /// <returns>The encoded message.</returns>
        /// <exception cref="Exception"></exception>
        /// <exception cref="NotSupportedException"></exception>
        internal override byte[] Build()
        {
            if (RecentBlockHash == null && NonceInformation == null)
                throw new Exception("recent block hash or nonce information is required");

            if (Instructions == null)
                throw new Exception("no instructions provided in the transaction");

            if (NonceInformation != null)
            {
                RecentBlockHash = NonceInformation.Nonce;

                _accountKeysList.Add(NonceInformation.Instruction.Keys);

                _accountKeysList.Add(
                    AccountMeta.ReadOnly(
                        new PublicKey(NonceInformation.Instruction.ProgramId), false));

                List<TransactionInstruction> newInstructions = new() { NonceInformation.Instruction };

                newInstructions.AddRange(Instructions);

                Instructions = newInstructions;
            }

            switch (Version)
            {
                case 0:
                    return BuildV0();

                case 1:
                    return BuildV1();

                default:
                    throw new NotSupportedException(
                        $"Unsupported version {Version}");
            }
        }
        private byte[] BuildV0()
        {
            _messageHeader = new MessageHeader();

            List<AccountMeta> keysList = GetAccountKeys();

            List<CompiledInstruction> compiledInstructions = new();

            foreach (TransactionInstruction instruction in Instructions)
            {
                int keyCount = instruction.Keys.Count;

                byte[] keyIndices = new byte[keyCount];

                if (instruction is VersionedTransactionInstruction vtx)
                {
                    keyIndices = vtx.KeyIndices;
                }
                else
                {
                    for (int i = 0; i < keyCount; i++)
                    {
                        keyIndices[i] = FindAccountIndex(keysList, instruction.Keys[i].PublicKey);
                    }
                }

                compiledInstructions.Add(
                    new CompiledInstruction
                    {
                        ProgramIdIndex = FindAccountIndex(keysList,instruction.ProgramId),

                        KeyIndicesCount = ShortVectorEncoding.EncodeLength(keyIndices.Length),

                        KeyIndices = keyIndices,

                        DataLength = ShortVectorEncoding.EncodeLength(instruction.Data.Length),

                        Data = instruction.Data
                    });
            }

            List<PublicKey> accountKeys = new(keysList.Count);

            foreach (AccountMeta accountMeta in keysList)
            {
                accountKeys.Add(new PublicKey(accountMeta.PublicKey));

                if (accountMeta.IsSigner)
                {
                    _messageHeader.RequiredSignatures++;

                    if (!accountMeta.IsWritable)
                        _messageHeader.ReadOnlySignedAccounts++;
                }
                else
                {
                    if (!accountMeta.IsWritable)
                        _messageHeader.ReadOnlyUnsignedAccounts++;
                }
            }

            VersionedMessage.MessageV0 message =
                new()
                {
                    Version = 0,
                    Header = _messageHeader,
                    RecentBlockhash = RecentBlockHash,
                    AccountKeys = accountKeys,
                    Instructions = compiledInstructions,
                    AddressTableLookups = AddressTableLookups
                };

            return message.SerializeV0();
        }
        private byte[] BuildV1()
        {
            _messageHeader = new MessageHeader();

            List<AccountMeta> keysList = GetAccountKeys();

            List<CompiledInstruction> compiledInstructions = new();

            foreach (TransactionInstruction instruction in Instructions)
            {
                int keyCount = instruction.Keys.Count;

                byte[] keyIndices = new byte[keyCount];

                if (instruction is VersionedTransactionInstruction vtx)
                {
                    keyIndices = vtx.KeyIndices;
                }
                else
                {
                    for (int i = 0; i < keyCount; i++)
                    {
                        keyIndices[i] =
                            FindAccountIndex(
                                keysList,
                                instruction.Keys[i].PublicKey);
                    }
                }

                compiledInstructions.Add(
                    new CompiledInstruction
                    {
                        ProgramIdIndex = FindAccountIndex(keysList, instruction.ProgramId),

                        KeyIndices = keyIndices,
                        KeyIndicesCount = ShortVectorEncoding.EncodeLength(keyIndices.Length),

                        Data = instruction.Data,
                        DataLength = ShortVectorEncoding.EncodeLength(instruction.Data.Length)
                    });
            }

            List<PublicKey> accountKeys = new(keysList.Count);

            foreach (AccountMeta accountMeta in keysList)
            {
                accountKeys.Add(new PublicKey(accountMeta.PublicKey));

                if (accountMeta.IsSigner)
                {
                    _messageHeader.RequiredSignatures++;

                    if (!accountMeta.IsWritable)
                        _messageHeader.ReadOnlySignedAccounts++;
                }
                else
                {
                    if (!accountMeta.IsWritable)
                        _messageHeader.ReadOnlyUnsignedAccounts++;
                }
            }

            VersionedMessage.MessageV1 message =
            new()
            {
                Version = 1,
                Header = _messageHeader,
                TransactionConfig = TransactionConfig,
                RecentBlockhash = RecentBlockHash,
                AccountKeys = accountKeys,
                Instructions = compiledInstructions
            };

            return message.SerializeV1();
        }
    }
}