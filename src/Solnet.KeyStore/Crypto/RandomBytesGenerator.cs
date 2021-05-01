using Org.BouncyCastle.Security;

namespace Solnet.KeyStore.Crypto
{
    /// <summary>
    /// Implements a random bytes generator based on the <see cref="SecureRandom"/> 
    /// </summary>
    public class RandomBytesGenerator : IRandomBytesGenerator
    {
        /// <summary>
        /// The <see cref="SecureRandom"/> generator.
        /// </summary>
        private static readonly SecureRandom Random = new SecureRandom();

        /// <summary>
        /// Generate a random initialization vector with 16 bytes.
        /// </summary>
        /// <returns>A byte array.</returns>
        public byte[] GenerateRandomInitializationVector()
        {
            return GenerateRandomBytes(16);
        }

        /// <summary>
        /// Generates a random salt with 32 bytes.
        /// </summary>
        /// <returns>A byte array.</returns>
        public byte[] GenerateRandomSalt()
        {
            return GenerateRandomBytes(32);
        }

        /// <summary>
        /// Generate a number of bytes.
        /// </summary>
        /// <param name="size">The number of bytes</param>
        /// <returns>A byte array.</returns>
        public byte[] GenerateRandomBytes(int size)
        {
            var bytes = new byte[size];
            Random.NextBytes(bytes);
            return bytes;
        }
    }
}