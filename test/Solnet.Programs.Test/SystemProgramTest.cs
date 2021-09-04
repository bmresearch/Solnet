using Microsoft.VisualStudio.TestTools.UnitTesting;
using Solnet.Programs.Models;
using Solnet.Wallet.Utilities;
using System;

namespace Solnet.Programs.Test
{
    [TestClass]
    public class SystemProgramTest
    {
        private static readonly Base58Encoder Encoder = new();

        private const string MnemonicWords =
            "route clerk disease box emerge airport loud waste attitude film army tray " +
            "forward deal onion eight catalog surface unit card window walnut wealth medal";

        private static readonly byte[] SystemProgramIdBytes =
        {
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
        };

        private static readonly byte[] CreateAccountInstructionBytes =
        {
            0, 0, 0, 0, 240, 29, 31, 0, 0, 0, 0, 0, 165,
            0, 0, 0, 0, 0, 0, 0, 6, 221, 246, 225, 215,
            101, 161, 147, 217, 203, 225, 70, 206,
            235, 121, 172, 28, 180, 133, 237, 95, 91,
            55, 145, 58, 140, 245, 133, 126, 255, 0, 169
        };

        private static readonly byte[] TransferInstructionBytes =
        {
            2, 0, 0, 0, 128, 150, 152, 0, 0, 0, 0, 0
        };

        private static readonly byte[] AssignInstructionBytes =
        {
            1, 0, 0, 0, 189, 31, 212, 204, 51, 65, 12, 40,
            137, 113, 214, 99, 175, 9, 119, 28, 19, 10, 56, 240,
            87, 136, 148, 225, 227, 13, 181, 127, 113, 230, 10, 186
        };

        private static readonly byte[] CreateAccountWithSeedInstructionBytes =
        {
            3, 0, 0, 0, 244, 171, 249, 196, 62, 132, 245, 193, 114,
            19, 34, 7, 37, 207, 38, 98, 69, 136, 106, 149, 175, 110, 143, 211,
            108, 198, 5, 239, 231, 182, 7, 20, 8, 0, 0, 0, 116, 101, 115,
            116, 83, 101, 101, 100, 64, 66, 15, 0, 0, 0, 0, 0, 232, 3, 0, 0,
            0, 0, 0, 0, 4, 23, 154, 206, 58, 166, 9, 125, 107, 80, 224, 57,
            235, 71, 51, 46, 27, 153, 48, 39, 162, 54, 144, 176, 6, 128, 214,
            189, 53, 152, 48, 38,
        };

        private static readonly byte[] AdvanceNonceAccountInstructionBytes = { 4, 0, 0, 0 };

        private static readonly byte[] WithdrawNonceAccountInstructionBytes =
        {
            5, 0, 0, 0, 64, 66, 15, 0, 0, 0, 0, 0
        };

        private static readonly byte[] InitializeNonceAccountInstructionBytes =
        {
            6, 0, 0, 0, 4, 23, 154, 206, 58, 166, 9, 125, 107, 80, 224, 57,
            235, 71, 51, 46, 27, 153, 48, 39, 162, 54, 144, 176, 6, 128, 214,
            189, 53, 152, 48, 38
        };

        private static readonly byte[] AuthorizeNonceAccountInstructionBytes =
        {
            7, 0, 0, 0, 4, 23, 154, 206, 58, 166, 9, 125, 107, 80, 224, 57,
            235, 71, 51, 46, 27, 153, 48, 39, 162, 54, 144, 176, 6, 128, 214,
            189, 53, 152, 48, 38
        };

        private static readonly byte[] AllocateInstructionBytes =
        {
            8, 0, 0, 0, 64, 66, 15, 0, 0, 0, 0, 0
        };

        private static readonly byte[] AllocateWithSeedInstructionBytes =
        {
            9, 0, 0, 0, 244, 171, 249, 196, 62, 132, 245, 193, 114, 19,
            34, 7, 37, 207, 38, 98, 69, 136, 106, 149, 175, 110, 143,
            211, 108, 198, 5, 239, 231, 182, 7, 20, 8, 0, 0, 0, 116,
            101, 115, 116, 83, 101, 101, 100, 232, 3, 0, 0, 0, 0, 0,
            0, 4, 23, 154, 206, 58, 166, 9, 125, 107, 80, 224, 57, 235,
            71, 51, 46, 27, 153, 48, 39, 162, 54, 144, 176, 6, 128, 214,
            189, 53, 152, 48, 38
        };

        private static readonly byte[] AssignWithSeedInstructionBytes =
        {
            10, 0, 0, 0, 244, 171, 249, 196, 62, 132, 245, 193, 114,
            19, 34, 7, 37, 207, 38, 98, 69, 136, 106, 149, 175, 110,
            143, 211, 108, 198, 5, 239, 231, 182, 7, 20, 8, 0, 0, 0,
            116, 101, 115, 116, 83, 101, 101, 100, 4, 23, 154, 206,
            58, 166, 9, 125, 107, 80, 224, 57, 235, 71, 51, 46, 27,
            153, 48, 39, 162, 54, 144, 176, 6, 128, 214, 189, 53,
            152, 48, 38
        };

        private static readonly byte[] TransferWithSeedInstructionBytes =
        {
            11, 0, 0, 0, 64, 66, 15, 0, 0, 0, 0, 0, 8, 0, 0, 0, 116, 101, 115, 116,
            83, 101, 101, 100, 4, 23, 154, 206, 58, 166, 9, 125, 107, 80,
            224, 57, 235, 71, 51, 46, 27, 153, 48, 39, 162, 54, 144, 176,
            6, 128, 214, 189, 53, 152, 48, 38,
        };

        private const long BalanceForRentExemption = 2039280L;

        private const string NonceAccountBase64Data =
            "AAAAAAEAAABHaauXIEuoP7DK7hf3ho8eB05SFYGg2J2UN52qZbc" +
            "XsnM+zs3rCNyHGAjze1Gvfq4gRzzrz7ggv4rYXkMo8P2DiBMAAAAAAAA=";

        private const string NonceAccountInvalidBase64Data =
            "77+977+977+977+9Ae+/ve+/ve+/vUdpwqvClyBLwqg/wrDDisOuF8O3wob" +
            "Cjx4HTlJSFcKBIMOYwp3ClDfCncKqZcK3F8Kycz7DjsONw6sIw5zChxgIw7N" +
            "7UcKvfsKuIEc8w6vDj8K4IMK/worDmF5DKMOww73Cg8KIE++/ve+/ve+/ve+/ve+/ve+/vQ==";

        [TestMethod]
        public void TestSystemProgramTransfer()
        {
            var wallet = new Wallet.Wallet(MnemonicWords);

            var fromAccount = wallet.GetAccount(0);
            var toAccount = wallet.GetAccount(1);

            var txInstruction = SystemProgram.Transfer(fromAccount, toAccount.PublicKey, 10000000);

            Assert.AreEqual(2, txInstruction.Keys.Count);
            CollectionAssert.AreEqual(TransferInstructionBytes, txInstruction.Data);
            CollectionAssert.AreEqual(SystemProgramIdBytes, txInstruction.ProgramId);
        }

        [TestMethod]
        public void TestSystemProgramCreateAccount()
        {
            var wallet = new Wallet.Wallet(MnemonicWords);

            var mintAccount = wallet.GetAccount(3);
            var ownerAccount = wallet.GetAccount(4);

            var txInstruction = SystemProgram.CreateAccount(
                ownerAccount,
                mintAccount,
                BalanceForRentExemption,
                TokenProgram.TokenAccountDataSize,
                TokenProgram.ProgramIdKey);

            Assert.AreEqual(2, txInstruction.Keys.Count);
            CollectionAssert.AreEqual(CreateAccountInstructionBytes, txInstruction.Data);
            CollectionAssert.AreEqual(SystemProgramIdBytes, txInstruction.ProgramId);
        }

        [TestMethod]
        public void TestSystemProgramAssign()
        {
            var wallet = new Wallet.Wallet(MnemonicWords);

            var account = wallet.GetAccount(4);
            var newOwner = wallet.GetAccount(5);

            var txInstruction = SystemProgram.Assign(account, newOwner.PublicKey);

            Assert.AreEqual(1, txInstruction.Keys.Count);
            CollectionAssert.AreEqual(AssignInstructionBytes, txInstruction.Data);
            CollectionAssert.AreEqual(SystemProgramIdBytes, txInstruction.ProgramId);
        }

        [TestMethod]
        public void TestSystemProgramCreateAccountWithSeed()
        {
            var wallet = new Wallet.Wallet(MnemonicWords);

            var baseAccount = wallet.GetAccount(6);
            var from = wallet.GetAccount(5);
            var to = wallet.GetAccount(4);
            var owner = wallet.GetAccount(3);

            var txInstruction = SystemProgram.CreateAccountWithSeed(
                from,
                to.PublicKey,
                baseAccount,
                "testSeed",
                1_000_000,
                1000,
                owner.PublicKey);

            Assert.AreEqual(3, txInstruction.Keys.Count);
            CollectionAssert.AreEqual(CreateAccountWithSeedInstructionBytes, txInstruction.Data);
            CollectionAssert.AreEqual(SystemProgramIdBytes, txInstruction.ProgramId);
        }

        [TestMethod]
        public void TestSystemProgramAdvanceNonceAccount()
        {
            var wallet = new Wallet.Wallet(MnemonicWords);

            var nonceAccount = wallet.GetAccount(69);
            var owner = wallet.GetAccount(3);

            var txInstruction = SystemProgram.AdvanceNonceAccount(
                nonceAccount.PublicKey, owner);

            Assert.AreEqual(3, txInstruction.Keys.Count);
            CollectionAssert.AreEqual(AdvanceNonceAccountInstructionBytes, txInstruction.Data);
            CollectionAssert.AreEqual(SystemProgramIdBytes, txInstruction.ProgramId);
        }

        [TestMethod]
        public void TestSystemProgramWithdrawNonceAccount()
        {
            var wallet = new Wallet.Wallet(MnemonicWords);

            var nonceAccount = wallet.GetAccount(69);
            var to = wallet.GetAccount(5);
            var owner = wallet.GetAccount(3);

            var txInstruction = SystemProgram.WithdrawNonceAccount(
                nonceAccount.PublicKey,
                to.PublicKey,
                owner,
                1_000_000);

            Assert.AreEqual(5, txInstruction.Keys.Count);
            CollectionAssert.AreEqual(WithdrawNonceAccountInstructionBytes, txInstruction.Data);
            CollectionAssert.AreEqual(SystemProgramIdBytes, txInstruction.ProgramId);
        }

        [TestMethod]
        public void TestSystemProgramInitializeNonceAccount()
        {
            var wallet = new Wallet.Wallet(MnemonicWords);

            var nonceAccount = wallet.GetAccount(69);
            var owner = wallet.GetAccount(3);

            var txInstruction = SystemProgram.InitializeNonceAccount(
                nonceAccount.PublicKey,
                owner);

            Assert.AreEqual(3, txInstruction.Keys.Count);
            CollectionAssert.AreEqual(InitializeNonceAccountInstructionBytes, txInstruction.Data);
            CollectionAssert.AreEqual(SystemProgramIdBytes, txInstruction.ProgramId);
        }

        [TestMethod]
        public void TestSystemProgramAuthorizeNonceAccount()
        {
            var wallet = new Wallet.Wallet(MnemonicWords);

            var nonceAccount = wallet.GetAccount(69);
            var owner = wallet.GetAccount(4);
            var newAuthority = wallet.GetAccount(3);

            var txInstruction = SystemProgram.AuthorizeNonceAccount(
                nonceAccount.PublicKey,
                owner, newAuthority.PublicKey);

            Assert.AreEqual(2, txInstruction.Keys.Count);
            CollectionAssert.AreEqual(AuthorizeNonceAccountInstructionBytes, txInstruction.Data);
            CollectionAssert.AreEqual(SystemProgramIdBytes, txInstruction.ProgramId);
        }

        [TestMethod]
        public void TestSystemProgramAllocate()
        {
            var wallet = new Wallet.Wallet(MnemonicWords);

            var nonceAccount = wallet.GetAccount(69);
            var owner = wallet.GetAccount(3);

            var txInstruction = SystemProgram.Allocate(nonceAccount, 1_000_000);

            Assert.AreEqual(1, txInstruction.Keys.Count);
            CollectionAssert.AreEqual(AllocateInstructionBytes, txInstruction.Data);
            CollectionAssert.AreEqual(SystemProgramIdBytes, txInstruction.ProgramId);
        }

        [TestMethod]
        public void TestSystemProgramAllocateWithSeed()
        {
            var wallet = new Wallet.Wallet(MnemonicWords);

            var baseAccount = wallet.GetAccount(6);
            var account = wallet.GetAccount(5);
            var owner = wallet.GetAccount(3);

            var txInstruction = SystemProgram.AllocateWithSeed(
                account.PublicKey,
                baseAccount,
                "testSeed",
                1_000,
                owner.PublicKey);

            Assert.AreEqual(2, txInstruction.Keys.Count);
            CollectionAssert.AreEqual(AllocateWithSeedInstructionBytes, txInstruction.Data);
            CollectionAssert.AreEqual(SystemProgramIdBytes, txInstruction.ProgramId);
        }

        [TestMethod]
        public void TestSystemProgramAssignWithSeed()
        {
            var wallet = new Wallet.Wallet(MnemonicWords);

            var baseAccount = wallet.GetAccount(6);
            var account = wallet.GetAccount(5);
            var owner = wallet.GetAccount(3);

            var txInstruction = SystemProgram.AssignWithSeed(
                account.PublicKey,
                baseAccount,
                "testSeed",
                owner.PublicKey);

            Assert.AreEqual(2, txInstruction.Keys.Count);
            CollectionAssert.AreEqual(AssignWithSeedInstructionBytes, txInstruction.Data);
            CollectionAssert.AreEqual(SystemProgramIdBytes, txInstruction.ProgramId);
        }

        [TestMethod]
        public void TestSystemProgramTransferWithSeed()
        {
            var wallet = new Wallet.Wallet(MnemonicWords);

            var baseAccount = wallet.GetAccount(6);
            var from = wallet.GetAccount(5);
            var to = wallet.GetAccount(4);
            var owner = wallet.GetAccount(3);

            var txInstruction = SystemProgram.TransferWithSeed(
                from.PublicKey,
                baseAccount,
                "testSeed",
                owner.PublicKey,
                to.PublicKey,
                1_000_000);

            Assert.AreEqual(3, txInstruction.Keys.Count);
            CollectionAssert.AreEqual(TransferWithSeedInstructionBytes, txInstruction.Data);
            CollectionAssert.AreEqual(SystemProgramIdBytes, txInstruction.ProgramId);
        }

        [TestMethod]
        public void TestNonceAccountDeserializationException()
        {
            var nonceAccount = NonceAccount.Deserialize(Convert.FromBase64String(NonceAccountInvalidBase64Data));
            Assert.IsNull(nonceAccount);
        }

        [TestMethod]
        public void TestNonceAccountDeserialization()
        {
            var nonceAccount = NonceAccount.Deserialize(Convert.FromBase64String(NonceAccountBase64Data));

            Assert.AreEqual(0UL, nonceAccount.Version);
            Assert.AreEqual(1UL, nonceAccount.State);
            Assert.AreEqual("5omQJtDUHA3gMFdHEQg1zZSvcBUVzey5WaKWYRmqF1Vj", nonceAccount.Authorized.Key);
            Assert.AreEqual("8ksS6xXd7vzNrpZfBTf9gJ87Bma5AjnQ9baEcT7xH5QE", nonceAccount.Nonce.Key);
            Assert.AreEqual(5000UL, nonceAccount.FeeCalculator.LamportsPerSignature);
        }
    }
}