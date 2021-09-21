using Microsoft.VisualStudio.TestTools.UnitTesting;
using Solnet.Wallet;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Text;

namespace Solnet.Programs.Test
{
    [TestClass]
    public class SharedMemoryProgramTest
    {
        private const string MnemonicWords =
            "route clerk disease box emerge airport loud waste attitude film army tray " +
            "forward deal onion eight catalog surface unit card window walnut wealth medal";


        [TestMethod]
        public void WriteEncodingTest()
        {
            var wallet = new Wallet.Wallet(MnemonicWords);
            var payload = Encoding.UTF8.GetBytes("Hello World!");
            var fromAccount = wallet.GetAccount(0);
            var toAccount = wallet.GetAccount(1);

            var tx = SharedMemoryProgram.Write(toAccount.PublicKey, payload, 0);

            var expectedData = new byte[payload.Length + 8];
            Array.Copy(payload, 0, expectedData, 8, payload.Length);

            CollectionAssert.AreEqual(expectedData, tx.Data);
            Assert.AreEqual(1, tx.Keys.Count);
        }

    }
}