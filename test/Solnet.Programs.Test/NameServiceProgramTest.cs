using Microsoft.VisualStudio.TestTools.UnitTesting;
using Solnet.Wallet;
using System;

namespace Solnet.Programs.Test
{
    [TestClass]
    public class NameServiceProgramTest
    {
        private const string MnemonicWords =
            "route clerk disease box emerge airport loud waste attitude film army tray " +
            "forward deal onion eight catalog surface unit card window walnut wealth medal";

        private const ulong Lamports = 25000UL;

        private static readonly byte[] NameServiceProgramIdBytes =
        {
            11, 173, 81, 244, 19, 193, 243, 169, 148, 96, 217, 0, 216,
            191, 46, 214, 146, 126, 202, 52, 215, 183, 132, 43 , 248, 16,
            169, 115, 8, 45, 30, 220
        };

        private static readonly byte[] ExpectedCreateNameRegistryData =
        {
            0, 32, 0, 0, 0, 96, 132, 162, 10, 103, 37, 156, 104, 198, 141, 215, 249, 118, 148, 76,
            232, 83, 82, 235, 177, 75, 58, 222, 245, 101, 180, 43, 77, 175, 113, 43, 12, 96,
            179, 25, 0, 0, 0, 0, 0, 72, 4, 0, 0
        };
        private static readonly byte[] ExpectedTransferNameRegistryData =
        {
            2, 228, 196, 51, 162, 47, 134, 99, 156, 133, 96, 217, 183, 39, 10, 246,
            58, 117, 0, 198, 160, 46, 245, 35, 25, 58, 83, 127, 244, 97, 11, 79, 178
        };

        private static readonly PublicKey TwitterHandleRegistry =
            new PublicKey("33zp4PEKByAevejja4wZNDpcEK3qz6k6cBHmM2gssW4P");

        private static ulong ReverseRegistryMinBalance = 1684320UL;

        [TestMethod]
        public void TestCreateNameRegistry()
        {
            var wallet = new Wallet.Wallet(MnemonicWords);

            var payer = wallet.GetAccount(10);
            var ownerAccount = wallet.GetAccount(111);

            var txInstruction = NameServiceProgram.CreateNameRegistry(
                TwitterHandleRegistry,
                payer, ownerAccount.PublicKey, ReverseRegistryMinBalance, 1096);

            Assert.AreEqual(6, txInstruction.Keys.Count);
            CollectionAssert.AreEqual(NameServiceProgramIdBytes, txInstruction.ProgramId);
            CollectionAssert.AreEqual(ExpectedCreateNameRegistryData, txInstruction.Data);
        }

        [TestMethod]
        public void TestCreateNameRegistryClass()
        {
            var wallet = new Wallet.Wallet(MnemonicWords);

            var payer = wallet.GetAccount(10);
            var parentNameOwner = wallet.GetAccount(100);
            var ownerAccount = wallet.GetAccount(111);

            var txInstruction = NameServiceProgram.CreateNameRegistry(
                TwitterHandleRegistry,
                payer, ownerAccount.PublicKey,
                ReverseRegistryMinBalance,
                1096,
                payer, parentNameOwner,
                TwitterHandleRegistry);

            Assert.AreEqual(7, txInstruction.Keys.Count);
            CollectionAssert.AreEqual(NameServiceProgramIdBytes, txInstruction.ProgramId);
            CollectionAssert.AreEqual(ExpectedCreateNameRegistryData, txInstruction.Data);
        }

        [TestMethod]
        public void TestUpdateNameRegistry()
        {
            var wallet = new Wallet.Wallet(MnemonicWords);
            var ownerAccount = wallet.GetAccount(10);
            var classAccount = wallet.GetAccount(100);
            var parentNameAccount = wallet.GetAccount(101);

            var txInstruction = NameServiceProgram.UpdateNameRegistry(
                parentNameAccount.PublicKey,
                5,
                new byte[] { 0, 1, 2, 3, 4, 5 },
                ownerAccount, classAccount);

            Assert.AreEqual(3, txInstruction.Keys.Count);
            CollectionAssert.AreEqual(NameServiceProgramIdBytes, txInstruction.ProgramId);
            CollectionAssert.AreEqual(new byte[] { 1, 5, 0, 0, 0, 0, 1, 2, 3, 4, 5 }, txInstruction.Data);
        }

        [TestMethod]
        public void TestTransferName()
        {
            var wallet = new Wallet.Wallet(MnemonicWords);
            var ownerAccount = wallet.GetAccount(10);
            var classAccount = wallet.GetAccount(100);
            var parentNameAccount = wallet.GetAccount(101);
            var newOwnerAccount = wallet.GetAccount(102);

            var txInstruction = NameServiceProgram.TransferNameRegistry(
                parentNameAccount.PublicKey,
                newOwnerAccount.PublicKey,
                ownerAccount, classAccount);

            Assert.AreEqual(3, txInstruction.Keys.Count);
            CollectionAssert.AreEqual(NameServiceProgramIdBytes, txInstruction.ProgramId);
            CollectionAssert.AreEqual(ExpectedTransferNameRegistryData, txInstruction.Data);
        }

        [TestMethod]
        public void TestDeleteNameRegistry()
        {
            var wallet = new Wallet.Wallet(MnemonicWords);
            var ownerAccount = wallet.GetAccount(10);
            var refundAccount = wallet.GetAccount(100);
            var parentNameAccount = wallet.GetAccount(101);

            var txInstruction = NameServiceProgram.DeleteNameRegistry(
                parentNameAccount.PublicKey,
                ownerAccount,
                refundAccount.PublicKey);

            Assert.AreEqual(3, txInstruction.Keys.Count);
            CollectionAssert.AreEqual(NameServiceProgramIdBytes, txInstruction.ProgramId);
            CollectionAssert.AreEqual(new byte[] { 3 }, txInstruction.Data);
        }
    }
}