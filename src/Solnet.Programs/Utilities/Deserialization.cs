using Solnet.Wallet;
using System;
using System.Buffers.Binary;
using System.Diagnostics;
using System.Numerics;

namespace Solnet.Programs.Utilities
{
    /// <summary>
    /// Extension methods for deserialization of program data using <see cref="ReadOnlySpan{T}"/>.
    /// </summary>
    public static class Deserialization
    {
        /// <summary>
        /// Get a 8-bit unsigned integer from the span at the given offset.
        /// </summary>
        /// <param name="data">The span to get data from.</param>
        /// <param name="offset">The offset at which the 8-bit unsigned integer begins.</param>
        /// <returns>The 8-bit unsigned integer.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the offset is too big for the span.</exception>
        public static byte GetU8(this ReadOnlySpan<byte> data, int offset)
        {
            if (offset > data.Length - 1)
                throw new ArgumentOutOfRangeException(nameof(offset));
            return data[offset];
        }

        /// <summary>
        /// Get a 16-bit unsigned integer from the span at the given offset.
        /// </summary>
        /// <param name="data">The span to get data from.</param>
        /// <param name="offset">The offset at which the 16-bit unsigned integer begins.</param>
        /// <returns>The 16-bit unsigned integer.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the offset is too big for the span.</exception>
        public static ushort GetU16(this ReadOnlySpan<byte> data, int offset)
        {
            if (offset + 2 > data.Length)
                throw new ArgumentOutOfRangeException(nameof(offset));
            return BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2));
        }

        /// <summary>
        /// Get a 32-bit unsigned integer from the span at the given offset.
        /// </summary>
        /// <param name="data">The span to get data from.</param>
        /// <param name="offset">The offset at which the 32-bit unsigned integer begins.</param>
        /// <returns>The 32-bit unsigned integer.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the offset is too big for the span.</exception>
        public static uint GetU32(this ReadOnlySpan<byte> data, int offset)
        {
            if (offset + 4 > data.Length)
                throw new ArgumentOutOfRangeException(nameof(offset));
            return BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(offset, 4));
        }

        /// <summary>
        /// Get a 64-bit unsigned integer from the span at the given offset.
        /// </summary>
        /// <param name="data">The span to get data from.</param>
        /// <param name="offset">The offset at which the 64-bit unsigned integer begins.</param>
        /// <returns>The 64-bit unsigned integer.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the offset is too big for the span.</exception>
        public static ulong GetU64(this ReadOnlySpan<byte> data, int offset)
        {
            if (offset + 8 > data.Length)
                throw new ArgumentOutOfRangeException(nameof(offset));
            return BinaryPrimitives.ReadUInt64LittleEndian(data.Slice(offset, 8));
        }

        /// <summary>
        /// Get a 8-bit signed integer from the span at the given offset.
        /// </summary>
        /// <param name="data">The span to get data from.</param>
        /// <param name="offset">The offset at which the 8-bit signed integer begins.</param>
        /// <returns>The 8-bit signed integer.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the offset is too big for the span.</exception>
        public static sbyte GetS8(this ReadOnlySpan<byte> data, int offset)
        {
            if (offset > data.Length - 1)
                throw new ArgumentOutOfRangeException(nameof(offset));
            return (sbyte)data[offset];
        }

        /// <summary>
        /// Get a 16-bit signed integer from the span at the given offset.
        /// </summary>
        /// <param name="data">The span to get data from.</param>
        /// <param name="offset">The offset at which the 16-bit signed integer begins.</param>
        /// <returns>The 16-bit signed integer.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the offset is too big for the span.</exception>
        public static short GetS16(this ReadOnlySpan<byte> data, int offset)
        {
            if (offset + 2 > data.Length)
                throw new ArgumentOutOfRangeException(nameof(offset));
            return BinaryPrimitives.ReadInt16LittleEndian(data.Slice(offset, 2));
        }

        /// <summary>
        /// Get a 32-bit signed integer from the span at the given offset.
        /// </summary>
        /// <param name="data">The span to get data from.</param>
        /// <param name="offset">The offset at which the 32-bit signed integer begins.</param>
        /// <returns>The 32-bit signed integer.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the offset is too big for the span.</exception>
        public static int GetS32(this ReadOnlySpan<byte> data, int offset)
        {
            if (offset + 4 > data.Length)
                throw new ArgumentOutOfRangeException(nameof(offset));
            return BinaryPrimitives.ReadInt32LittleEndian(data.Slice(offset, 4));
        }

        /// <summary>
        /// Get a 64-bit signed integer from the span at the given offset.
        /// </summary>
        /// <param name="data">The span to get data from.</param>
        /// <param name="offset">The offset at which the 64-bit signed integer begins.</param>
        /// <returns>The 64-bit signed integer.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the offset is too big for the span.</exception>
        public static long GetS64(this ReadOnlySpan<byte> data, int offset)
        {
            if (offset + 8 > data.Length)
                throw new ArgumentOutOfRangeException(nameof(offset));
            return BinaryPrimitives.ReadInt64LittleEndian(data.Slice(offset, 8));
        }
        
        /// <summary>
        /// Get a span from the read-only span at the given offset with the given length.
        /// </summary>
        /// <param name="data">The span to get data from.</param>
        /// <param name="offset">The offset at which the desired span begins.</param>
        /// <param name="length">The desired length for the new span.</param>
        /// <returns>A <see cref="Span{T}"/> of bytes.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the offset is too big for the span.</exception>
        public static Span<byte> GetSpan(this ReadOnlySpan<byte> data, int offset, int length)
        {
            if (offset + length > data.Length)
                throw new ArgumentOutOfRangeException(nameof(offset));
            byte[] buffer = new byte[length];
            data.Slice(offset, length).CopyTo(buffer);
            return buffer;
        }

        /// <summary>
        /// Get a <see cref="PublicKey"/> encoded as a 32 byte array from the span at the given offset.
        /// </summary>
        /// <param name="data">The span to get data from.</param>
        /// <param name="offset">The offset at which the 32 byte array begins.</param>
        /// <returns>The <see cref="PublicKey"/> instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the offset is too big for the span.</exception>
        public static PublicKey GetPubKey(this ReadOnlySpan<byte> data, int offset)
        {
            if (offset + 32 > data.Length)
                throw new ArgumentOutOfRangeException(nameof(offset));
            return new PublicKey(data.Slice(offset, 32).ToArray());
        }

        /// <summary>
        /// Get an arbitrarily long number from the span at the given offset, specifying it's length in bytes.
        /// Optionally specify if it's signed and the endianness.
        /// </summary>
        /// <param name="data">The span to get data from.</param>
        /// <param name="offset">The offset at which the arbitrarily long number begins.</param>
        /// <param name="length">The byte-length of the arbitrarily long number.</param>
        /// <param name="isSigned">Whether the value uses signed encoding.</param>
        /// <param name="isBigEndian">Whether the value is in big-endian byte order.</param>
        /// <returns>The <see cref="BigInteger"/> instance that represents the value.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the offset is too big for the span.</exception>
        public static BigInteger GetBigInt(this ReadOnlySpan<byte> data, int offset, int length, 
            bool isSigned = false, bool isBigEndian = false)
        {
            if (offset + length > data.Length)
                throw new ArgumentOutOfRangeException(nameof(offset));
            return new BigInteger(data.Slice(offset, length), isSigned, isBigEndian);
        }

        /// <summary>
        /// Get a double-precision floating-point number from the span at the given offset.
        /// </summary>
        /// <param name="data">The span to get data from.</param>
        /// <param name="offset">The offset at which the arbitrarily long number begins.</param>
        /// <returns>The <see cref="double"/> instance that represents the value.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the offset is too big for the span.</exception>
        public static double GetDouble(this ReadOnlySpan<byte> data, int offset)
        {
            if (offset + 8 > data.Length)
                throw new ArgumentOutOfRangeException(nameof(offset));

            return BinaryPrimitives.ReadDoubleLittleEndian(data.Slice(offset, 8));
        }
        
        /// <summary>
        /// Get a single-precision floating-point number from the span at the given offset.
        /// </summary>
        /// <param name="data">The span to get data from.</param>
        /// <param name="offset">The offset at which the arbitrarily long number begins.</param>
        /// <returns>The <see cref="float"/> instance that represents the value.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the offset is too big for the span.</exception>
        public static float GetSingle(this ReadOnlySpan<byte> data, int offset)
        {
            if (offset + 4 > data.Length)
                throw new ArgumentOutOfRangeException(nameof(offset));

            return BinaryPrimitives.ReadSingleLittleEndian(data.Slice(offset, 4));
        }
    }
}