using Sol.Unity.Rpc.Utilities;
using Sol.Unity.Wallet;
using Sol.Unity.Wallet.Utilities;
using System;
using System.Buffers.Binary;
using System.Diagnostics;
using System.IO;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;

namespace Sol.Unity.Programs.Utilities
{
    /// <summary>
    /// Extension methods for serialization of program data using <see cref="ReadOnlySpan{T}"/>.
    /// </summary>
    public static class Serialization
    {
        /// <summary>
        /// Write a 8-bit unsigned integer to the byte array at the given offset.
        /// </summary>
        /// <param name="data">The byte array to write data to.</param>
        /// <param name="value">The 8-bit unsigned integer value to write.</param>
        /// <param name="offset">The offset at which to write the 8-bit unsigned integer.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the offset is too big for the data array.</exception>
        public static void WriteU8(this byte[] data, byte value, int offset)
        {
            if (offset > data.Length - sizeof(byte))
                throw new ArgumentOutOfRangeException(nameof(offset));
            data[offset] = value;
        }
        /// <summary>
        /// Write a boolean to the byte array at the given offset.
        /// </summary>
        /// <param name="data">The byte array to write data to.</param>
        /// <param name="value">The boolean value to write.</param>
        /// <param name="offset">The offset at which to write the 8-bit unsigned integer.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the offset is too big for the data array.</exception>
        public static unsafe void WriteBool(this byte[] data, bool value, int offset)
        {
            if (offset > data.Length - sizeof(byte))
                throw new ArgumentOutOfRangeException(nameof(offset));
            data[offset] =  *((byte*)(&value));;
        }

        /// <summary>
        /// Write a 16-bit unsigned integer to the byte array at the given offset.
        /// </summary>
        /// <param name="data">The byte array to write data to.</param>
        /// <param name="value">The 16-bit unsigned integer value to write.</param>
        /// <param name="offset">The offset at which to write the 16-bit unsigned integer.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the offset is too big for the data array.</exception>
        public static void WriteU16(this byte[] data, ushort value, int offset)
        {
            if (offset + sizeof(ushort) > data.Length)
                throw new ArgumentOutOfRangeException(nameof(offset));
            BinaryPrimitives.WriteUInt16LittleEndian(data.AsSpan(offset, sizeof(ushort)), value);
        }

        /// <summary>
        /// Write a 32-bit unsigned integer to the byte array at the given offset.
        /// </summary>
        /// <param name="data">The byte array to write data to.</param>
        /// <param name="value">The 32-bit unsigned integer value to write.</param>
        /// <param name="offset">The offset at which to write the 32-bit unsigned integer.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the offset is too big for the data array.</exception>
        public static void WriteU32(this byte[] data, uint value, int offset)
        {
            if (offset + sizeof(uint) > data.Length)
                throw new ArgumentOutOfRangeException(nameof(offset));
            BinaryPrimitives.WriteUInt32LittleEndian(data.AsSpan(offset, sizeof(uint)), value);
        }

        /// <summary>
        /// Write a 64-bit unsigned integer to the byte array at the given offset.
        /// </summary>
        /// <param name="data">The byte array to write data to.</param>
        /// <param name="value">The 64-bit unsigned integer value to write.</param>
        /// <param name="offset">The offset at which to write the 64-bit unsigned integer.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the offset is too big for the data array.</exception>
        public static void WriteU64(this byte[] data, ulong value, int offset)
        {
            if (offset + sizeof(ulong) > data.Length)
                throw new ArgumentOutOfRangeException(nameof(offset));
            BinaryPrimitives.WriteUInt64LittleEndian(data.AsSpan(offset, sizeof(ulong)), value);
        }

        /// <summary>
        /// Write a 8-bit signed integer to the byte array at the given offset.
        /// </summary>
        /// <param name="data">The byte array to write data to.</param>
        /// <param name="value">The 8-bit signed integer value to write.</param>
        /// <param name="offset">The offset at which to write the 8-bit signed integer.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the offset is too big for the data array.</exception>
        public static void WriteS8(this byte[] data, sbyte value, int offset)
        {
            if (offset > data.Length - sizeof(sbyte))
                throw new ArgumentOutOfRangeException(nameof(offset));
            data[offset] = (byte)value;
        }

        /// <summary>
        /// Write a 16-bit signed integer to the byte array at the given offset.
        /// </summary>
        /// <param name="data">The byte array to write data to.</param>
        /// <param name="value">The 16-bit signed integer value to write.</param>
        /// <param name="offset">The offset at which to write the 16-bit signed integer.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the offset is too big for the data array.</exception>
        public static void WriteS16(this byte[] data, short value, int offset)
        {
            if (offset + sizeof(short) > data.Length)
                throw new ArgumentOutOfRangeException(nameof(offset));
            BinaryPrimitives.WriteInt16LittleEndian(data.AsSpan(offset, sizeof(short)), value);
        }

        /// <summary>
        /// Write a 32-bit signed integer to the byte array at the given offset.
        /// </summary>
        /// <param name="data">The byte array to write data to.</param>
        /// <param name="value">The 32-bit signed integer value to write.</param>
        /// <param name="offset">The offset at which to write the 32-bit signed integer.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the offset is too big for the data array.</exception>
        public static void WriteS32(this byte[] data, int value, int offset)
        {
            if (offset + sizeof(int) > data.Length)
                throw new ArgumentOutOfRangeException(nameof(offset));
            BinaryPrimitives.WriteInt32LittleEndian(data.AsSpan(offset, sizeof(int)), value);
        }

        /// <summary>
        /// Write a 64-bit signed integer to the byte array at the given offset.
        /// </summary>
        /// <param name="data">The byte array to write data to.</param>
        /// <param name="value">The 64-bit signed integer value to write.</param>
        /// <param name="offset">The offset at which to write the 64-bit signed integer.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the offset is too big for the data array.</exception>
        public static void WriteS64(this byte[] data, long value, int offset)
        {
            if (offset + sizeof(long) > data.Length)
                throw new ArgumentOutOfRangeException(nameof(offset));
            BinaryPrimitives.WriteInt64LittleEndian(data.AsSpan(offset, sizeof(long)), value);
        }

        /// <summary>
        /// Write a span of bytes to the byte array at the given offset.
        /// </summary>
        /// <param name="data">The byte array to write data to.</param>
        /// <param name="span">The <see cref="ReadOnlySpan{T}"/> to write.</param>
        /// <param name="offset">The offset at which to write the <see cref="ReadOnlySpan{T}"/>.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the offset is too big for the data array.</exception>
        public static void WriteSpan(this byte[] data, ReadOnlySpan<byte> span, int offset)
        {
            if (offset + span.Length > data.Length)
                throw new ArgumentOutOfRangeException(nameof(offset));
            span.CopyTo(data.AsSpan(offset, span.Length));
        }

        /// <summary>
        /// Write a <see cref="PublicKey"/> encoded as a 32 byte array to the byte array at the given offset.
        /// </summary>
        /// <param name="data">The byte array to write data to.</param>
        /// <param name="publicKey">The <see cref="PublicKey"/> to write.</param>
        /// <param name="offset">The offset at which to write the <see cref="PublicKey"/>.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the offset is too big for the data array.</exception>
        public static void WritePubKey(this byte[] data, PublicKey publicKey, int offset)
        {
            if (offset + publicKey.KeyBytes.Length > data.Length)
                throw new ArgumentOutOfRangeException(nameof(offset));
            publicKey.KeyBytes.AsSpan().CopyTo(data.AsSpan(offset, publicKey.KeyBytes.Length));
        }

        /// <summary>
        /// Write an arbitrarily long number to the byte array at the given offset, specifying it's length in bytes.
        /// Optionally specify if it's signed and the endianness.
        /// </summary>
        /// <param name="data">The byte array to write data to.</param>
        /// <param name="bigInteger">The <see cref="BigInteger"/> to write.</param>
        /// <param name="offset">The offset at which to write the <see cref="BigInteger"/>.</param>
        /// <param name="length">The length in bytes.</param>
        /// <param name="isUnsigned">Whether the value does not use signed encoding.</param>
        /// <param name="isBigEndian">Whether the value is in big-endian byte order.</param>
        /// <returns>An integer representing the number of bytes written to the byte array.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the offset is too big for the data array.</exception>
        public static int WriteBigInt(this byte[] data, BigInteger bigInteger, int offset, int length,
            bool isUnsigned = false, bool isBigEndian = false)
        {
            int byteCount = bigInteger.ToByteArray().Length;
            if (byteCount > length) throw new ArgumentOutOfRangeException($"BigInt too big.");
            if (length + offset > data.Length) throw new ArgumentOutOfRangeException(nameof(offset));
            
            var bigIntegerSpan = new Span<byte>(bigInteger.ToByteArray());
            var dataSpan = data.AsSpan(offset, byteCount);
            bigIntegerSpan.CopyTo(dataSpan);
            
            if(!isUnsigned && bigInteger.Sign < 0)
            {
                while (byteCount < length) data[offset + byteCount++] = 0xFF;
            }

            return byteCount;
        }

        /// <summary>
        /// Write a double-precision floating-point value to the byte array at the given offset.
        /// </summary>
        /// <param name="data">The byte array to write data to.</param>
        /// <param name="value">The <see cref="double"/> to write.</param>
        /// <param name="offset">The offset at which to write the <see cref="double"/>.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the offset is too big for the data array.</exception>
        public static void WriteDouble(this byte[] data, double value, int offset)
        {
            if (offset + sizeof(double) > data.Length)
                throw new ArgumentOutOfRangeException(nameof(offset));

            var valueBytes = BitConverter.GetBytes(value);
            var valueSpan = new Span<byte>(valueBytes);
            var dataSpan = data.AsSpan(offset, sizeof(double));
            valueSpan.CopyTo(dataSpan);
        }

        /// <summary>
        /// Write a single-precision floating-point value to the byte array at the given offset.
        /// </summary>
        /// <param name="data">The byte array to write data to.</param>
        /// <param name="value">The <see cref="float"/> to write.</param>
        /// <param name="offset">The offset at which to write the <see cref="float"/>.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the offset is too big for the data array.</exception>
        public static void WriteSingle(this byte[] data, float value, int offset)
        {
            if (offset + sizeof(float) > data.Length)
                throw new ArgumentOutOfRangeException(nameof(offset));

            var valueBytes = BitConverter.GetBytes(value);
            var valueSpan = new Span<byte>(valueBytes);
            var dataSpan = data.AsSpan(offset, sizeof(float));
            valueSpan.CopyTo(dataSpan);
        }

        /// <summary>
        /// Write a UTF8 string value to the byte array at the given offset.
        /// </summary>
        /// <param name="data">The byte array to write data to.</param>
        /// <param name="value">The <see cref="string"/> to write.</param>
        /// <param name="offset">The offset at which to write the <see cref="string"/>.</param>
        /// <returns>Returns the number of bytes written.</returns>
        public static int WriteBorshString(this byte[] data, string value, int offset)
        {
            byte[] stringBytes = Encoding.UTF8.GetBytes(value);

            if(offset + sizeof(uint) + stringBytes.Length > data.Length)
                throw new ArgumentOutOfRangeException(nameof(offset));

            data.WriteU32((uint)stringBytes.Length, offset);
            data.WriteSpan(stringBytes, offset + sizeof(uint));

            return stringBytes.Length + sizeof(uint);
        }

        /// <summary>
        /// Encodes a string for a transaction
        /// </summary>
        /// <param name="data"> the string to be encoded</param>
        /// <returns></returns>
        public static byte[] EncodeBincodeString(string data)
        {
            byte[] stringBytes = Encoding.UTF8.GetBytes(data);
          
            byte[] encoded = new byte[stringBytes.Length+sizeof(ulong)];

            encoded.WriteU64((ulong)stringBytes.Length, 0);
            encoded.WriteSpan(stringBytes, 8);
          
            return encoded;
        }
    }
}