using Microsoft.VisualStudio.TestTools.UnitTesting;
using Solnet.KeyStore.Exceptions;
using Solnet.KeyStore.Services;
using System;
using System.IO;
using System.Text.Json;

namespace Solnet.KeyStore.Test
{
    [TestClass]
    public class SecretKeyStoreServiceTest
    {
        private const string InvalidPath = "Resources/DoesNotExist.json";
        private const string InvalidEmptyFilePath = "Resources/InvalidEmptyFile.json";
        private const string ValidKeyStorePath = "Resources/ValidKeyStore.json";
        private const string InvalidKeyStorePath = "Resources/InvalidKeyStore.json";
        private const string ValidPbkdf2KeyStorePath = "Resources/ValidPbkdf2KeyStore.json";
        private const string InvalidPbkdf2KeyStorePath = "Resources/InvalidPbkdf2KeyStore.json";

        private static readonly byte[] SeedWithoutPassphrase =
        {
            124,36,217,106,151,19,165,102,96,101,74,81,
            237,254,232,133,28,167,31,35,119,188,66,40,
            101,104,25,103,139,83,57,7,19,215,6,113,22,
            145,107,209,208,107,159,40,223,19,82,53,136,
            255,40,171,137,93,9,205,28,7,207,88,194,91,
            219,232
        };
        private static readonly byte[] SeedWithPassphrase =
        {
            163,4,184,24,182,219,174,214,13,54,158,198,
            63,202,76,3,190,224,76,202,160,96,124,95,89,
            155,113,10,46,218,154,74,125,7,103,78,0,51,
            244,192,221,12,200,148,9,252,4,117,193,123,
            102,56,255,105,167,180,125,222,19,111,219,18,
            115,0
        };

        private const string ExpectedKeyStoreAddress = "4n8BE7DHH4NudifUBrwPbvNPs2F86XcagT7C2JKdrWrR";

        private static readonly SecretKeyStoreService KeyStore = new();

        [TestMethod]
        [ExpectedException(typeof(FileNotFoundException))]
        public void TestKeyStorePathNotFound()
        {
            _ = KeyStore.DecryptKeyStoreFromFile("randomPassword", InvalidPath);
        }

        [TestMethod]
        [ExpectedException(typeof(JsonException))]
        public void TestKeyStoreInvalidEmptyFilePath()
        {
            _ = KeyStore.DecryptKeyStoreFromFile("randomPassword", InvalidEmptyFilePath);
        }

        [TestMethod]
        public void TestKeyStoreValid()
        {
            var seed = KeyStore.DecryptKeyStoreFromFile("randomPassword", ValidKeyStorePath);

            CollectionAssert.AreEqual(SeedWithPassphrase, seed);
        }

        [TestMethod]
        [ExpectedException(typeof(DecryptionException))]
        public void TestKeyStoreInvalidPassword()
        {
            _ = KeyStore.DecryptKeyStoreFromFile("randomPassworasdd", ValidKeyStorePath);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestKeyStoreInvalid()
        {
            _ = KeyStore.DecryptKeyStoreFromFile("randomPassword", InvalidKeyStorePath);
        }

        [TestMethod]
        public void TestKeyStoreSerialize()
        {
            var json = KeyStore.EncryptAndGenerateDefaultKeyStoreAsJson("randomPassword", SeedWithPassphrase,
                ExpectedKeyStoreAddress);

            var keyStoreAddress = SecretKeyStoreService.GetAddressFromKeyStore(json);

            Assert.AreEqual(ExpectedKeyStoreAddress, keyStoreAddress);
        }

        [TestMethod]
        public void TestKeyStoreGenerateKeyStore()
        {
            var json = KeyStore.EncryptAndGenerateDefaultKeyStoreAsJson("randomPassword", SeedWithPassphrase,
                ExpectedKeyStoreAddress);

            var keyStoreAddress = SecretKeyStoreService.GetAddressFromKeyStore(json);

            Assert.AreEqual(ExpectedKeyStoreAddress, keyStoreAddress);
        }

        [TestMethod]
        public void TestKeyStoreGetAddress()
        {
            var fileJson = File.ReadAllText(ValidPbkdf2KeyStorePath);
            var keyStoreAddress = SecretKeyStoreService.GetAddressFromKeyStore(fileJson);

            Assert.AreEqual(ExpectedKeyStoreAddress, keyStoreAddress);
        }

        [TestMethod]
        public void TestValidPbkdf2KeyStore()
        {
            var ks = new KeyStorePbkdf2Service();
            var fileJson = File.ReadAllText(ValidPbkdf2KeyStorePath);
            var seed = ks.DecryptKeyStoreFromJson("randomPassword", fileJson);
            CollectionAssert.AreEqual(SeedWithPassphrase, seed);
        }

        [TestMethod]
        public void TestValidPbkdf2KeyStoreSerialize()
        {
            var ks = new KeyStorePbkdf2Service();
            var json = ks.EncryptAndGenerateKeyStoreAsJson("randomPassword", SeedWithPassphrase,
                ExpectedKeyStoreAddress);

            var keyStoreAddress = SecretKeyStoreService.GetAddressFromKeyStore(json);

            Assert.AreEqual(ExpectedKeyStoreAddress, keyStoreAddress);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestInvalidPbkdf2KeyStore()
        {
            var ks = new KeyStorePbkdf2Service();
            var fileJson = File.ReadAllText(InvalidPbkdf2KeyStorePath);
            _ = ks.DecryptKeyStoreFromJson("randomPassword", fileJson);
        }
    }
}