using Microsoft.VisualStudio.TestTools.UnitTesting;
using Solnet.Wallet;
using System.Collections.Generic;

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
        
        private static readonly byte[] ExpectedInitializeMultiSignatureData = { 2,3 };

        private static readonly byte[] ExpectedMintToData =
        {
            7, 168, 97, 0, 0, 0, 0, 0, 0
        };
        private static readonly byte[] ExpectedMintToCheckedData =
        {
            14, 168, 97, 0, 0, 0, 0, 0, 0,2
        };
        
        private static readonly byte[] ExpectedBurnData =
        {
            8, 168, 97, 0, 0, 0, 0, 0, 0
        };
        private static readonly byte[] ExpectedBurnCheckedData =
        {
            15, 168, 97, 0, 0, 0, 0, 0, 0,2
        };

        private static readonly byte[] ExpectedInitializeAccountData = { 1 };

        private static readonly byte[] ExpectedApproveData = { 4, 168, 97, 0, 0, 0, 0, 0, 0 };
        private static readonly byte[] ExpectedApproveCheckedData = { 13, 168, 97, 0, 0, 0, 0, 0, 0, 2 };

        private static readonly byte[] ExpectedRevokeData = { 5 };

        [TestMethod]
        public void TestTransfer()
        {
            var wallet = new Wallet.Wallet(MnemonicWords);

            var ownerAccount = wallet.GetAccount(10);
            var initialAccount = wallet.GetAccount(24);
            var newAccount = wallet.GetAccount(26);

            var txInstruction = TokenProgram.Transfer(
                initialAccount.PublicKey,
                newAccount.PublicKey,
                25000,
                ownerAccount);

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
                initialAccount.PublicKey,
                newAccount.PublicKey,
                25000,
                2,
                ownerAccount,
                mintAccount.PublicKey);


            Assert.AreEqual(4, txInstruction.Keys.Count);
            CollectionAssert.AreEqual(TokenProgramIdBytes, txInstruction.ProgramId);
            CollectionAssert.AreEqual(ExpectedTransferCheckedData, txInstruction.Data);
        }
        
        [TestMethod]
        public void TestTransferCheckedMultiSignature()
        {
            var wallet = new Wallet.Wallet(MnemonicWords);

            var mintAccount = wallet.GetAccount(21);
            var ownerAccount = wallet.GetAccount(10);
            var initialAccount = wallet.GetAccount(26);
            var newAccount = wallet.GetAccount(27);
            var signers = new List<Account>();
            
            for (int i = 0; i < 5; i++)
            {
                signers.Add(wallet.GetAccount(420+i));
            }
            
            var txInstruction = TokenProgram.TransferChecked(
                initialAccount.PublicKey,
                newAccount.PublicKey,
                25000,
                2,
                ownerAccount,
                mintAccount.PublicKey,
                signers);


            Assert.AreEqual(9, txInstruction.Keys.Count);
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
                    initialAccount.PublicKey,
                    mintAccount.PublicKey,
                    ownerAccount.PublicKey);

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
                mintAccount.PublicKey,
                2,
                ownerAccount.PublicKey,
                ownerAccount.PublicKey);

            Assert.AreEqual(2, txInstruction.Keys.Count);
            CollectionAssert.AreEqual(TokenProgramIdBytes, txInstruction.ProgramId);
            CollectionAssert.AreEqual(ExpectedInitializeMintData, txInstruction.Data);
        }

        [TestMethod]
        public void TestInitializeMultisig()
        {
            var wallet = new Wallet.Wallet(MnemonicWords);

            var multiSig = wallet.GetAccount(420);
            var signers = new List<PublicKey>();

            for (int i = 0; i < 5; i++)
            {
                signers.Add(wallet.GetAccount(420+i).PublicKey);
            }
            var txInstruction = TokenProgram.InitializeMultiSignature(multiSig.PublicKey, signers, 3);
            
            Assert.AreEqual(7, txInstruction.Keys.Count);
            CollectionAssert.AreEqual(TokenProgramIdBytes, txInstruction.ProgramId);
            CollectionAssert.AreEqual(ExpectedInitializeMultiSignatureData, txInstruction.Data);
            
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
                    mintAccount.PublicKey,
                    initialAccount.PublicKey,
                    25000,
                    ownerAccount);

            Assert.AreEqual(3, txInstruction.Keys.Count);
            CollectionAssert.AreEqual(TokenProgramIdBytes, txInstruction.ProgramId);
            CollectionAssert.AreEqual(ExpectedMintToData, txInstruction.Data);
        }
        
        [TestMethod]
        public void TestMintToChecked()
        {
            var wallet = new Wallet.Wallet(MnemonicWords);

            var mintAccount = wallet.GetAccount(21);
            var ownerAccount = wallet.GetAccount(10);
            var initialAccount = wallet.GetAccount(22);

            var txInstruction =
                TokenProgram.MintToChecked(
                    mintAccount.PublicKey,
                    initialAccount.PublicKey,
                    ownerAccount,
                    25000,
                    2);

            Assert.AreEqual(3, txInstruction.Keys.Count);
            CollectionAssert.AreEqual(TokenProgramIdBytes, txInstruction.ProgramId);
            CollectionAssert.AreEqual(ExpectedMintToCheckedData, txInstruction.Data);
        }

        [TestMethod]
        public void TestMintToCheckedMultiSignature()
        {
            var wallet = new Wallet.Wallet(MnemonicWords);

            var mintAccount = wallet.GetAccount(21);
            var ownerAccount = wallet.GetAccount(10);
            var initialAccount = wallet.GetAccount(22);
            var signers = new List<Account>();

            for (int i = 0; i < 5; i++)
            {
                signers.Add(wallet.GetAccount(420+i));
            }
            var txInstruction =
                TokenProgram.MintToChecked(
                    mintAccount.PublicKey,
                    initialAccount.PublicKey,
                    ownerAccount,
                    25000,
                    2, signers);

            Assert.AreEqual(8, txInstruction.Keys.Count);
            CollectionAssert.AreEqual(TokenProgramIdBytes, txInstruction.ProgramId);
            CollectionAssert.AreEqual(ExpectedMintToCheckedData, txInstruction.Data);
        }
        
        [TestMethod]
        public void TestBurnChecked()
        {
            var wallet = new Wallet.Wallet(MnemonicWords);

            var mintAccount = wallet.GetAccount(21);
            var ownerAccount = wallet.GetAccount(10);
            var initialAccount = wallet.GetAccount(22);

            var txInstruction =
                TokenProgram.BurnChecked(
                    mintAccount.PublicKey,
                    initialAccount.PublicKey,
                    ownerAccount,
                    25000,
                    2);

            Assert.AreEqual(3, txInstruction.Keys.Count);
            CollectionAssert.AreEqual(TokenProgramIdBytes, txInstruction.ProgramId);
            CollectionAssert.AreEqual(ExpectedBurnCheckedData, txInstruction.Data);
        }

        [TestMethod]
        public void TestBurnCheckedMultiSignature()
        {
            var wallet = new Wallet.Wallet(MnemonicWords);

            var mintAccount = wallet.GetAccount(21);
            var ownerAccount = wallet.GetAccount(10);
            var initialAccount = wallet.GetAccount(22);
            var signers = new List<Account>();

            for (int i = 0; i < 5; i++)
            {
                signers.Add(wallet.GetAccount(420+i));
            }
            var txInstruction =
                TokenProgram.BurnChecked(
                    mintAccount.PublicKey,
                    initialAccount.PublicKey,
                    ownerAccount,
                    25000,
                    2, signers);

            Assert.AreEqual(8, txInstruction.Keys.Count);
            CollectionAssert.AreEqual(TokenProgramIdBytes, txInstruction.ProgramId);
            CollectionAssert.AreEqual(ExpectedBurnCheckedData, txInstruction.Data);
        }
        
        [TestMethod]
        public void TestApprove()
        {
            var wallet = new Wallet.Wallet(MnemonicWords);

            var sourceAccount = wallet.GetAccount(69);
            var delegateAccount = wallet.GetAccount(420);
            var ownerAccount = wallet.GetAccount(1);

            var txInstruction =
                TokenProgram.Approve(
                    sourceAccount.PublicKey, 
                    delegateAccount.PublicKey,
                    ownerAccount,
                    25000);

            Assert.AreEqual(3, txInstruction.Keys.Count);
            CollectionAssert.AreEqual(TokenProgramIdBytes, txInstruction.ProgramId);
            CollectionAssert.AreEqual(ExpectedApproveData, txInstruction.Data);
        }
        
        [TestMethod]
        public void TestApproveMultiSignature()
        {
            var wallet = new Wallet.Wallet(MnemonicWords);

            var sourceAccount = wallet.GetAccount(69);
            var delegateAccount = wallet.GetAccount(420);
            var ownerAccount = wallet.GetAccount(1);
            var signers = new List<Account>();
            
            for (int i = 0; i < 5; i++)
            {
                signers.Add(wallet.GetAccount(420+i));
            }
            var txInstruction =
                TokenProgram.Approve(
                    sourceAccount.PublicKey, 
                    delegateAccount.PublicKey,
                    ownerAccount,
                    25000, signers);

            Assert.AreEqual(8, txInstruction.Keys.Count);
            CollectionAssert.AreEqual(TokenProgramIdBytes, txInstruction.ProgramId);
            CollectionAssert.AreEqual(ExpectedApproveData, txInstruction.Data);
        }
        
        [TestMethod]
        public void TestApproveChecked()
        {
            var wallet = new Wallet.Wallet(MnemonicWords);

            var mintAccount = wallet.GetAccount(21);
            var sourceAccount = wallet.GetAccount(69);
            var delegateAccount = wallet.GetAccount(420);
            var ownerAccount = wallet.GetAccount(1);
            var signers = new List<Account>();
            
            for (int i = 0; i < 5; i++)
            {
                signers.Add(wallet.GetAccount(420+i));
            }
            
            var txInstruction =
                TokenProgram.ApproveChecked(
                    sourceAccount.PublicKey, 
                    delegateAccount.PublicKey,
                    25000,
                    2,
                    ownerAccount,
                    mintAccount.PublicKey,
                    signers);

            Assert.AreEqual(9, txInstruction.Keys.Count);
            CollectionAssert.AreEqual(TokenProgramIdBytes, txInstruction.ProgramId);
            CollectionAssert.AreEqual(ExpectedApproveCheckedData, txInstruction.Data);
        }
        
        [TestMethod]
        public void TestApproveCheckedMultiSignature()
        {
            var wallet = new Wallet.Wallet(MnemonicWords);

            var sourceAccount = wallet.GetAccount(69);
            var delegateAccount = wallet.GetAccount(420);
            var ownerAccount = wallet.GetAccount(1);
            var signers = new List<Account>();
            
            for (int i = 0; i < 5; i++)
            {
                signers.Add(wallet.GetAccount(420+i));
            }
            var txInstruction =
                TokenProgram.Approve(
                    sourceAccount.PublicKey, 
                    delegateAccount.PublicKey,
                    ownerAccount,
                    25000, signers);

            Assert.AreEqual(8, txInstruction.Keys.Count);
            CollectionAssert.AreEqual(TokenProgramIdBytes, txInstruction.ProgramId);
            CollectionAssert.AreEqual(ExpectedApproveData, txInstruction.Data);
        }

        [TestMethod]
        public void TestRevoke()
        {
            var wallet = new Wallet.Wallet(MnemonicWords);

            var delegateAccount = wallet.GetAccount(420);
            var ownerAccount = wallet.GetAccount(1);

            var txInstruction =
                TokenProgram.Revoke(delegateAccount.PublicKey, ownerAccount);

            Assert.AreEqual(2, txInstruction.Keys.Count);
            CollectionAssert.AreEqual(TokenProgramIdBytes, txInstruction.ProgramId);
            CollectionAssert.AreEqual(ExpectedRevokeData, txInstruction.Data);
        }
        
        [TestMethod]
        public void TestRevokeMultiSignature()
        {
            var wallet = new Wallet.Wallet(MnemonicWords);

            var delegateAccount = wallet.GetAccount(420);
            var ownerAccount = wallet.GetAccount(1);
            var signers = new List<Account>();
            
            for (int i = 0; i < 5; i++)
            {
                signers.Add(wallet.GetAccount(420+i));
            }
            var txInstruction =
                TokenProgram.Revoke(delegateAccount.PublicKey, ownerAccount, signers);

            Assert.AreEqual(7, txInstruction.Keys.Count);
            CollectionAssert.AreEqual(TokenProgramIdBytes, txInstruction.ProgramId);
            CollectionAssert.AreEqual(ExpectedRevokeData, txInstruction.Data);
        }

        [TestMethod]
        public void TestSetAuthority()
        {
            
        }
        
        [TestMethod]
        public void TestSetAuthorityMultiSignature()
        {
            
        }
        
        [TestMethod]
        public void TestBurn()
        {
            
        }
        
        [TestMethod]
        public void TestBurnMultiSignature()
        {
            
        }
        
        [TestMethod]
        public void TestCloseAccount()
        {
            
        }
        
        [TestMethod]
        public void TestCloseAccountMultiSignature()
        {
            
        }
        
        [TestMethod]
        public void TestFreezeAccount()
        {
            
        }
        
        [TestMethod]
        public void TestFreezeAccountMultiSignature()
        {
            
        }
        
        [TestMethod]
        public void TestThawAccount()
        {
            
        }
        
        [TestMethod]
        public void TestThawAccountMultiSignature()
        {
            
        }
    }
}