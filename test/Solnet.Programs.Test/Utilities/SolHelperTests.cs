using Microsoft.VisualStudio.TestTools.UnitTesting;
using Solnet.Programs.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solnet.Programs.Test.Utilities
{
    [TestClass]
    public class SolHelperTests
    {

        [TestMethod]
        public void TestSolHelper()
        {
            Assert.AreEqual((ulong)168855000000, SolHelper.ConvertToLamports(168.855M));
            Assert.AreEqual(168.855M, SolHelper.ConvertToSol((ulong)168855000000));
            Assert.AreEqual(168.855000000M, SolHelper.ConvertToSol((ulong)168855000000));
        }

    }
}
