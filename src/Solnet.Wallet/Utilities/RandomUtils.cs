using Org.BouncyCastle.Security;
using System;

namespace Solnet.Wallet.Utilities
{
    /// <summary>
    /// Implements a random number generator using the crypto service provider.
    /// </summary>
    public class RngCryptoServiceProviderRandom : IRandom
    {
        /// <summary>
        /// The instance of the crypto service provider.
        /// </summary>
        private readonly SecureRandom _instance;

        /// <summary>
        /// Initialize the random number generator.
        /// </summary>
        public RngCryptoServiceProviderRandom()
        {
            _instance = new SecureRandom();
        }

        #region IRandom Members

        /// <inheritdoc cref="IRandom.GetBytes(byte[])"/>
        public void GetBytes(byte[] output)
        {
            _instance.NextBytes(output);
        }

        #endregion
    }

    /// <summary>
    /// Specifies functionality for a random number generator.
    /// </summary>
    public interface IRandom
    {
        /// <summary>
        /// Get bytes.
        /// </summary>
        /// <param name="output">The output array of bytes.</param>
        void GetBytes(byte[] output);
    }

    /// <summary>
    /// Implements utilities to be used with random number generation.
    /// </summary>
    public static class RandomUtils
    {
        /// <summary>
        /// Whether to use additional entropy or not.
        /// </summary>
        public static bool UseAdditionalEntropy { get; set; } = true;

        /// <summary>
        /// Initialize the static instance of the random number generator.
        /// </summary>
        static RandomUtils()
        {
            Random = new RngCryptoServiceProviderRandom();
            AddEntropy(Guid.NewGuid().ToByteArray());
        }

        /// <summary>
        /// The random number generator.
        /// </summary>
        public static IRandom Random
        {
            get;
            set;
        }

        /// <summary>
        /// Get random bytes.
        /// </summary>
        /// <param name="length">The number of bytes to get.</param>
        /// <returns>The byte array.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the random number generator has not been initialized</exception>
        public static byte[] GetBytes(int length)
        {
            byte[] data = new byte[length];
            if (Random == null)
                throw new InvalidOperationException("You must initialize the random number generator before generating numbers.");
            Random.GetBytes(data);
            PushEntropy(data);
            return data;
        }

        /// <summary>
        /// Get random bytes.
        /// </summary>
        /// <param name="output">The array of bytes to write the random bytes to.</param>
        /// <returns>The byte array.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the random number generator has not been initialized</exception>
        public static byte[] GetBytes(byte[] output)
        {
            if (Random == null)
                throw new InvalidOperationException("You must initialize the random number generator before generating numbers.");
            Random.GetBytes(output);
            PushEntropy(output);
            return output;
        }

        /// <summary>
        /// Pushes entropy to the given array of bytes.
        /// </summary>
        /// <param name="data">The array of bytes.</param>
        private static void PushEntropy(byte[] data)
        {
            if (!UseAdditionalEntropy || _additionalEntropy == null || data.Length == 0)
                return;
            int pos = _entropyIndex;
            var entropy = _additionalEntropy;
            for (int i = 0; i < data.Length; i++)
            {
                data[i] ^= entropy[pos % 32];
                pos++;
            }
            entropy = Utils.Sha256(data);
            for (int i = 0; i < data.Length; i++)
            {
                data[i] ^= entropy[pos % 32];
                pos++;
            }
            _entropyIndex = pos % 32;
        }

        /// <summary>
        /// The additional entropy.
        /// </summary>
        private static volatile byte[] _additionalEntropy = null;

        /// <summary>
        /// The entropy index..
        /// </summary>
        private static volatile int _entropyIndex = 0;

        /// <summary>
        /// Add entropy to the given data.
        /// </summary>
        /// <param name="data">The data to add entropy to.</param>
        /// <exception cref="ArgumentNullException">Thrown if the data array is null.</exception>
        public static void AddEntropy(byte[] data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));
            var entropy = Utils.Sha256(data);
            if (_additionalEntropy == null)
                _additionalEntropy = entropy;
            else
            {
                for (int i = 0; i < 32; i++)
                {
                    _additionalEntropy[i] ^= entropy[i];
                }
                _additionalEntropy = Utils.Sha256(_additionalEntropy);
            }
        }
    }
}