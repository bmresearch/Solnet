// unset

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Solnet.Programs.Test
{
    [TestClass]
    public class AssociatedTokenAccountProgramTest
    {
        private const string MnemonicWords =
            "route clerk disease box emerge airport loud waste attitude film army tray " +
            "forward deal onion eight catalog surface unit card window walnut wealth medal";

        private static readonly byte[] ProgramIdBytes =
        {
            140, 151, 37, 143, 78, 36, 137, 241, 187, 61, 16, 41, 20,
            142, 13, 131, 11, 90, 19, 153, 218, 255, 16, 132, 4, 142, 123,
            216, 219, 233, 248, 89
        };

        [TestMethod]
        public void CreateAssociatedTokenAccountTest()
        {
            var wallet = new Wallet.Wallet(MnemonicWords);

            var ownerAccount = wallet.GetAccount(10);
            var mintAccount = wallet.GetAccount(21);

            var txInstruction =
                AssociatedTokenAccountProgram.CreateAssociatedTokenAccount(
                    ownerAccount,
                    ownerAccount.PublicKey,
                    mintAccount.PublicKey);

            Assert.HasCount(7, txInstruction.Keys);
            CollectionAssert.AreEqual(ProgramIdBytes, txInstruction.ProgramId);
            CollectionAssert.AreEqual(System.Array.Empty<byte>(), txInstruction.Data);
        }

        [TestMethod]
        public void CreateAssociatedTokenAccountForToken2022Test()
        {
            var wallet = new Wallet.Wallet(MnemonicWords);

            var ownerAccount = wallet.GetAccount(10);
            var mintAccount = wallet.GetAccount(21);

            var txInstruction = AssociatedTokenAccountProgram.CreateAssociatedTokenAccount(
                ownerAccount,
                ownerAccount.PublicKey,
                mintAccount.PublicKey,
                Token2022Program.ProgramIdKey);

            Assert.AreEqual(Token2022Program.ProgramIdKey, txInstruction.Keys[5].PublicKey);

            var legacyAta = AssociatedTokenAccountProgram.DeriveAssociatedTokenAccount(ownerAccount.PublicKey, mintAccount.PublicKey);
            var token2022Ata = AssociatedTokenAccountProgram.DeriveAssociatedTokenAccount(
                ownerAccount.PublicKey,
                mintAccount.PublicKey,
                Token2022Program.ProgramIdKey);

            Assert.AreNotEqual(legacyAta, token2022Ata);
        }
    }
}