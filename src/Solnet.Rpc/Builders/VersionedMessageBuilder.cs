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
        /// Address Table Lookups
        /// </summary>
        public List<MessageAddressTableLookup> AddressTableLookups { get; set; }
        /// <summary>
        /// Account Keys
        /// </summary>
        public IList<PublicKey> AccountKeys { get; internal set; }

        /// <summary>
        /// Builds the message into the wire format.
        /// </summary>
        /// <returns>The encoded message.</returns>
        internal override byte[] Build()
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

                if (instruction.GetType() == typeof(VersionedTransactionInstruction))
                {
                    keyIndices = ((VersionedTransactionInstruction)instruction).KeyIndices;
                }
                else
                {
                    for (int i = 0; i < keyCount; i++)
                    {
                        keyIndices[i] = FindAccountIndex(keysList, instruction.Keys[i].PublicKey);
                    }
                }

                CompiledInstruction compiledInstruction = new()
                {
                    ProgramIdIndex = FindAccountIndex(keysList, instruction.ProgramId),
                    KeyIndicesCount = ShortVectorEncoding.EncodeLength(keyIndices.Length),
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
                accountKeysBuffer.Write(accountMeta.PublicKeyBytes, 0, accountMeta.PublicKeyBytes.Length);
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

            buffer.Write(new byte[] { 128 }, 0, 1);
            buffer.Write(messageHeaderBytes, 0, messageHeaderBytes.Length);
            buffer.Write(accountAddressesLength, 0, accountAddressesLength.Length);
            buffer.Write(accountKeysBuffer.ToArray(), 0, accountKeysBuffer.ToArray().Length);
            var encodedRecentBlockHash = Encoders.Base58.DecodeData(RecentBlockHash);
            buffer.Write(encodedRecentBlockHash, 0, encodedRecentBlockHash.Length);
            buffer.Write(instructionsLength, 0, instructionsLength.Length);

            foreach (CompiledInstruction compiledInstruction in compiledInstructions)
            {
                buffer.WriteByte(compiledInstruction.ProgramIdIndex);
                buffer.Write(compiledInstruction.KeyIndicesCount, 0, compiledInstruction.KeyIndicesCount.Length);
                buffer.Write(compiledInstruction.KeyIndices, 0, compiledInstruction.KeyIndices.Length);
                buffer.Write(compiledInstruction.DataLength, 0, compiledInstruction.DataLength.Length);
                buffer.Write(compiledInstruction.Data, 0, compiledInstruction.Data.Length);
            }

            #endregion

            var serializeAddressTableLookups = AddressTableLookupUtils.SerializeAddressTableLookups(AddressTableLookups);
            buffer.Write(serializeAddressTableLookups, 0, serializeAddressTableLookups.Length);

            return buffer.ToArray();
        }
    }
}