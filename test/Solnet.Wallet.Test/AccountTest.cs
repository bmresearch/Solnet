using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Solnet.Wallet.Test
{
    [TestClass]
    public class AccountTest
    {
        private static readonly byte[] PrivateKey =
        {
            227, 215, 255, 79, 160, 83, 24, 167, 124, 73, 168, 45,
            235, 105, 253, 165, 194, 54, 12, 95, 5, 47, 21, 158, 120,
            155, 199, 182, 101, 212, 80, 173, 138, 180, 156, 252, 109,
            252, 108, 26, 186, 0, 196, 69, 57, 102, 15, 151, 149, 242,
            119, 181, 171, 113, 120, 224, 0, 118, 155, 61, 246, 56, 178, 47
        };
        private static readonly byte[] PublicKey =
        {
            138, 180, 156, 252, 109, 252, 108, 26, 186, 0,
            196, 69, 57, 102, 15, 151, 149, 242, 119, 181,
            171, 113, 120, 224, 0, 118, 155, 61, 246, 56, 178, 47
        };
        
        private static readonly byte[] InvalidPrivateKey =
        {
            227, 215, 255, 79, 160, 83, 24, 167, 124, 73, 168, 45,
            235, 105, 253, 165, 194, 54, 12, 95, 5, 47, 21, 158, 120,
            155, 199, 182, 101, 212, 80, 173, 138, 180, 156, 252, 109,
            252, 108, 26, 186, 0, 196, 69, 57, 242, 119, 181, 171, 113,
            120, 224, 0, 118, 155, 61, 246, 56, 178, 47
        };
        private static readonly byte[] InvalidPublicKey =
        {
            138, 180, 156, 252, 109, 252, 108, 26, 186, 0,
            196, 69, 57, 102, 15, 151, 149, 242, 119, 181,
            171, 113, 120, 224, 0, 118, 155, 61
        };
        
        
        private static readonly byte[] SerializedMessage =
        {
            1, 0, 2, 4, 138, 180, 156, 252, 109, 252, 108, 26, 186, 0,
            196, 69, 57, 102, 15, 151, 149, 242, 119, 181, 171, 113,
            120, 224, 0, 118, 155, 61, 246, 56, 178, 47, 173, 126, 102,
            53, 246, 163, 32, 189, 27, 84, 69, 94, 217, 196, 152, 178,
            198, 116, 124, 160, 230, 94, 226, 141, 220, 221, 119, 21,
            204, 242, 204, 164, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 5, 74, 83, 80,
            248, 93, 200, 130, 214, 20, 165, 86, 114, 120, 138, 41, 109,
            223, 30, 171, 171, 208, 166, 6, 120, 136, 73, 50, 244, 238,
            246, 160, 61, 96, 239, 228, 59, 10, 206, 186, 110, 68, 55,
            160, 108, 50, 58, 247, 220, 116, 182, 121, 237, 126, 42, 184,
            248, 125, 83, 253, 85, 181, 215, 93, 2, 2, 2, 0, 1, 12, 2, 0, 0, 0,
            128, 150, 152, 0, 0, 0, 0, 0, 3, 1, 0, 21, 72, 101, 108, 108, 111,
            32, 102, 114, 111, 109, 32, 83, 111, 108, 46, 78, 101, 116, 32, 58, 41
        };
        private static readonly byte[] SerializedMessageSignature =
        {
            234, 147, 144, 17, 200, 57, 8, 154, 139, 86, 156, 12, 7, 143, 144,
            85, 27, 151, 186, 223, 246, 231, 186, 81, 69, 107, 126, 76, 119,
            14, 112, 57, 38, 5, 28, 109, 99, 30, 249, 154, 87, 241, 28, 161,
            178, 165, 146, 73, 179, 4, 71, 133, 203, 145, 125, 252, 200, 249,
            38, 105, 30, 113, 73, 8
        };

        private const string ExpectedEncodedPublicKey = "ALSzrjtGi8MZGmAZa6ZhtUZq3rwurWuJqWFdgcj9MMFL";
        private const string ExpectedEncodedPrivateKey = "5ZD7ntKtyHrnqMhfSuKBLdqHzT5N3a2aYnCGBcz4N78b84TKpjwQ4QBsapEnpnZFchM7F1BpqDkSuLdwMZwM8hLi";

        [TestMethod]
        public void TestAccountNoKeys()
        {
            var account = new Account();
            Assert.IsNotNull(account.PrivateKey, "account.PrivateKey != null");
            Assert.IsNotNull(account.PublicKey, "account.PublicKey != null");
            Assert.IsNotNull(account.GetPrivateKey, "account.GetPrivateKey != null");
            Assert.IsNotNull(account.GetPublicKey, "account.GetPublicKey != null");
        }
        
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestAccountInvalidKeys()
        {
            _ = new Account(InvalidPrivateKey, InvalidPublicKey);
        }
        
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestAccountInvalidPrivateKey()
        {
            _ = new Account(InvalidPrivateKey, PublicKey);
        }
        
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestAccountInvalidPublicKey()
        {
            _ = new Account(PrivateKey, InvalidPublicKey);
        }
        
        [TestMethod]
        public void TestAccountGetPrivateKey()
        {
            var account = new Account(PrivateKey, PublicKey);
            CollectionAssert.AreEqual(PrivateKey, account.PrivateKey);
            CollectionAssert.AreEqual(PublicKey, account.PublicKey);
            Assert.AreEqual(ExpectedEncodedPublicKey, account.GetPublicKey);
        }
        
        [TestMethod]
        public void TestAccountGetPublicKey()
        {
            var account = new Account(PrivateKey, PublicKey);
            CollectionAssert.AreEqual(PrivateKey, account.PrivateKey);
            CollectionAssert.AreEqual(PublicKey, account.PublicKey);
            Assert.AreEqual(ExpectedEncodedPrivateKey, account.GetPrivateKey);
        }

        [TestMethod]
        public void TestAccountSign()
        {
            var account = new Account(PrivateKey, PublicKey);
            CollectionAssert.AreEqual(PrivateKey, account.PrivateKey);
            CollectionAssert.AreEqual(PublicKey, account.PublicKey);
            CollectionAssert.AreEqual(SerializedMessageSignature, account.Sign(SerializedMessage));
        }
        
        [TestMethod]
        public void TestAccountVerify()
        {
            var account = new Account(PrivateKey, PublicKey);
            CollectionAssert.AreEqual(PrivateKey, account.PrivateKey);
            CollectionAssert.AreEqual(PublicKey, account.PublicKey);
            Assert.IsTrue(account.Verify(SerializedMessage, SerializedMessageSignature));
        }
    }
}