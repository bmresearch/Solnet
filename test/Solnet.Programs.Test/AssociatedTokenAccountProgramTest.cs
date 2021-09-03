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

            Assert.AreEqual(7, txInstruction.Keys.Count);
            CollectionAssert.AreEqual(ProgramIdBytes, txInstruction.ProgramId);
            CollectionAssert.AreEqual(System.Array.Empty<byte>(), txInstruction.Data);
        }
    }
}