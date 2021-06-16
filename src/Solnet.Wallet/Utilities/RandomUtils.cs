using System;
using System.Security.Cryptography;

namespace Solnet.Wallet.Utilities
{
    public class RNGCryptoServiceProviderRandom : IRandom
    {
        readonly RNGCryptoServiceProvider _Instance;
        public RNGCryptoServiceProviderRandom()
        {
            _Instance = new RNGCryptoServiceProvider();
        }
        #region IRandom Members

        public void GetBytes(byte[] output)
        {
            _Instance.GetBytes(output);
        }

        #endregion
    }

    public interface IRandom
    {
        void GetBytes(byte[] output);
    }

    public static class RandomUtils
    {
        public static bool UseAdditionalEntropy { get; set; } = true;

        static RandomUtils()
        {
            Random = new RNGCryptoServiceProviderRandom();
            AddEntropy(Guid.NewGuid().ToByteArray());
        }

        public static IRandom Random
        {
            get;
            set;
        }

        public static byte[] GetBytes(int length)
        {
            byte[] data = new byte[length];
            if (Random == null)
                throw new InvalidOperationException("You must set the RNG (RandomUtils.Random) before generating random numbers");
            Random.GetBytes(data);
            PushEntropy(data);
            return data;
        }

        private static void PushEntropy(byte[] data)
        {
            if (!UseAdditionalEntropy || additionalEntropy == null || data.Length == 0)
                return;
            int pos = entropyIndex;
            var entropy = additionalEntropy;
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
            entropyIndex = pos % 32;
        }

        static volatile byte[] additionalEntropy = null;
        static volatile int entropyIndex = 0;

        public static void AddEntropy(byte[] data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));
            var entropy = Utils.Sha256(data);
            if (additionalEntropy == null)
                additionalEntropy = entropy;
            else
            {
                for (int i = 0; i < 32; i++)
                {
                    additionalEntropy[i] ^= entropy[i];
                }
                additionalEntropy = Utils.Sha256(additionalEntropy);
            }
        }
    }
}