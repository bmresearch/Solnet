using Chaos.NaCl;
using Org.BouncyCastle.Crypto.Digests;
using System;
using System.Collections.Generic;

namespace Solnet.Wallet.Utilities
{
    /// <summary>
    /// Implements utility methods to be used in the wallet.
    /// </summary>
    internal static class Utils
    {
        /// <summary>
        /// Adds or replaces a value in a dictionary.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="key">The key to add or replace.</param>
        /// <param name="value">The value.</param>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        internal static void AddOrReplace<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            if (dictionary.ContainsKey(key))
                dictionary[key] = value;
            else
                dictionary.Add(key, value);
        }

        /// <summary>
        /// Attempts to get a value from a dictionary.
        /// </summary>
        /// <param name="dictionary">The dictionary to get the value from.</param>
        /// <param name="key">The key to get.</param>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <returns>The value.</returns>
        internal static TValue TryGet<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
        {
            dictionary.TryGetValue(key, out TValue value);
            return value;
        }

        /// <summary>
        /// Slices the array, returning a new array starting at <c>start</c> index and ending at <c>end</c> index.
        /// </summary>
        /// <param name="source">The array to slice.</param>
        /// <param name="start">The starting index of the slicing.</param>
        /// <param name="end">The ending index of the slicing.</param>
        /// <typeparam name="T">The array type.</typeparam>
        /// <returns>The sliced array.</returns>
        internal static T[] Slice<T>(this T[] source, int start, int end)
        {
            if (end < 0)
                end = source.Length;

            var len = end - start;

            // Return new array.
            var res = new T[len];
            for (var i = 0; i < len; i++) res[i] = source[i + start];
            return res;
        }

        /// <summary>
        /// Slices the array, returning a new array starting at <c>start</c>.
        /// </summary>
        /// <param name="source">The array to slice.</param>
        /// <param name="start">The starting index of the slicing.</param>
        /// <typeparam name="T">The array type.</typeparam>
        /// <returns>The sliced array.</returns>
        internal static T[] Slice<T>(this T[] source, int start)
        {
            return Slice(source, start, -1);
        }

        /// <summary>
        /// Calculates the Sha256 of the given data.
        /// </summary>
        /// <param name="data">The data to hash.</param>
        /// <returns>The hash.</returns>
        public static byte[] Sha256(ReadOnlySpan<byte> data)
        {
            return Sha256(data.ToArray(), 0, data.Length);
        }

        /// <summary>
        /// Calculates the SHA256 of the given data.
        /// </summary>
        /// <param name="data">The data to hash.</param>
        /// <param name="offset">The offset at which to start.</param>
        /// <param name="count">The number of bytes to in the array to use as data.</param>
        /// <returns>The hash.</returns>
        private static byte[] Sha256(byte[] data, int offset, int count)
        {
            byte[] i = new byte[32];
            Sha256Digest digest = new();
            digest.BlockUpdate(data, offset, count);
            digest.DoFinal(i, 0);
            return i;
        }

        /// <summary>
        /// Gets the corresponding ed25519 key pair from the passed seed.
        /// </summary>
        /// <param name="seed">The seed</param>
        /// <returns>The key pair.</returns>
        internal static (byte[] privateKey, byte[] publicKey) EdKeyPairFromSeed(byte[] seed) =>
            new(Ed25519.ExpandedPrivateKeyFromSeed(seed), Ed25519.PublicKeyFromSeed(seed));
    }
}