using Solnet.Programs.TokenSwap;
using Solnet.Rpc.Builders;
using Solnet.Rpc.Models;
using Solnet.Wallet;
using Solnet.Wallet.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Solnet.Programs
{
    /// <summary>
    /// Implements instruction decoder functionality.
    /// </summary>
    public static class InstructionDecoder
    {
        /// <summary>
        /// The dictionary which maps the program public keys to their decoding method.
        /// </summary>
        private static readonly Dictionary<string, DecodeMethodType> InstructionDictionary = new();

        /// <summary>
        /// The method type which is used to perform instruction decoding.
        /// </summary>
        public delegate DecodedInstruction DecodeMethodType(ReadOnlySpan<byte> data, IList<PublicKey> keys, byte[] keyIndices);

        /// <summary>
        /// Initialize the instruction decoder instance.
        /// </summary>
        static InstructionDecoder()
        {
            InstructionDictionary.Add(MemoProgram.ProgramIdKey, MemoProgram.Decode);
            InstructionDictionary.Add(MemoProgram.ProgramIdKeyV2, MemoProgram.Decode);
            InstructionDictionary.Add(SystemProgram.ProgramIdKey, SystemProgram.Decode);
            InstructionDictionary.Add(TokenProgram.ProgramIdKey, TokenProgram.Decode);
            InstructionDictionary.Add(TokenSwapProgram.TokenSwapProgramIdKey, TokenSwapProgram.Decode);
            InstructionDictionary.Add(AssociatedTokenAccountProgram.ProgramIdKey, AssociatedTokenAccountProgram.Decode);
            InstructionDictionary.Add(NameServiceProgram.ProgramIdKey, NameServiceProgram.Decode);
            InstructionDictionary.Add(SharedMemoryProgram.ProgramIdKey, SharedMemoryProgram.Decode);
            InstructionDictionary.Add(StakeProgram.ProgramIdKey, StakeProgram.Decode);
        }

        /// <summary>
        /// Register the public key of a program and it's method used for instruction decoding.
        /// </summary>
        /// <param name="programKey">The public key of the program to decode data from.</param>
        /// <param name="decodingMethod">The method which is called to perform instruction decoding for the program.</param>
        public static void Register(PublicKey programKey, DecodeMethodType decodingMethod)
        {
            InstructionDictionary.Add(programKey, decodingMethod);
        }

        /// <summary>
        /// Decodes the given instruction data for a given program key 
        /// </summary>
        /// <param name="programKey">The public key of the program to decode data from.</param>
        /// <param name="data">The instruction data to decode.</param>
        /// <param name="keys">The account keys present in the transaction.</param>
        /// <param name="keyIndices">The indices of the account keys for the instruction as they appear in the transaction.</param>
        /// <returns>The decoded instruction data.</returns>
        public static DecodedInstruction Decode(PublicKey programKey, ReadOnlySpan<byte> data, List<PublicKey> keys, byte[] keyIndices)
        {
            bool exists = InstructionDictionary.TryGetValue(programKey, out DecodeMethodType method);
            return !exists ? null : method?.Invoke(data, keys, keyIndices);
        }

        /// <summary>
        /// Decodes the instructions present in the given transaction and it's metadata information.
        /// </summary>
        /// <param name="txMetaInfo">The transaction metadata info object.</param>
        /// <returns>The decoded instructions data.</returns>
        public static List<DecodedInstruction> DecodeInstructions(TransactionMetaInfo txMetaInfo)
        {
            List<DecodedInstruction> decodedInstructions = new();

            for (int i = 0; i < txMetaInfo.Transaction.Message.Instructions.Length; i++)
            {
                DecodedInstruction decodedInstruction = null;
                InstructionInfo instructionInfo = txMetaInfo.Transaction.Message.Instructions[i];
                string programKey = txMetaInfo.Transaction.Message.AccountKeys[instructionInfo.ProgramIdIndex];
                bool registered = InstructionDictionary.TryGetValue(programKey, out DecodeMethodType method);

                if (!registered)
                {
                    decodedInstruction = AddUnknownInstruction(
                        instructionInfo, programKey, txMetaInfo.Transaction.Message.AccountKeys,
                        txMetaInfo.Transaction.Message.Instructions[i].Accounts);
                }
                else
                {
                    decodedInstruction = method.Invoke(
                                        Encoders.Base58.DecodeData(instructionInfo.Data),
                                        txMetaInfo.Transaction.Message.AccountKeys.Select(a => new PublicKey(a)).ToList(),
                                        instructionInfo.Accounts.Select(instr => (byte)instr).ToArray());
                }

                foreach (InnerInstruction innerInstruction in txMetaInfo.Meta.InnerInstructions)
                {
                    if (innerInstruction.Index != i) continue;

                    foreach (InstructionInfo innerInstructionInfo in innerInstruction.Instructions)
                    {
                        DecodedInstruction innerDecodedInstruction = null;
                        programKey = txMetaInfo.Transaction.Message.AccountKeys[innerInstructionInfo.ProgramIdIndex];
                        registered = InstructionDictionary.TryGetValue(programKey, out method);

                        if (!registered)
                        {
                            innerDecodedInstruction = AddUnknownInstruction(
                                innerInstructionInfo, programKey, txMetaInfo.Transaction.Message.AccountKeys,
                                txMetaInfo.Transaction.Message.Instructions[i].Accounts);
                        }
                        else
                        {
                            innerDecodedInstruction = method.Invoke(
                                Encoders.Base58.DecodeData(innerInstructionInfo.Data),
                                txMetaInfo.Transaction.Message.AccountKeys.Select(a => new PublicKey(a)).ToList(),
                                innerInstructionInfo.Accounts.Select(instr => (byte)instr).ToArray());
                        }
                        if (innerDecodedInstruction != null)
                            decodedInstruction.InnerInstructions.Add(innerDecodedInstruction);
                    }
                }
                if (decodedInstruction != null)
                    decodedInstructions.Add(decodedInstruction);
            }
            return decodedInstructions;
        }

        /// <summary>
        /// Decodes the instructions present in the given transaction and its metadata information.
        /// </summary>
        /// <param name="message">The message object.</param>
        /// <returns>The decoded instructions data.</returns>
        public static List<DecodedInstruction> DecodeInstructions(Message message)
        {
            List<DecodedInstruction> decodedInstructions = new();

            foreach (CompiledInstruction compiledInstruction in message.Instructions)
            {
                string programKey = message.AccountKeys[compiledInstruction.ProgramIdIndex];
                bool registered = InstructionDictionary.TryGetValue(programKey, out DecodeMethodType method);

                if (!registered)
                {
                    DecodedInstruction decodedInstruction = new()
                    {
                        InstructionName = "Unknown",
                        ProgramName = "Unknown",
                        Values = new Dictionary<string, object>
                        {
                            { "Data", Encoders.Base58.EncodeData(compiledInstruction.Data) }
                        },
                        InnerInstructions = new List<DecodedInstruction>(),
                        PublicKey = message.AccountKeys[compiledInstruction.ProgramIdIndex]
                    };
                    for (int i = 0; i < compiledInstruction.KeyIndices.Length; i++)
                    {
                        decodedInstruction.Values.Add($"Account {i + 1}", message.AccountKeys[i]);
                    }
                    decodedInstructions.Add(decodedInstruction);
                    continue;
                }

                decodedInstructions.Add(method.Invoke(
                    compiledInstruction.Data,
                    message.AccountKeys,
                    compiledInstruction.KeyIndices));
            }
            return decodedInstructions;
        }

        /// <summary>
        /// Adds an unknown instruction to the given list of decoded instructions, with the given instruction info.
        /// </summary>
        private static DecodedInstruction AddUnknownInstruction(
            InstructionInfo instructionInfo, string programKey, IReadOnlyList<string> keys, IReadOnlyList<int> keyIndices)
        {
            DecodedInstruction decodedInstruction = new()
            {
                Values = new Dictionary<string, object>
                {
                    {"Data", instructionInfo.Data}
                },
                InnerInstructions = new List<DecodedInstruction>(),
                InstructionName = "Unknown",
                ProgramName = "Unknown",
                PublicKey = new PublicKey(programKey)
            };
            for (int j = 0; j < keyIndices.Count; j++)
            {
                decodedInstruction.Values.Add($"Account {j + 1}", keys[keyIndices[j]]);
            }

            return decodedInstruction;
        }
    }
}