using Microsoft.VisualStudio.TestTools.UnitTesting;
using Solnet.Programs;
using Solnet.Rpc.Builders;
using Solnet.Rpc.Models;
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

        private const string Nonce = "2S1kjspXLPs6jpNVXQfNMqZzzSrKLbGdr9Fxap5h1DLN";

        private static byte[] CompiledMessageBytes =
        {
            1, 0, 2, 5, 71, 105, 171, 151, 32, 75, 168, 63, 176, 202, 238, 23, 247, 134, 143, 30, 7, 78, 82, 21,
            129, 160, 216, 157, 148, 55, 157, 170, 101, 183, 23, 178, 132, 220, 206, 171, 228, 52, 112, 149, 218,
            174, 194, 90, 142, 185, 112, 195, 57, 102, 90, 129, 121, 155, 30, 112, 20, 223, 14, 67, 131, 142, 36,
            162, 223, 244, 229, 56, 86, 243, 0, 74, 86, 58, 56, 142, 17, 130, 113, 147, 61, 1, 136, 126, 243, 22,
            226, 173, 108, 74, 212, 104, 81, 199, 120, 180, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6, 167, 213, 23, 25, 44, 86, 142, 224, 138, 132, 95, 115, 210,
            151, 136, 207, 3, 92, 49, 69, 178, 26, 179, 68, 216, 6, 46, 169, 64, 0, 0, 21, 68, 15, 82, 0, 49, 0,
            146, 241, 176, 13, 84, 249, 55, 39, 9, 212, 80, 57, 8, 193, 89, 211, 49, 162, 144, 45, 140, 117, 21, 46,
            83, 2, 3, 3, 2, 4, 0, 4, 4, 0, 0, 0, 3, 2, 0, 1, 12, 2, 0, 0, 0, 0, 202, 154, 59, 0, 0, 0, 0
        };

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
                .AddInstruction(MemoProgram.NewMemo(fromAccount, "Hello from Sol.Net :)"))
                .Build(fromAccount);

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
                .AddInstruction(MemoProgram.NewMemo(fromAccount, "Hello from Sol.Net :)"))
                .Build(fromAccount);
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
                .AddInstruction(MemoProgram.NewMemo(fromAccount, "Hello from Sol.Net :)"))
                .Build(fromAccount);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void TestTransactionBuilderBuildEmptySignersException()
        {
            var wallet = new Wallet.Wallet(MnemonicWords);
            var fromAccount = wallet.GetAccount(0);
            var toAccount = wallet.GetAccount(1);
            _ = new TransactionBuilder()
                .SetRecentBlockHash(Blockhash)
                .AddInstruction(SystemProgram.Transfer(fromAccount, toAccount.PublicKey, 10000000))
                .AddInstruction(MemoProgram.NewMemo(fromAccount, "Hello from Sol.Net :)"))
                .Build(new List<Account>());
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void TestTransactionBuilderBuildNullInstructionsException()
        {
            var wallet = new Wallet.Wallet(MnemonicWords);
            var fromAccount = wallet.GetAccount(0);
            _ = new TransactionBuilder().SetRecentBlockHash(Blockhash)
                .Build(fromAccount);
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
                    TokenProgram.TokenAccountDataSize,
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
                .Build(new List<Account> { ownerAccount, mintAccount, initialAccount });

            Assert.AreEqual(ExpectedTransactionHashCreateInitializeAndMintTo, Convert.ToBase64String(tx));
        }

        [TestMethod]
        public void CompileMessageTest()
        {
            Wallet.Wallet wallet = new(MnemonicWords);

            Account ownerAccount = wallet.GetAccount(10);
            Account nonceAccount = wallet.GetAccount(1119);
            Account toAccount = wallet.GetAccount(1);
            NonceInformation nonceInfo = new()
            {
                Nonce = Nonce,
                Instruction = SystemProgram.AdvanceNonceAccount(
                    nonceAccount.PublicKey,
                    ownerAccount
                )
            };

            byte[] txBytes = new TransactionBuilder()
                .SetFeePayer(ownerAccount)
                .SetNonceInformation(nonceInfo)
                .AddInstruction(
                    SystemProgram.Transfer(
                        ownerAccount,
                        toAccount,
                        1_000_000_000)
                )
                .CompileMessage();

            CollectionAssert.AreEqual(CompiledMessageBytes, txBytes);
        }


        [TestMethod]
        public void TestTransactionInstructionTest()
        {
            Wallet.Wallet wallet = new(MnemonicWords);

            Account ownerAccount = wallet.GetAccount(10);
            var memo = MemoProgram.NewMemo(ownerAccount, "Hello");
            var created = TransactionInstructionFactory.Create(new PublicKey(memo.ProgramId), memo.Keys, memo.Data);

            Assert.AreEqual(Convert.ToBase64String(memo.ProgramId), Convert.ToBase64String(created.ProgramId));
            Assert.AreSame(memo.Keys, created.Keys);
            Assert.AreEqual(Convert.ToBase64String(memo.Data), Convert.ToBase64String(created.Data));

        }
    }
}