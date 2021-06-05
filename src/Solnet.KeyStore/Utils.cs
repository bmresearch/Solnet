using System;
using System.Collections.Generic;
using System.Linq;

namespace Solnet.KeyStore
{
    public static class Utils
    {

        /// <summary>
        /// Helper function to convert a character to byte.
        /// </summary>
        /// <param name="character">The character to convert</param>
        /// <param name="index">The value that represents the character's position.</param>
        /// <param name="shift">Number of bits to shift.</param>
        /// <returns>The corresponding byte.</returns>
        /// <exception cref="FormatException">Throws format exception when the character is not a valid alphanumeric character.</exception>
        private static byte FromCharacterToByte(char character, int index, int shift = 0)
        {
            var value = (byte)character;
            if (0x40 < value && 0x47 > value || 0x60 < value && 0x67 > value)
            {
                if (0x40 == (0x40 & value))
                    if (0x20 == (0x20 & value))
                        value = (byte)((value + 0xA - 0x61) << shift);
                    else
                        value = (byte)((value + 0xA - 0x41) << shift);
            }
            else if (0x29 < value && 0x40 > value)
            {
                value = (byte)((value - 0x30) << shift);
            }
            else
            {
                throw new FormatException(
                    $"Character '{character}' at index '{index}' is not valid alphanumeric character.");
            }

            return value;
        }

        /// <summary>
        /// Performs the hex to byte array conversion.
        /// </summary>
        /// <param name="value">The string to convert to byte array.</param>
        /// <returns>The corresponding byte array.</returns>
        private static byte[] HexToByteArrayInternal(string value)
        {
            byte[] bytes = null;
            if (string.IsNullOrEmpty(value))
            {
                bytes = new byte[] { };
            }
            else
            {
                var stringLength = value.Length;
                var writeIndex = 0;
                bytes = new byte[stringLength / 2]; // Initialize our byte array to hold the converted string.

                for (var readIndex = 0; readIndex < value.Length; readIndex += 2)
                {
                    var upper = FromCharacterToByte(value[readIndex], readIndex, 4);
                    var lower = FromCharacterToByte(value[readIndex + 1], readIndex + 1);

                    bytes[writeIndex++] = (byte)(upper | lower);
                }
            }

            return bytes;
        }

        /// <summary>
        /// Convert the passed string to a byte array.
        /// </summary>
        /// <param name="value">The string to convert</param>
        /// <returns>The equivalent byte array.</returns>
        /// <exception cref="FormatException">Throws format exception when the string could not be converted.</exception>
        public static byte[] HexToByteArray(this string value)
        {
            try
            {
                return HexToByteArrayInternal(value);
            }
            catch (FormatException ex)
            {
                throw new FormatException($"String '{value}' could not be converted to byte array (not hex?).", ex);
            }
        }

        /// <summary>
        /// Convert a byte array into a hexadecimal string.
        /// </summary>
        /// <param name="value">The byte array to convert.</param>
        /// <returns>The string as hex.</returns>
        public static string ToHex(this byte[] value)
        {
            return string.Concat(value.Select(b => b.ToString("x2")).ToArray());
        }

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
            var newS = data.AsSpan(1, data.Length - 1);

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