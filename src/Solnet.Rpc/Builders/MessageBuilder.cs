using Solnet.Rpc.Models;
using Solnet.Rpc.Utilities;
using Solnet.Wallet;
using Solnet.Wallet.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Solnet.Rpc.Builders
{
    /// <summary>
    /// A compiled instruction within the message.
    /// </summary>
    public class MessageBuilder
    {

        /// <summary>
        /// The length of the block hash.
        /// </summary>
        private const int BlockHashLength = 32;

        /// <summary>
        /// The message header.
        /// </summary>
        private MessageHeader _messageHeader;

        /// <summary>
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
        internal PublicKey FeePayer { get; set; }

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
        /// Builds the message into the wire format.
        /// </summary>
        /// <returns>The encoded message.</returns>
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
                _accountKeysList.Add(AccountMeta.ReadOnly(new PublicKey(NonceInformation.Instruction.ProgramId),
                    false));
                List<TransactionInstruction> newInstructions = new() { NonceInformation.Instruction };
                newInstructions.AddRange(Instructions);
                Instructions = newInstructions;
            }

            _messageHeader = new MessageHeader();

            List<AccountMeta> keysList = GetAccountKeys();
            byte[] accountAddressesLength = ShortVectorEncoding.EncodeLength(keysList.Count);
            int compiledInstructionsLength = 0;
            List<CompiledInstruction> compiledInstructions = new();

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

            int messageBufferSize = MessageHeader.Layout.HeaderLength + BlockHashLength +
                                    accountAddressesLength.Length +
                                    +instructionsLength.Length + compiledInstructionsLength + accountKeysBufferSize;
            MemoryStream buffer = new MemoryStream(messageBufferSize);
            byte[] messageHeaderBytes = _messageHeader.ToBytes();

            buffer.Write(messageHeaderBytes);
            buffer.Write(accountAddressesLength);
            buffer.Write(accountKeysBuffer.ToArray());
            buffer.Write(Encoders.Base58.DecodeData(RecentBlockHash));
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
        /// Gets the keys for the accounts present in the message.
        /// </summary>
        /// <returns>The list of <see cref="AccountMeta"/>.</returns>
        private List<AccountMeta> GetAccountKeys()
        {
            List<AccountMeta> newList = new();
            IList<AccountMeta> keysList = _accountKeysList.AccountList;
            int feePayerIndex = FindAccountIndex(keysList, FeePayer.KeyBytes);

            if (feePayerIndex == -1)
            {
                newList.Add(AccountMeta.Writable(FeePayer, true));
            }
            else
            {
                keysList.RemoveAt(feePayerIndex);
                newList.Add(AccountMeta.Writable(FeePayer, true));
            }

            newList.AddRange(keysList);

            return newList;
        }

        /// <summary>
        /// Finds the index of the given public key in the accounts list.
        /// </summary>
        /// <param name="accountMetas">The <see cref="AccountMeta"/>.</param>
        /// <param name="publicKey">The public key.</param>
        /// <returns>The index of the</returns>
        private static int FindAccountIndex(IList<AccountMeta> accountMetas, byte[] publicKey)
        {
            string encodedKey = Encoders.Base58.EncodeData(publicKey);
            for (int index = 0; index < accountMetas.Count; index++)
            {
                if (accountMetas[index].PublicKey == encodedKey) return index;
            }

            return -1;
        }
    }
}