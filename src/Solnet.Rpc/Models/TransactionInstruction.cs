using Solnet.Rpc.Utilities;
using System;
using System.Collections.Generic;

namespace Solnet.Rpc.Models
{
    /// <summary>
    /// Represents a transaction instruction before being compiled into the transaction's message.
    /// </summary>
    public class TransactionInstruction
    {
        /// <summary>
        /// The program ID associated with the instruction.
        /// </summary>
        public byte[] ProgramId { get; init; }

        /// <summary>
        /// The keys associated with the instruction.
        /// </summary>
        public IList<AccountMeta> Keys { get; init; }

        /// <summary>
        /// The instruction-specific data.
        /// </summary>
        public byte[] Data { get; init; }
    }

    /// <summary>
    /// A compiled instruction within the transaction's message.
    /// </summary>
    public class CompiledInstruction
    {
        #region Layout

        /// <summary>
        /// Represents the layout of the <see cref="CompiledInstruction"/> encoded values.
        /// </summary>
        internal static class Layout
        {
            /// <summary>
            /// The offset at which the program's id index value begins.
            /// </summary>
            internal const int ProgramIdIndexOffset = 0;
        }

        #endregion

        /// <summary>
        /// The index of the program's key in the transaction's account keys.
        /// </summary>
        public byte ProgramIdIndex { get; init; }

        /// <summary>
        /// The <see cref="ShortVectorEncoding"/> encoded length representing the number of key indices.
        /// </summary>
        public byte[] KeyIndicesCount { get; init; }

        /// <summary>
        /// The indices of the account keys for the instruction as they appear in the transaction.
        /// </summary>
        public byte[] KeyIndices { get; init; }

        /// <summary>
        /// The <see cref="ShortVectorEncoding"/> encoded length representing the number of key indices.
        /// </summary>
        public byte[] DataLength { get; init; }

        /// <summary>
        /// The instruction data.
        /// </summary>
        public byte[] Data { get; init; }

        /// <summary>
        /// Get the length of the compiled instruction.
        /// </summary>
        /// <returns>The length.</returns>
        internal int Length()
        {
            return 1 + KeyIndicesCount.Length + KeyIndices.Length + DataLength.Length + Data.Length;
        }

        /// <summary>
        /// Attempts to deserialize a compiled instruction from the given data.
        /// </summary>
        /// <param name="data">The data to deserialize.</param>
        public static (CompiledInstruction Instruction, int Length) Deserialize(ReadOnlySpan<byte> data)
        {
            int instructionLength = 0;
            // Read the programId index
            byte programIdIndex = data[Layout.ProgramIdIndexOffset];
            instructionLength += 1; // ProgramIdIndex is zero

            // Read the number of keys for the instruction
            ReadOnlySpan<byte> encodedKeyIndicesLength =
                data.Slice(instructionLength, ShortVectorEncoding.SpanLength);
            (int keyIndicesLength, int keyIndicesLengthEncodedLength) =
                ShortVectorEncoding.DecodeLength(encodedKeyIndicesLength);
            instructionLength += keyIndicesLengthEncodedLength;

            // Read the key indices for the instruction accounts
            byte[] keyIndices = new byte[keyIndicesLength];
            for (int j = 0; j < keyIndicesLength; j++)
            {
                keyIndices[j] = data[instructionLength];
                instructionLength++;
            }

            // Read the length of the instruction's data
            ReadOnlySpan<byte> encodedDataLength =
                data.Length > instructionLength + ShortVectorEncoding.SpanLength ?
                    data.Slice(instructionLength, ShortVectorEncoding.SpanLength)
                    : data.Slice(instructionLength, data.Length - instructionLength);

            (int dataLength, int dataLengthEncodedLength) = ShortVectorEncoding.DecodeLength(encodedDataLength);
            instructionLength += dataLengthEncodedLength;

            // Read the instruction data
            byte[] instructionEncodedData = data.Slice(instructionLength, dataLength).ToArray();
            instructionLength += dataLength;

            return (Instruction: new CompiledInstruction
            {
                ProgramIdIndex = programIdIndex,
                KeyIndicesCount = encodedKeyIndicesLength[..keyIndicesLengthEncodedLength].ToArray(),
                KeyIndices = keyIndices,
                DataLength = encodedDataLength[..dataLengthEncodedLength].ToArray(),
                Data = instructionEncodedData
            }, Length: instructionLength);
        }
    }
}