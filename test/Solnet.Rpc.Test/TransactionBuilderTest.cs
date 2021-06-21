using Microsoft.VisualStudio.TestTools.UnitTesting;
using Solnet.Programs;
using Solnet.Rpc.Builders;
using Solnet.Wallet;
using System;
using System.Collections.Generic;

namespace Solnet.Rpc.Test
{
    [TestClass]
    public class TransactionBuilderTest
    {
        private const string MnemonicWords =
            "route clerk disease box emerge airport loud waste attitude film army tray" +
            " forward deal onion eight catalog surface unit card window walnut wealth medal";

        private const string Blockhash = "5cZja93sopRB9Bkhckj5WzCxCaVyriv2Uh5fFDPDFFfj";

        private const string ExpectedTransactionHashWithTransferAndMemo =
            "AV9Xyi1t5dscb5+097PVDAP8fq/6HDRoNTQx9ZD2picvZNDUy9seCEKgsTNKgeTXtQ+pNEYB" +
            "4PfxPX+9bQI7hgkBAAIEUy4zulRg8z2yKITZaNwcnq6G6aH8D0ITae862qbJ+3eE3M6r5DRw" +
            "ldquwlqOuXDDOWZagXmbHnAU3w5Dg44kogAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA" +
            "AAAABUpTUPhdyILWFKVWcniKKW3fHqur0KYGeIhJMvTu9qBEixald4nI54jqHpYLSWViej50" +
            "bnmzhen0yUOsH2zbbgICAgABDAIAAACAlpgAAAAAAAMBABVIZWxsbyBmcm9tIFNvbC5OZXQgOik=";

        private const string ExpectedTransactionHashCreateInitializeAndMintTo =
            "A5X22for3AxcX09IKX5Cbrpvv4k/1TcdTY2wf6vkq7Wcb/3fwMjA0vCshKkBG0EXQM2oKanIaQilKC/L" +
            "KLmTYwc2yOVXu0TZCGwraCrxf4Pr8KpvTZZcUz/s4sls3VzGRqQmIhR3nXBR/O3\u002B4ZdICd8hYXb" +
            "USqUBE\u002B4qCwpbC7gLlVo1ErARFL9csoTPvxA3/00wTxbs01sXlAH5t\u002ByAiwlan7B24Za3d" +
            "CYydaczAOenGVU0nxBrz/gdFZgCJArZAAMABAdHaauXIEuoP7DK7hf3ho8eB05SFYGg2J2UN52qZbcXs" +
            "k\u002BnIqdN4P6YFyTS64cak6Wd2hx9Qsbwf4gfPc5VPJvFTT4lvYz77q8imSqvzO/5qiFW9tKqfO4l5F" +
            "KhFh6lZQsAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAan1RcZLFxRIYzJTD1K8X9Y2u4Im6H9" +
            "ROPb2YoAAAAABt324ddloZPZy\u002BFGzut5rBy0he1fWzeROoz1hX7/AKkFSlNQ\u002BF3IgtYUpVZye" +
            "Iopbd8eq6vQpgZ4iEky9O72oOD/Y3arpTMrvjv2uP0ZD3LVkDTmRAfOpQ603IYXOGjCBgMCAAE0AAAAAGBN" +
            "FgAAAAAAUgAAAAAAAAAG3fbh12Whk9nL4UbO63msHLSF7V9bN5E6jPWFfv8AqQUCAQRDAAJHaauXIEuoP7DK" +
            "7hf3ho8eB05SFYGg2J2UN52qZbcXsgFHaauXIEuoP7DK7hf3ho8eB05SFYGg2J2UN52qZbcXsgMCAAI0AAAA" +
            "APAdHwAAAAAApQAAAAAAAAAG3fbh12Whk9nL4UbO63msHLSF7V9bN5E6jPWFfv8AqQUEAgEABAEBBQMBAgA" +
            "JB6hhAAAAAAAABgECEkhlbGxvIGZyb20gU29sLk5ldA==";

        [TestMethod]
        public void TestTransactionBuilderBuild()
        {
            var wallet = new Wallet.Wallet(MnemonicWords);
            var fromAccount = wallet.GetAccount(0);
            var toAccount = wallet.GetAccount(1);
            var tx = new TransactionBuilder()
                .SetRecentBlockHash(Blockhash)
                .SetFeePayer(fromAccount)
                .AddInstruction(SystemProgram.Transfer(fromAccount, toAccount.PublicKey, 10000000))
                .AddInstruction(MemoProgram.NewMemo(fromAccount, "Hello from Sol.Net :)")).Build();

            Assert.AreEqual(ExpectedTransactionHashWithTransferAndMemo, Convert.ToBase64String(tx));
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void TestTransactionBuilderBuildNullBlockhashException()
        {
            var wallet = new Wallet.Wallet(MnemonicWords);
            var fromAccount = wallet.GetAccount(0);
            var toAccount = wallet.GetAccount(1);
            _ = new TransactionBuilder().SetFeePayer(fromAccount)
                .AddInstruction(SystemProgram.Transfer(fromAccount, toAccount.PublicKey, 10000000))
                .AddInstruction(MemoProgram.NewMemo(fromAccount, "Hello from Sol.Net :)")).Build();
        }
        
        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void TestTransactionBuilderBuildNullFeePayerException()
        {
            var wallet = new Wallet.Wallet(MnemonicWords);
            var fromAccount = wallet.GetAccount(0);
            var toAccount = wallet.GetAccount(1);
            _ = new TransactionBuilder()
                .SetRecentBlockHash(Blockhash)
                .AddInstruction(SystemProgram.Transfer(fromAccount, toAccount.PublicKey, 10000000))
                .AddInstruction(MemoProgram.NewMemo(fromAccount, "Hello from Sol.Net :)")).Build();
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void TestTransactionBuilderBuildNullInstructionsException()
        {
            var wallet = new Wallet.Wallet(MnemonicWords);
            var fromAccount = wallet.GetAccount(0);
            _ = new TransactionBuilder().SetRecentBlockHash(Blockhash).Build();
        }

        [TestMethod]
        public void CreateInitializeAndMintToTest()
        {
            var wallet = new Wallet.Wallet(MnemonicWords);

            var blockHash = "G9JC6E7LfG6ayxARq5zDV5RdDr6P8NJEdzTUJ8ttrSKs";
            var minBalanceForAccount = 2039280UL;
            var minBalanceForMintAccount = 1461600UL;

            var mintAccount = wallet.GetAccount(17);
            var ownerAccount = wallet.GetAccount(10);
            var initialAccount = wallet.GetAccount(18);

            var tx = new TransactionBuilder()
                .SetRecentBlockHash(blockHash)
                .SetFeePayer(ownerAccount)
                .AddInstruction(
                    SystemProgram.CreateAccount(
                    ownerAccount,
                    mintAccount,
                    minBalanceForMintAccount,
                    TokenProgram.MintAccountDataSize,
                    TokenProgram.ProgramIdKey))
                .AddInstruction(
                    TokenProgram.InitializeMint(
                    mintAccount.PublicKey,
                    2,
                    ownerAccount.PublicKey,
                    ownerAccount.PublicKey))
                .AddInstruction(
                    SystemProgram.CreateAccount(
                    ownerAccount,
                    initialAccount,
                    minBalanceForAccount,
                    SystemProgram.AccountDataSize,
                    TokenProgram.ProgramIdKey))
                .AddInstruction(
                    TokenProgram.InitializeAccount(
                    initialAccount.PublicKey,
                    mintAccount.PublicKey,
                    ownerAccount.PublicKey))
                .AddInstruction(
                    TokenProgram.MintTo(
                    mintAccount.PublicKey,
                    initialAccount.PublicKey,
                    25000,
                    ownerAccount))
                .AddInstruction(MemoProgram.NewMemo(initialAccount, "Hello from Sol.Net"))
                .Build();

            Assert.AreEqual(ExpectedTransactionHashCreateInitializeAndMintTo, Convert.ToBase64String(tx));
        }
    }
}