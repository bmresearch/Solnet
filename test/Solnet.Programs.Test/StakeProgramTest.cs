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
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
        };
        private static readonly byte[] InitializeInstructionBytes =
        {
            //
        };
        private static readonly byte[] AuthorizeInstructionBytes =
        {
            1, 6, 29, 129, 91, 218, 96, 191, 52, 227, 118, 199, 113, 55, 3, 29, 202, 205, 69, 103, 77, 144, 20, 137, 37, 53, 184, 83, 41, 37, 245, 0, 132, 194, 143, 36, 222, 153, 234, 195, 134, 47, 231, 100, 144, 102, 51, 115, 10, 115, 3, 82, 21, 163, 162, 246, 64, 238, 36, 135, 202, 239, 143, 150, 8, 3, 2, 2, 5, 244, 171, 249, 196, 62, 132, 245, 193, 114, 19, 34, 7, 37, 207, 38, 98, 69, 136, 106, 149, 175, 110, 143, 211, 108, 198, 5, 239, 231, 182, 7, 20, 189, 31, 212, 204, 51, 65, 12, 40, 137, 113, 214, 99, 175, 9, 119, 28, 19, 10, 56, 240, 87, 136, 148, 225, 227, 13, 181, 127, 113, 230, 10, 186, 4, 23, 154, 206, 58, 166, 9, 125, 107, 80, 224, 57, 235, 71, 51, 46, 27, 153, 48, 39, 162, 54, 144, 176, 6, 128, 214, 189, 53, 152, 48, 38, 6, 167, 213, 23, 24, 199, 116, 201, 40, 86, 99, 152, 105, 29, 94, 182, 139, 94, 184, 163, 155, 75, 109, 92, 115, 85, 91, 33, 0, 0, 0, 0, 6, 161, 216, 23, 145, 55, 84, 42, 152, 52, 55, 189, 254, 42, 122, 178, 85, 127, 83, 92, 138, 120, 114, 43, 104, 164, 157, 192, 0, 0, 0, 0, 107, 206, 16, 200, 38, 251, 189, 190, 216, 198, 3, 156, 64, 67, 64, 38, 40, 193, 135, 105, 31, 232, 28, 59, 108, 56, 194, 233, 106, 233, 106, 72, 1, 4, 4, 0, 3, 1, 2, 68, 1, 0, 0, 0, 233, 219, 240, 9, 143, 216, 128, 216, 67, 144, 130, 254, 239, 2, 3, 192, 182, 231, 253, 136, 201, 238, 30, 197, 208, 57, 73, 173, 45, 119, 235, 205, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
        };
        private static readonly byte[] DelegateStakeInstructionBytes =
        {
            //
        };
        private static readonly byte[] SplitInstructionBytes =
        {
            //
        };
        private static readonly byte[] WithdrawInstructionBytes =
        {
            //
        };
        private static readonly byte[] DeactivateInstructionBytes =
        {
            //
        };
        private static readonly byte[] SetLockupInstructionBytes =
        {
            //
        };
        private static readonly byte[] MergeInstructionBytes =
        {
            //
        };
        private static readonly byte[] AuthorizeWithSeedInstructionBytes =
        {
            //
        };
        private static readonly byte[] InitializeCheckedInstructionBytes =
        {
            //
        };
        private static readonly byte[] AuthorizeCheckedInstructionBytes =
        {
            //
        };
        private static readonly byte[] AuthorizeCheckedWithSeedInstructionBytes =
        {
            //
        };
        private static readonly byte[] SetLockupCheckedInstructionBytes =
        {
            //
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
                new Authorized { staker = staker.PublicKey, withdrawer = withdrawer.PublicKey },
                new Lockup { custodian = custodian.PublicKey, epoch = ulong.Parse(DateTime.UnixEpoch.ToString()), unix_timestamp = DateTimeOffset.Now.ToUnixTimeSeconds() });

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

            Assert.AreEqual(3, txInstruction.Keys.Count);
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

            Assert.AreEqual(5, txInstruction.Keys.Count);
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
                new Lockup { custodian = custodian.PublicKey, epoch = ulong.Parse(DateTime.UnixEpoch.ToString()), unix_timestamp = DateTimeOffset.Now.ToUnixTimeSeconds() },
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
                new Authorized { staker = staker.PublicKey, withdrawer = withdrawer.PublicKey });

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
                new Lockup { custodian = custodian.PublicKey, epoch = ulong.Parse(DateTime.UnixEpoch.ToString()), unix_timestamp = DateTimeOffset.Now.ToUnixTimeSeconds() },
                custodian.PublicKey);

            Assert.AreEqual(3, txInstruction.Keys.Count);
            CollectionAssert.AreEqual(SetLockupCheckedInstructionBytes, txInstruction.Data);
            CollectionAssert.AreEqual(StakeProgramIdBytes, txInstruction.ProgramId);
        }
    }
}
