using Microsoft.VisualStudio.TestTools.UnitTesting;
using Solnet.Wallet.Bip39;
using System;
using System.Collections.Generic;

namespace Solnet.Wallet.Test
{
    [TestClass]
    public class WalletTest
    {
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
        private static readonly byte[] SerializedMessageSignatureBip39 =
        {
            28, 126, 243, 240, 127, 153, 168, 18, 202, 11, 27, 255, 242, 180, 193, 230, 100,
            109, 213, 104, 22, 230, 164, 231, 20, 10, 64, 213, 212, 108, 210, 59, 174, 106,
            61, 254, 120, 250, 15, 109, 254, 142, 88, 176, 145, 111, 0, 231, 29, 225, 10, 193,
            135, 130, 54, 21, 25, 48, 147, 4, 138, 171, 252, 15
        };

        private const string MnemonicWords =
            "lens scheme misery search address destroy shallow police picture gown apart rural cotton vivid cage disagree enrich govern history kit early near cloth alarm";
        private const string Bip39Passphrase = "bip39passphrase";

        private static readonly Mnemonic Mnemonic = new(MnemonicWords);

        /// <summary>
        /// Expected key pairs from wallet initialization using the above parameters, as output from sollet.io
        /// </summary>
        private static readonly List<(string PublicKey, string PrivateKey)> ExpectedSolletKeys = new()
        {
            ("ALSzrjtGi8MZGmAZa6ZhtUZq3rwurWuJqWFdgcj9MMFL",
                "5ZD7ntKtyHrnqMhfSuKBLdqHzT5N3a2aYnCGBcz4N78b84TKpjwQ4QBsapEnpnZFchM7F1BpqDkSuLdwMZwM8hLi"),
            ("CgFKZ1VLJvip93rh7qKqiGwZjxXb4XXC4GhBGBizuWUb",
                "5hTHMuq5vKJachfenfKeAoDhMttXFfN77G51L8KiVRsZqRmzFvNLUdMFDRYgTfuX6yy9g6gCpatzray4XFX5B8xb"),
            ("C6jL32xjsGr9fmMdd56TF9oQURN19EfemFxkdpzRoyxm",
                "UYhpZrPoRGvHur6ZunZT6VraiTC85NjGsFDrm8LLx3kZkThHEUGSkAuJhn2KUAt2o2Nf3EeFhEW52REzmD3iPgV")
        };

        /// <summary>
        /// Expected key pair from wallet initialization using the above parameters, as output from solana-keygen cli tool
        /// </summary>
        private const string ExpectedSolanaKeygenPrivateKey = "4G39ryne39vSdXj8v2dVEuN7jMrbMLRD6BtPXydtHoqHHs8SyTAvtjScrzGxvUDo4p6Fz3QaxqF3FUHxn3k68D6M";
        private const string ExpectedSolanaKeygenPublicKey = "4n8BE7DHH4NudifUBrwPbvNPs2F86XcagT7C2JKdrWrR";

        private static Wallet SetupWalletFromMnemonicWords(SeedMode seedMode)
        {
            return seedMode switch
            {
                SeedMode.Bip39 => new Wallet(MnemonicWords, WordList.English, Bip39Passphrase, SeedMode.Bip39),
                SeedMode.Ed25519Bip32 => new Wallet(MnemonicWords, WordList.English),
                _ => throw new ArgumentOutOfRangeException(nameof(seedMode), seedMode, "this should never happen")
            };
        }

        private static Wallet SetupWalletFromSeed(SeedMode seedMode)
        {
            return seedMode switch
            {
                SeedMode.Bip39 => new Wallet(SeedWithPassphrase, Bip39Passphrase, SeedMode.Bip39),
                SeedMode.Ed25519Bip32 => new Wallet(SeedWithoutPassphrase),
                _ => throw new ArgumentOutOfRangeException(nameof(seedMode), seedMode, "this should never happen")
            };
        }

        private static Wallet SetupWalletFromMnemonic(SeedMode seedMode)
        {
            return seedMode switch
            {
                SeedMode.Bip39 => new Wallet(Mnemonic, Bip39Passphrase, SeedMode.Bip39),
                SeedMode.Ed25519Bip32 => new Wallet(Mnemonic),
                _ => throw new ArgumentOutOfRangeException(nameof(seedMode), seedMode, "this should never happen")
            };
        }

        [TestMethod]
        public void TestWallet()
        {
            var wallet = new Wallet(WordCount.TwentyFour, WordList.English);
            Assert.IsNotNull(wallet.Account, "_wallet.account != null");
            Assert.IsNotNull(wallet.Account.PrivateKey, "_wallet.account.PrivateKey != null");
            Assert.IsNotNull(wallet.Account.PublicKey, "_wallet.account.PublicKey != null");
            Assert.IsNotNull(wallet.Account.PrivateKey.KeyBytes, "_wallet.account.PrivateKeyBytes != null");
            Assert.IsNotNull(wallet.Account.PublicKey.KeyBytes, "_wallet.account.PublicKeyBytes != null");

            var signature = wallet.Account.Sign(SerializedMessage);
            Assert.IsTrue(wallet.Account.Verify(SerializedMessage, signature));

            var sig = wallet.Sign(SerializedMessage, 2);
            Assert.IsTrue(wallet.Verify(SerializedMessage, sig, 2));
        }

        [TestMethod]
        public void TestWalletEd25519Bip32FromWords()
        {
            var wallet = SetupWalletFromMnemonicWords(SeedMode.Ed25519Bip32);

            CollectionAssert.AreEqual(SeedWithoutPassphrase, wallet.DeriveMnemonicSeed());

            for (var i = 0; i < ExpectedSolletKeys.Count; i++)
            {
                var account = wallet.GetAccount(i);
                Assert.AreEqual(ExpectedSolletKeys[i].PublicKey, account.PublicKey.Key);
                Assert.AreEqual(ExpectedSolletKeys[i].PrivateKey, account.PrivateKey.Key);
            }
        }

        [TestMethod]
        public void TestWalletBip39FromWords()
        {
            var wallet = SetupWalletFromMnemonicWords(SeedMode.Bip39);

            CollectionAssert.AreEqual(SeedWithPassphrase, wallet.DeriveMnemonicSeed());

            Assert.AreEqual(ExpectedSolanaKeygenPublicKey, wallet.Account.PublicKey.Key);
            Assert.AreEqual(ExpectedSolanaKeygenPrivateKey, wallet.Account.PrivateKey.Key);
        }

        [TestMethod]
        public void TestWalletEd25519Bip32FromSeed()
        {
            var wallet = SetupWalletFromSeed(SeedMode.Ed25519Bip32);

            CollectionAssert.AreEqual(SeedWithoutPassphrase, wallet.DeriveMnemonicSeed());

            for (var i = 0; i < ExpectedSolletKeys.Count; i++)
            {
                var account = wallet.GetAccount(i);
                Assert.AreEqual(ExpectedSolletKeys[i].PublicKey, account.PublicKey.Key);
                Assert.AreEqual(ExpectedSolletKeys[i].PrivateKey, account.PrivateKey.Key);
            }
        }

        [TestMethod]
        public void TestWalletBip39FromSeed()
        {
            var wallet = SetupWalletFromSeed(SeedMode.Bip39);

            CollectionAssert.AreEqual(SeedWithPassphrase, wallet.DeriveMnemonicSeed());

            Assert.AreEqual(ExpectedSolanaKeygenPublicKey, wallet.Account.PublicKey.Key);
            Assert.AreEqual(ExpectedSolanaKeygenPrivateKey, wallet.Account.PrivateKey.Key);
        }


        [TestMethod]
        public void TestWalletEd25519Bip32FromMnemonic()
        {
            var wallet = SetupWalletFromMnemonic(SeedMode.Ed25519Bip32);

            CollectionAssert.AreEqual(SeedWithoutPassphrase, wallet.DeriveMnemonicSeed());

            for (var i = 0; i < ExpectedSolletKeys.Count; i++)
            {
                var account = wallet.GetAccount(i);
                Assert.AreEqual(ExpectedSolletKeys[i].PublicKey, account.PublicKey.Key);
                Assert.AreEqual(ExpectedSolletKeys[i].PrivateKey, account.PrivateKey.Key);
            }
        }

        [TestMethod]
        public void TestWalletBip39FromMnemonic()
        {
            var wallet = SetupWalletFromMnemonic(SeedMode.Bip39);

            CollectionAssert.AreEqual(SeedWithPassphrase, wallet.DeriveMnemonicSeed());

            Assert.AreEqual(ExpectedSolanaKeygenPublicKey, wallet.Account.PublicKey.Key);
            Assert.AreEqual(ExpectedSolanaKeygenPrivateKey, wallet.Account.PrivateKey.Key);
        }

        [TestMethod]
        public void TestAccountSignEd25519Bip32()
        {
            var wallet = SetupWalletFromMnemonicWords(SeedMode.Ed25519Bip32);

            CollectionAssert.AreEqual(SerializedMessageSignature, wallet.Account.Sign(SerializedMessage));
            CollectionAssert.AreEqual(SerializedMessageSignature, wallet.GetAccount(0).Sign(SerializedMessage));
        }

        [TestMethod]
        public void TestWalletSignEd25519Bip32()
        {
            var wallet = SetupWalletFromMnemonicWords(SeedMode.Ed25519Bip32);

            CollectionAssert.AreEqual(SerializedMessageSignature, wallet.Account.Sign(SerializedMessage));
            CollectionAssert.AreEqual(SerializedMessageSignature, wallet.Sign(SerializedMessage));
        }

        [TestMethod]
        public void TestAccountVerifyEd25519Bip32()
        {
            var wallet = SetupWalletFromMnemonicWords(SeedMode.Ed25519Bip32);

            Assert.IsTrue(wallet.Account.Verify(SerializedMessage, SerializedMessageSignature));
            Assert.IsTrue(wallet.GetAccount(0).Verify(SerializedMessage, SerializedMessageSignature));
        }

        [TestMethod]
        public void TestWalletVerifyEd25519Bip32()
        {
            var wallet = SetupWalletFromMnemonicWords(SeedMode.Ed25519Bip32);

            Assert.IsTrue(wallet.Account.Verify(SerializedMessage, SerializedMessageSignature));
            Assert.IsTrue(wallet.Verify(SerializedMessage, SerializedMessageSignature));
        }

        [TestMethod]
        public void TestWalletSignBip39()
        {
            var wallet = SetupWalletFromMnemonicWords(SeedMode.Bip39);

            Assert.ThrowsException<Exception>(() => wallet.Sign(SerializedMessage, 1));
            Assert.ThrowsException<Exception>(() => wallet.GetAccount(0).Sign(SerializedMessage));

            CollectionAssert.AreEqual(SerializedMessageSignatureBip39, wallet.Sign(SerializedMessage));
        }

        [TestMethod]
        public void TestAccountSignBip39()
        {
            var wallet = SetupWalletFromMnemonicWords(SeedMode.Bip39);

            Assert.ThrowsException<Exception>(() => wallet.Sign(SerializedMessage, 1));
            Assert.ThrowsException<Exception>(() => wallet.GetAccount(0).Sign(SerializedMessage));

            CollectionAssert.AreEqual(SerializedMessageSignatureBip39, wallet.Account.Sign(SerializedMessage));
        }

        [TestMethod]
        public void TestWalletVerifyBip39()
        {
            var wallet = SetupWalletFromMnemonicWords(SeedMode.Bip39);

            Assert.ThrowsException<Exception>(() => wallet.Verify(SerializedMessage, SerializedMessageSignature, 1));
            Assert.ThrowsException<Exception>(() => wallet.GetAccount(0).Verify(SerializedMessage, SerializedMessageSignature));

            Assert.IsTrue(wallet.Account.Verify(SerializedMessage, SerializedMessageSignatureBip39));
        }
    }
}