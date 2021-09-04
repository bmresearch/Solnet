// unset

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Solnet.Wallet.Test
{
    [TestClass]
    public class KeysTest
    {
        private const string PrivateKeyString = "5ZD7ntKtyHrnqMhfSuKBLdqHzT5N3a2aYnCGBcz4N78b84TKpjwQ4QBsapEnpnZFchM7F1BpqDkSuLdwMZwM8hLi";
        private static readonly byte[] ExpectedPrivateKeyBytes =
        {
            227, 215, 255, 79, 160, 83, 24, 167, 124, 73, 168, 45,
            235, 105, 253, 165, 194, 54, 12, 95, 5, 47, 21, 158, 120,
            155, 199, 182, 101, 212, 80, 173, 138, 180, 156, 252, 109,
            252, 108, 26, 186, 0, 196, 69, 57, 102, 15, 151, 149, 242,
            119, 181, 171, 113, 120, 224, 0, 118, 155, 61, 246, 56, 178, 47
        };
        private const string ExpectedPrivateKey =
            "c1BzdtL4RByNQnzcaUq3WuNLuyY4tQogGT7JWwy4YGBE8FGSgWUH8eNJFyJgXNYtwTKq4emhC4V132QX9REwujm";
        private static readonly byte[] PrivateKeyBytes =
        {
            30, 47, 124, 64, 115, 181, 108, 148, 133, 204, 66, 60, 190,
            64, 208, 182, 169, 19, 112, 20, 186, 227, 179, 134, 96, 155,
            90, 163, 54, 6, 152, 33, 123, 172, 114, 217, 192, 233, 194,
            40, 233, 234, 173, 25, 163, 56, 237, 112, 216, 151, 21, 209,
            120, 79, 46, 85, 162, 195, 155, 97, 136, 88, 16, 64
        };
        private static readonly byte[] InvalidPrivateKeyBytes =
        {
            30, 47, 124, 64, 115, 181, 108, 148, 133, 204, 66, 60, 190,
            64, 208, 182, 169, 19, 112, 20, 186, 227, 179, 134, 96, 155,
            90, 163, 54, 6, 152, 33, 123, 172, 114, 217, 192, 233, 194,
            40, 233, 234, 173, 25, 163, 56, 237, 112, 216, 151, 21, 209,
            120, 79, 46, 85, 162, 195, 155, 97, 136, 88, 16, 64, 0
        };

        private const string PublicKeyString = "9KmfMX4Ne5ocb8C7PwjmJTWTpQTQcPhkeD2zY35mawhq";
        private static readonly byte[] PublicKeyBytes =
        {
            123, 172, 114, 217, 192, 233, 194, 40, 233, 234, 173, 25,
            163, 56, 237, 112, 216, 151, 21, 209, 120, 79, 46, 85,
            162, 195, 155, 97, 136, 88, 16, 64
        };
        private static readonly byte[] InvalidPublicKeyBytes =
        {
            123, 172, 114, 217, 192, 233, 194, 40, 233, 234, 173, 25,
            163, 56, 237, 112, 216, 151, 21, 209, 120, 79, 46, 85,
            162, 195, 155, 97, 136, 88, 16, 64, 0
        };

        [TestMethod]
        public void TestPrivateKey()
        {
            PrivateKey pk = new(PrivateKeyString);
            CollectionAssert.AreEqual(ExpectedPrivateKeyBytes, pk.KeyBytes);
        }

        [TestMethod]
        public void TestPrivateKeySpan()
        {
            PrivateKey pk = new(PrivateKeyBytes.AsSpan());
            Assert.AreEqual(ExpectedPrivateKey, pk);
        }

        [TestMethod]
        public void TestPrivateKeyToString()
        {
            PrivateKey pk = new(PrivateKeyBytes.AsSpan());
            Assert.AreEqual(ExpectedPrivateKey, pk.ToString());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestInvalidPrivateKeyBytes()
        {
            _ = new PrivateKey(InvalidPrivateKeyBytes.AsSpan());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestNullPrivateKeyBytes()
        {
            byte[] key = null;
            _ = new PrivateKey(key);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestNullPrivateKeyString()
        {
            string key = null;
            _ = new PrivateKey(key);
        }

        [TestMethod]
        public void TestPublicKeySpan()
        {
            PublicKey pk = new(PublicKeyBytes.AsSpan());
            Assert.AreEqual(pk.Key, PublicKeyString);
        }

        [TestMethod]
        public void TestPublicKeyToString()
        {
            PublicKey pk = new(PublicKeyBytes);
            Assert.AreEqual(pk.Key, pk.ToString());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestInvalidPublicKeyBytes()
        {
            _ = new PublicKey(InvalidPublicKeyBytes.AsSpan());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestNullPublicKeyString()
        {
            string key = null;
            _ = new PublicKey(key);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestNullPublicKeyBytes()
        {
            byte[] key = null;
            _ = new PublicKey(key);
        }

        [TestMethod]
        public void TestPrivateKeyExplicitStringOperator()
        {
            PrivateKey pk = TestExplicitOperator((PrivateKey)PrivateKeyString);
            Assert.IsInstanceOfType(pk, typeof(PrivateKey));
        }

        [TestMethod]
        public void TestPrivateKeyImplicitStringOperator()
        {
            PrivateKey pk = new(PrivateKeyString);
            string pkString = TestImplicitStringOperator(pk);
            Assert.IsInstanceOfType(pkString, typeof(string));
        }

        [TestMethod]
        public void TestPrivateKeyExplicitBytesOperator()
        {
            PrivateKey pk = TestExplicitOperator((PrivateKey)PrivateKeyBytes);
            Assert.IsInstanceOfType(pk, typeof(PrivateKey));
        }

        [TestMethod]
        public void TestPrivateKeyImplicitBytesOperator()
        {
            PrivateKey pk = new(PrivateKeyString);
            byte[] pkBytes = TestImplicitByteArrayOperator(pk);
            Assert.IsInstanceOfType(pkBytes, typeof(byte[]));
        }

        [TestMethod]
        public void TestPublicKeyExplicitStringOperator()
        {
            PublicKey pk = TestExplicitOperator((PublicKey)PublicKeyString);
            Assert.IsInstanceOfType(pk, typeof(PublicKey));
        }

        [TestMethod]
        public void TestPublicKeyImplicitStringOperator()
        {
            PublicKey pk = new(PublicKeyString);
            string pkString = TestImplicitStringOperator(pk);
            Assert.IsInstanceOfType(pkString, typeof(string));
        }

        [TestMethod]
        public void TestPublicKeyExplicitBytesOperator()
        {
            PublicKey pk = TestExplicitOperator((PublicKey)PublicKeyBytes);
            Assert.IsInstanceOfType(pk, typeof(PublicKey));
        }

        [TestMethod]
        public void TestPublicKeyImplicitBytesOperator()
        {
            PublicKey pk = new(PublicKeyString);
            byte[] pkBytes = TestImplicitByteArrayOperator(pk);
            Assert.IsInstanceOfType(pkBytes, typeof(byte[]));
        }

        private static PrivateKey TestExplicitOperator(PrivateKey key)
        {
            return key;
        }

        private static PublicKey TestExplicitOperator(PublicKey key)
        {
            return key;
        }

        private static string TestImplicitStringOperator(string key)
        {
            return key;
        }
        private static byte[] TestImplicitByteArrayOperator(byte[] key)
        {
            return key;
        }
    }
}