// unset

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Solnet.Wallet.Test
{
    [TestClass]
    public class KeysTest
    {
        private static readonly string InvalidPublicKeyLength = "ALSzrjtGi8MZGmAZa6ZhtUZq3rwurWuJqWFdgcj9MMFLasdasasd";
        private const string InvalidPrivateKeyLength = "5ZD7ntKtyHrnqMhfSuKBLdqHzT5N3a2aYnCGBasdcz4N78b84TKpasdwQ4QBsapEnpnZFchM7F1BpqDasdkSuLdwMZwM8hLi";
        private const string PrivateKey = "5ZD7ntKtyHrnqMhfSuKBLdqHzT5N3a2aYnCGBcz4N78b84TKpjwQ4QBsapEnpnZFchM7F1BpqDkSuLdwMZwM8hLi";
        private static readonly byte[] ExpectedPrivateKeyBytes =
        {
            227, 215, 255, 79, 160, 83, 24, 167, 124, 73, 168, 45,
            235, 105, 253, 165, 194, 54, 12, 95, 5, 47, 21, 158, 120,
            155, 199, 182, 101, 212, 80, 173, 138, 180, 156, 252, 109,
            252, 108, 26, 186, 0, 196, 69, 57, 102, 15, 151, 149, 242,
            119, 181, 171, 113, 120, 224, 0, 118, 155, 61, 246, 56, 178, 47
        };

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestPublicKeyStringException()
        {
            _ = new PublicKey(InvalidPublicKeyLength);
        }
        
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestPrivateKeyStringException()
        {
            _ = new PublicKey(InvalidPrivateKeyLength);
        }

        [TestMethod]
        public void TestPrivateKeyBytes()
        {
            var pk = new PrivateKey(PrivateKey);
            CollectionAssert.AreEqual(ExpectedPrivateKeyBytes, pk.KeyBytes);
        }
    }
}