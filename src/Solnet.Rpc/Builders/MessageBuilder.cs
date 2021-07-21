using Solnet.Rpc.Models;
using Solnet.Rpc.Utilities;
using Solnet.Wallet;
using Solnet.Wallet.Utilities;
using System;
using System.Collections.Generic;
using System.IO;

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
        private static readonly Base58Encoder Encoder = new();

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
        internal List<TransactionInstruction> Instructions { get; private set; }

        /// <summary>
        /// The hash of a recent block.
        /// </summary>
        internal string RecentBlockHash { get; set; }
        
        /// <summary>
        /// The nonce information to be used instead of the recent blockhash.
        /// </summary>
        internal NonceInformation NonceInformation { get; set; }

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
            Instructions = new List<TransactionInstruction>();
        }

        /// <summary>
        /// Add an instruction to the message.
        /// </summary>
        /// <param name="instruction">The instruction to add to the message.</param>
        /// <returns>The message builder, so instruction addition can be chained.</returns>
        internal MessageBuilder AddInstruction(TransactionInstruction instruction)
        {
            _accountKeysList.Add(instruction.Keys);
            _accountKeysList.Add(AccountMeta.ReadOnly(new PublicKey(instruction.ProgramId), false));
            Instructions.Add(instruction);
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal byte[] Build()
        {
            if (RecentBlockHash == null && NonceInformation == null)
                throw new Exception("recent block hash or nonce information is required");
            if (Instructions == null)
                throw new Exception("no instructions provided in the transaction");

            // In case the user specified nonce information, we'll use it.
            if (NonceInformation != null)
            {
                RecentBlockHash = NonceInformation.Nonce;
                _accountKeysList.Add(NonceInformation.Instruction.Keys);
                _accountKeysList.Add(AccountMeta.ReadOnly(new PublicKey(NonceInformation.Instruction.ProgramId), false));
                List<TransactionInstruction> newInstructions = new (){NonceInformation.Instruction};
                newInstructions.AddRange(Instructions);
                Instructions = newInstructions;
            }
            
            if (RecentBlockHash == null)
                throw new Exception("either recent block hash was not provided or nonce information is invalid");

            _messageHeader = new MessageHeader();

            List<AccountMeta> keysList = GetAccountKeys();
            byte[] accountAddressesLength = ShortVectorEncoding.EncodeLength(keysList.Count);
            int compiledInstructionsLength = 0;
            List<CompiledInstruction> compiledInstructions = new ();

            foreach (TransactionInstruction instruction in Instructions)
            {
                int keyCount = instruction.Keys.Count;
                byte[] keyIndices = new byte[keyCount];

                for (int i = 0; i < keyCount; i++)
                {
                    keyIndices[i] = (byte)FindAccountIndex(keysList, instruction.Keys[i].PublicKeyBytes);
                }

                CompiledInstruction compiledInstruction = new CompiledInstruction
                {
                    ProgramIdIndex = (byte)FindAccountIndex(keysList, instruction.ProgramId),
                    KeyIndicesCount = ShortVectorEncoding.EncodeLength(keyCount),
                    KeyIndices = keyIndices,
                    DataLength = ShortVectorEncoding.EncodeLength(instruction.Data.Length),
                    Data = instruction.Data
                };
                compiledInstructions.Add(compiledInstruction);
                compiledInstructionsLength += compiledInstruction.Length();
            }

            int accountKeysBufferSize = _accountKeysList.AccountList.Count * 32;
            MemoryStream accountKeysBuffer = new MemoryStream(accountKeysBufferSize);
            byte[] instructionsLength = ShortVectorEncoding.EncodeLength(compiledInstructions.Count);

            foreach (AccountMeta accountMeta in keysList)
            {
                accountKeysBuffer.Write(accountMeta.PublicKeyBytes);
                if (accountMeta.IsSigner)
                {
                    _messageHeader.RequiredSignatures += 1;
                    if (!accountMeta.IsWritable)
                        _messageHeader.ReadOnlySignedAccounts += 1;
                }
                else
                {
                    if (!accountMeta.IsWritable)
                        _messageHeader.ReadOnlyUnsignedAccounts += 1;
                }
            }

            #region Build Message Body
            
            int messageBufferSize = MessageHeader.HeaderLength + BlockHashLength + accountAddressesLength.Length +
                                    + instructionsLength.Length + compiledInstructionsLength + accountKeysBufferSize;
            MemoryStream buffer = new MemoryStream(messageBufferSize);
            byte[] messageHeaderBytes = _messageHeader.ToBytes();

            buffer.Write(messageHeaderBytes);
            buffer.Write(accountAddressesLength);
            buffer.Write(accountKeysBuffer.ToArray());
            buffer.Write(Encoder.DecodeData(RecentBlockHash));
            buffer.Write(instructionsLength);

            foreach (CompiledInstruction compiledInstruction in compiledInstructions)
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
            IList<AccountMeta> keysList = _accountKeysList.AccountList;
            int feePayerIndex = FindAccountIndex(keysList, FeePayer.PublicKey.KeyBytes);
            if (feePayerIndex == -1)
            {
                _accountKeysList.Add(AccountMeta.Writable(FeePayer.PublicKey, true));
            }
            else
            {
                keysList.RemoveAt(feePayerIndex);
            }

            List<AccountMeta> newList = new List<AccountMeta>
            {
                AccountMeta.Writable(FeePayer.PublicKey, true)
            };
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
        private static int FindAccountIndex(IList<AccountMeta> accountMetas, byte[] publicKey)
        {
            string encodedKey = Encoder.EncodeData(publicKey);
            for (int index = 0; index < accountMetas.Count; index++)
            {
                if (accountMetas[index].PublicKey == encodedKey) return index;
            }

            return -1;
        }
    }
}