using Org.BouncyCastle.Security;
using System;

namespace Solnet.KeyStore.Crypto
{
    public class RandomBytesGenerator : IRandomBytesGenerator
    {
        private static readonly SecureRandom Random = new SecureRandom();

        public byte[] GenerateRandomInitializationVector()
        {
            return GenerateRandomBytes(16);
        }

        public byte[] GenerateRandomSalt()
        {
            return GenerateRandomBytes(32);
        }

        private static byte[] GenerateRandomBytes(int size)
        {
            Span<byte> bytes = stackalloc byte[size];
            Random.NextBytes(bytes);
            return bytes.ToArray();
        }
    }
}