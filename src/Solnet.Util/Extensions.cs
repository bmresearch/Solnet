using System;
using System.Linq;

namespace Solnet.Util
{
    /// <summary>
    /// Implements extensions to encode and decode addresses and keys.
    /// </summary>
    public static class Extensions
    {
        public static T[] Slice<T>(this T[] source, int start, int end)
        {
            if (end < 0)
                end = source.Length;

            var len = end - start;

            // Return new array.
            var res = new T[len];
            for (var i = 0; i < len; i++) res[i] = source[i + start];
            return res;
        }

        public static T[] Slice<T>(this T[] source, int start)
        {
            return Slice<T>(source, start, -1);
        }
            
        /// <summary>
        /// Formats a byte array into a string in order to be compatible with the original solana-keygen made in rust.
        /// </summary>
        /// <param name="bytes">The byte array to be formatted.</param>
        /// <returns>A formatted string.</returns>
        public static string ToStringByteArray(this byte[] bytes) => "[" + string.Join(",", bytes) + "]";
        
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
            return bytes;
        }
    }
}