using NBitcoin.DataEncoders;
using Solnet.Wallet.Key;

namespace Solnet.Wallet
{
    public class Account
    {
        /// <summary>
        /// The base58 encoder instance.
        /// </summary>
        private static readonly Base58Encoder Encoder = new Base58Encoder();
        
        /// <summary>
        /// The key pair associated with this account.
        /// </summary>
        private KeyPair _keyPair;

        /// <summary>
        /// Initialize an account with the passed private and public keys.
        /// </summary>
        /// <param name="privateKey">The private key.</param>
        /// <param name="publicKey">The public key.</param>
        public Account(byte[] privateKey, byte[] publicKey)
        {
            _keyPair = new KeyPair()
            {
                PrivateKey = privateKey,
                PublicKey = publicKey
            };
        }

        /// <summary>
        /// Get the private key encoded as base58.
        /// </summary>
        public string EncodedPrivateKey => Encoder.EncodeData(_keyPair.PrivateKey);

        /// <summary>
        /// Get the public key encoded as base58.
        /// </summary>
        public string EncodedPublicKey => Encoder.EncodeData(_keyPair.PublicKey);

        /// <summary>
        /// Get the public key as a byte array.
        /// </summary>
        public byte[] PublicKey => _keyPair.PublicKey;

        /// <summary>
        /// Get the private key as a byte array.
        /// </summary>
        public byte[] PrivateKey => _keyPair.PrivateKey;
    }
}