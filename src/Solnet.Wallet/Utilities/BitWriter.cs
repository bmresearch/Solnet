using Solnet.Wallet.Bip39;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Solnet.Wallet.Utilities
{
    /// <summary>
    /// Implements a bit writer.
    /// </summary>
    internal class BitWriter
    {
        /// <summary>
        /// The values of the bit writer.
        /// </summary>
        private readonly List<bool> _values = new();

        /// <summary>
        /// The number of values.
        /// </summary>
        private int Count => _values.Count;

        /// <summary>
        /// Writes a value to the writer buffer.
        /// </summary>
        /// <param name="value">The value.</param>
        private void Write(bool value)
        {
            _values.Insert(Position, value);
            Position++;
        }

        /// <summary>
        /// Writes a byte array to the writer buffer.
        /// </summary>
        /// <param name="bytes">The byte array.</param>
        internal void Write(byte[] bytes)
        {
            Write(bytes, bytes.Length * 8);
        }

        /// <summary>
        /// Writes a byte array to the writer buffer.
        /// </summary>
        /// <param name="bytes">The byte array.</param>
        /// <param name="bitCount">The bit count.</param>
        public void Write(byte[] bytes, int bitCount)
        {
            bytes = SwapEndianBytes(bytes);
            BitArray array = new(bytes);
            _values.InsertRange(Position, array.OfType<bool>().Take(bitCount));
            Position += bitCount;
        }

        /// <summary>
        /// Gets the bit writer's buffer as a byte array.
        /// </summary>
        /// <returns>The byte array.</returns>
        public byte[] ToBytes()
        {
            BitArray array = ToBitArray();
            byte[] bytes = ToByteArray(array);
            bytes = SwapEndianBytes(bytes);
            return bytes;
        }

        /// <summary>
        /// Convert a bit array to a byte array.
        /// </summary>
        /// <param name="bits">The bit array to convert.</param>
        /// <returns>The byte array.</returns>
        private static byte[] ToByteArray(BitArray bits)
        {
            int arrayLength = bits.Length / 8;
            if (bits.Length % 8 != 0)
                arrayLength++;
            byte[] array = new byte[arrayLength];

            for (int i = 0; i < bits.Length; i++)
            {
                int b = i / 8;
                int offset = i % 8;
                array[b] |= bits.Get(i) ? (byte)(1 << offset) : (byte)0;
            }
            return array;
        }

        /// <summary>
        /// Gets the bit writer's buffer as a bit array.
        /// </summary>
        /// <returns>The bit array.</returns>
        public BitArray ToBitArray()
        {
            return new(_values.ToArray());
        }

        /// <summary>
        /// Gets the bit writer's buffer as an array of integers.
        /// </summary>
        /// <returns>The array of integers.</returns>
        public int[] ToIntegers()
        {
            BitArray array = new(_values.ToArray());
            return ToIntegers(array);
        }

        /// <summary>
        /// Swaps the endianness of the bytes.
        /// </summary>
        /// <param name="bytes">The bytes to swap.</param>
        /// <returns>The swapped byte array.</returns>
        private static byte[] SwapEndianBytes(IReadOnlyList<byte> bytes)
        {
            byte[] output = new byte[bytes.Count];
            for (int i = 0; i < output.Length; i++)
            {
                byte newByte = 0;
                for (int ib = 0; ib < 8; ib++)
                {
                    newByte += (byte)(((bytes[i] >> ib) & 1) << (7 - ib));
                }
                output[i] = newByte;
            }
            return output;
        }

        /// <summary>
        /// The current position of the bit writer.
        /// </summary>
        public int Position { get; set; }

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="bitArray"></param>
        /// <param name="bitCount"></param>
        public void Write(BitArray bitArray, int bitCount)
        {
            for (int i = 0; i < bitCount; i++)
            {
                Write(bitArray.Get(i));
            }
        }

        /// <summary>
        /// Convert the bit array to integers.
        /// </summary>
        /// <param name="bits">The bit array.</param>
        /// <returns>The int array.</returns>
        public static int[] ToIntegers(BitArray bits)
        {
            return
                bits
                    .OfType<bool>()
                    .Select((v, i) => new
                    {
                        Group = i / 11,
                        Value = v ? 1 << (10 - (i % 11)) : 0
                    })
                    .GroupBy(_ => _.Group, _ => _.Value)
                    .Select(g => g.Sum())
                    .ToArray();
        }

        /// <summary>
        /// Encode the writer as string.
        /// </summary>
        /// <returns>The string.</returns>
        public override string ToString()
        {
            StringBuilder builder = new(_values.Count);
            for (int i = 0; i < Count; i++)
            {
                if (i != 0 && i % 8 == 0)
                    builder.Append(' ');
                builder.Append(_values[i] ? "1" : "0");
            }
            return builder.ToString();
        }
    }
}