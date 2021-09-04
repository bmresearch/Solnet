using Solnet.Rpc.Models;
using System;

namespace Solnet.Rpc.Utilities
{
    /// <summary>
    /// Implements the short vector encoding used in Solana transaction <see cref="Message"/>s.
    /// </summary>
    internal static class ShortVectorEncoding
    {
        /// <summary>
        /// The length of the compact-u16 multi-byte encoding.
        /// </summary>
        internal const int SpanLength = 3;

        /// <summary>
        /// Encodes the number of account keys present in the transaction as a short vector, see remarks.
        /// <remarks>
        /// See the documentation for more information on this encoding:
        /// https://docs.solana.com/developing/programming-model/transactions#compact-array-format
        /// </remarks>
        /// </summary>
        /// <param name="len">The number of account keys present in the transaction.</param>
        /// <returns>The short vector encoded data.</returns>
        internal static byte[] EncodeLength(int len)
        {
            byte[] output = new byte[10];
            int remLen = len;
            int cursor = 0;

            for (; ; )
            {
                int elem = remLen & 0x7f;
                remLen >>= 7;
                if (remLen == 0)
                {
                    output[cursor] = (byte)elem;
                    break;
                }
                elem |= 0x80;
                output[cursor] = (byte)elem;
                cursor += 1;
            }

            byte[] bytes = new byte[cursor + 1];
            Array.Copy(output, 0, bytes, 0, cursor + 1);

            return bytes;
        }

        /// <summary>
        /// Decodes the number of account keys present in the transaction following a specific format.
        /// <remarks>
        /// See the documentation for more information on this encoding:
        /// https://docs.solana.com/developing/programming-model/transactions#compact-array-format
        /// </remarks>
        /// </summary>
        /// <param name="data">The short vector encoded data.</param>
        /// <returns>The number of account keys present in the transaction.</returns>
        internal static (int Value, int Length) DecodeLength(ReadOnlySpan<byte> data)
        {
            int len = 0;
            int size = 0;

            foreach (byte elem in data)
            {
                len |= (elem & 0x7f) << (size * 7);
                size += 1;

                if ((elem & 0x80) == 0) break;
            }
            return (len, size);
        }
    }
}