using Microsoft.VisualStudio.TestTools.UnitTesting;
using Solnet.Wallet;
using System;
using System.IO;

namespace Solnet.KeyStore.Test
{
    [TestClass]
    public class SolanaKeygenKeyStoreTest
    {
        private const string InvalidPath = "Resources/DoesNotExist.txt";
        private const string InvalidEmptyFilePath = "Resources/InvalidEmptyFile.txt";
        private const string ValidKeyStorePath = "Resources/ValidSolanaKeygenKeyStore.txt";
        private const string InvalidKeyStorePath = "Resources/InvalidSolanaKeygenKeyStore.txt";
        private const string ValidKeyStoreSavePath = "Resources/ValidSolanaKeygenSave.txt";

        private const string ExpectedKeyStoreAddress = "4n8BE7DHH4NudifUBrwPbvNPs2F86XcagT7C2JKdrWrR";

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
            var keyStore = new SolanaKeyStoreService();
            _ = keyStore.RestoreKeystore(InvalidPath);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestKeyStoreInvalidEmptyFilePath()
        {
            var keyStore = new SolanaKeyStoreService();
            _ = keyStore.RestoreKeystore(InvalidEmptyFilePath);
        }

        [TestMethod]
        public void TestKeyStoreValid()
        {
            var keyStore = new SolanaKeyStoreService();
            var wallet = keyStore.RestoreKeystore(ValidKeyStorePath);

            Assert.AreEqual(wallet.Account.GetPublicKey, ExpectedKeyStoreAddress);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestKeyStoreInvalid()
        {
            var keyStore = new SolanaKeyStoreService();
            _ = keyStore.RestoreKeystore(InvalidKeyStorePath);
        }

        [TestMethod]
        public void TestKeyStoreFull()
        {
            var keyStore = new SolanaKeyStoreService();
            var walletToSave = new Wallet.Wallet(SeedWithPassphrase, "bip39passphrase", SeedMode.Bip39);
            keyStore.SaveKeystore(ValidKeyStoreSavePath, walletToSave);
            var restoredWallet = keyStore.RestoreKeystore(ValidKeyStorePath, "bip39passphrase");

            Assert.AreEqual(ExpectedKeyStoreAddress, walletToSave.Account.GetPublicKey);
            Assert.AreEqual(ExpectedKeyStoreAddress, restoredWallet.Account.GetPublicKey);
        }

        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void TestKeyStoreDecryptNotImplemented()
        {

            var keyStore = new SolanaKeyStoreService();
            _ = keyStore.DecryptAndRestoreKeystore("Some/Path");
        }

        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void TestKeyStoreEncryptNotImplemented()
        {
            var keyStore = new SolanaKeyStoreService();
            keyStore.EncryptAndSaveKeystore("Some/Path", new Wallet.Wallet(SeedWithPassphrase));
        }
    }
}