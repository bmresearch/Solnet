using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Solnet.Rpc.Builders;
using Solnet.Wallet;

namespace Solnet.Programs.Test
{
    [TestClass]
    public class TokenProgramTest
    {
        private const string MnemonicWords =
            "route clerk disease box emerge airport loud waste attitude film army tray " +
            "forward deal onion eight catalog surface unit card window walnut wealth medal";

        private static readonly byte[] TokenProgramIdBytes =
        {
            6, 221, 246, 225, 215, 101, 161, 147, 217, 203,
            225, 70, 206, 235, 121, 172, 28, 180, 133, 237,
            95, 91, 55, 145, 58, 140, 245, 133, 126, 255, 0, 169
        };

        private static readonly byte[] ExpectedTransferData =
        {
            3, 168, 97, 0, 0, 0, 0, 0, 0
        };

        private static readonly byte[] ExpectedTransferCheckedData =
        {
            12, 168, 97, 0, 0, 0, 0, 0, 0, 2
        };

        private static readonly byte[] ExpectedInitializeMintData =
        {
            0, 2, 71, 105, 171, 151, 32, 75, 168, 63, 176, 202, 238, 23, 247, 134, 143, 30, 7, 78, 82, 21, 129,
            160, 216, 157, 148, 55, 157, 170, 101, 183, 23, 178, 1, 71, 105, 171, 151, 32, 75, 168, 63, 176,
            202, 238, 23, 247, 134, 143, 30, 7, 78, 82, 21, 129, 160, 216, 157, 148, 55, 157, 170, 101, 183, 23, 178
        };

        private static readonly byte[] ExpectedMintToData =
        {
            7, 168, 97, 0, 0, 0, 0, 0, 0
        };

        private static readonly byte[] ExpectedInitializeAccountData = {1};

        [TestMethod]
        public void TestTransfer()
        {
            var wallet = new Wallet.Wallet(MnemonicWords);

            var ownerAccount = wallet.GetAccount(10);
            var initialAccount = wallet.GetAccount(24);
            var newAccount = wallet.GetAccount(26);

            var txInstruction = TokenProgram.Transfer(
                initialAccount.GetPublicKey,
                newAccount.GetPublicKey,
                25000,
                ownerAccount.GetPublicKey);

            Assert.AreEqual(3, txInstruction.Keys.Count);
            CollectionAssert.AreEqual(TokenProgramIdBytes, txInstruction.ProgramId);
            CollectionAssert.AreEqual(ExpectedTransferData, txInstruction.Data);
        }

        [TestMethod]
        public void TestTransferChecked()
        {
            var wallet = new Wallet.Wallet(MnemonicWords);

            var mintAccount = wallet.GetAccount(21);
            var ownerAccount = wallet.GetAccount(10);
            var initialAccount = wallet.GetAccount(26);
            var newAccount = wallet.GetAccount(27);

            var txInstruction = TokenProgram.TransferChecked(
                initialAccount.GetPublicKey,
                newAccount.GetPublicKey,
                25000,
                2,
                ownerAccount.GetPublicKey,
                mintAccount.GetPublicKey);


            Assert.AreEqual(4, txInstruction.Keys.Count);
            CollectionAssert.AreEqual(TokenProgramIdBytes, txInstruction.ProgramId);
            CollectionAssert.AreEqual(ExpectedTransferCheckedData, txInstruction.Data);
        }

        [TestMethod]
        public void TestInitializeAccount()
        {
            var wallet = new Wallet.Wallet(MnemonicWords);

            var mintAccount = wallet.GetAccount(21);
            var ownerAccount = wallet.GetAccount(10);
            var initialAccount = wallet.GetAccount(22);

            var txInstruction =
                TokenProgram.InitializeAccount(
                    initialAccount.GetPublicKey,
                    mintAccount.GetPublicKey,
                    ownerAccount.GetPublicKey);

            Assert.AreEqual(4, txInstruction.Keys.Count);
            CollectionAssert.AreEqual(TokenProgramIdBytes, txInstruction.ProgramId);
            CollectionAssert.AreEqual(ExpectedInitializeAccountData, txInstruction.Data);
        }

        [TestMethod]
        public void TestInitializeMint()
        {
            var wallet = new Wallet.Wallet(MnemonicWords);

            var mintAccount = wallet.GetAccount(21);
            var ownerAccount = wallet.GetAccount(10);

            var txInstruction = TokenProgram.InitializeMint(
                mintAccount.GetPublicKey,
                2,
                ownerAccount.GetPublicKey,
                ownerAccount.GetPublicKey);

            Assert.AreEqual(2, txInstruction.Keys.Count);
            CollectionAssert.AreEqual(TokenProgramIdBytes, txInstruction.ProgramId);
            CollectionAssert.AreEqual(ExpectedInitializeMintData, txInstruction.Data);
        }

        [TestMethod]
        public void TestMintTo()
        {
            var wallet = new Wallet.Wallet(MnemonicWords);

            var mintAccount = wallet.GetAccount(21);
            var ownerAccount = wallet.GetAccount(10);
            var initialAccount = wallet.GetAccount(22);

            var txInstruction =
                TokenProgram.MintTo(
                    mintAccount.GetPublicKey,
                    initialAccount.GetPublicKey,
                    25000,
                    ownerAccount.GetPublicKey);

            Assert.AreEqual(3, txInstruction.Keys.Count);
            CollectionAssert.AreEqual(TokenProgramIdBytes, txInstruction.ProgramId);
            CollectionAssert.AreEqual(ExpectedMintToData, txInstruction.Data);
        }
    }
}