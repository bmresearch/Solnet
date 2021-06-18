// unset

using Solnet.Wallet.Utilities;
using System;

namespace Solnet.Wallet
{
    /// <summary>
    /// Implements the public key functionality.
    /// </summary>
    public class PublicKey
    {
        /// <summary>
        /// Public key length.
        /// </summary>
        private const int PublicKeyLength = 32;

        /// <summary>
        /// The key as base-58 encoded string.
        /// </summary>
        public string Key;

        /// <summary>
        /// The bytes of the key.
        /// </summary>
        public byte[] KeyBytes;

        /// <summary>
        /// Initialize the public key from the given byte array.
        /// </summary>
        /// <param name="key">The public key as byte array.</param>
        public PublicKey(byte[] key)
        {
            if (key.Length != PublicKeyLength)
                throw new ArgumentException("invalid key length", nameof(key));
            KeyBytes = key;
            Key = Encoders.Base58.EncodeData(key);
        }
        
        /// <summary>
        /// Initialize the public key from the given string.
        /// </summary>
        /// <param name="key">The public key as base58 encoded string.</param>
        public PublicKey(string key)
        {
            byte[] decodedKey = Encoders.Base58.DecodeData(key);
            if (decodedKey.Length != PublicKeyLength)
                throw new ArgumentException("invalid key length", nameof(key));
            KeyBytes = decodedKey;
            Key = key;
        }
    }
}