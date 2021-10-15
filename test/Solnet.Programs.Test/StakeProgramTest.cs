using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solnet.Programs.Test
{
    [TestClass]
    public class StakeProgramTest
    {
        private static readonly byte[] StakeProgramIdBytes =
        {
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
        };

        private static readonly byte[] AuthorizeInstructionBytes =
        {
            //
        };
        [TestMethod]
        public void TestStakeProgramInitialize()
        {
            var wallet = new Wallet.Wallet(mnemonicWords: "");
        }
        [TestMethod]
        public void TestStakeProgramAuthorize()
        {
            var wallet = new Wallet.Wallet(mnemonicWords: "");

            var baseAccount = wallet.GetAccount(6);
            var from = wallet.GetAccount(5);
            var to = wallet.GetAccount(4);
            var owner = wallet.GetAccount(3);

            var txInstruction = StakeProgram.Authorize(baseAccount.PublicKey,from.PublicKey,to.PublicKey,Models.Stake.State.StakeAuthorize.Staker,owner.PublicKey);

            Assert.AreEqual(3, txInstruction.Keys.Count);
            CollectionAssert.AreEqual(AuthorizeInstructionBytes, txInstruction.Data);
            CollectionAssert.AreEqual(StakeProgramIdBytes, txInstruction.ProgramId);
        }
    }
}
