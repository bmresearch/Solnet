// unset

using Solnet.Wallet.Utilities;
using System;
using System.Diagnostics;

namespace Solnet.Wallet
{
    /// <summary>
    /// Implements the private key functionality.
    /// </summary>
    [DebuggerDisplay("Key = {" + nameof(Key) + "}")]
    public class PrivateKey
    {
        /// <summary>
        /// Private key length.
        /// </summary>
        private const int PrivateKeyLength = 64;

        /// <summary>
        /// The key as base-58 encoded string.
        /// </summary>
        public string Key;

        /// <summary>
        /// The bytes of the key.
        /// </summary>
        public byte[] KeyBytes;

        /// <summary>
        /// Initialize the private key from the given byte array.
        /// </summary>
        /// <param name="key">The private key as byte array.</param>
        public PrivateKey(byte[] key)
        {
            if (key.Length != PrivateKeyLength)
                throw new ArgumentException("invalid key length", nameof(key));
            KeyBytes = key;
            Key = Encoders.Base58.EncodeData(key);
        }
        
        /// <summary>
        /// Initialize the private key from the given string.
        /// </summary>
        /// <param name="key">The private key as base58 encoded string.</param>
        public PrivateKey(string key)
        {
            byte[] decodedKey = Encoders.Base58.DecodeData(key);
            if (decodedKey.Length != PrivateKeyLength)
                throw new ArgumentException("invalid key length", nameof(key));
            KeyBytes = decodedKey;
            Key = key;
        }
        
        /// <summary>
        /// Conversion between a <see cref="PrivateKey"/> object and the corresponding base-58 encoded private key.
        /// </summary>
        /// <param name="privateKey">The PrivateKey object.</param>
        /// <returns>The base-58 encoded private key.</returns>
        public static implicit operator string(PrivateKey privateKey) => privateKey.Key;
        
        /// <summary>
        /// Conversion between a base-58 encoded private key and the <see cref="PrivateKey"/> object.
        /// </summary>
        /// <param name="address">The base-58 encoded private key.</param>
        /// <returns>The PrivateKey object.</returns>
        public static explicit operator PrivateKey(string address) => new (address);
        
        /// <summary>
        /// Conversion between a <see cref="PrivateKey"/> object and the private key as a byte array.
        /// </summary>
        /// <param name="privateKey">The PrivateKey object.</param>
        /// <returns>The private key as a byte array.</returns>
        public static implicit operator byte[](PrivateKey privateKey) => privateKey.KeyBytes;
        
        /// <summary>
        /// Conversion between a private key as a byte array and the corresponding <see cref="PrivateKey"/> object.
        /// </summary>
        /// <param name="keyBytes">The private key as a byte array.</param>
        /// <returns>The PrivateKey object.</returns>
        public static explicit operator PrivateKey(byte[] keyBytes) => new (keyBytes);
    }
}