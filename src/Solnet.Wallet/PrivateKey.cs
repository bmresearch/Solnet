// unset

using Solnet.Wallet.Utilities;
using System;

namespace Solnet.Wallet
{
    /// <summary>
    /// Implements the private key functionality.
    /// </summary>
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
            KeyBytes = Encoders.Base58.DecodeData(key);
            Key = key;
        }
    }
}