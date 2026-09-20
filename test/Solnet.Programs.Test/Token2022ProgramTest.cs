using Microsoft.VisualStudio.TestTools.UnitTesting;
using Solnet.Wallet;
using System.Collections.Generic;

namespace Solnet.Programs.Test
{
    [TestClass]
    public class Token2022ProgramTest
    {
        private const string MnemonicWords =
            "route clerk disease box emerge airport loud waste attitude film army tray " +
            "forward deal onion eight catalog surface unit card window walnut wealth medal";

        [TestMethod]
        public void TransferUsesToken2022ProgramIdTest()
        {
            Wallet.Wallet wallet = new(MnemonicWords);
            Account owner = wallet.GetAccount(0);
            Account destination = wallet.GetAccount(1);

            var instruction = Token2022Program.Transfer(owner.PublicKey, destination.PublicKey, 1_000, owner.PublicKey);

            CollectionAssert.AreEqual(Token2022Program.ProgramIdKey.KeyBytes, instruction.ProgramId);
            Assert.AreEqual(3, instruction.Data[0]);
        }

        [TestMethod]
        public void InitializeAccount3EncodesOwnerWithoutRentTest()
        {
            Wallet.Wallet wallet = new(MnemonicWords);
            Account account = wallet.GetAccount(0);
            Account mint = wallet.GetAccount(1);
            Account owner = wallet.GetAccount(2);

            var instruction = Token2022Program.InitializeAccount3(account.PublicKey, mint.PublicKey, owner.PublicKey);

            Assert.HasCount(2, instruction.Keys);
            Assert.AreEqual(18, instruction.Data[0]);
            CollectionAssert.AreEqual(owner.PublicKey.KeyBytes, instruction.Data[1..33]);
        }

        [TestMethod]
        public void GetAccountDataSizeEncodesExtensionTypesTest()
        {
            Wallet.Wallet wallet = new(MnemonicWords);
            Account mint = wallet.GetAccount(1);

            var instruction = Token2022Program.GetAccountDataSize(mint.PublicKey,
                new[] { Token2022ExtensionType.ImmutableOwner, Token2022ExtensionType.MemoTransfer });

            CollectionAssert.AreEqual(new byte[] { 21, 7, 0, 8, 0 }, instruction.Data);
        }

        [TestMethod]
        public void InstructionDecoderDecodesToken2022InstructionTest()
        {
            Wallet.Wallet wallet = new(MnemonicWords);
            Account account = wallet.GetAccount(0);

            var instruction = Token2022Program.InitializeImmutableOwner(account.PublicKey);
            var decoded = InstructionDecoder.Decode(Token2022Program.ProgramIdKey, instruction.Data,
                new List<PublicKey> { account.PublicKey }, new byte[] { 0 });

            Assert.IsNotNull(decoded);
            Assert.AreEqual("Token 2022 Program", decoded.ProgramName);
            Assert.AreEqual("Initialize Immutable Owner", decoded.InstructionName);
        }

        [TestMethod]
        public void InitializeMintCloseAuthorityEncodesOptionalAuthorityTest()
        {
            Wallet.Wallet wallet = new(MnemonicWords);
            Account mint = wallet.GetAccount(0);
            Account closeAuthority = wallet.GetAccount(1);

            var instruction = Token2022Program.InitializeMintCloseAuthority(mint.PublicKey, closeAuthority.PublicKey);

            Assert.AreEqual(25, instruction.Data[0]);
            Assert.AreEqual(1, instruction.Data[1]);
            CollectionAssert.AreEqual(closeAuthority.PublicKey.KeyBytes, instruction.Data[2..34]);
        }
    }
}