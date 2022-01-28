using Chaos.NaCl;
using Org.BouncyCastle.Security;
using Solnet.Wallet.Utilities;
using System.Diagnostics;

namespace Solnet.Wallet
{
    /// <summary>
    /// Implements account functionality.
    /// </summary>
    [DebuggerDisplay("PubKey = {ToString()}")]
    public class Account
    {
        /// <summary>
        /// The private key.
        /// </summary>
        public PrivateKey PrivateKey { get; }

        /// <summary>
        /// The public key.
        /// </summary>
        public PublicKey PublicKey { get; }

        /// <summary>
        /// Initialize an account. Generating a random seed for the Ed25519 key pair.
        /// </summary>
        public Account()
        {
            byte[] seed = GenerateRandomSeed();

            (byte[] privateKey, byte[] publicKey) = Utils.EdKeyPairFromSeed(seed);

            PrivateKey = new PrivateKey(privateKey);
            PublicKey = new PublicKey(publicKey);
        }

        /// <summary>
        /// Initialize an account with the passed private and public keys.
        /// </summary>
        /// <param name="privateKey">The private key.</param>
        /// <param name="publicKey">The public key.</param>
        public Account(string privateKey, string publicKey)
        {
            PrivateKey = new PrivateKey(privateKey);
            PublicKey = new PublicKey(publicKey);
        }

        /// <inheritdoc cref="Account(string,string)"/>
        public Account(byte[] privateKey, byte[] publicKey)
        {
            PrivateKey = new PrivateKey(privateKey);
            PublicKey = new PublicKey(publicKey);
        }

        /// <summary>
        /// Verify the signed message.
        /// </summary>
        /// <param name="message">The signed message.</param>
        /// <param name="signature">The signature of the message.</param>
        /// <returns></returns>
        public bool Verify(byte[] message, byte[] signature) => PublicKey.Verify(message, signature);

        /// <summary>
        /// Sign the data.
        /// </summary>
        /// <param name="message">The data to sign.</param>
        /// <returns>The signature of the data.</returns>
        public byte[] Sign(byte[] message) => PrivateKey.Sign(message);

        /// <summary>
        /// Generates a random seed for the Ed25519 key pair.
        /// </summary>
        /// <returns>The seed as byte array.</returns>
        private static byte[] GenerateRandomSeed()
        {
            byte[] bytes = new byte[Ed25519.PrivateKeySeedSizeInBytes];
            RandomUtils.GetBytes(bytes);
            return bytes;
        }

        /// <inheritdoc cref="Equals(object)"/>
        public override bool Equals(object obj)
        {
            if (obj is Account account) return account.PublicKey == this.PublicKey;

            return false;
        }

        /// <summary>
        /// Conversion between a <see cref="Account"/> object and the corresponding private key.
        /// </summary>
        /// <param name="account">The Account object.</param>
        /// <returns>The private key.</returns>
        public static implicit operator PrivateKey(Account account) => account.PrivateKey;

        /// <summary>
        /// Conversion between a <see cref="Account"/> object and the corresponding public key.
        /// </summary>
        /// <param name="account">The Account object.</param>
        /// <returns>The public key as a byte array.</returns>
        public static implicit operator PublicKey(Account account) => account.PublicKey;

        /// <inheritdoc cref="ToString"/>
        public override string ToString() => PublicKey;

        /// <inheritdoc cref="GetHashCode"/>
        public override int GetHashCode() => PublicKey.GetHashCode();
    }
}