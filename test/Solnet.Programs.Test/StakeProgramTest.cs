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
        [TestMethod]
        public void TestStakeProgramAuthorize()
        {
            var wallet = new Wallet.Wallet(mnemonicWords: "");

            var baseAccount = wallet.GetAccount(6);
            var from = wallet.GetAccount(5);
            var to = wallet.GetAccount(4);
            var owner = wallet.GetAccount(3);

            var txInstruction = StakeProgram.Authorize(aaaa,bbbb,cccc,dddd,eeeee);
        }
    }
}
