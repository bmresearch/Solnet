using System;
using Chaos.NaCl;
using NBitcoin.DataEncoders;

namespace Solnet.Wallet
{
    public class Account
    {
        /// <summary>
        /// The base58 encoder instance.
        /// </summary>
        private static readonly Base58Encoder Encoder = new ();

        /// <summary>
        /// Private key length.
        /// </summary>
        private const int PrivateKeyLength = 64;
        
        /// <summary>
        /// Public key length.
        /// </summary>
        private const int PublicKeyLength = 32;

        /// <summary>
        /// The private key.
        /// </summary>
        private readonly byte[] _privateKey;

        /// <summary>
        /// The public key.
        /// </summary>
        private readonly byte[] _publicKey;

        /// <summary>
        /// Initialize an account with the passed private and public keys.
        /// </summary>
        /// <param name="privateKey">The private key.</param>
        /// <param name="publicKey">The public key.</param>
        public Account(byte[] privateKey, byte[] publicKey)
        {
            if (privateKey.Length != PrivateKeyLength)
                throw new ArgumentException("invalid key length", nameof(privateKey));
            if (publicKey.Length != PublicKeyLength)
                throw new ArgumentException("invalid key length", nameof(privateKey));
            
            _privateKey = privateKey;
            _publicKey = publicKey;
        }

        /// <summary>
        /// Get the private key encoded as base58.
        /// </summary>
        public string GetPrivateKey => Encoder.EncodeData(_privateKey);

        /// <summary>
        /// Get the public key encoded as base58.
        /// </summary>
        public string GetPublicKey => Encoder.EncodeData(_publicKey);

        /// <summary>
        /// Get the public key as a byte array.
        /// </summary>
        public byte[] PublicKey => _publicKey;

        /// <summary>
        /// Get the private key as a byte array.
        /// </summary>
        public byte[] PrivateKey => _privateKey;
        
        
        /// <summary>
        /// Verify the signed message.
        /// </summary>
        /// <param name="message">The signed message.</param>
        /// <param name="signature">The signature of the message.</param>
        /// <returns></returns>
        public bool Verify(byte[] message, byte[] signature)
        {
            return Ed25519.Verify(signature, message, _publicKey);
        }
        
        /// <summary>
        /// Sign the data.
        /// </summary>
        /// <param name="message">The data to sign.</param>
        /// <returns>The signature of the data.</returns>
        public byte[] Sign(byte[] message)
        {
            var signature = new ArraySegment<byte>();
            Ed25519.Sign(signature, new ArraySegment<byte>(message), new ArraySegment<byte>(_privateKey));
            return signature.ToArray();
        }
    }
}