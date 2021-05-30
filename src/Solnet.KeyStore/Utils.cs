using System;
using System.Collections.Generic;

namespace Solnet.KeyStore
{
    public static class Utils
    {
        /// <summary>
        /// Formats a byte array into a string in order to be compatible with the original solana-keygen made in rust.
        /// </summary>
        /// <param name="bytes">The byte array to be formatted.</param>
        /// <returns>A formatted string.</returns>
        public static string ToStringByteArray(this IEnumerable<byte> bytes) => "[" + string.Join(",", bytes) + "]";

        /// <summary>
        /// Formats a string into a byte array in order to be compatible with the original solana-keygen made in rust.
        /// </summary>
        /// <param name="data">The string to be formatted.</param>
        /// <returns>A formatted byte array.</returns>
        public static byte[] FromStringByteArray(this string data)
        {
            var bytes = new byte[64];
            var index = 0;
            var i = 0;
            var newS = data.AsSpan(1, data.Length-1);
            
            while ((i = newS.IndexOf(',')) != -1)
            {
                bytes[index++] = byte.Parse(newS[..i]);
                newS = newS[(i + 1)..];
            }
            
            bytes[index] = byte.Parse(newS[..^1]);
            if (index != 63)
                throw new ArgumentException("invalid string for conversion", nameof(data));
            return bytes;
        }
    }
}