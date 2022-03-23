using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sol.Unity.Wallet;
using System;
using System.IO;

namespace Sol.Unity.KeyStore.Test
{
    [TestClass]
    public class SolanaKeygenKeyStoreTest
    {
        private static readonly SolanaKeyStoreService KeyStoreService = new();

        private const string InvalidPath = "Resources/DoesNotExist.txt";
        private const string InvalidEmptyFilePath = "Resources/InvalidEmptyFile.txt";
        private const string ValidKeyStorePath = "Resources/ValidSolanaKeygenKeyStore.txt";
        private const string InvalidKeyStorePath = "Resources/InvalidSolanaKeygenKeyStore.txt";
        private const string ValidKeyStoreSavePath = "Resources/ValidSolanaKeygenSave.txt";

        private const string ExpectedKeyStoreAddress = "4n8BE7DHH4NudifUBrwPbvNPs2F86XcagT7C2JKdrWrR";

        private const string StringKeyStoreSeedWithoutPassphrase =
            "[69,191,12,22,125,16,119,72,240,150,74,197,249,221,54,164,172,222,248,202,22,242,96,43,105,164,101,52,155,41,46,6,107,27,120,68,31,183,113,110,148,151,206,38,195,198,108,78,97,66,196,191,82,41,240,33,253,9,89,19,75,196,171,104]";
        private const string ExpectedStringKeyStoreAddress = "8D6vFRiysWWBwuf3HY7RrPt8EiFoP9o94LzySZqD4HsV";

        private static readonly byte[] SeedWithPassphrase =
        {
            163,4,184,24,182,219,174,214,13,54,158,198,
            63,202,76,3,190,224,76,202,160,96,124,95,89,
            155,113,10,46,218,154,74,125,7,103,78,0,51,
            244,192,221,12,200,148,9,252,4,117,193,123,
            102,56,255,105,167,180,125,222,19,111,219,18,
            115,0
        };

        [TestMethod]
        [ExpectedException(typeof(FileNotFoundException))]
        public void TestKeyStoreFileNotFound()
        {
            _ = KeyStoreService.RestoreKeystoreFromFile(InvalidPath);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestKeyStoreInvalidEmptyFilePath()
        {
            _ = KeyStoreService.RestoreKeystoreFromFile(InvalidEmptyFilePath);
        }

        [TestMethod]
        public void TestKeyStoreValid()
        {
            var wallet = KeyStoreService.RestoreKeystoreFromFile(ValidKeyStorePath);

            Assert.AreEqual(wallet.Account.PublicKey.Key, ExpectedKeyStoreAddress);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestKeyStoreInvalid()
        {
            _ = KeyStoreService.RestoreKeystoreFromFile(InvalidKeyStorePath);
        }

        [TestMethod]
        public void TestKeyStoreFull()
        {
            var walletToSave = new Unity.Wallet.Wallet(SeedWithPassphrase, "bip39passphrase", SeedMode.Bip39);
            KeyStoreService.SaveKeystore(ValidKeyStoreSavePath, walletToSave);
            var restoredWallet = KeyStoreService.RestoreKeystoreFromFile(ValidKeyStorePath, "bip39passphrase");

            Assert.AreEqual(ExpectedKeyStoreAddress, walletToSave.Account.PublicKey.Key);
            Assert.AreEqual(ExpectedKeyStoreAddress, restoredWallet.Account.PublicKey.Key);
        }

        [TestMethod]
        public void TestRestoreKeyStore()
        {
            var wallet = KeyStoreService.RestoreKeystore(StringKeyStoreSeedWithoutPassphrase);
            Assert.AreEqual(ExpectedStringKeyStoreAddress, wallet.Account.PublicKey.Key);
        }

    }
}