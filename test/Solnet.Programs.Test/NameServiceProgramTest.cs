using Microsoft.VisualStudio.TestTools.UnitTesting;
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
            11, 173, 81, 244, 19, 193, 243, 169, 148, 96, 217, 0, 216,
            191, 46, 214, 146, 126, 202, 52, 215, 183, 132, 43 , 248, 16,
            169, 115, 8, 45, 30, 220
        };
        
        [TestMethod]
        public void TestCreateNameRegistry()
        {
            
            var wallet = new Wallet.Wallet(MnemonicWords);

            var payer = wallet.GetAccount(10);
            var ownerAccount = wallet.GetAccount(111);
        }
        
        [TestMethod]
        public void TestCreateNameRegistryClass()
        {
            var wallet = new Wallet.Wallet(MnemonicWords);
            var ownerAccount = wallet.GetAccount(10);
            var classAccount = wallet.GetAccount(110);
        }
        
        [TestMethod]
        public void TestCreateNameRegistryParentName()
        {
            var wallet = new Wallet.Wallet(MnemonicWords);
            var ownerAccount = wallet.GetAccount(10);
            var classAccount = wallet.GetAccount(100);
            var parentNameAccount = wallet.GetAccount(101);

        }
        
        [TestMethod]
        public void TestUpdateNameRegistryOwner()
        {
            var wallet = new Wallet.Wallet(MnemonicWords);
            var ownerAccount = wallet.GetAccount(10);
            var classAccount = wallet.GetAccount(100);
            var parentNameAccount = wallet.GetAccount(101);
            var txInstruction = NameServiceProgram.UpdateNameRegistry(
                parentNameAccount.PublicKey,
                5, 
                new byte[] { 0, 1, 2, 3, 4, 5},
                ownerAccount);
            
            
            Assert.AreEqual(2, txInstruction.Keys.Count);
        }
        
        [TestMethod]
        public void TestUpdateNameRegistryClass()
        {
            var wallet = new Wallet.Wallet(MnemonicWords);
            var ownerAccount = wallet.GetAccount(10);
            var classAccount = wallet.GetAccount(100);
            var parentNameAccount = wallet.GetAccount(101);
            
            var txInstruction = NameServiceProgram.UpdateNameRegistry(
                parentNameAccount.PublicKey,
                5, 
                new byte[] { 0, 1, 2, 3, 4, 5},
                nameClass: classAccount);
            
            Assert.AreEqual(2, txInstruction.Keys.Count);
        }

        [TestMethod]
        public void TestTransferNameRegistryOwner()
        {
            var wallet = new Wallet.Wallet(MnemonicWords);
            var ownerAccount = wallet.GetAccount(10);
            var parentNameAccount = wallet.GetAccount(101);
            var newOwnerAccount = wallet.GetAccount(102);
            
            var txInstruction = NameServiceProgram.TransferNameRegistry(
                parentNameAccount.PublicKey, 
                newOwnerAccount.PublicKey, 
                ownerAccount);
            
            Assert.AreEqual(2, txInstruction.Keys.Count);
        }
        
        [TestMethod]
        public void TestTransferNameRegistryClass()
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
        }

    }
}