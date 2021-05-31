using System;
using System.Collections.Generic;
using System.IO;
using NBitcoin.DataEncoders;
using Solnet.Rpc.Models;
using Solnet.Rpc.Utilities;
using Solnet.Wallet;

namespace Solnet.Rpc.Builders
{
    /// <summary>
    /// The message builder.
    /// </summary>
    internal class MessageBuilder
    {
        /// <summary>
        /// The base58 encoder instance.
        /// </summary>
        private static readonly Base58Encoder Encoder = new ();

        /// <summary>
        /// The message header.
        /// </summary>
        private class MessageHeader
        {
            /// <summary>
            /// The message header length.
            /// </summary>
            internal const int HeaderLength = 3;

            /// <summary>
            /// The number of required signatures.
            /// </summary>
            internal byte RequiredSignatures { get; set; }

            /// <summary>
            /// The number of read-only signed accounts.
            /// </summary>
            internal byte ReadOnlySignedAccounts { get; set; }
            
            /// <summary>
            /// The number of read-only non-signed accounts.
            /// </summary>
            internal byte ReadOnlyUnsignedAccounts { get; set; }

            /// <summary>
            /// Convert the message header to byte array format.
            /// </summary>
            /// <returns>The byte array.</returns>
            internal byte[] ToBytes()
            {
                return new[] {RequiredSignatures, ReadOnlySignedAccounts, ReadOnlyUnsignedAccounts};
            }
        }

        /// <summary>
        /// A compiled instruction within the message.
        /// </summary>
        private class CompiledInstruction
        {
            internal byte ProgramIdIndex { get; init; }

            internal byte[] KeyIndicesCount { get; init; }

            internal byte[] KeyIndices { get; init; }
            
            internal byte[] DataLength { get; init; }
            
            internal byte[] Data { get; init; }

            /// <summary>
            /// Get the length of the compiled instruction.
            /// </summary>
            /// <returns>The length.</returns>
            internal int Length()
            {
                return 1 + KeyIndicesCount.Length + KeyIndices.Length + DataLength.Length + Data.Length;
            }
        }

        /// <summary>
        /// The length of the block hash.
        /// </summary>
        private const int BlockHashLength = 32;

        /// <summary>
        /// The message header.
        /// </summary>
        private MessageHeader _messageHeader;

        /// <summary>with read-write accounts first and read-only accounts following.
        /// The account keys list.
        /// </summary>
        private readonly AccountKeysList _accountKeysList;

        /// <summary>
        /// The list of instructions contained within this transaction.
        /// </summary>
        private readonly List<TransactionInstruction> _instructions;
        
        /// <summary>
        /// The hash of a recent block.
        /// </summary>
        internal string RecentBlockHash { get; set; }

        /// <summary>
        /// The transaction fee payer.
        /// </summary>
        internal Account FeePayer { get; set; }

        /// <summary>
        /// Initialize the message builder.
        /// </summary>
        internal MessageBuilder()
        {
            _accountKeysList = new AccountKeysList();
            _instructions = new List<TransactionInstruction>();
        }

        /// <summary>
        /// Add an instruction to the message.
        /// </summary>
        /// <param name="instruction">The instruction to add to the message.</param>
        /// <returns>The message builder, so instruction addition can be chained.</returns>
        internal MessageBuilder AddInstruction(TransactionInstruction instruction)
        {
            _accountKeysList.Add(instruction.Keys);
            _accountKeysList.Add(new AccountMeta(instruction.ProgramId, false, false));
            _instructions.Add(instruction);

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal byte[] Build()
        {
            if (RecentBlockHash == null) 
                // werent we supposed to fetch it from the api in this case?
                throw new Exception("recent block hash is required");
            if (_instructions == null) 
                throw new Exception("no instructions provided in the transaction");

            _messageHeader = new MessageHeader();
            
            var keysList = GetAccountKeys();
            var accountAddressesLength = ShortVectorEncoding.EncodeLength(keysList.Count);
            var compiledInstructionsLength = 0;
            var compiledInstructions = new List<CompiledInstruction>();

            foreach (var instruction in _instructions)
            {
                var keyCount = instruction.Keys.Count;
                var keyIndices = new byte[keyCount];

                for (var i = 0; i < keyCount; i++)
                {
                    keyIndices[i] = (byte) FindAccountIndex(keysList, instruction.Keys[i].PublicKey);
                }

                var compiledInstruction = new CompiledInstruction
                {
                    ProgramIdIndex = (byte) FindAccountIndex(keysList, instruction.ProgramId),
                    KeyIndicesCount = ShortVectorEncoding.EncodeLength(keyCount),
                    KeyIndices = keyIndices,
                    DataLength = ShortVectorEncoding.EncodeLength(instruction.Data.Length),
                    Data = instruction.Data
                };
                compiledInstructions.Add(compiledInstruction);
                compiledInstructionsLength += compiledInstruction.Length();
            }


            var accountKeysBufferSize = _accountKeysList.AccountList.Count * 32;
            var accountKeysBuffer = new MemoryStream(accountKeysBufferSize);
            var instructionsLength = ShortVectorEncoding.EncodeLength(compiledInstructions.Count);
            
            foreach (var accountMeta in keysList)
            {
                accountKeysBuffer.Write(accountMeta.PublicKey);
                if (accountMeta.Signer)
                {
                    _messageHeader.RequiredSignatures += 1;
                    if (!accountMeta.Writable)
                        _messageHeader.ReadOnlySignedAccounts += 1;
                }
                else 
                {
                    if (!accountMeta.Writable)
                        _messageHeader.ReadOnlyUnsignedAccounts += 1;
                }
            }

            #region Build Message Body

            
            var messageBufferSize = MessageHeader.HeaderLength + BlockHashLength + accountAddressesLength.Length +
                                    + instructionsLength.Length + compiledInstructionsLength + accountKeysBufferSize;
            var buffer = new MemoryStream(messageBufferSize);
            var messageHeaderBytes = _messageHeader.ToBytes();

            buffer.Write(messageHeaderBytes);
            buffer.Write(accountAddressesLength);
            buffer.Write(accountKeysBuffer.ToArray());
            buffer.Write(Encoder.DecodeData(RecentBlockHash));
            buffer.Write(instructionsLength);
            
            foreach (var compiledInstruction in compiledInstructions)
            {
                buffer.WriteByte(compiledInstruction.ProgramIdIndex);
                buffer.Write(compiledInstruction.KeyIndicesCount);
                buffer.Write(compiledInstruction.KeyIndices);
                buffer.Write(compiledInstruction.DataLength);
                buffer.Write(compiledInstruction.Data);
            }

            #endregion
            
            return buffer.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private List<AccountMeta> GetAccountKeys()
        {
            var keysList = _accountKeysList.AccountList;
            var feePayerIndex = FindAccountIndex(keysList, FeePayer.PublicKey);

            var newList = new List<AccountMeta>
            {
                new (keysList[feePayerIndex].PublicKey, true, true)
            };
            keysList.RemoveAt(feePayerIndex);
            newList.AddRange(keysList);
            
            return newList;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="accountMetas"></param>
        /// <param name="publicKey"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private int FindAccountIndex(IList<AccountMeta> accountMetas, byte[] publicKey)
        {
            for (var index = 0; index < accountMetas.Count; index++)
            {
                if (Encoder.EncodeData(accountMetas[index].PublicKey) == Encoder.EncodeData(publicKey)) return index;
            }

            throw new Exception($"could not find account index for public key: {Encoder.EncodeData(publicKey)}");
        }
    }
}