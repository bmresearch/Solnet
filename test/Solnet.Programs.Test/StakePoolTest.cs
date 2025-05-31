using Microsoft.VisualStudio.TestTools.UnitTesting;
using Solnet.Programs.StakePool;
using Solnet.Programs.StakePool.Models;
using Solnet.Wallet;
using Solnet.Programs.TokenSwap.Models;
using System;
using System.Linq;

namespace Solnet.Programs.Test
{
    [TestClass]
    public class StakePoolTest
    {
        private static readonly PublicKey StakePool = new("11111111111111111111111111111111");
        private static readonly PublicKey Manager = new("22222222222222222222222222222222");
        private static readonly PublicKey Staker = new("33333333333333333333333333333333");
        private static readonly PublicKey WithdrawAuthority = new("44444444444444444444444444444444");
        private static readonly PublicKey ValidatorList = new("55555555555555555555555555555555");
        private static readonly PublicKey ReserveStake = new("66666666666666666666666666666666");
        private static readonly PublicKey PoolMint = new("77777777777777777777777777777777");
        private static readonly PublicKey ManagerPoolAccount = new("88888888888888888888888888888888");
        private static readonly PublicKey TokenProgramId = new("99999999999999999999999999999999");
        // Add the missing declaration for DepositAuthority
        private static readonly PublicKey DepositAuthority = new("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa");
        private static readonly PublicKey StakeAccount = new("bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb");
        private static readonly PublicKey ValidatorAccount = new("cccccccccccccccccccccccccccccccc");
        private static readonly PublicKey TransientStakeAccount = new("dddddddddddddddddddddddddddddddd");
        private static readonly PublicKey EphemeralStake = new("eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee");
        private static readonly PublicKey Validator = new("ffffffffffffffffffffffffffffffff");

        private static Fee DummyFee() => new Fee();

        [TestMethod]
        public void Initialize_CreatesCorrectInstruction()
        {
            var program = new StakePoolProgram();
            var instr = program.Initialize(
                StakePool, Manager, Staker, WithdrawAuthority, ValidatorList, ReserveStake, PoolMint,
                ManagerPoolAccount, TokenProgramId, DummyFee(), DummyFee(), DummyFee(), DummyFee(),
                DepositAuthority, 42);

            CollectionAssert.AreEqual(StakePoolProgram.StakePoolProgramIdKey.KeyBytes, instr.ProgramId);
            Assert.IsTrue(instr.Keys.Count >= 4); // At least the required keys
            Assert.IsTrue(instr.Data.Length > 0);
        }

        [TestMethod]
        public void AddValidatorToPool_CreatesCorrectInstruction()
        {
            var program = new StakePoolProgram();
            var instr = program.AddValidatorToPool(
                StakePool, Staker, ReserveStake, WithdrawAuthority, ValidatorList,
                StakeAccount, ValidatorAccount, 123);

            CollectionAssert.AreEqual(StakePoolProgram.StakePoolProgramIdKey.KeyBytes, instr.ProgramId);
            Assert.IsTrue(instr.Keys.Count > 0);
            Assert.IsTrue(instr.Data.Length > 0);
        }

        [TestMethod]
        public void RemoveValidatorFromPool_CreatesCorrectInstruction()
        {
            var program = new StakePoolProgram();
            var instr = program.RemoveValidatorFromPool(
                StakePool, Staker, WithdrawAuthority, ValidatorList, StakeAccount, TransientStakeAccount);

            CollectionAssert.AreEqual(StakePoolProgram.StakePoolProgramIdKey.KeyBytes, instr.ProgramId);
            Assert.IsTrue(instr.Keys.Count > 0);
            Assert.IsTrue(instr.Data.Length > 0);
        }

        [TestMethod]
        public void DecreaseValidatorStake_CreatesCorrectInstruction()
        {
            var program = new StakePoolProgram();
            var instr = program.DecreaseValidatorStakeWithReserve(
                StakePool, Staker, WithdrawAuthority, ValidatorList, ReserveStake, StakeAccount,
                TransientStakeAccount, 1000, 55);

            CollectionAssert.AreEqual(StakePoolProgram.StakePoolProgramIdKey.KeyBytes, instr.ProgramId);
            Assert.IsTrue(instr.Keys.Count > 0);
            Assert.IsTrue(instr.Data.Length > 0);
        }

        [TestMethod]
        public void DecreaseAdditionalValidatorStake_CreatesCorrectInstruction()
        {
            var program = new StakePoolProgram();
            var instr = program.DecreaseAdditionalValidatorStake(
                StakePool, Staker, WithdrawAuthority, ValidatorList, ReserveStake, StakeAccount,
                EphemeralStake, TransientStakeAccount, 1000, 55, 77);

            CollectionAssert.AreEqual(StakePoolProgram.StakePoolProgramIdKey.KeyBytes, instr.ProgramId);
            Assert.IsTrue(instr.Keys.Count > 0);
            Assert.IsTrue(instr.Data.Length > 0);
        }

        [TestMethod]
        public void DecreaseValidatorStakeWithReserve_CreatesCorrectInstruction()
        {
            var program = new StakePoolProgram();
            var instr = program.DecreaseValidatorStakeWithReserve(
                StakePool, Staker, WithdrawAuthority, ValidatorList, ReserveStake, StakeAccount,
                TransientStakeAccount, 1000, 55);

            CollectionAssert.AreEqual(StakePoolProgram.StakePoolProgramIdKey.KeyBytes, instr.ProgramId);
            Assert.IsTrue(instr.Keys.Count > 0);
            Assert.IsTrue(instr.Data.Length > 0);
        }

        [TestMethod]
        public void IncreaseValidatorStake_CreatesCorrectInstruction()
        {
            var program = new StakePoolProgram();
            var instr = program.IncreaseValidatorStake(
                StakePool, Staker, WithdrawAuthority, ValidatorList, ReserveStake, TransientStakeAccount,
                StakeAccount, 1000, 55);

            CollectionAssert.AreEqual(StakePoolProgram.StakePoolProgramIdKey.KeyBytes, instr.ProgramId);
            Assert.IsTrue(instr.Keys.Count > 0);
            Assert.IsTrue(instr.Data.Length > 0);
        }

        [TestMethod]
        public void IncreaseAdditionalValidatorStake_CreatesCorrectInstruction()
        {
            var program = new StakePoolProgram();
            var instr = program.IncreaseAdditionalValidatorStake(
                StakePool, Staker, WithdrawAuthority, ValidatorList, ReserveStake, EphemeralStake,
                TransientStakeAccount, StakeAccount, Validator, 1000, 55, 77);

            CollectionAssert.AreEqual(StakePoolProgram.StakePoolProgramIdKey.KeyBytes, instr.ProgramId);
            Assert.IsTrue(instr.Keys.Count > 0);
            Assert.IsTrue(instr.Data.Length > 0);
        }

        [TestMethod]
        public void SetPreferredDepositValidator_CreatesCorrectInstruction()
        {
            var program = new StakePoolProgram();
            var instr = program.SetPreferredDepositValidator(
                StakePool, Staker, ValidatorList, PreferredValidatorType.Deposit, Validator);

            CollectionAssert.AreEqual(StakePoolProgram.StakePoolProgramIdKey.KeyBytes, instr.ProgramId);
            Assert.AreEqual(3, instr.Keys.Count);
            Assert.IsTrue(instr.Data.Length > 0);
        }
    }
}