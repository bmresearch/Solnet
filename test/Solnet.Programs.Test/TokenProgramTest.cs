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

        private static readonly byte[] ExpectedSetAuthorityOwnerData =
        {
            6, 3, 1, 33, 79, 28, 109, 23, 45, 121, 163, 226, 87, 237, 185,
            47, 29, 248, 108, 218, 51, 132, 22, 227, 114, 38, 230, 154, 241, 16,
            104, 196, 10, 219, 24
        };
        private static readonly byte[] ExpectedSetAuthorityCloseData =
        {
            6, 4, 1, 33, 79, 28, 109, 23, 45, 121, 163, 226, 87, 237, 185,
            47, 29, 248, 108, 218, 51, 132, 22, 227, 114, 38, 230, 154, 241, 16,
            104, 196, 10, 219, 24
        };
        private static readonly byte[] ExpectedSetAuthorityFreezeData =
        {
            6, 2, 1, 33, 79, 28, 109, 23, 45, 121, 163, 226, 87, 237, 185,
            47, 29, 248, 108, 218, 51, 132, 22, 227, 114, 38, 230, 154, 241, 16,
            104, 196, 10, 219, 24
        };
        private static readonly byte[] ExpectedSetAuthorityMintData =
        {
            6, 1, 1, 33, 79, 28, 109, 23, 45, 121, 163, 226, 87, 237, 185,
            47, 29, 248, 108, 218, 51, 132, 22, 227, 114, 38, 230, 154, 241, 16,
            104, 196, 10, 219, 24
        };

        private static readonly byte[] ExpectedCloseAccountData = {9};
        private static readonly byte[] ExpectedFreezeAccountData = {10};
        private static readonly byte[] ExpectedThawAccountData = {11};

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
=======
                TokenProgram.Revoke(
                    delegateAccount.PublicKey,
                    ownerAccount, null);
>>>>>>> Refactoring account and keys for better integration with programs.

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
        public void TestSetAuthorityOwner()
        {
            var wallet = new Wallet.Wallet(MnemonicWords);

            var account = wallet.GetAccount(1000);
            var ownerAccount = wallet.GetAccount(1);
            var newOwnerAccount = wallet.GetAccount(2);
            var signers = new List<Account>();
            
            for (int i = 0; i < 5; i++)
            {
                signers.Add(wallet.GetAccount(420+i));
            }

            var txInstruction =
                TokenProgram.SetAuthority(
                    account.PublicKey, 
                    AuthorityType.AccountOwner, 
                    ownerAccount, 
                    newOwnerAccount.PublicKey);

            Assert.AreEqual(3, txInstruction.Keys.Count);
            CollectionAssert.AreEqual(TokenProgramIdBytes, txInstruction.ProgramId);
            CollectionAssert.AreEqual(ExpectedSetAuthorityOwnerData, txInstruction.Data);
        }
        
        [TestMethod]
        public void TestSetAuthorityOwnerMultiSignature()
        {
            var wallet = new Wallet.Wallet(MnemonicWords);

            var account = wallet.GetAccount(1000);
            var ownerAccount = wallet.GetAccount(1);
            var newOwnerAccount = wallet.GetAccount(2);
            var signers = new List<Account>();
            
            for (int i = 0; i < 5; i++)
            {
                signers.Add(wallet.GetAccount(420+i));
            }

            var txInstruction =
                TokenProgram.SetAuthority(
                    account.PublicKey, 
                    AuthorityType.AccountOwner, 
                    ownerAccount, 
                    newOwnerAccount.PublicKey,
                    signers);

            Assert.AreEqual(8, txInstruction.Keys.Count);
            CollectionAssert.AreEqual(TokenProgramIdBytes, txInstruction.ProgramId);
            CollectionAssert.AreEqual(ExpectedSetAuthorityOwnerData, txInstruction.Data);
        }
        
        [TestMethod]
        public void TestSetAuthorityClose()
        {
            var wallet = new Wallet.Wallet(MnemonicWords);

            var account = wallet.GetAccount(1000);
            var ownerAccount = wallet.GetAccount(1);
            var newOwnerAccount = wallet.GetAccount(2);

            var txInstruction =
                TokenProgram.SetAuthority(
                    account.PublicKey, 
                    AuthorityType.CloseAccount, 
                    ownerAccount, 
                    newOwnerAccount.PublicKey);

            Assert.AreEqual(3, txInstruction.Keys.Count);
            CollectionAssert.AreEqual(TokenProgramIdBytes, txInstruction.ProgramId);
            CollectionAssert.AreEqual(ExpectedSetAuthorityCloseData, txInstruction.Data);
        }
        
        [TestMethod]
        public void TestSetAuthorityFreeze()
        {
            var wallet = new Wallet.Wallet(MnemonicWords);

            var account = wallet.GetAccount(1000);
            var ownerAccount = wallet.GetAccount(1);
            var newOwnerAccount = wallet.GetAccount(2);

            var txInstruction =
                TokenProgram.SetAuthority(
                    account.PublicKey, 
                    AuthorityType.FreezeAccount, 
                    ownerAccount, 
                    newOwnerAccount.PublicKey);

            Assert.AreEqual(3, txInstruction.Keys.Count);
            CollectionAssert.AreEqual(TokenProgramIdBytes, txInstruction.ProgramId);
            CollectionAssert.AreEqual(ExpectedSetAuthorityFreezeData, txInstruction.Data);
        }
        
        [TestMethod]
        public void TestSetAuthorityMint()
        {
            var wallet = new Wallet.Wallet(MnemonicWords);

            var account = wallet.GetAccount(1000);
            var ownerAccount = wallet.GetAccount(1);
            var newOwnerAccount = wallet.GetAccount(2);

            var txInstruction =
                TokenProgram.SetAuthority(
                    account.PublicKey, 
                    AuthorityType.MintTokens, 
                    ownerAccount, 
                    newOwnerAccount.PublicKey);

            Assert.AreEqual(3, txInstruction.Keys.Count);
            CollectionAssert.AreEqual(TokenProgramIdBytes, txInstruction.ProgramId);
            CollectionAssert.AreEqual(ExpectedSetAuthorityMintData, txInstruction.Data);
        }
        
        [TestMethod]
        public void TestBurn()
        {
            var wallet = new Wallet.Wallet(MnemonicWords);

            var mintAccount = wallet.GetAccount(21);
            var ownerAccount = wallet.GetAccount(10);
            var initialAccount = wallet.GetAccount(22);


            var txInstruction =
                TokenProgram.Burn(
                    initialAccount.PublicKey, 
                    mintAccount.PublicKey, 25000UL, ownerAccount);

            Assert.AreEqual(3, txInstruction.Keys.Count);
            CollectionAssert.AreEqual(TokenProgramIdBytes, txInstruction.ProgramId);
            CollectionAssert.AreEqual(ExpectedBurnData, txInstruction.Data);
        }
        
        [TestMethod]
        public void TestBurnMultiSignature()
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
                TokenProgram.Burn(
                    initialAccount.PublicKey, 
                    mintAccount.PublicKey, 25000UL, ownerAccount, signers);

            Assert.AreEqual(8, txInstruction.Keys.Count);
            CollectionAssert.AreEqual(TokenProgramIdBytes, txInstruction.ProgramId);
            CollectionAssert.AreEqual(ExpectedBurnData, txInstruction.Data);
        }
        
        [TestMethod]
        public void TestCloseAccount()
        {
            var wallet = new Wallet.Wallet(MnemonicWords);

            var ownerAccount = wallet.GetAccount(10);
            var initialAccount = wallet.GetAccount(22);

            var txInstruction =
                TokenProgram.CloseAccount(
                    initialAccount.PublicKey, 
                    ownerAccount.PublicKey, 
                    ownerAccount,
                    TokenProgram.ProgramIdKey);

            Assert.AreEqual(3, txInstruction.Keys.Count);
            CollectionAssert.AreEqual(TokenProgramIdBytes, txInstruction.ProgramId);
            CollectionAssert.AreEqual(ExpectedCloseAccountData, txInstruction.Data);
        }
        
        [TestMethod]
        public void TestCloseAccountMultiSignature()
        {
            var wallet = new Wallet.Wallet(MnemonicWords);

            var ownerAccount = wallet.GetAccount(10);
            var initialAccount = wallet.GetAccount(22);
            var signers = new List<Account>();

            for (int i = 0; i < 5; i++)
            {
                signers.Add(wallet.GetAccount(420+i));
            }

            var txInstruction =
                TokenProgram.CloseAccount(
                    initialAccount.PublicKey, 
                    ownerAccount.PublicKey, 
                    ownerAccount,
                    TokenProgram.ProgramIdKey, signers);

            Assert.AreEqual(8, txInstruction.Keys.Count);
            CollectionAssert.AreEqual(TokenProgramIdBytes, txInstruction.ProgramId);
            CollectionAssert.AreEqual(ExpectedCloseAccountData, txInstruction.Data);
        }
        
        [TestMethod]
        public void TestFreezeAccount()
        {
            var wallet = new Wallet.Wallet(MnemonicWords);

            var mintAccount = wallet.GetAccount(21);
            var ownerAccount = wallet.GetAccount(10);
            var initialAccount = wallet.GetAccount(22);

            var txInstruction =
                TokenProgram.FreezeAccount(
                    initialAccount.PublicKey, 
                    mintAccount.PublicKey, 
                    ownerAccount,
                    TokenProgram.ProgramIdKey);

            Assert.AreEqual(3, txInstruction.Keys.Count);
            CollectionAssert.AreEqual(TokenProgramIdBytes, txInstruction.ProgramId);
            CollectionAssert.AreEqual(ExpectedFreezeAccountData, txInstruction.Data);
        }
        
        [TestMethod]
        public void TestFreezeAccountMultiSignature()
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
                TokenProgram.FreezeAccount(
                    initialAccount.PublicKey, 
                    mintAccount.PublicKey, 
                    ownerAccount,
                    TokenProgram.ProgramIdKey, signers);

            Assert.AreEqual(8, txInstruction.Keys.Count);
            CollectionAssert.AreEqual(TokenProgramIdBytes, txInstruction.ProgramId);
            CollectionAssert.AreEqual(ExpectedFreezeAccountData, txInstruction.Data);
        }
        
        [TestMethod]
        public void TestThawAccount()
        {
            var wallet = new Wallet.Wallet(MnemonicWords);

            var mintAccount = wallet.GetAccount(21);
            var ownerAccount = wallet.GetAccount(10);
            var initialAccount = wallet.GetAccount(22);
            var txInstruction =
                TokenProgram.ThawAccount(
                    initialAccount.PublicKey, 
                    mintAccount.PublicKey, 
                    ownerAccount,
                    TokenProgram.ProgramIdKey);

            Assert.AreEqual(3, txInstruction.Keys.Count);
            CollectionAssert.AreEqual(TokenProgramIdBytes, txInstruction.ProgramId);
            CollectionAssert.AreEqual(ExpectedThawAccountData, txInstruction.Data);
        }
        
        [TestMethod]
        public void TestThawAccountMultiSignature()
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
                TokenProgram.ThawAccount(
                    initialAccount.PublicKey, 
                    mintAccount.PublicKey, 
                    ownerAccount,
                    TokenProgram.ProgramIdKey, signers);

            Assert.AreEqual(8, txInstruction.Keys.Count);
            CollectionAssert.AreEqual(TokenProgramIdBytes, txInstruction.ProgramId);
            CollectionAssert.AreEqual(ExpectedThawAccountData, txInstruction.Data);
        }
    }
}