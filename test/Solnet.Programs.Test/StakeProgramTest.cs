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
            "route clerk disease box emerge airport loud waste attitude film army tray " +
            "forward deal onion eight catalog surface unit card window walnut wealth medal";

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
            //
        };
        private static readonly byte[] InitializeInstructionBytes =
        {
            //
        };
        private static readonly byte[] AuthorizeInstructionBytes =
        {
            //
        };
        private static readonly byte[] InitializeInstructionBytes =
        {
            //
        };
        private static readonly byte[] AuthorizeInstructionBytes =
        {
            //
        };
        private static readonly byte[] InitializeInstructionBytes =
        {
            //
        };
        private static readonly byte[] AuthorizeInstructionBytes =
        {
            //
        };
        private static readonly byte[] InitializeInstructionBytes =
        {
            //
        };
        private static readonly byte[] AuthorizeInstructionBytes =
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
            CollectionAssert.AreEqual(AuthorizeInstructionBytes, txInstruction.Data);
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
            CollectionAssert.AreEqual(AuthorizeInstructionBytes, txInstruction.Data);
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
