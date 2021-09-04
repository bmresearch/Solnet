// unset

using System;
using System.Collections;
using System.Linq;

namespace Solnet.Wallet.Utilities
{
    /// <summary>
    /// Implements a base58 encoder.
    /// </summary>
    public sealed class Base58Encoder : DataEncoder
    {
        /// <summary>
        /// The base58 characters.
        /// </summary>
        private static readonly char[] PszBase58 = "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz".ToCharArray();

        /// <summary>
        /// 
        /// </summary>
        private static readonly int[] MapBase58 = {
            -1,-1,-1,-1,-1,-1,-1,-1, -1,-1,-1,-1,-1,-1,-1,-1,
            -1,-1,-1,-1,-1,-1,-1,-1, -1,-1,-1,-1,-1,-1,-1,-1,
            -1,-1,-1,-1,-1,-1,-1,-1, -1,-1,-1,-1,-1,-1,-1,-1,
            -1, 0, 1, 2, 3, 4, 5, 6,  7, 8,-1,-1,-1,-1,-1,-1,
            -1, 9,10,11,12,13,14,15, 16,-1,17,18,19,20,21,-1,
            22,23,24,25,26,27,28,29, 30,31,32,-1,-1,-1,-1,-1,
            -1,33,34,35,36,37,38,39, 40,41,42,43,-1,44,45,46,
            47,48,49,50,51,52,53,54, 55,56,57,-1,-1,-1,-1,-1,
            -1,-1,-1,-1,-1,-1,-1,-1, -1,-1,-1,-1,-1,-1,-1,-1,
            -1,-1,-1,-1,-1,-1,-1,-1, -1,-1,-1,-1,-1,-1,-1,-1,
            -1,-1,-1,-1,-1,-1,-1,-1, -1,-1,-1,-1,-1,-1,-1,-1,
            -1,-1,-1,-1,-1,-1,-1,-1, -1,-1,-1,-1,-1,-1,-1,-1,
            -1,-1,-1,-1,-1,-1,-1,-1, -1,-1,-1,-1,-1,-1,-1,-1,
            -1,-1,-1,-1,-1,-1,-1,-1, -1,-1,-1,-1,-1,-1,-1,-1,
            -1,-1,-1,-1,-1,-1,-1,-1, -1,-1,-1,-1,-1,-1,-1,-1,
            -1,-1,-1,-1,-1,-1,-1,-1, -1,-1,-1,-1,-1,-1,-1,-1,
        };
        /// <summary>
        /// Fast check if the string to know if base58 str
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public bool IsMaybeEncoded(string str)
        {
            bool maybeB58 = str.All(t => ((IList)PszBase58).Contains(t));

            return maybeB58 && str.Length > 0;
        }

        /// <summary>
        /// Encode the data.
        /// </summary>
        /// <param name="data">The data to encode.</param>
        /// <param name="offset">The offset at which to start encoding.</param>
        /// <param name="count">The number of bytes to encode.</param>
        /// <returns>The encoded data.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the data array is null.</exception>
        public override string EncodeData(byte[] data, int offset, int count)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            // Skip & count leading zeroes.
            int zeroes = 0;
            int length = 0;
            while (offset != count && data[offset] == 0)
            {
                offset++;
                zeroes++;
            }

            // Allocate enough space in big-endian base58 representation.
            int size = (count - offset) * 138 / 100 + 1; // log(256) / log(58), rounded up.
            byte[] b58 = new byte[size];

            // Process the bytes.
            while (offset != count)
            {
                int carry = data[offset];
                int i = 0;

                // Apply "b58 = b58 * 256 + ch".
                for (int it = size - 1; (carry != 0 || i < length) && it >= 0; i++, it--)
                {
                    carry += 256 * b58[it];
                    b58[it] = (byte)(carry % 58);
                    carry /= 58;
                }

                length = i;
                offset++;
            }

            // Skip leading zeroes in base58 result.
            int it2 = (size - length);
            while (it2 != size && b58[it2] == 0)
                it2++;

            // Translate the result into a string.
            char[] str = new char[zeroes + size - it2];
            Array.Fill(str, '1', 0, zeroes);
            int i2 = zeroes;
            while (it2 != size)
                str[i2++] = PszBase58[b58[it2++]];
            return new string(str);
        }

        /// <summary>
        /// Decode the data.
        /// </summary>
        /// <param name="encoded">The data to decode.</param>
        /// <returns>The decoded data.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the encoded data is null.</exception>
        /// <exception cref="FormatException">Thrown if the data is invalid.</exception>
        public override byte[] DecodeData(string encoded)
        {
            if (encoded == null)
                throw new ArgumentNullException(nameof(encoded));
            int psz = 0;

            // Skip leading spaces.
            while (psz < encoded.Length && IsSpace(encoded[psz]))
                psz++;

            // Skip and count leading '1's.
            int zeroes = 0;
            int length = 0;
            while (psz < encoded.Length && encoded[psz] == '1')
            {
                zeroes++;
                psz++;
            }

            // Allocate enough space in big-endian base256 representation.
            int size = (encoded.Length - psz) * 733 / 1000 + 1; // log(58) / log(256), rounded up.
            byte[] b256 = new byte[size];

            // Process the characters.
            while (psz < encoded.Length && !IsSpace(encoded[psz]))
            {
                // Decode base58 character
                int carry = MapBase58[(byte)encoded[psz]];
                if (carry == -1)  // Invalid b58 character
                    throw new FormatException("Invalid base58 data");
                int i = 0;
                for (int it = size - 1; (carry != 0 || i < length) && it >= 0; i++, it--)
                {
                    carry += 58 * b256[it];
                    b256[it] = (byte)(carry % 256);
                    carry /= 256;
                }
                length = i;
                psz++;
            }

            // Skip trailing spaces.
            while (psz < encoded.Length && IsSpace(encoded[psz]))
                psz++;
            if (psz != encoded.Length)
                throw new FormatException("Invalid base58 data");
            // Skip leading zeroes in b256.
            int it2 = size - length;
            // Copy result into output vector.
            byte[] vch = new byte[zeroes + size - it2];
            Array.Fill<byte>(vch, 0, 0, zeroes);
            int i2 = zeroes;
            while (it2 != size)
                vch[i2++] = b256[it2++];
            return vch;
        }
    }
}