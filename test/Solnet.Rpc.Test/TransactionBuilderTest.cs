using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Solnet.Programs;
using Solnet.Rpc.Builders;

namespace Solnet.Rpc.Test
{
    [TestClass]
    public class TransactionBuilderTest
    {
        private const string Blockhash = "5cZja93sopRB9Bkhckj5WzCxCaVyriv2Uh5fFDPDFFfj";

        private const string ExpectedTransactionHashWithTransferAndMemo =
            "AV9Xyi1t5dscb5+097PVDAP8fq/6HDRoNTQx9ZD2picvZNDUy9seCEKgsTNKgeTXtQ+pNEYB4PfxPX+9bQI7hgkBAAIEUy4zulRg8z2yKITZaNwcnq6G6aH8D0ITae862qbJ+3eE3M6r5DRwldquwlqOuXDDOWZagXmbHnAU3w5Dg44kogAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAABUpTUPhdyILWFKVWcniKKW3fHqur0KYGeIhJMvTu9qBEixald4nI54jqHpYLSWViej50bnmzhen0yUOsH2zbbgICAgABDAIAAACAlpgAAAAAAAMBABVIZWxsbyBmcm9tIFNvbC5OZXQgOik=";

        [TestMethod]
        public void TestTransactionBuilderBuild()
        {
            var wallet =
                new Wallet.Wallet(
                    "route clerk disease box emerge airport loud waste attitude film army tray forward deal onion eight catalog surface unit card window walnut wealth medal");
            var fromAccount = wallet.GetAccount(0);
            var toAccount = wallet.GetAccount(1);
            var tx = new TransactionBuilder().
                SetRecentBlockHash(Blockhash).
                AddInstruction(SystemProgram.Transfer(fromAccount.GetPublicKey, toAccount.GetPublicKey, 10000000)).
                AddInstruction(MemoProgram.NewMemo(fromAccount, "Hello from Sol.Net :)")).
                Build(fromAccount);
            
            Assert.AreEqual(ExpectedTransactionHashWithTransferAndMemo, Convert.ToBase64String(tx));
        }
        
        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void TestTransactionBuilderBuildNullBlockhashException()
        {
            var wallet = new Wallet.Wallet("route clerk disease box emerge airport loud waste attitude film army tray forward deal onion eight catalog surface unit card window walnut wealth medal");
            var fromAccount = wallet.GetAccount(0);
            var toAccount = wallet.GetAccount(1);
            _ = new TransactionBuilder().
                AddInstruction(SystemProgram.Transfer(fromAccount.GetPublicKey, toAccount.GetPublicKey, 10000000)).
                AddInstruction(MemoProgram.NewMemo(fromAccount, "Hello from Sol.Net :)")).
                Build(fromAccount);
        }
        
        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void TestTransactionBuilderBuildNullInstructionsException()
        {
            var wallet = new Wallet.Wallet("route clerk disease box emerge airport loud waste attitude film army tray forward deal onion eight catalog surface unit card window walnut wealth medal");
            var fromAccount = wallet.GetAccount(0);
            _ = new TransactionBuilder().
                SetRecentBlockHash(Blockhash).
                Build(fromAccount);
        }
        
    }
}