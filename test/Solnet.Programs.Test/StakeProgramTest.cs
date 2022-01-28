using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Solnet.Programs.Models.Stake.State;

namespace Solnet.Programs.Test
{
    [TestClass]
    public class StakeProgramTest
    {
        private const string MnemonicWords =
            "hockey impact dove smoke hard garlic liberty rabbit equip fiction glue badge broken " +
            "version pattern anchor science obtain hundred rug blanket defense mistake glove";

        private static readonly byte[] StakeProgramIdBytes =
        {
            6, 161, 216, 23, 145, 55, 84, 42, 152, 52, 55, 189, 254, 42, 122, 178, 85, 127, 83, 92, 138, 120, 114, 43, 104, 164, 157, 192, 0, 0, 0, 0
        };
        private static readonly byte[] InitializeInstructionBytes =
        {
            0, 0, 0, 0, 76, 174, 165, 233, 163, 14, 217, 33, 70, 126, 243, 60, 190, 239, 79, 31, 224, 40, 233, 215, 221, 65, 203, 24, 160, 209, 227, 94, 135, 248, 122, 74, 248, 84, 219, 115, 8, 125, 192, 165, 16, 40, 75, 233, 213, 1, 60, 251, 227, 240, 54, 187, 56, 156, 251, 152, 129, 168, 217, 86, 76, 98, 88, 24, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 13, 223, 26, 29, 16, 236, 203, 38, 73, 235, 143, 32, 67, 134, 192, 95, 200, 236, 229, 110, 1, 16, 230, 255, 188, 14, 101, 132, 109, 242, 124, 105
        };
        private static readonly byte[] AuthorizeInstructionBytes =
        {
            1, 0, 0, 0, 248, 84, 219, 115, 8, 125, 192, 165, 16, 40, 75, 233, 213, 1, 60, 251, 227, 240, 54, 187, 56, 156, 251, 152, 129, 168, 217, 86, 76, 98, 88, 24, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
        };
        private static readonly byte[] DelegateStakeInstructionBytes =
        {
            2, 0, 0, 0
        };
        private static readonly byte[] SplitInstructionBytes =
        {
            3, 0, 0, 0, 64, 66, 15, 0, 0, 0, 0, 0
        };
        private static readonly byte[] WithdrawInstructionBytes =
        {
           4, 0, 0, 0, 64, 66, 15, 0, 0, 0, 0, 0
        };
        private static readonly byte[] DeactivateInstructionBytes =
        {
            5, 0, 0, 0
        };
        private static readonly byte[] SetLockupInstructionBytes =
        {
            6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 206, 68, 136, 104, 203, 252, 147, 24, 24, 140, 114, 18, 199, 188, 229, 248, 250, 220, 24, 172, 85, 160, 190, 232, 244, 250, 90, 60, 108, 252, 171, 144
        };
        private static readonly byte[] MergeInstructionBytes =
        {
            7, 0, 0, 0
        };
        private static readonly byte[] AuthorizeWithSeedInstructionBytes =
        {
            8, 0, 0, 0, 248, 84, 219, 115, 8, 125, 192, 165, 16, 40, 75, 233, 213, 1, 60, 251, 227, 240, 54, 187, 56, 156, 251, 152, 129, 168, 217, 86, 76, 98, 88, 24, 0, 0, 0, 0, 12, 0, 0, 0, 0, 0, 0, 0, 116, 101, 115, 116, 84, 104, 105, 115, 83, 101, 101, 100, 206, 68, 136, 104, 203, 252, 147, 24, 24, 140, 114, 18, 199, 188, 229, 248, 250, 220, 24, 172, 85, 160, 190, 232, 244, 250, 90, 60, 108, 252, 171, 144
        };
        private static readonly byte[] InitializeCheckedInstructionBytes =
        {
            9, 0, 0, 0
        };
        private static readonly byte[] AuthorizeCheckedInstructionBytes =
        {
            1, 0, 0, 0, 0, 0, 0, 0
        };
        private static readonly byte[] AuthorizeCheckedWithSeedInstructionBytes =
        {
            11, 0, 0, 0, 12, 0, 0, 0, 0, 0, 0, 0, 116, 101, 115, 116, 84, 104, 105, 115, 83, 101, 101, 100, 0, 0, 0, 0, 206, 68, 136, 104, 203, 252, 147, 24, 24, 140, 114, 18, 199, 188, 229, 248, 250, 220, 24, 172, 85, 160, 190, 232, 244, 250, 90, 60, 108, 252, 171, 144
        };
        private static readonly byte[] SetLockupCheckedInstructionBytes =
        {
            6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
        };
        [TestMethod]
        public void TestStakeProgramInitialize()
        {
            var wallet = new Wallet.Wallet(mnemonicWords: MnemonicWords);

            var baseAccount = wallet.GetAccount(6);
            var staker = wallet.GetAccount(5);
            var withdrawer = wallet.GetAccount(4);
            var custodian = wallet.GetAccount(3);

            var txInstruction = StakeProgram.Initialize(
                baseAccount.PublicKey,
                new Authorized 
                { 
                    Staker = staker, 
                    Withdrawer = withdrawer 
                },
                new Lockup { 
                    Custodian = custodian.PublicKey, 
                    Epoch = 0, 
                    UnixTimestamp = 0
                });

            Assert.AreEqual(2, txInstruction.Keys.Count);

            CollectionAssert.AreEqual(InitializeInstructionBytes, txInstruction.Data);
            CollectionAssert.AreEqual(StakeProgramIdBytes, txInstruction.ProgramId);
        }
        [TestMethod]
        public void TestStakeProgramAuthorize()
        {
            var wallet = new Wallet.Wallet(mnemonicWords: MnemonicWords);

            var baseAccount = wallet.GetAccount(6);
            var authorizedAccount = wallet.GetAccount(5);
            var newAuthorizedAccount = wallet.GetAccount(4);
            var custodian = wallet.GetAccount(3);

            var txInstruction = StakeProgram.Authorize(
                baseAccount.PublicKey, 
                authorizedAccount.PublicKey, 
                newAuthorizedAccount.PublicKey,
                StakeAuthorize.Staker, 
                custodian.PublicKey);

            Assert.AreEqual(4, txInstruction.Keys.Count);

            CollectionAssert.AreEqual(AuthorizeInstructionBytes, txInstruction.Data);
            CollectionAssert.AreEqual(StakeProgramIdBytes, txInstruction.ProgramId);
        }
        [TestMethod]
        public void TestStakeProgramDelegateStake()
        {
            var wallet = new Wallet.Wallet(mnemonicWords: MnemonicWords);

            var baseAccount = wallet.GetAccount(6);
            var authorizedAccount = wallet.GetAccount(5);
            var voteAccount = wallet.GetAccount(4);

            var txInstruction = StakeProgram.DelegateStake(
                baseAccount.PublicKey,
                authorizedAccount.PublicKey, 
                voteAccount.PublicKey);

            Assert.AreEqual(6, txInstruction.Keys.Count);
            CollectionAssert.AreEqual(DelegateStakeInstructionBytes, txInstruction.Data);
            CollectionAssert.AreEqual(StakeProgramIdBytes, txInstruction.ProgramId);
        }
        [TestMethod]
        public void TestStakeProgramSplitStake()
        {
            var wallet = new Wallet.Wallet(mnemonicWords: MnemonicWords);

            var baseAccount = wallet.GetAccount(6);
            var splitAccount = wallet.GetAccount(5);
            var authorizedAccount = wallet.GetAccount(4);

            var txInstruction = StakeProgram.Split(
                baseAccount.PublicKey,
                authorizedAccount.PublicKey,
                1_000_000,
                splitAccount.PublicKey);

            Assert.AreEqual(3, txInstruction.Keys.Count);
            CollectionAssert.AreEqual(SplitInstructionBytes, txInstruction.Data);
            CollectionAssert.AreEqual(StakeProgramIdBytes, txInstruction.ProgramId);
        }
        [TestMethod]
        public void TestStakeProgramWithdrawStake()
        {
            var wallet = new Wallet.Wallet(mnemonicWords: MnemonicWords);

            var baseAccount = wallet.GetAccount(6);
            var withdrawer = wallet.GetAccount(4);
            var to = wallet.GetAccount(3);
            var custodian = wallet.GetAccount(2);

            var txInstruction = StakeProgram.Withdraw(
                baseAccount.PublicKey,
                withdrawer.PublicKey,
                to.PublicKey,
                1_000_000,
                custodian.PublicKey);

            Assert.AreEqual(6, txInstruction.Keys.Count);
            
            CollectionAssert.AreEqual(WithdrawInstructionBytes, txInstruction.Data);
            CollectionAssert.AreEqual(StakeProgramIdBytes, txInstruction.ProgramId);
        }

        [TestMethod]
        public void TestStakeProgramDeactivateStake()
        {
            var wallet = new Wallet.Wallet(mnemonicWords: MnemonicWords);

            var baseAccount = wallet.GetAccount(6);
            var authorized = wallet.GetAccount(5);

            var txInstruction = StakeProgram.Deactivate(
                baseAccount.PublicKey,
                authorized.PublicKey);

            Assert.AreEqual(3, txInstruction.Keys.Count);
            CollectionAssert.AreEqual(DeactivateInstructionBytes, txInstruction.Data);
            CollectionAssert.AreEqual(StakeProgramIdBytes, txInstruction.ProgramId);
        }
        [TestMethod]
        public void TestStakeProgramSetLockup()
        {
            var wallet = new Wallet.Wallet(mnemonicWords: MnemonicWords);

            var baseAccount = wallet.GetAccount(6);
            var custodian = wallet.GetAccount(2);

            var txInstruction = StakeProgram.SetLockup(
                baseAccount.PublicKey,
                new Lockup 
                { 
                    Custodian = custodian.PublicKey, 
                    Epoch = 0, 
                    UnixTimestamp = 0 
                },
                custodian.PublicKey);

            Assert.AreEqual(2, txInstruction.Keys.Count);
            CollectionAssert.AreEqual(SetLockupInstructionBytes, txInstruction.Data);
            CollectionAssert.AreEqual(StakeProgramIdBytes, txInstruction.ProgramId);
        }
        [TestMethod]
        public void TestStakeProgramMergeStake()
        {
            var wallet = new Wallet.Wallet(mnemonicWords: MnemonicWords);

            var destinationAccount = wallet.GetAccount(6);
            var source = wallet.GetAccount(5);
            var authorized = wallet.GetAccount(4);

            var txInstruction = StakeProgram.Merge(
                destinationAccount.PublicKey,
                source.PublicKey,
                authorized.PublicKey);

            Assert.AreEqual(5, txInstruction.Keys.Count);
            CollectionAssert.AreEqual(MergeInstructionBytes, txInstruction.Data);
            CollectionAssert.AreEqual(StakeProgramIdBytes, txInstruction.ProgramId);
        }
        [TestMethod]
        public void TestStakeProgramAuthorizeWithSeed()
        {
            var wallet = new Wallet.Wallet(mnemonicWords: MnemonicWords);

            var baseAccount = wallet.GetAccount(6);
            var authorityBaseAccount = wallet.GetAccount(5);
            var newAuthorizedAccount = wallet.GetAccount(4);
            var custodian = wallet.GetAccount(3);
            var authorityOwnerAccount = wallet.GetAccount(2);

            var txInstruction = StakeProgram.AuthorizeWithSeed(
                baseAccount.PublicKey,
                authorityBaseAccount.PublicKey,
                "testThisSeed",
                authorityOwnerAccount.PublicKey,
                newAuthorizedAccount.PublicKey,
                StakeAuthorize.Staker,
                custodian.PublicKey);

            Assert.AreEqual(4, txInstruction.Keys.Count);
            CollectionAssert.AreEqual(AuthorizeWithSeedInstructionBytes, txInstruction.Data);
            CollectionAssert.AreEqual(StakeProgramIdBytes, txInstruction.ProgramId);
        }
        [TestMethod]
        public void TestStakeProgramInitializeChecked()
        {
            var wallet = new Wallet.Wallet(mnemonicWords: MnemonicWords);

            var baseAccount = wallet.GetAccount(6);
            var staker = wallet.GetAccount(5);
            var withdrawer = wallet.GetAccount(4);

            var txInstruction = StakeProgram.InitializeChecked(
                baseAccount.PublicKey,
                new Authorized { Staker = staker.PublicKey, Withdrawer = withdrawer.PublicKey });

            Assert.AreEqual(4, txInstruction.Keys.Count);
            CollectionAssert.AreEqual(InitializeCheckedInstructionBytes, txInstruction.Data);
            CollectionAssert.AreEqual(StakeProgramIdBytes, txInstruction.ProgramId);
        }
        [TestMethod]
        public void TestStakeProgramAuthorizeChecked()
        {
            var wallet = new Wallet.Wallet(mnemonicWords: MnemonicWords);

            var baseAccount = wallet.GetAccount(6);
            var authorizedAccount = wallet.GetAccount(5);
            var newAuthorizedAccount = wallet.GetAccount(4);
            var custodian = wallet.GetAccount(3);

            var txInstruction = StakeProgram.AuthorizeChecked(
                baseAccount.PublicKey,
                authorizedAccount.PublicKey,
                newAuthorizedAccount.PublicKey,
                StakeAuthorize.Staker,
                custodian.PublicKey);

            Assert.AreEqual(5, txInstruction.Keys.Count);
            CollectionAssert.AreEqual(AuthorizeCheckedInstructionBytes, txInstruction.Data);
            CollectionAssert.AreEqual(StakeProgramIdBytes, txInstruction.ProgramId);
        }
        [TestMethod]
        public void TestStakeProgramAuthorizeCheckedWithSeed()
        {
            var wallet = new Wallet.Wallet(mnemonicWords: MnemonicWords);

            var baseAccount = wallet.GetAccount(6);
            var authorityBaseAccount = wallet.GetAccount(5);
            var newAuthorizedAccount = wallet.GetAccount(4);
            var custodian = wallet.GetAccount(3);
            var authorityOwnerAccount = wallet.GetAccount(2);

            var txInstruction = StakeProgram.AuthorizeCheckedWithSeed(
                baseAccount.PublicKey,
                authorityBaseAccount.PublicKey,
                "testThisSeed",
                authorityOwnerAccount.PublicKey,
                newAuthorizedAccount.PublicKey,
                StakeAuthorize.Staker,
                custodian.PublicKey);

            Assert.AreEqual(5, txInstruction.Keys.Count);
            CollectionAssert.AreEqual(AuthorizeCheckedWithSeedInstructionBytes, txInstruction.Data);
            CollectionAssert.AreEqual(StakeProgramIdBytes, txInstruction.ProgramId);
        }
        [TestMethod]
        public void TestStakeProgramSetLockupChecked()
        {
            var wallet = new Wallet.Wallet(mnemonicWords: MnemonicWords);

            var baseAccount = wallet.GetAccount(6);
            var custodian = wallet.GetAccount(2);

            var txInstruction = StakeProgram.SetLockupChecked(
                baseAccount.PublicKey,
                new Lockup {Custodian = custodian.PublicKey, Epoch = 0, UnixTimestamp = 0 },
                custodian.PublicKey);

            Assert.AreEqual(3, txInstruction.Keys.Count);
            CollectionAssert.AreEqual(SetLockupCheckedInstructionBytes, txInstruction.Data);
            CollectionAssert.AreEqual(StakeProgramIdBytes, txInstruction.ProgramId);
        }
    }
}
