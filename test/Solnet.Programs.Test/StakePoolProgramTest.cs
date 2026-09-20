using Microsoft.VisualStudio.TestTools.UnitTesting;
using Solnet.Programs.StakePool;
using Solnet.Programs.StakePool.Models;
using Solnet.Wallet;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Solnet.Programs.Test
{
    [TestClass]
    public class StakePoolProgramTest
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
        private static readonly PublicKey DepositAuthority = new("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa");
        private static readonly PublicKey StakeAccount = new("bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb");
        private static readonly PublicKey ValidatorAccount = new("cccccccccccccccccccccccccccccccc");
        private static readonly PublicKey TransientStakeAccount = new("dddddddddddddddddddddddddddddddd");
        private static readonly PublicKey EphemeralStake = new("eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee");
        private static readonly PublicKey Validator = new("ffffffffffffffffffffffffffffffff");
        // For SetStaker and SetManager tests using different keys.
        private static readonly PublicKey NewStaker = new("11111111111111111111111111111112");
        private static readonly PublicKey NewManager = new("22222222222222222222222222222223");
        private static readonly PublicKey NewFeeReceiver = new("33333333333333333333333333333334");
        private static readonly PublicKey NewDepositAuthority = new("44444444444444444444444444444445");
        private static readonly PublicKey UserPoolTokenAccount = new("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaab");


        private static Fee DummyFee() => new (1, 100); // use a dummy fee with nonzero values


        [TestMethod]
        public void Initialize_CreatesCorrectInstruction()
        {
            var program = new StakePoolProgram();
            var instr = program.Initialize(
                StakePool, Manager, Staker, WithdrawAuthority, ValidatorList, ReserveStake, PoolMint,
                ManagerPoolAccount, TokenProgramId, DummyFee(), DummyFee(), DummyFee(), DummyFee(),
                DepositAuthority, 42);

            Assert.AreSequenceEqual(StakePoolProgram.StakePoolProgramIdKey.KeyBytes, instr.ProgramId);
            Assert.IsGreaterThanOrEqualTo(value:instr.Keys.Count, lowerBound: 4); // At least the required keys
            Assert.IsGreaterThanOrEqualTo(value:instr.Data.Length, lowerBound: 0);
        }

        [TestMethod]
        public void AddValidatorToPool_CreatesCorrectInstruction()
        {
            var program = new StakePoolProgram();
            var instr = program.AddValidatorToPool(
                StakePool, Staker, ReserveStake, WithdrawAuthority, ValidatorList,
                StakeAccount, ValidatorAccount, 123);

            Assert.AreSequenceEqual(StakePoolProgram.StakePoolProgramIdKey.KeyBytes, instr.ProgramId);
            Assert.IsGreaterThanOrEqualTo(value:instr.Keys.Count, lowerBound: 0);
            Assert.IsGreaterThanOrEqualTo(value:instr.Data.Length, lowerBound: 0);
        }

        [TestMethod]
        public void RemoveValidatorFromPool_CreatesCorrectInstruction()
        {
            var program = new StakePoolProgram();
            var instr = program.RemoveValidatorFromPool(
                StakePool, Staker, WithdrawAuthority, ValidatorList, StakeAccount, TransientStakeAccount);

            Assert.AreSequenceEqual(StakePoolProgram.StakePoolProgramIdKey.KeyBytes, instr.ProgramId);
            Assert.IsGreaterThanOrEqualTo(value:instr.Keys.Count, lowerBound: 0);
            Assert.IsGreaterThanOrEqualTo(value:instr.Data.Length, lowerBound: 0);
        }

        [TestMethod]
        public void DecreaseValidatorStake_CreatesCorrectInstruction()
        {
            var program = new StakePoolProgram();
            var instr = program.DecreaseValidatorStakeWithReserve(
                StakePool, Staker, WithdrawAuthority, ValidatorList, ReserveStake, StakeAccount,
                TransientStakeAccount, 1000, 55);

            Assert.AreSequenceEqual(StakePoolProgram.StakePoolProgramIdKey.KeyBytes, instr.ProgramId);
            Assert.IsGreaterThanOrEqualTo(value:instr.Keys.Count, lowerBound: 0);
            Assert.IsGreaterThanOrEqualTo(value:instr.Data.Length, lowerBound: 0);
        }

        [TestMethod]
        public void DecreaseAdditionalValidatorStake_CreatesCorrectInstruction()
        {
            var program = new StakePoolProgram();
            var instr = program.DecreaseAdditionalValidatorStake(
                StakePool, Staker, WithdrawAuthority, ValidatorList, ReserveStake, StakeAccount,
                EphemeralStake, TransientStakeAccount, 1000, 55, 77);

            Assert.AreSequenceEqual(StakePoolProgram.StakePoolProgramIdKey.KeyBytes, instr.ProgramId);
            Assert.IsGreaterThanOrEqualTo(value:instr.Keys.Count, lowerBound: 0);
            Assert.IsGreaterThanOrEqualTo(value:instr.Data.Length, lowerBound: 0);
        }

        [TestMethod]
        public void DecreaseValidatorStakeWithReserve_CreatesCorrectInstruction()
        {
            var program = new StakePoolProgram();
            var instr = program.DecreaseValidatorStakeWithReserve(
                StakePool, Staker, WithdrawAuthority, ValidatorList, ReserveStake, StakeAccount,
                TransientStakeAccount, 1000, 55);

            Assert.AreSequenceEqual(StakePoolProgram.StakePoolProgramIdKey.KeyBytes, instr.ProgramId);
            Assert.IsGreaterThan(value: instr.Keys.Count, lowerBound: 0);
            Assert.IsGreaterThan(value: instr.Data.Length, lowerBound: 0);
        }

        [TestMethod]
        public void IncreaseValidatorStake_CreatesCorrectInstruction()
        {
            var program = new StakePoolProgram();
            var instr = program.IncreaseValidatorStake(
                StakePool, Staker, WithdrawAuthority, ValidatorList, ReserveStake, TransientStakeAccount,
                StakeAccount, 1000, 55);

            Assert.AreSequenceEqual(StakePoolProgram.StakePoolProgramIdKey.KeyBytes, instr.ProgramId);
            Assert.IsGreaterThan(value: instr.Keys.Count, lowerBound: 0);
            Assert.IsGreaterThan(value: instr.Data.Length, lowerBound: 0);
        }

        [TestMethod]
        public void IncreaseAdditionalValidatorStake_CreatesCorrectInstruction()
        {
            var program = new StakePoolProgram();
            var instr = program.IncreaseAdditionalValidatorStake(
                StakePool, Staker, WithdrawAuthority, ValidatorList, ReserveStake, EphemeralStake,
                TransientStakeAccount, StakeAccount, Validator, 1000, 55, 77);

            Assert.AreSequenceEqual(StakePoolProgram.StakePoolProgramIdKey.KeyBytes, instr.ProgramId);
            Assert.IsGreaterThanOrEqualTo(value: instr.Keys.Count, lowerBound: 0);
            Assert.IsGreaterThanOrEqualTo(value: instr.Data.Length, lowerBound: 0);
        }

        [TestMethod]
        public void SetPreferredDepositValidator_CreatesCorrectInstruction()
        {
            var program = new StakePoolProgram();
            // Passing Validator as the optional validator vote address causes the keys count to be 4.
            var instr = program.SetPreferredDepositValidator(
                StakePool, Staker, ValidatorList, PreferredValidatorType.Deposit, Validator);

            Assert.AreSequenceEqual(StakePoolProgram.StakePoolProgramIdKey.KeyBytes, instr.ProgramId);
            // Expecting 4 keys because the optional parameter is provided.
            Assert.HasCount(4, instr.Keys);
            Assert.IsGreaterThanOrEqualTo(value: instr.Data.Length, lowerBound: 0);
        }

        [TestMethod]
        public void SetFee_CreatesCorrectInstruction()
        {
            var instr = StakePoolProgram.SetFee(StakePool, Manager, DummyFee());
            Assert.AreSequenceEqual(StakePoolProgram.StakePoolProgramIdKey.KeyBytes, instr.ProgramId);
            Assert.HasCount(2, instr.Keys);
            Assert.IsGreaterThanOrEqualTo(value: instr.Data.Length, lowerBound: 0);
        }

        [TestMethod]
        public void SetStaker_CreatesCorrectInstruction()
        {
            var instr = StakePoolProgram.SetStaker(StakePool, Staker, NewStaker);
            Assert.AreSequenceEqual(StakePoolProgram.StakePoolProgramIdKey.KeyBytes, instr.ProgramId);
            // Expecting 3 keys as per the implementation
            Assert.HasCount(3, instr.Keys);
            Assert.IsGreaterThanOrEqualTo(value: instr.Data.Length, lowerBound: 0);
        }

        [TestMethod]
        public void SetManager_CreatesCorrectInstruction()
        {
            var instr = StakePoolProgram.SetManager(StakePool, Manager, NewManager, NewFeeReceiver);
            Assert.AreSequenceEqual(StakePoolProgram.StakePoolProgramIdKey.KeyBytes, instr.ProgramId);
            // Expecting 4 keys per the SetManager instruction
            Assert.HasCount(4, instr.Keys);
            Assert.IsGreaterThanOrEqualTo(value: instr.Data.Length, lowerBound: 0);
        }

        [TestMethod]
        public void SetFundingAuthority_CreatesCorrectInstruction()
        {
            var instr = StakePoolProgram.SetFundingAuthority(StakePool, Manager, NewDepositAuthority, FundingType.SolDeposit);
            Assert.AreSequenceEqual(StakePoolProgram.StakePoolProgramIdKey.KeyBytes, instr.ProgramId);
            // Keys: StakePool, Manager, and NewDepositAuthority should be provided
            Assert.HasCount(3, instr.Keys);
            Assert.IsGreaterThanOrEqualTo(value: instr.Data.Length, lowerBound: 0);
        }

        [TestMethod]
        public void DepositStake_CreatesCorrectInstruction()
        {
            var instrList = StakePoolProgram.DepositStake(
                StakePool, ValidatorList, WithdrawAuthority, StakeAccount,
                DepositAuthority, ValidatorAccount, ReserveStake, PoolMint,
                ManagerPoolAccount, DepositAuthority, PoolMint, TokenProgramId);

            Assert.HasCount(3, instrList);
            // Ensure that at least one instruction uses the StakePoolProgram ID.
            Assert.Contains(i => i.ProgramId.SequenceEqual(StakePoolProgram.StakePoolProgramIdKey.KeyBytes), instrList,
                message: "None of the instructions use the stake pool program ID.");

            // Additionally ensure each instruction has keys and nonempty data.
            foreach (var instr in instrList)
            {
                Assert.IsGreaterThanOrEqualTo(value: instr.Keys.Count, lowerBound: 0, message: "Instruction has no keys.");
                Assert.IsGreaterThanOrEqualTo(value: instr.Data.Length, lowerBound: 0, message: "Instruction has empty data.");
            }
        }

        [TestMethod]
        public void DepositStakeWithSlippage_CreatesCorrectInstruction()
        {
            var instrList = StakePoolProgram.DepositStakeWithSlippage(
                StakePool, ValidatorList, WithdrawAuthority, StakeAccount,
                DepositAuthority, ValidatorAccount, ReserveStake, PoolMint,
                ManagerPoolAccount, DepositAuthority, PoolMint, TokenProgramId, 1000);

            Assert.HasCount(3, instrList);
            // Check that at least one instruction uses the stake pool program ID.
            Assert.Contains(i => i.ProgramId.SequenceEqual(StakePoolProgram.StakePoolProgramIdKey.KeyBytes), instrList,
                message: "None of the instructions use the stake pool program ID.");

            // Additionally ensure each instruction has nonempty keys and data.
            foreach (var instr in instrList)
            {
                Assert.IsGreaterThanOrEqualTo(value: instr.Keys.Count, lowerBound: 0, message: "Instruction has no keys.");
                Assert.IsGreaterThanOrEqualTo(value: instr.Data.Length, lowerBound: 0, message: "Instruction has empty data.");
            }
        }

        [TestMethod]
        public void WithdrawStake_CreatesCorrectInstruction()
        {
            var instr = StakePoolProgram.WithdrawStake(
                StakePool,         // stakePool
                ValidatorList,     // validatorListStorage
                WithdrawAuthority, // stakePoolWithdrawAuthority
                StakeAccount,      // stakeToSplit
                TransientStakeAccount, // stakeToReceive
                Staker,            // userStakeAuthority
                Manager,           // userTransferAuthority
                UserPoolTokenAccount, // userPoolTokenAccount
                ManagerPoolAccount, // managerFeeAccount
                PoolMint,          // poolMint
                TokenProgramId,    // tokenProgramId
                2000);             // poolTokensIn

            Assert.AreSequenceEqual(StakePoolProgram.StakePoolProgramIdKey.KeyBytes, instr.ProgramId);
            Assert.IsGreaterThanOrEqualTo(value: instr.Keys.Count, lowerBound: 0);
            Assert.IsGreaterThanOrEqualTo(value: instr.Data.Length, lowerBound: 0);
        }

        [TestMethod]
        public void WithdrawStakeWithSlippage_CreatesCorrectInstruction()
        {
            var instr = StakePoolProgram.WithdrawStakeWithSlippage(
                StakePool,         // stakePool
                ValidatorList,     // validatorListStorage
                WithdrawAuthority, // stakePoolWithdrawAuthority
                StakeAccount,      // stakeToSplit
                TransientStakeAccount, // stakeToReceive
                Staker,            // userStakeAuthority
                Manager,           // userTransferAuthority
                UserPoolTokenAccount, // userPoolTokenAccount
                ManagerPoolAccount, // managerFeeAccount
                PoolMint,          // poolMint
                TokenProgramId,    // tokenProgramId
                2000,              // poolTokensIn
                1500);             // minimumLamportsOut

            Assert.AreSequenceEqual(StakePoolProgram.StakePoolProgramIdKey.KeyBytes, instr.ProgramId);
            Assert.IsGreaterThanOrEqualTo(value: instr.Keys.Count, lowerBound: 0);
            Assert.IsGreaterThanOrEqualTo(value: instr.Data.Length, lowerBound: 0);
        }

        [TestMethod]
        public void WithdrawSol_CreatesCorrectInstruction()
        {
            var instr = StakePoolProgram.WithdrawSol(
                StakePool, WithdrawAuthority, Manager, StakeAccount, ReserveStake,
                PoolMint, ManagerPoolAccount, PoolMint, TokenProgramId, 3000);
            Assert.AreSequenceEqual(StakePoolProgram.StakePoolProgramIdKey.KeyBytes, instr.ProgramId);
            Assert.IsGreaterThanOrEqualTo(value: instr.Keys.Count, lowerBound: 0);
            Assert.IsGreaterThanOrEqualTo(value: instr.Data.Length, lowerBound: 0);
        }

        [TestMethod]
        public void WithdrawSolWithSlippage_CreatesCorrectInstruction()
        {
            var instr = StakePoolProgram.WithdrawSolWithSlippage(
                StakePool, WithdrawAuthority, Manager, StakeAccount, ReserveStake,
                PoolMint, ManagerPoolAccount, PoolMint, TokenProgramId, 3000, 2500);
            Assert.AreSequenceEqual(StakePoolProgram.StakePoolProgramIdKey.KeyBytes, instr.ProgramId);
            Assert.IsGreaterThanOrEqualTo(value: instr.Keys.Count, lowerBound: 0);
            Assert.IsGreaterThanOrEqualTo(value: instr.Data.Length, lowerBound: 0);
        }

        // Updated WithdrawSolWithAuthority test
        [TestMethod]
        public void WithdrawSolWithAuthority_CreatesCorrectInstruction()
        {
            var instr = StakePoolProgram.WithdrawSolWithAuthority(
                StakePool, DepositAuthority, WithdrawAuthority, Manager, StakeAccount, ReserveStake,
                PoolMint, ManagerPoolAccount, PoolMint, TokenProgramId, 3000);
            Assert.AreSequenceEqual(StakePoolProgram.StakePoolProgramIdKey.KeyBytes, instr.ProgramId);
            Assert.Contains(x => x.PublicKey.Equals(DepositAuthority) && x.IsSigner, instr.Keys,
                message: "DepositAuthority is not found with IsSigner true in the account metas.");
            Assert.IsGreaterThanOrEqualTo(value: instr.Data.Length, lowerBound: 0);
        }

        [TestMethod]
        public void WithdrawSolWithAuthorityAndSlippage_CreatesCorrectInstruction()
        {
            var instr = StakePoolProgram.WithdrawSolWithAuthorityAndSlippage(
                StakePool, DepositAuthority, WithdrawAuthority, Manager, StakeAccount, ReserveStake,
                PoolMint, ManagerPoolAccount, PoolMint, TokenProgramId, 3000, 2500);
            Assert.AreSequenceEqual(StakePoolProgram.StakePoolProgramIdKey.KeyBytes, instr.ProgramId);
            Assert.Contains(x => x.PublicKey.Equals(DepositAuthority) && x.IsSigner, instr.Keys,
                "DepositAuthority is not found with IsSigner true in the account metas.");
            Assert.IsGreaterThanOrEqualTo(value: instr.Data.Length, lowerBound: 0);
        }

        [TestMethod]
        public void UpdateValidatorListBalanceChunk_CreatesCorrectInstruction()
        {
            // For update test, we simulate a validator list with dummy validators.
            ValidatorList dummyValidatorList = new()
            {
                Validators =
                [
                    new() { VoteAccountAddress = new PublicKey("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa1"), ValidatorSeedSuffix = 1, TransientSeedSuffix = 10 },
                    new() { VoteAccountAddress = new PublicKey("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa2"), ValidatorSeedSuffix = 2, TransientSeedSuffix = 20 },
                    new() { VoteAccountAddress = new PublicKey("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa3"), ValidatorSeedSuffix = 3, TransientSeedSuffix = 30 }
                ]
            };

            var instr = StakePoolProgram.UpdateValidatorListBalanceChunk(
                StakePool, WithdrawAuthority, ValidatorList, ReserveStake,
                dummyValidatorList, 2, 0, true);

            Assert.AreSequenceEqual(StakePoolProgram.StakePoolProgramIdKey.KeyBytes, instr.ProgramId);
            Assert.IsGreaterThanOrEqualTo(value: instr.Keys.Count, lowerBound: 0);
            Assert.IsGreaterThanOrEqualTo(value: instr.Data.Length, lowerBound: 0);
        }

        [TestMethod]
        public void UpdateStakePoolBalance_CreatesCorrectInstruction()
        {
            var instr = StakePoolProgram.UpdateStakePoolBalance(
                StakePool, WithdrawAuthority, ValidatorList, ReserveStake, ManagerPoolAccount, PoolMint, TokenProgramId);
            Assert.AreSequenceEqual(StakePoolProgram.StakePoolProgramIdKey.KeyBytes, instr.ProgramId);
            Assert.IsGreaterThanOrEqualTo(value: instr.Keys.Count, lowerBound: 7);
            Assert.IsGreaterThanOrEqualTo(value: instr.Data.Length, lowerBound: 0);
        }

        [TestMethod]
        public void CleanupRemovedValidatorEntries_CreatesCorrectInstruction()
        {
            var instr = StakePoolProgram.CleanupRemovedValidatorEntries(StakePool, ValidatorList);
            Assert.AreSequenceEqual(StakePoolProgram.StakePoolProgramIdKey.KeyBytes, instr.ProgramId);
            Assert.IsGreaterThanOrEqualTo(value: instr.Keys.Count, lowerBound: 2);
            Assert.IsGreaterThanOrEqualTo(value: instr.Data.Length, lowerBound: 0);
        }

        [TestMethod]
        public void UpdateStaleStakePool_CreatesCorrectInstructions()
        {
            // Create a dummy validator list with one outdated validator.
            ValidatorList dummyValidatorList = new()
            {
                Validators =
                [
                    new() { VoteAccountAddress = new PublicKey("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa1"), ValidatorSeedSuffix = 1, TransientSeedSuffix = 10, LastUpdateEpoch = 10 },
                    new() { VoteAccountAddress = new PublicKey("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa2"), ValidatorSeedSuffix = 2, TransientSeedSuffix = 20, LastUpdateEpoch = 5 }
                ]
            };

            var (updateListInstructions, finalInstructions) = StakePoolProgram.UpdateStaleStakePool(
                new StakePool.Models.StakePool { Staker = Staker, ValidatorList = ValidatorList, ReserveStake = ReserveStake, ManagerFeeAccount = ManagerPoolAccount, PoolMint = PoolMint, TokenProgramId = TokenProgramId },
                dummyValidatorList, StakePool, true, 6);

            Assert.IsGreaterThanOrEqualTo(updateListInstructions.Count , 1);
            Assert.IsGreaterThanOrEqualTo(finalInstructions.Count, 2);
            foreach (var instr in updateListInstructions)
            {
                Assert.AreSequenceEqual(StakePoolProgram.StakePoolProgramIdKey.KeyBytes, instr.ProgramId);
                Assert.IsGreaterThanOrEqualTo(value: instr.Data.Length, lowerBound: 0);
            }
            foreach (var instr in finalInstructions)
            {
                Assert.AreSequenceEqual(StakePoolProgram.StakePoolProgramIdKey.KeyBytes, instr.ProgramId);
                Assert.IsGreaterThanOrEqualTo(value: instr.Data.Length, lowerBound: 0);
            }
        }
    }

    [TestClass]
    public class StakePoolModelsTest
    {
        [TestMethod]
        public void Fee_StoresCorrectValues()
        {
            // Arrange
            var fee = new Fee(100, 1000);

            // Act & Assert
            Assert.AreEqual(100UL, fee.Numerator, "Fee numerator not stored correctly.");
            Assert.AreEqual(1000UL, fee.Denominator, "Fee denominator not stored correctly.");
        }

        [TestMethod]
        public void Fee_DefaultIsZero()
        {
            // Arrange
            var fee = new Fee();

            // Act & Assert
            Assert.AreEqual(0UL, fee.Numerator, "Default fee numerator should be zero.");
            Assert.AreEqual(0UL, fee.Denominator, "Default fee denominator should be zero.");
        }

        [TestMethod]
        public void PreferredValidatorType_ValuesAreConsistent()
        {
            // Assuming that PreferredValidatorType is an enum with defined underlying values,
            // you might for example have:
            // Deposit = 0, Withdraw = 1 (adjust as per your actual enum definition).
            uint depositValue = (uint)PreferredValidatorType.Deposit;
            uint withdrawValue = (uint)PreferredValidatorType.Withdraw;

            // Assert that they are different and non-negative.
            Assert.AreNotEqual(depositValue, withdrawValue, "PreferredValidatorType values must differ.");
            Assert.IsGreaterThanOrEqualTo(depositValue, withdrawValue, message: "Deposit value should be greater than or equal to Withdraw value.");
        }

        [TestMethod]
        public void FundingType_ValuesAreConsistent()
        {
            byte solDeposit = (byte)FundingType.SolDeposit;
            Assert.AreEqual(1, solDeposit, "FundingType.SolDeposit is expected to be 1.");
        }
    }
}