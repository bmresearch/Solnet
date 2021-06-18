using System;
using System.Collections.Generic;

namespace Solnet.Wallet.Utilities
{
    /// <summary>
    /// Implements a big endian buffer.
    /// </summary>
    public class BigEndianBuffer
    {
        /// <summary>
        /// The internal list of bytes.
        /// </summary>
        private readonly List<byte> _bytes = new();

        /// <summary>
        /// Write an unsigned integer to the buffer.
        /// </summary>
        /// <param name="i">The unsigned integer.</param>
        public void WriteUInt(uint i)
        {
            _bytes.Add((byte)((i >> 0x18) & 0xff));
            _bytes.Add((byte)((i >> 0x10) & 0xff));
            _bytes.Add((byte)((i >> 8) & 0xff));
            _bytes.Add((byte)(i & 0xff));
        }

        /// <summary>
        /// Write a byte to the buffer.
        /// </summary>
        /// <param name="b">The byte.</param>
        public void Write(byte b)
        {
            _bytes.Add(b);
        }

        /// <summary>
        /// Write a byte array to the buffer.
        /// </summary>
        /// <param name="bytes">The byte array.</param>
        public void Write(byte[] bytes)
        {
            Write(bytes, 0, bytes.Length);
        }

        /// <summary>
        /// Write a byte array to the buffer.
        /// </summary>
        /// <param name="bytes">The byte array.</param>
        /// <param name="offset">The offset at which to start encoding.</param>
        /// <param name="count">The number of bytes to encode.</param>
        private void Write(byte[] bytes, int offset, int count)
        {
            var newBytes = new byte[count];
            Array.Copy(bytes, offset, newBytes, 0, count);

            _bytes.AddRange(newBytes);
        }

        /// <summary>
        /// Get the buffer's content as a byte array.
        /// </summary>
        /// <returns>A byte array.</returns>
        public byte[] ToArray()
        {
            return _bytes.ToArray();
        }
    }
}