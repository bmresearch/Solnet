using Microsoft.VisualStudio.TestTools.UnitTesting;
using Solnet.Programs.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solnet.Programs.Test
{
    [TestClass]
    public class SerializationTest
    {
        [TestMethod]
        public void TryCreateWithSeed()
        {
            Assert.IsTrue(
                Serialization.TryCreateWithSeed(
                    SystemProgram.ProgramIdKey,
                    "limber chicken: 4/45",
                    SystemProgram.ProgramIdKey,
                    out var res));

            Assert.AreEqual("9h1HyLCW5dZnBVap8C5egQ9Z6pHyjsh5MNy83iPqqRuq", res.Key);
        }
    }
}
